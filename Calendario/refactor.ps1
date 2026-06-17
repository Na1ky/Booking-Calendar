$files = Get-ChildItem -Path .\View -Filter *.cs | Where-Object { $_.Name -notmatch '\.Designer\.cs$' }
foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $content = $content -replace 'public partial class (.*?) : Form', "using MaterialSkin.Controls;`r`n    public partial class `$1 : MaterialForm"
    Set-Content -Path $file.FullName -Value $content
}
