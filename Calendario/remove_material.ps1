$files = Get-ChildItem -Path ".\" -Filter *.cs -Recurse

foreach ($file in $files) {
    if ($file.Name -eq "ThemeHelper.cs") { continue }
    
    $content = Get-Content $file.FullName -Raw

    # Rimuovi using MaterialSkin
    $content = $content -replace 'using MaterialSkin;', ''
    $content = $content -replace 'using MaterialSkin\.Controls;', ''

    # Rimuovi eredità MaterialForm
    $content = $content -replace 'public partial class (.*?) : MaterialForm', 'public partial class $1 : Form'

    # Ripristina bottoni e label nel designer
    $content = $content -replace 'MaterialSkin\.Controls\.MaterialButton', 'System.Windows.Forms.Button'
    $content = $content -replace 'MaterialSkin\.Controls\.MaterialLabel', 'System.Windows.Forms.Label'
    $content = $content -replace 'MaterialSkin\.Controls\.MaterialTextBox', 'System.Windows.Forms.TextBox'
    
    # Rimuovi l'inizializzazione del ThemeManager se è rimasta nei costruttori (eccetto FrmMain che l'ho già tolta)
    if ($file.Name -notmatch "FrmMain.cs" -and $file.Name -notmatch "Designer") {
        $content = $content -replace 'MaterialSkinManager\.Instance.*?Theme = MaterialSkinManager\.Themes\.DARK;', ''
    }

    Set-Content -Path $file.FullName -Value $content -NoNewline
}

Write-Host "MaterialSkin removed from all files."
