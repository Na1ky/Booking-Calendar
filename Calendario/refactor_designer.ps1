$files = Get-ChildItem -Path .\View -Filter *.Designer.cs
foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $content = $content -replace 'System\.Windows\.Forms\.Button', 'MaterialSkin.Controls.MaterialButton'
    $content = $content -replace 'System\.Windows\.Forms\.Label', 'MaterialSkin.Controls.MaterialLabel'
    $content = $content -replace '(?m)^\s*this\..*?\.UseVisualStyleBackColor = (true|false);\s*$', ''
    $content = $content -replace '(?m)^\s*this\..*?\.BackColor = .*?;\s*$', ''
    $content = $content -replace '(?m)^\s*this\..*?\.Font = .*?;\s*$', ''
    $content = $content -replace '(?m)^\s*this\..*?\.ForeColor = .*?;\s*$', ''
    Set-Content -Path $file.FullName -Value $content
}
