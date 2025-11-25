# Script PowerShell pour configurer la base de données SQLite
# Ce script installe dotnet-ef (si nécessaire) et crée/applique les migrations

Write-Host "Configuration de la base de données..." -ForegroundColor Green

# Se placer dans le répertoire du projet
Set-Location $PSScriptRoot

# Vérifier si dotnet-ef est installé
$efTool = dotnet tool list -g | Select-String "dotnet-ef"
if (-not $efTool) {
    Write-Host "Installation de dotnet-ef..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-ef
    Write-Host "dotnet-ef installé avec succès!" -ForegroundColor Green
    Write-Host "ATTENTION: Vous devrez peut-être redémarrer PowerShell pour que l'outil soit disponible." -ForegroundColor Yellow
    Write-Host "Tentative de mise à jour du PATH..." -ForegroundColor Yellow
    $env:Path = [System.Environment]::GetEnvironmentVariable("Path","Machine") + ";" + [System.Environment]::GetEnvironmentVariable("Path","User")
} else {
    Write-Host "dotnet-ef est déjà installé." -ForegroundColor Green
}

# Vérifier si dotnet-ef est maintenant disponible
try {
    $null = Get-Command dotnet-ef -ErrorAction Stop
    Write-Host "dotnet-ef est disponible." -ForegroundColor Green
} catch {
    Write-Host "ERREUR: dotnet-ef n'est pas disponible dans le PATH." -ForegroundColor Red
    Write-Host "Veuillez redémarrer PowerShell et réessayer, ou utilisez la commande complète:" -ForegroundColor Yellow
    Write-Host "dotnet tool run dotnet-ef migrations add InitialCreate" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Tentative avec 'dotnet tool run'..." -ForegroundColor Yellow
}

Write-Host "Création de la migration InitialCreate..." -ForegroundColor Yellow

# Essayer d'abord avec dotnet-ef directement
$migrationCreated = $false
try {
    dotnet ef migrations add InitialCreate 2>&1 | Out-Null
    if ($LASTEXITCODE -eq 0) {
        $migrationCreated = $true
    }
} catch {
    # Si ça échoue, essayer avec dotnet tool run
    Write-Host "Tentative avec 'dotnet tool run'..." -ForegroundColor Yellow
    dotnet tool run dotnet-ef migrations add InitialCreate
    if ($LASTEXITCODE -eq 0) {
        $migrationCreated = $true
    }
}

if (-not $migrationCreated) {
    Write-Host "Erreur lors de la création de la migration." -ForegroundColor Red
    Write-Host "Essayez manuellement avec:" -ForegroundColor Yellow
    Write-Host "  dotnet tool run dotnet-ef migrations add InitialCreate" -ForegroundColor Cyan
    exit 1
}

Write-Host "Migration créée avec succès!" -ForegroundColor Green

Write-Host "Application de la migration à la base de données..." -ForegroundColor Yellow

# Essayer d'abord avec dotnet-ef directement
$dbUpdated = $false
try {
    dotnet ef database update 2>&1 | Out-Null
    if ($LASTEXITCODE -eq 0) {
        $dbUpdated = $true
    }
} catch {
    # Si ça échoue, essayer avec dotnet tool run
    Write-Host "Tentative avec 'dotnet tool run'..." -ForegroundColor Yellow
    dotnet tool run dotnet-ef database update
    if ($LASTEXITCODE -eq 0) {
        $dbUpdated = $true
    }
}

if ($dbUpdated) {
    Write-Host "Base de données créée et migrations appliquées avec succès!" -ForegroundColor Green
    Write-Host "Vous pouvez maintenant lancer l'application avec: dotnet run" -ForegroundColor Green
} else {
    Write-Host "Erreur lors de l'application de la migration." -ForegroundColor Red
    Write-Host "Essayez manuellement avec:" -ForegroundColor Yellow
    Write-Host "  dotnet tool run dotnet-ef database update" -ForegroundColor Cyan
    exit 1
}

