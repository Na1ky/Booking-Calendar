$files = Get-ChildItem -Path ".\View" -Filter *.cs | Where-Object { $_.Name -notmatch "Designer" }

foreach ($file in $files) {
    $content = Get-Content $file.FullName
    $newContent = @()
    foreach ($line in $content) {
        if ($line -notmatch "MaterialSkinManager") {
            $newContent += $line
        }
    }
    Set-Content -Path $file.FullName -Value $newContent
}
Write-Host "Removed MaterialSkinManager lines from View forms."
