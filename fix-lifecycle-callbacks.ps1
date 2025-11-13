# Fix script for Battle.Lifecycle.cs callback invocations
# This script searches for the callback invocations and provides the replacements needed

Write-Host "Callback Invocation Fixes for Battle.Lifecycle.cs" -ForegroundColor Cyan
Write-Host ""

$filePath = "C:\VSProjects\ApogeeVGC\ApogeeVGC\Sim\BattleClasses\Battle.Lifecycle.cs"
$content = Get-Content -Path $filePath -Raw

# Pattern 1: BeforeTurnCallback
# OLD: btmAction.Move.BeforeTurnCallback(this, btmAction.Pokemon, target, btmAction.Move);
# NEW: Battle.InvokeCallback<VoidResult>(btmAction.Move.BeforeTurnCallback, this, btmAction.Pokemon, target, btmAction.Move);

$pattern1 = 'btmAction\.Move\.BeforeTurnCallback\(this, btmAction\.Pokemon, target,\s+btmAction\.Move\);'
$replacement1 = @'
this.InvokeCallback<object>(
 btmAction.Move.BeforeTurnCallback,
     this,
        btmAction.Pokemon,
  target,
         btmAction.Move);
'@

if ($content -match $pattern1) {
    Write-Host "Found BeforeTurnCallback pattern" -ForegroundColor Yellow
    $content = $content -replace $pattern1, $replacement1
} else {
    Write-Host "BeforeTurnCallback pattern not found - checking alternate patterns..." -ForegroundColor Red
}

# Pattern 2: PriorityChargeCallback
# OLD: pcmAction.Move.PriorityChargeCallback(this, pcmAction.Pokemon);
# NEW: Battle.InvokeCallback<VoidResult>(pcmAction.Move.PriorityChargeCallback, this, pcmAction.Pokemon);

$pattern2 = 'pcmAction\.Move\.PriorityChargeCallback\(this, pcmAction\.Pokemon\);'
$replacement2 = @'
this.InvokeCallback<object>(
      pcmAction.Move.PriorityChargeCallback,
             this,
       pcmAction.Pokemon);
'@

if ($content -match $pattern2) {
  Write-Host "Found PriorityChargeCallback pattern" -ForegroundColor Yellow
    $content = $content -replace $pattern2, $replacement2
} else {
 Write-Host "PriorityChargeCallback pattern not found - checking alternate patterns..." -ForegroundColor Red
}

# Write the updated content
Set-Content -Path $filePath -Value $content -NoNewline

Write-Host ""
Write-Host "File updated successfully!" -ForegroundColor Green
Write-Host "Please review the changes and test the build." -ForegroundColor Cyan
