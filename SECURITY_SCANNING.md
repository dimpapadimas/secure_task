# Gitleaks Secret Scanning - Test Secrets

This directory contains **intentional test secrets** for demonstrating and validating the Gitleaks secret scanning workflow.

## ⚠️ Important Notes

These are **NOT real secrets**. They are test data designed to trigger Gitleaks detection rules to validate our DevSecOps pipeline.

## Test Secrets Location

### `Config/DatabaseConfig.cs`
Contains 10 different types of hardcoded secrets:

1. **AWS Access Key ID**: `AKIAIOSFODNN7EXAMPLE`
2. **AWS Secret Access Key**: `wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY`
3. **SQL Server Connection String** with password
4. **Generic API Key**: OpenAI-style key format
5. **JWT Secret Key**: Hardcoded JWT signing key
6. **Azure Storage Account Key**: Connection string with key
7. **GitHub Personal Access Token**: `ghp_...` format
8. **Slack Webhook URL**: Full webhook endpoint
9. **Private Key**: RSA private key fragment
10. **Stripe API Key**: Test mode key
11. **SendGrid API Key**: Full API key format

### `.env.example`
Contains development credentials in JSON format:
- Database connection string with credentials
- API keys
- Secret tokens
- AWS credentials

## Expected Gitleaks Detections

When running the Gitleaks scan, you should see detections for:

- ✅ AWS credentials (both access key and secret)
- ✅ Database passwords in connection strings
- ✅ API keys (various formats)
- ✅ JWT secrets
- ✅ Private keys
- ✅ OAuth tokens
- ✅ Webhook URLs with secrets

## Workflow Overview

### Reusable Workflow: `reusable-gitleaks-scan.yml`

**Purpose**: Air-gapped compatible secret scanning workflow

**Features**:
- Downloads Gitleaks binary directly (simulates pre-installed tools)
- Scans full git history
- Generates JSON report
- Auto-uploads to DefectDojo
- Creates engagement automatically
- Does NOT fail by default (configurable)

**Inputs**:
- `fail-on-detection`: Whether to fail on secrets (default: false)
- `gitleaks-version`: Version to download (default: 8.21.2)

**Secrets**:
- `DEFECTDOJO_TOKEN`: Required for DefectDojo upload

**Outputs**:
- JSON report artifact (90-day retention)
- DefectDojo integration
- GitHub Actions summary

### Caller Workflow: `secret-scanning.yml`

**Triggers**:
- Push to `main` or `develop`
- Pull requests to `main`
- Manual dispatch (with optional failure mode)

**Configuration**:
- Product: `SecureTaskApi`
- DefectDojo: `https://demo.defectdojo.org`
- Auto-create engagement: ✅ Enabled

## Setup Requirements

### GitHub Secrets

Add the following secret to your repository:

```
DEFECTDOJO_TOKEN=<your-defectdojo-api-token>
```

To get a DefectDojo token:
1. Go to https://demo.defectdojo.org
2. Login or create account
3. Navigate to: API v2 Key (User settings)
4. Generate new token

### Air-Gapped Simulation

The workflow downloads Gitleaks at runtime to simulate an environment where:
- Runners don't have internet during execution
- Tools are pre-installed on runners
- Only tool binaries are available (no GitHub Actions marketplace)

In a real air-gapped environment, you would:
1. Pre-install Gitleaks on runners: `/usr/local/bin/gitleaks`
2. Remove the download step
3. Use the pre-installed binary directly

## DefectDojo Integration

The workflow automatically:
1. ✅ Creates product if it doesn't exist (`SecureTaskApi`)
2. ✅ Creates engagement with current date
3. ✅ Uploads scan results
4. ✅ Sets findings to active
5. ✅ Marks findings as unverified (requires review)

## Testing the Workflow

### Local Testing

```bash
# Download Gitleaks
wget https://github.com/gitleaks/gitleaks/releases/download/v8.21.2/gitleaks_8.21.2_linux_x64.tar.gz
tar -xzf gitleaks_8.21.2_linux_x64.tar.gz

# Run scan
./gitleaks detect --source=. --report-path=gitleaks-report.json --report-format=json --verbose

# Check results
cat gitleaks-report.json | jq '.'
```

### Expected Output

You should see approximately **10-15 findings** from the test secrets.

## Remediation Examples

The `DatabaseConfig.cs` file includes examples of **proper secret management**:

```csharp
// ❌ BAD: Hardcoded
public const string ApiKey = "sk-proj-123456...";

// ✅ GOOD: Environment variable
public static string GetApiKey()
{
    return Environment.GetEnvironmentVariable("API_KEY") 
           ?? throw new InvalidOperationException("API_KEY not set");
}
```

## Integration with Main CI/CD

This workflow runs independently but can be integrated:

```yaml
jobs:
  security-scan:
    uses: ./.github/workflows/reusable-gitleaks-scan.yml
    secrets:
      DEFECTDOJO_TOKEN: ${{ secrets.DEFECTDOJO_TOKEN }}
    with:
      fail-on-detection: true  # Block merges with secrets
```

## Cleanup

To remove test secrets after validation:
1. Delete `Config/DatabaseConfig.cs`
2. Delete `.env.example`
3. Update this README
4. Commit and verify clean scan

---

**Last Updated**: 2024
**Maintainer**: Jim (EU Council DevSecOps)
