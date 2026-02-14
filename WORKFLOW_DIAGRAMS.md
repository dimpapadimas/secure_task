```mermaid
graph TB
    Start([Push/PR/Manual Trigger]) --> Caller[secret-scanning.yml]
    
    Caller -->|calls| Reusable[reusable-gitleaks-scan.yml]
    Caller -->|passes| Token[DEFECTDOJO_TOKEN]
    
    subgraph "Reusable Workflow Jobs"
        Reusable --> Checkout[📥 Checkout Code<br/>fetch-depth: 0]
        Checkout --> Download[📦 Download Gitleaks<br/>v8.21.2]
        Download --> Scan[🔍 Run Gitleaks Scan<br/>--report-format=json]
        
        Scan --> Report{Report<br/>Generated?}
        
        Report -->|Yes| Analyze[📊 Analyze Results<br/>Count secrets]
        Report -->|No| Fail1[❌ Fail - No Report]
        
        Analyze --> Count{Secrets<br/>Found?}
        
        Count -->|Yes| Summary1[⚠️ Create Warning Summary<br/>List detected secrets]
        Count -->|No| Summary2[✅ Create Success Summary<br/>All clear!]
        
        Summary1 --> Upload1[📤 Upload to DefectDojo]
        Summary2 --> Upload2[📤 Upload to DefectDojo]
        
        Upload1 --> API[DefectDojo API Call<br/>POST /api/v2/import-scan/]
        Upload2 --> API
        
        API --> Create{Product<br/>Exists?}
        
        Create -->|No| CreateProd[Create Product:<br/>SecureTaskApi]
        Create -->|Yes| Engagement[Create Engagement:<br/>GitHub Actions - DATE]
        CreateProd --> Engagement
        
        Engagement --> Import[Import Findings<br/>Active & Unverified]
        Import --> Artifact[📦 Upload Artifact<br/>gitleaks-report.json]
        
        Artifact --> CheckFail{fail-on-detection<br/>== true?}
        
        CheckFail -->|Yes + Secrets| Fail2[❌ Fail Workflow]
        CheckFail -->|No| Success[✅ Success]
        CheckFail -->|Yes + No Secrets| Success
    end
    
    Success --> Results1[📊 View in GitHub Actions]
    Success --> Results2[📊 View in DefectDojo]
    Fail2 --> Results1
    Fail2 --> Results2
    
    Results1 --> End1([End])
    Results2 --> End2([End])
    
    style Start fill:#e1f5fe
    style Success fill:#c8e6c9
    style Fail1 fill:#ffcdd2
    style Fail2 fill:#ffcdd2
    style Summary1 fill:#fff9c4
    style Summary2 fill:#c8e6c9
    style API fill:#e1bee7
    style Import fill:#e1bee7
    
    classDef secretsFound fill:#fff9c4,stroke:#f57f17,stroke-width:2px
    classDef noSecrets fill:#c8e6c9,stroke:#388e3c,stroke-width:2px
    classDef defectdojo fill:#e1bee7,stroke:#7b1fa2,stroke-width:2px
    
    class Count,Summary1 secretsFound
    class Summary2 noSecrets
    class API,Create,CreateProd,Engagement,Import defectdojo
```

# Gitleaks Workflow Architecture

## Component Diagram

```mermaid
graph LR
    subgraph "GitHub Repository"
        Code[Source Code<br/>with Test Secrets]
        Workflows[.github/workflows/]
        Secrets[GitHub Secrets<br/>DEFECTDOJO_TOKEN]
    end
    
    subgraph "Workflows"
        Caller[secret-scanning.yml<br/>Caller Workflow]
        Reusable[reusable-gitleaks-scan.yml<br/>Reusable Workflow]
    end
    
    subgraph "Security Tools"
        Gitleaks[Gitleaks v8.21.2<br/>Secret Scanner]
    end
    
    subgraph "Outputs"
        JSON[gitleaks-report.json]
        Summary[GitHub Actions<br/>Summary]
        Artifact[Workflow Artifacts<br/>90 days retention]
    end
    
    subgraph "DefectDojo"
        Product[Product:<br/>SecureTaskApi]
        Engagement[Engagement:<br/>GitHub Actions - DATE]
        Findings[Security Findings<br/>Active & Unverified]
    end
    
    Code --> Workflows
    Workflows --> Caller
    Caller -->|calls| Reusable
    Caller -->|provides| Secrets
    
    Reusable -->|downloads| Gitleaks
    Gitleaks -->|scans| Code
    Gitleaks -->|generates| JSON
    
    JSON --> Summary
    JSON --> Artifact
    JSON -->|uploads to| Product
    
    Product --> Engagement
    Engagement --> Findings
    
    style Code fill:#ffebee
    style Gitleaks fill:#e3f2fd
    style JSON fill:#f3e5f5
    style Product fill:#e8f5e9
    style Findings fill:#fff3e0
```

## Data Flow

