# Script to add nullability validation to EventHandlerInfo concrete classes
# This script adds:
# 1. ParameterNullability array
# 2. ReturnTypeNullable property
# 3. ValidateConfiguration() call

$handlersPath = "C:\VSProjects\ApogeeVGC\ApogeeVGC\Sim\Events\Handlers"
$files = Get-ChildItem -Path $handlersPath -Recurse -Filter "*EventInfo.cs"

Write-Host "Found $($files.Count) EventInfo files"
Write-Host "This script will add nullability validation to each file."
Write-Host ""

$updatedCount = 0
$skippedCount = 0
$errorCount = 0

foreach ($file in $files) {
    try {
    $content = Get-Content -Path $file.FullName -Raw
   
        # Skip if already has ValidateConfiguration
        if ($content -match "ValidateConfiguration\(\)") {
   Write-Host "SKIP: $($file.Name) - Already updated" -ForegroundColor Yellow
            $skippedCount++
  continue
        }
   
        # Skip if doesn't have ExpectedParameterTypes (not a standard EventHandlerInfo)
        if ($content -notmatch "ExpectedParameterTypes\s*=") {
 Write-Host "SKIP: $($file.Name) - No ExpectedParameterTypes found" -ForegroundColor Yellow
          $skippedCount++
         continue
   }
  
  # Find the constructor closing brace
        $pattern = '(ExpectedReturnType\s*=\s*typeof\([^)]+\);)'
      
        if ($content -match $pattern) {
            # Count parameters from ExpectedParameterTypes
   if ($content -match 'ExpectedParameterTypes\s*=\s*(?:new\[\]\s*{|\[)([^\]]+)[\]}]') {
     $paramsSection = $matches[1]
      $paramCount = ([regex]::Matches($paramsSection, 'typeof\(')).Count
      
                # Create nullability array (assuming all non-nullable for now)
    $nullabilityArray = "new[] { " + (("false, " * $paramCount).TrimEnd(", ")) + " }"
              
     # Determine if return type is nullable
       $returnTypeNullable = "false"
        if ($content -match 'ExpectedReturnType\s*=\s*typeof\(([^)]+)\)') {
 $returnType = $matches[1]
    # Reference types that might be nullable
        if ($returnType -match '\?' -or $returnType -eq "object") {
     $returnTypeNullable = "true"
                    }
}
           
    # Build the new code to insert
   $insertCode = @"

        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = $nullabilityArray;
        ReturnTypeNullable = $returnTypeNullable;
    
    // Validate configuration
        ValidateConfiguration();
"@
            
         # Insert before the closing brace of the constructor
     $content = $content -replace '(ExpectedReturnType\s*=\s*typeof\([^)]+\);)', "`$1$insertCode"
          
                # Write back
    Set-Content -Path $file.FullName -Value $content -NoNewline
                
     Write-Host "UPDATED: $($file.Name) - Added validation ($paramCount params)" -ForegroundColor Green
       $updatedCount++
}
         else {
    Write-Host "ERROR: $($file.Name) - Could not parse ExpectedParameterTypes" -ForegroundColor Red
     $errorCount++
     }
        }
   else {
            Write-Host "SKIP: $($file.Name) - Could not find ExpectedReturnType" -ForegroundColor Yellow
            $skippedCount++
        }
    }
    catch {
 Write-Host "ERROR: $($file.Name) - $_" -ForegroundColor Red
        $errorCount++
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Summary:" -ForegroundColor Cyan
Write-Host "  Updated: $updatedCount" -ForegroundColor Green
Write-Host "  Skipped: $skippedCount" -ForegroundColor Yellow
Write-Host "  Errors:  $errorCount" -ForegroundColor Red
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "1. Review the changes with git diff"
Write-Host "2. Manually adjust nullable parameters where needed"
Write-Host "3. Build the project to verify all changes compile"
