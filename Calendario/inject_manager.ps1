$files = Get-ChildItem -Path .\View -Filter *.cs | Where-Object { $_.Name -notmatch '\.Designer\.cs$' }
foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $injection = "InitializeComponent();`r`n            MaterialSkin.MaterialSkinManager.Instance.AddFormToManage(this);`r`n            MaterialSkin.MaterialSkinManager.Instance.Theme = MaterialSkin.MaterialSkinManager.Themes.DARK;"
    $content = $content -replace 'InitializeComponent\(\);', $injection
    Set-Content -Path $file.FullName -Value $content
}
