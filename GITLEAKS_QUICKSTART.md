# Quick Start: Gitleaks Secret Scanning

## 🚀 One-Time Setup

### 1. Add DefectDojo Token to GitHub Secrets

```bash
# GitHub Repository -> Settings -> Secrets and variables -> Actions -> New repository secret
Name: DEFECTDOJO_TOKEN
Value: <your-token-from-defectdojo>
```

Get your token from: https://demo.defectdojo.org/api/key-v2

### 2. Verify Workflows Exist

```bash
.github/workflows/
├── reusable-gitleaks-scan.yml  # Reusable workflow
└── secret-scanning.yml          # Caller workflow
```

## 🔍 Running the Scan

### Automatic Triggers

The scan runs automatically on:
- ✅ Push to `main` or `develop` branches
- ✅ Pull requests to `main`

### Manual Trigger

1. Go to: **Actions** → **Secret Scanning** → **Run workflow**
2. Select branch: `main` or `develop`
3. (Optional) Check "Fail on detection" to block on secrets
4. Click **Run workflow**

## 📊 Viewing Results

### GitHub Actions

1. **Go to Actions tab** in your repository
2. **Click on latest workflow run**
3. **Open "🔐 Secret Scan" job**
4. **View the summary** for detected secrets
5. **Download artifacts** for full JSON report

### DefectDojo

1. **Go to**: https://demo.defectdojo.org
2. **Navigate to**: Products → **SecureTaskApi**
3. **View findings** from the latest scan
4. **Check engagement** for scan date and results

## 📋 Expected Results (Test Secrets)

The scan should detect approximately **10-15 secrets**:

- AWS Access Keys (2)
- Database Passwords (1)
- API Keys (3)
- JWT Secrets (1)
- Private Keys (1)
- GitHub Tokens (1)
- Webhook URLs (1)
- Azure Storage Keys (1)

## 🔧 Workflow Configuration

### Fail on Detection (Blocking Mode)

Edit `.github/workflows/secret-scanning.yml`:

```yaml
jobs:
  gitleaks:
    uses: ./.github/workflows/reusable-gitleaks-scan.yml
    secrets:
      DEFECTDOJO_TOKEN: ${{ secrets.DEFECTDOJO_TOKEN }}
    with:
      fail-on-detection: true  # ← Change to true to block PRs
      gitleaks-version: '8.21.2'
```

### Update Gitleaks Version

```yaml
with:
  gitleaks-version: '8.22.0'  # ← Update version here
```

## 🧪 Local Testing

### Install Gitleaks

```bash
# macOS
brew install gitleaks

# Linux (manual)
wget https://github.com/gitleaks/gitleaks/releases/download/v8.21.2/gitleaks_8.21.2_linux_x64.tar.gz
tar -xzf gitleaks_8.21.2_linux_x64.tar.gz
sudo mv gitleaks /usr/local/bin/
```

### Run Local Scan

```bash
cd /Users/tyler/Developer/eu_council/secure_task

# Scan git history (committed files only)
gitleaks detect --source=. --report-path=gitleaks-report.json --report-format=json --verbose

# Scan filesystem (including uncommitted files) - RECOMMENDED FOR LOCAL TESTING
gitleaks detect --source=. --no-git --report-path=gitleaks-report.json --report-format=json --verbose

# Scan staged files (before committing)
gitleaks protect --staged --verbose

# View results
cat gitleaks-report.json | jq '.[] | {rule: .RuleID, file: .File, line: .StartLine}'
```

### Analyze Report

```bash
# Count secrets
jq 'length' gitleaks-report.json

# List by type
jq -r '.[] | .RuleID' gitleaks-report.json | sort | uniq -c

# Show details (redacted)
jq -r '.[] | "[\(.RuleID)] \(.File):\(.StartLine)"' gitleaks-report.json
```

## 🔄 Integration with Main CI/CD

### Add to Existing Pipeline

Edit `.github/workflows/dotnet-ci.yml`:

```yaml
jobs:
  # ... existing jobs ...
  
  security-secrets:
    name: 🔐 Secret Scanning
    uses: ./.github/workflows/reusable-gitleaks-scan.yml
    secrets:
      DEFECTDOJO_TOKEN: ${{ secrets.DEFECTDOJO_TOKEN }}
    with:
      fail-on-detection: true
```

## 🛠️ Troubleshooting

### Issue: DefectDojo Upload Failed

**Check**:
1. Token is valid: https://demo.defectdojo.org/api/key-v2
2. Token is added to GitHub Secrets
3. Network connectivity from runner

**Fix**:
```bash
# Test token manually
curl -X GET "https://demo.defectdojo.org/api/v2/products/" \
  -H "Authorization: Token YOUR_TOKEN"
```

### Issue: No Secrets Detected

**Check**:
1. Test secrets file exists: `SecureTaskApi/Config/DatabaseConfig.cs`
2. Gitleaks version is correct
3. Git history is fetched (`fetch-depth: 0`)

### Issue: Scan Takes Too Long

**Optimize**:
```yaml
- name: 📥 Checkout code
  uses: actions/checkout@v4
  with:
    fetch-depth: 1  # Scan only latest commit (faster but less thorough)
```

## 📝 Best Practices

### ✅ DO:
- Run on every PR
- Review findings in DefectDojo
- Rotate exposed secrets immediately
- Use environment variables for real secrets
- Keep test secrets clearly marked

### ❌ DON'T:
- Ignore secret detections without review
- Commit real credentials (even in private repos)
- Disable scanning to "fix" builds
- Store secrets in code comments
- Use production secrets in development

## 🗑️ Cleanup Test Secrets

When done testing:

```bash
# Remove test secrets
rm SecureTaskApi/Config/DatabaseConfig.cs
rm .env.example

# Update .gitignore
# Remove the note about DatabaseConfig.cs

# Commit
git add -A
git commit -m "Remove test secrets after validation"
git push
```

Verify clean:
```bash
gitleaks detect --source=. --verbose
# Should show: "No leaks found"
```

## 📚 Additional Resources

- **Gitleaks Docs**: https://github.com/gitleaks/gitleaks
- **DefectDojo API**: https://demo.defectdojo.org/api/v2/doc/
- **Security Guide**: `SECURITY_SCANNING.md`
- **EU Council DevSecOps**: Contact Jim for support

---

**Last Updated**: 2024  
**Version**: 1.0  
**Maintained By**: Jim (External Contractor - Application Security)
