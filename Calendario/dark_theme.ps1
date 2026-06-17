$file = ".\FrmMain.cs"
$content = Get-Content $file -Raw

# Replace Colors in FrmMain.cs
$content = $content -replace 'Color\.LightBlue', 'Color.FromArgb(50, 50, 50)'
$content = $content -replace 'Color\.LightGray', 'Color.FromArgb(45, 45, 48)'
$content = $content -replace 'cell\.Style\.BackColor = Color\.White', 'cell.Style.BackColor = Color.FromArgb(30, 30, 30)'
$content = $content -replace 'cell\.Style\.ForeColor = Color\.Black', 'cell.Style.ForeColor = Color.White'
$content = $content -replace 'dgvCalendario\.Rows\[i\]\.Cells\[j\]\.Style\.BackColor = Color\.White', 'dgvCalendario.Rows[i].Cells[j].Style.BackColor = Color.FromArgb(30, 30, 30)'
$content = $content -replace 'dgvCalendario\.Rows\[i\]\.Cells\[j\]\.Style\.ForeColor = Color\.Black', 'dgvCalendario.Rows[i].Cells[j].Style.ForeColor = Color.White'
$content = $content -replace 'Color\.Gray', 'Color.FromArgb(45, 45, 48)'

Set-Content -Path $file -Value $content
