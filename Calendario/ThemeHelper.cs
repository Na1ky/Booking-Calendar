using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Calendario
{
    public static class ThemeHelper
    {
        [DllImport("DwmApi")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        public static void ApplyTheme(Form form)
        {
            // Abilita la barra del titolo scura (Immersive Dark Mode)
            if (IsWindows10OrGreater(17763))
            {
                var attribute = DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1;
                if (IsWindows10OrGreater(18985))
                {
                    attribute = DWMWA_USE_IMMERSIVE_DARK_MODE;
                }

                int[] useImmersiveDarkMode = { 1 };
                DwmSetWindowAttribute(form.Handle, attribute, useImmersiveDarkMode, 4);
            }

            // Colori di base e stile finestra
            form.BackColor = Color.FromArgb(10, 13, 28);
            form.ForeColor = Color.FromArgb(226, 232, 255);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterParent;
            form.MaximizeBox = false;
            form.MinimizeBox = false;
            form.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);

            // Stile per tutti i controlli nel form
            ApplyThemeToControls(form.Controls);
        }

        private static void ApplyThemeToControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                if (control is GroupBox gb)
                {
                    gb.ForeColor = Color.FromArgb(124, 58, 237); // Accento viola per i titoli
                }
                else if (control is Button btn)
                {
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.BackColor = Color.FromArgb(124, 58, 237); // Accento viola
                    btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(91, 33, 182); // Hover
                    btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(76, 29, 149); // Press
                    btn.ForeColor = Color.White;
                    btn.Cursor = Cursors.Hand;
                    btn.Padding = new Padding(5);
                }
                else if (control is TextBox tb)
                {
                    tb.BackColor = Color.FromArgb(20, 24, 48); // Card background
                    tb.ForeColor = Color.FromArgb(226, 232, 255);
                    tb.BorderStyle = BorderStyle.FixedSingle;
                }
                else if (control is ComboBox cb)
                {
                    cb.FlatStyle = FlatStyle.Flat;
                    cb.BackColor = Color.FromArgb(20, 24, 48);
                    cb.ForeColor = Color.FromArgb(226, 232, 255);
                }
                else if (control is Label lbl)
                {
                    lbl.ForeColor = Color.FromArgb(226, 232, 255);
                }
                else if (control is DataGridView dgv)
                {
                    dgv.BackgroundColor = Color.FromArgb(10, 13, 28);
                    dgv.ForeColor = Color.Black; // Il contenuto della griglia
                    dgv.DefaultCellStyle.BackColor = Color.FromArgb(20, 24, 48);
                    dgv.DefaultCellStyle.ForeColor = Color.White;
                    dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(14, 17, 35);
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    dgv.EnableHeadersVisualStyles = false;
                }
                
                // Ricorsione per i controlli figli
                if (control.HasChildren)
                {
                    ApplyThemeToControls(control.Controls);
                }
            }
        }

        private static bool IsWindows10OrGreater(int build)
        {
            return Environment.OSVersion.Version.Major >= 10 && Environment.OSVersion.Version.Build >= build;
        }
    }
}
