<#
.SYNOPSIS
  Publish a Unity WebGL build folder to the repository `docs/` for GitHub Pages.

.DESCRIPTION
  Copies files from a local WebGL build output into `docs/`, removes any
  precompressed files (`*.br`, `*.gz`), ensures `docs/.nojekyll` exists,
  and optionally commits and pushes the changes to the current git remote.

.PARAMETER Source
  Path to the folder that contains the built WebGL output (must contain index.html).

.PARAMETER DocsDir
  Destination docs directory (default: docs).

.PARAMETER AdjustExtensions
  If true, replace references to `.js.br`, `.data.br`, `.wasm.br` in `index.html`.

.PARAMETER Commit
  If true, run `git add` and `git commit` for the docs changes.

.PARAMETER Push
  If true, run `git push` after a successful commit.

.PARAMETER TestServer
  If true, starts a local http server to test the deployed `docs/` folder.

.PARAMETER Port
  Port to run the test server on (default 8001).

EXAMPLE
  .\publish-webgl.ps1 -Source "C:\path\to\WebGLBuild" -Commit -Push

#>

param(
    [Parameter(Mandatory=$false)] [string]$Source = "Builds\WebBuild",
    [Parameter(Mandatory=$false)] [string]$DocsDir = "docs",
    [Parameter(Mandatory=$false)] [switch]$AdjustExtensions,
    [Parameter(Mandatory=$false)] [switch]$Commit,
    [Parameter(Mandatory=$false)] [switch]$Push,
    [Parameter(Mandatory=$false)] [switch]$TestServer,
    [Parameter(Mandatory=$false)] [int]$Port = 8001
)

function Abort($msg){ Write-Error $msg; exit 1 }

$cwd = Get-Location
$sourcePath = Resolve-Path -LiteralPath $Source -ErrorAction SilentlyContinue
if (-not $sourcePath) { Abort "Source path '$Source' not found. Build your WebGL output first and supply its path." }
$sourcePath = $sourcePath.ProviderPath

if (-not (Test-Path (Join-Path $sourcePath 'index.html'))) {
    Write-Warning "Source does not contain index.html. Are you pointing at the WebGL output folder?"
}

Write-Output "Publishing WebGL from: $sourcePath -> $DocsDir"

if (-not (Test-Path $DocsDir)) { New-Item -ItemType Directory -Path $DocsDir | Out-Null }

# Remove contents of docs (but keep the folder)
Get-ChildItem -Path $DocsDir -Force | ForEach-Object {
    Remove-Item -LiteralPath $_.FullName -Recurse -Force -ErrorAction SilentlyContinue
}

# Copy files
Copy-Item -Path (Join-Path $sourcePath '*') -Destination $DocsDir -Recurse -Force

# Remove any pre-compressed files that might confuse GitHub Pages
Get-ChildItem -Path $DocsDir -Include *.br,*.gz -Recurse -Force -ErrorAction SilentlyContinue | ForEach-Object {
    Write-Output "Removing compressed file: $($_.FullName)"
    Remove-Item -LiteralPath $_.FullName -Force -ErrorAction SilentlyContinue
}

# Ensure .nojekyll
New-Item -Path (Join-Path $DocsDir '.nojekyll') -ItemType File -Force | Out-Null

if ($AdjustExtensions) {
    $indexFile = Join-Path $DocsDir 'index.html'
    if (Test-Path $indexFile) {
        (Get-Content $indexFile) -replace '\.js\.br','.js' -replace '\.data\.br','.data' -replace '\.wasm\.br','.wasm' | Set-Content $indexFile
        Write-Output "Patched index.html to remove .br extensions"
    }
}

if ($Commit) {
    Push-Location $cwd
    try {
        git add $DocsDir -A
        $commitMsg = "Publish WebGL build to docs/ ($(Get-Date -Format o))"
        $status = git commit -m $commitMsg 2>&1
        if ($LASTEXITCODE -ne 0) { Write-Output "No commit created (git output): $status" } else { Write-Output "Committed docs/" }
        if ($Push -and $LASTEXITCODE -eq 0) {
            git push
        }
    } catch {
        Write-Warning "Git commit/push failed: $_"
    } finally { Pop-Location }
}

if ($TestServer) {
    Write-Output "Starting local HTTP server at http://localhost:$Port to test docs/ (Ctrl+C to stop)"
    python -m http.server $Port --directory $DocsDir
}

Write-Output "Done."
