#Requires -Version 5.1
param(
    [string]$OutputDir = "$PSScriptRoot\..\src\EDEA\Resources"
)

$ErrorActionPreference = "Stop"

$zipUrl = "https://github.com/EDCD/coriolis-data/archive/refs/heads/master.zip"
$tempDir = "$env:TEMP\edea_coriolis_data"
$zipFile = "$env:TEMP\edea_coriolis_data.zip"

if (Test-Path $tempDir) { Remove-Item -Recurse -Force $tempDir }
New-Item -ItemType Directory -Path $tempDir | Out-Null

Write-Host "Downloading EDCD/coriolis-data..."
Invoke-RestMethod -Uri $zipUrl -OutFile $zipFile

Write-Host "Extracting..."
Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::ExtractToDirectory($zipFile, $tempDir)

$coriolisRoot = (Get-ChildItem -Path $tempDir -Directory | Select-Object -First 1).FullName

# --- Generate mc.dat (FSD + Guardian FSD Booster modules) ---
$mc = [ordered]@{}

# FSDs
$fsdFile = Join-Path $coriolisRoot "modules/standard/frame_shift_drive.json"
$fsd = Get-Content $fsdFile -Raw | ConvertFrom-Json
foreach ($m in $fsd.fsd) {
    if ($m.symbol -match "Missing") { continue }
    $id = $m.symbol.ToLower()
    $isSco = ($m.symbol -match "Overcharge") -or ($m.name -match "SCO")
    $type = if ($isSco) { "cfsdo" } else { "cfsd" }
    $name = if ($m.name) { $m.name } else { $m.ukName }
    if ($name -eq "FSD") { $name = "Frame Shift Drive" }
    $mc[$id] = [ordered]@{
        FuelPower      = $m.fuelpower
        FuelMultiplier = $m.fuelmul
        OptimalMass    = $m.optmass
        MaxFuelPerJump = $m.maxfuel
        Id             = $id
        Type           = $type
        Name           = $name
        Class          = [string]$m.class
        Rating         = $m.rating
    }
}

# Guardian FSD Boosters
$gfsbFile = Join-Path $coriolisRoot "modules/internal/guardian_fsd_booster.json"
$gfsb = Get-Content $gfsbFile -Raw | ConvertFrom-Json
foreach ($m in $gfsb.gfsb) {
    $id = $m.symbol.ToLower()
    $mc[$id] = [ordered]@{
        JumpBoost = $m.jumpboost
        Id        = $id
        Type      = "ifsdb"
        Name      = "Guardian Frame Shift Drive Booster"
        Class     = [string]$m.class
        Rating    = $m.rating
    }
}

$mcJson = $mc | ConvertTo-Json -Compress
$mcBase64 = [Convert]::ToBase64String([Text.Encoding]::UTF8.GetBytes($mcJson))
$mcBase64 | Set-Content (Join-Path $OutputDir "mc.dat") -NoNewline -Encoding ASCII

# Cleanup
Remove-Item $zipFile -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force $tempDir -ErrorAction SilentlyContinue

Write-Host "Generated mc.dat in $OutputDir"
