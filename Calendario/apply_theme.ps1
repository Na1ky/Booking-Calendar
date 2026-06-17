$files = Get-ChildItem -Path ".\View" -Filter *.cs | Where-Object { $_.Name -notmatch "Designer" }

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw

    if ($content -notmatch "ThemeHelper.ApplyTheme") {
        # Trova InitializeComponent(); e aggiungi ThemeHelper.ApplyTheme(this); subito dopo
        $content = $content -replace "InitializeComponent\(\);", "InitializeComponent();`r`n            ThemeHelper.ApplyTheme(this);"
        Set-Content -Path $file.FullName -Value $content -NoNewline
    }
}
Write-Host "ThemeHelper applied to View forms."
