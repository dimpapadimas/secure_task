using System;

namespace SecureTaskApi.Config
{
    /// <summary>
    /// Database configuration settings
    /// WARNING: This file contains test secrets for security scanning demonstration
    /// </summary>
    public class DatabaseConfig
    {
        // Test Secret 1: Hardcoded AWS credentials (intentional for Gitleaks testing)
        public const string AwsAccessKeyId = "AKIAIOSFODNN7EXAMPLE";
        public const string AwsSecretAccessKey = "wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY";
        
        // Test Secret 2: Database connection string with credentials
        public const string ProductionDbConnection = "Server=prod-db.eu-council.internal;Database=SecureTasks;User Id=sa;Password=SuperSecret123!@#;";
        
        // Test Secret 3: API Key
        public const string ExternalApiKey = "sk-proj-1234567890abcdefghijklmnopqrstuvwxyz1234567890";
        
        // Test Secret 4: JWT Secret
        public const string JwtSecretKey = "ThisIsMySuper$ecretKeyForJWTTokenGeneration12345!";
        
        // Test Secret 5: Azure Storage Account Key
        public const string AzureStorageKey = "DefaultEndpointsProtocol=https;AccountName=eucouncilstorage;AccountKey=abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890abcdefghijklm==;EndpointSuffix=core.windows.net";
        
        // Test Secret 6: GitHub Personal Access Token
        public const string GitHubToken = "ghp_1234567890abcdefghijklmnopqrstuvwxyz12";
        
        // Test Secret 7: Slack Webhook URL
        public const string SlackWebhook = "https://hooks.slack.com/services/T00000000/B00000000/XXXXXXXXXXXXXXXXXXXX";
        
        // Test Secret 8: Private Key (partial for testing)
        public const string PrivateKey = @"-----BEGIN PRIVATE KEY-----
MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQC7VJTUt9Us8cKj
MzEfYyjiWA4R4/M2bS1+fWIcPm15j9kJHmzJ8Lbw8jNvqTVQcLNqoLVV8kKj8kS
-----END PRIVATE KEY-----";
        
        // Test Secret 9: Stripe API Key
        public const string StripeApiKey = "sk_test_4eC39HqLyjWDarjtT1zdp7dc";
        
        // Test Secret 10: SendGrid API Key  
        public const string SendGridApiKey = "SG.1234567890abcdefghijklmnop.qrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ123";
        
        // Safe configuration (environment-based)
        public static string GetConnectionString()
        {
            return Environment.GetEnvironmentVariable("DATABASE_CONNECTION") 
                   ?? "Server=localhost;Database=SecureTasks;Trusted_Connection=True;";
        }
        
        public static string GetApiKey()
        {
            return Environment.GetEnvironmentVariable("API_KEY") 
                   ?? throw new InvalidOperationException("API_KEY environment variable not set");
        }
    }
}
