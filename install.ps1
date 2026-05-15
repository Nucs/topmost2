[CmdletBinding()]
param(
    [string]$Repo = "Nucs/topmost2",
    [string]$Version = "latest",
    [string]$InstallDir = (Join-Path $env:LOCALAPPDATA "Programs\TopMost2"),
    [switch]$NoStartup,
    [switch]$NoLaunch
)

$ErrorActionPreference = "Stop"

function Write-Step {
    param([string]$Message)
    Write-Host "TopMost2: $Message"
}

function Get-ReleaseAsset {
    param(
        [string]$Repository,
        [string]$ReleaseVersion,
        [string]$AssetName
    )

    $headers = @{
        "Accept" = "application/vnd.github+json"
        "User-Agent" = "TopMost2 installer"
    }

    if ($ReleaseVersion -eq "latest") {
        $releaseUrl = "https://api.github.com/repos/$Repository/releases/latest"
    }
    else {
        $releaseUrl = "https://api.github.com/repos/$Repository/releases/tags/$ReleaseVersion"
    }

    $release = Invoke-RestMethod -Uri $releaseUrl -Headers $headers
    $asset = $release.assets | Where-Object { $_.name -eq $AssetName } | Select-Object -First 1

    if (-not $asset) {
        throw "Could not find $AssetName in release $($release.tag_name)."
    }

    return $asset
}

function Get-SourceExe {
    $scriptDir = if ($PSScriptRoot) { $PSScriptRoot } else { (Get-Location).Path }
    $localExe = Join-Path $scriptDir "TopMost2.exe"

    if (Test-Path -LiteralPath $localExe) {
        Write-Step "using local TopMost2.exe"
        return $localExe
    }

    Write-Step "downloading TopMost2.exe from GitHub release"
    $asset = Get-ReleaseAsset -Repository $Repo -ReleaseVersion $Version -AssetName "TopMost2.exe"
    $downloadPath = Join-Path ([IO.Path]::GetTempPath()) ("TopMost2-" + [guid]::NewGuid() + ".exe")
    Invoke-WebRequest -Uri $asset.browser_download_url -OutFile $downloadPath
    return $downloadPath
}

$sourceExe = Get-SourceExe
$installExe = Join-Path $InstallDir "TopMost2.exe"

Write-Step "installing to $installExe"
New-Item -ItemType Directory -Force -Path $InstallDir | Out-Null

Get-CimInstance Win32_Process -Filter "Name = 'TopMost2.exe'" |
    Where-Object { $_.ExecutablePath -eq $installExe } |
    ForEach-Object {
        Write-Step "stopping running process $($_.ProcessId)"
        Stop-Process -Id $_.ProcessId -Force
    }

Copy-Item -LiteralPath $sourceExe -Destination $installExe -Force

if (Get-Command Unblock-File -ErrorAction SilentlyContinue) {
    Unblock-File -LiteralPath $installExe
}

if (-not $NoStartup) {
    $startupDir = [Environment]::GetFolderPath("Startup")
    $shortcutPath = Join-Path $startupDir "TopMost2.lnk"
    Write-Step "creating startup shortcut"

    $wsh = New-Object -ComObject WScript.Shell
    $shortcut = $wsh.CreateShortcut($shortcutPath)
    $shortcut.TargetPath = $installExe
    $shortcut.Arguments = ""
    $shortcut.WorkingDirectory = $InstallDir
    $shortcut.IconLocation = $installExe
    $shortcut.Description = "Start TopMost2 at Windows startup"
    $shortcut.Save()
}

if (-not $NoLaunch) {
    Write-Step "starting TopMost2"
    Start-Process -FilePath $installExe -WorkingDirectory $InstallDir -WindowStyle Hidden
}

$hash = (Get-FileHash -Algorithm SHA256 -LiteralPath $installExe).Hash
Write-Step "installed successfully"
Write-Host "Path:   $installExe"
Write-Host "SHA256: $hash"