```mermaid
sequenceDiagram
    participant Dev as Developer
    participant GH as GitHub Actions
    participant GL as Gitleaks
    participant DD as DefectDojo
    
    Dev->>GH: Push/PR to main
    GH->>GH: Trigger secret-scanning.yml
    GH->>GL: Download Gitleaks v8.21.2
    GH->>GL: Run scan on repository
    GL->>GL: Analyze code & git history
    GL->>GH: Generate gitleaks-report.json
    
    alt Secrets Found
        GL-->>GH: Return 10+ findings
        GH->>GH: Create warning summary
    else No Secrets
        GL-->>GH: Return 0 findings
        GH->>GH: Create success summary
    end
    
    GH->>DD: POST /api/v2/import-scan/
    DD->>DD: Auto-create SecureTaskApi product
    DD->>DD: Create engagement
    DD->>DD: Import findings
    DD-->>GH: 201 Created
    
    GH->>GH: Upload artifact (JSON)
    GH->>Dev: Show summary in Actions
    DD->>Dev: View findings in dashboard
    
    alt fail-on-detection: true && secrets found
        GH-->>Dev: ❌ Workflow failed
    else
        GH-->>Dev: ✅ Workflow succeeded
    end
```

## Test Secrets Coverage

```mermaid
mindmap
  root((Test Secrets<br/>10+ Types))
    Cloud Providers
      AWS Access Keys
      Azure Storage Keys
    Authentication
      API Keys
        Generic API Key
        Stripe Key
        SendGrid Key
      OAuth Tokens
        GitHub PAT
      JWT Secrets
    Infrastructure
      Database Passwords
        SQL Connection Strings
      Private Keys
        RSA Keys
    Webhooks
      Slack Webhooks
```

## DefectDojo Integration Flow

```
┌─────────────────────────────────────────────────────────────┐
│                    GitHub Actions Runner                     │
│                                                              │
│  ┌────────────────────────────────────────────────────┐    │
│  │ 1. Download Gitleaks Binary                        │    │
│  │    wget gitleaks_8.21.2_linux_x64.tar.gz          │    │
│  └────────────────────────────────────────────────────┘    │
│                          ↓                                   │
│  ┌────────────────────────────────────────────────────┐    │
│  │ 2. Run Secret Scan                                 │    │
│  │    ./gitleaks detect --report-format=json         │    │
│  └────────────────────────────────────────────────────┘    │
│                          ↓                                   │
│  ┌────────────────────────────────────────────────────┐    │
│  │ 3. Analyze gitleaks-report.json                   │    │
│  │    - Count: 10-15 secrets expected                │    │
│  │    - Types: AWS, DB, API keys, etc.              │    │
│  └────────────────────────────────────────────────────┘    │
│                          ↓                                   │
│  ┌────────────────────────────────────────────────────┐    │
│  │ 4. Upload to DefectDojo                           │    │
│  │    POST /api/v2/import-scan/                      │    │
│  │    - file: gitleaks-report.json                   │    │
│  │    - scan_type: "Gitleaks Scan"                   │    │
│  │    - product_name: "SecureTaskApi"                │    │
│  │    - auto_create_context: true                    │    │
│  └────────────────────────────────────────────────────┘    │
│                          ↓                                   │
│  ┌────────────────────────────────────────────────────┐    │
│  │ 5. Upload Artifact                                │    │
│  │    Retention: 90 days                             │    │
│  └────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────────┐
│                      DefectDojo Server                       │
│                  https://demo.defectdojo.org                │
│                                                              │
│  ┌────────────────────────────────────────────────────┐    │
│  │ Product: SecureTaskApi                            │    │
│  │   └─ Engagement: GitHub Actions - 2024-01-15     │    │
│  │       └─ Findings (10-15):                        │    │
│  │           • AWS Access Key (High)                 │    │
│  │           • Database Password (High)              │    │
│  │           • API Keys (Medium)                     │    │
│  │           • JWT Secret (Medium)                   │    │
│  │           • Private Key (High)                    │    │
│  │           • ... and more                          │    │
│  └────────────────────────────────────────────────────┘    │
│                                                              │
│  Status: All findings marked as:                            │
│    - Active: ✅ Yes                                         │
│    - Verified: ❌ No (requires review)                      │
│    - Severity: Info/Low/Medium/High/Critical                │
└─────────────────────────────────────────────────────────────┘
```

## Air-Gapped Adaptation

### Current Implementation (Testing)
```yaml
- name: Download Gitleaks
  run: |
    wget https://github.com/gitleaks/gitleaks/releases/download/v8.21.2/gitleaks_8.21.2_linux_x64.tar.gz
    tar -xzf gitleaks_8.21.2_linux_x64.tar.gz
    chmod +x gitleaks
```

### Production Air-Gapped Environment
```yaml
- name: Use Pre-installed Gitleaks
  run: |
    # Gitleaks is pre-installed at /usr/local/bin/gitleaks
    gitleaks version
```

**Pre-installation on runners:**
```bash
# One-time setup on each runner
sudo wget https://github.com/gitleaks/gitleaks/releases/download/v8.21.2/gitleaks_8.21.2_linux_x64.tar.gz
sudo tar -xzf gitleaks_8.21.2_linux_x64.tar.gz -C /usr/local/bin/
sudo chmod +x /usr/local/bin/gitleaks
sudo rm gitleaks_8.21.2_linux_x64.tar.gz
```
