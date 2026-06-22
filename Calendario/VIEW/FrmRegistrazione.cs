using Calendario.CONTROLLER;
using System;
using System.Windows.Forms;

namespace Calendario.VIEW
{
    public partial class FrmRegistrazione : Form
    {
        private readonly AuthController _authController;

        public FrmRegistrazione()
        {
            InitializeComponent();
            _authController = new AuthController();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;
            string confPassword = txtConfirmPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Compila tutti i campi.", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!email.Contains("@") || !email.Contains("."))
            {
                MessageBox.Show("Inserisci un indirizzo email valido.", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("La password deve contenere almeno 6 caratteri.", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password != confPassword)
            {
                MessageBox.Show("Le password non coincidono.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (_authController.Register(username, email, password))
            {
                MessageBox.Show("Registrazione completata con successo! Ora puoi accedere.", "Successo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close(); // Torna al login
            }
        }

        private void lblLogin_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
