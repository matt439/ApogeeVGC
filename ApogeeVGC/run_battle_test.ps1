# ApogeeVGC Battle Test Runner
# This script runs the battle simulator and captures output for comparison with TypeScript reference

param(
    [int]$TimeoutSeconds = 30,
    [string]$OutputFile = "battle_output.txt"
)

Write-Host "=== ApogeeVGC Battle Test Runner ===" -ForegroundColor Cyan
Write-Host "Timeout: $TimeoutSeconds seconds" -ForegroundColor Gray
Write-Host "Output file: $OutputFile" -ForegroundColor Gray
Write-Host ""

# Build the project first
Write-Host "Building project..." -ForegroundColor Yellow
$buildOutput = dotnet build ApogeeVGC\ApogeeVGC.csproj 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    Write-Host $buildOutput
    exit 1
}
Write-Host "Build successful!" -ForegroundColor Green
Write-Host ""

# Run the battle with timeout
Write-Host "Starting battle simulation..." -ForegroundColor Yellow

# Start the process
$psi = New-Object System.Diagnostics.ProcessStartInfo
$psi.FileName = "dotnet"
$psi.Arguments = "run --project ApogeeVGC\ApogeeVGC.csproj --no-build"
$psi.UseShellExecute = $false
$psi.RedirectStandardOutput = $true
$psi.RedirectStandardError = $true
$psi.CreateNoWindow = $true

$process = New-Object System.Diagnostics.Process
$process.StartInfo = $psi

# Capture output
$outputBuilder = New-Object System.Text.StringBuilder
$errorBuilder = New-Object System.Text.StringBuilder

$outputHandler = {
    if (-not [string]::IsNullOrEmpty($EventArgs.Data)) {
        [void]$Event.MessageData.AppendLine($EventArgs.Data)
   Write-Host $EventArgs.Data
    }
}

$errorHandler = {
    if (-not [string]::IsNullOrEmpty($EventArgs.Data)) {
        [void]$Event.MessageData.AppendLine("STDERR: " + $EventArgs.Data)
        Write-Host $EventArgs.Data -ForegroundColor Red
    }
}

$outputEvent = Register-ObjectEvent -InputObject $process -EventName OutputDataReceived -Action $outputHandler -MessageData $outputBuilder
$errorEvent = Register-ObjectEvent -InputObject $process -EventName ErrorDataReceived -Action $errorHandler -MessageData $errorBuilder

try {
    [void]$process.Start()
    $process.BeginOutputReadLine()
    $process.BeginErrorReadLine()
    
    # Wait with timeout
    $completed = $process.WaitForExit($TimeoutSeconds * 1000)
    
    if (-not $completed) {
        Write-Host ""
        Write-Host "Battle timed out after $TimeoutSeconds seconds. Stopping process..." -ForegroundColor Yellow
        $process.Kill()
 $process.WaitForExit()
    }
    
  # Give events time to finish
    Start-Sleep -Milliseconds 500
    
} finally {
  # Cleanup
    Unregister-Event -SourceIdentifier $outputEvent.Name -ErrorAction SilentlyContinue
    Unregister-Event -SourceIdentifier $errorEvent.Name -ErrorAction SilentlyContinue
    
    # Write output to file
    $output = $outputBuilder.ToString()
    $errors = $errorBuilder.ToString()
    
    $fullOutput = @"
=== ApogeeVGC Battle Output ===
Generated: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
Duration: $TimeoutSeconds seconds (timeout) or until completion

=== STANDARD OUTPUT ===
$output

=== STANDARD ERROR ===
$errors

=== END OF OUTPUT ===
"@
    
    $fullOutput | Out-File -FilePath $OutputFile -Encoding UTF8
    
    Write-Host ""
    Write-Host "Output saved to: $OutputFile" -ForegroundColor Green
    
    # Show statistics
    $lines = ($output -split "`n").Count
    $protocolLines = ($output -split "`n" | Where-Object { $_ -match '^\|' }).Count
    $debugLines = ($output -split "`n" | Where-Object { $_ -match '^\[' }).Count
    
    Write-Host ""
    Write-Host "=== Statistics ===" -ForegroundColor Cyan
    Write-Host "Total lines: $lines" -ForegroundColor Gray
  Write-Host "Protocol messages: $protocolLines" -ForegroundColor Gray
    Write-Host "Debug messages: $debugLines" -ForegroundColor Gray
}

Write-Host ""
Write-Host "Test complete! Check $OutputFile for full output." -ForegroundColor Green
Write-Host "See BATTLE_OUTPUT_COMPARISON.md for detailed analysis." -ForegroundColor Cyan
