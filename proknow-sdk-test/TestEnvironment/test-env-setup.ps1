# Step 1: Check if config.json file exists
$configPath = Join-Path -Path $PSScriptRoot -ChildPath "etc/config.json"
if (-not (Test-Path $configPath)) {
    Write-Host "Config file not found at $configPath" -ForegroundColor Red
    exit 1
}

# Step 2: Get accessKeyId and secretAccessKey from config.json
$config = Get-Content $configPath | ConvertFrom-Json
$accessKeyId = $config.security.accessKeyId
$secretAccessKey = $config.security.secretAccessKey

# Step 3: Call GET http://localhost:3005/api/status
$statusResponse = Invoke-WebRequest -Uri "http://localhost:3005/api/status"
if ($statusResponse.StatusCode -ne 200) {
    Write-Host "Failed to get status of ProKnow. Make sure to run 'docker-compose up -d' first. Status code: $($statusResponse.StatusCode)" -ForegroundColor Red
    exit 1
}

# Step 4: Call GET http://localhost:3005/api/organizations using Basic Auth
$basicAuth = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes("$($accessKeyId):$($secretAccessKey)"))
$headers = @{
    Authorization = "Basic $basicAuth"
}
$organizationsResponse = Invoke-WebRequest -Uri "http://localhost:3005/api/organizations" -Headers $headers
if ($organizationsResponse.StatusCode -ne 200) {
    Write-Host "Failed to get list of existing organizations from ProKnow. Status code: $($organizationsResponse.StatusCode)" -ForegroundColor Red
    exit 1
}

# Step 5: Check if organization ".NET SDK Testing" exists
$organizations = $organizationsResponse.Content | ConvertFrom-Json
$orgExists = $organizations | Where-Object { $_.name -eq ".NET SDK Testing" }
if ($orgExists) {
    Write-Host ".NET SDK Testing organization already exists. Exiting." -ForegroundColor Yellow
    Write-Output $orgExists
    exit 0
}

# Step 6: Call POST http://localhost:3005/api/organizations using Basic Auth
$body = @{
    subdomain = "pk-test"
    name = ".NET SDK Testing"
    user = @{
        email = "admin@proknow.com"
        name = "Administrator"
        password = "Focus1.1"
        api_key = @{
            name = ".NET SDK Testing API Key"
        }
    }
} | ConvertTo-Json

$headers = @{
    Authorization = "Basic $basicAuth"
    "Content-Type" = "application/json"
}

$newOrgResponse = Invoke-WebRequest -Uri "http://localhost:3005/api/organizations" -Method Post -Body $body -Headers $headers
if ($newOrgResponse.StatusCode -ne 200) {
    Write-Host "Failed to create organization for SDK testing. Status code: $($newOrgResponse.StatusCode)" -ForegroundColor Red
    exit 1
}

# Step 7: Record apiKeyId and apiKeySecret
$newOrgContent = $newOrgResponse.Content | ConvertFrom-Json
$apiKeyId = $newOrgContent.user.api_key.id
$apiKeySecret = $newOrgContent.user.api_key.secret

# Step 8: Write credentials to credentials.json
$credentialsPath = "credentials.json"
if (Test-Path $credentialsPath) {
    Remove-Item $credentialsPath -Force
}

$credentials = @{
    id = $apiKeyId
    secret = $apiKeySecret
} | ConvertTo-Json
$credentials | Out-File -FilePath $credentialsPath -Encoding UTF8

Write-Host "Credentials written to $($PWD.Path)\$credentialsPath" -ForegroundColor Cyan

Write-Host "NOTE: Be sure to update your .runsettings file with the location of the credentials.json file" -ForegroundColor Yellow
