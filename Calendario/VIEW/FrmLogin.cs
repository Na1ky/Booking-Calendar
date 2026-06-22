using Calendario.CONTROLLER;
using System;
using System.Windows.Forms;

namespace Calendario.VIEW
{
    public partial class FrmLogin : Form
    {
        private readonly AuthController _authController;

        public FrmLogin()
        {
            InitializeComponent();
            _authController = new AuthController();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            bool rememberMe = chkRememberMe.Checked;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Inserisci username e password.", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_authController.Login(username, password, rememberMe))
            {
                // Login con successo, apro form main
                this.Hide();
                var main = new FrmMain();
                main.FormClosed += (s, args) => this.Close();
                main.Show();
            }
            else
            {
                MessageBox.Show("Credenziali non valide.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lblRegister_Click(object sender, EventArgs e)
        {
            var frmReg = new FrmRegistrazione();
            this.Hide();
            frmReg.FormClosed += (s, args) => this.Show();
            frmReg.Show();
        }

        private void lblRemember_Click(object sender, EventArgs e)
        {
            chkRememberMe.Checked = !chkRememberMe.Checked;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
