using Calendario.MODEL;
using MongoDB.Driver;
using System;
using System.Configuration;
using System.Windows.Forms;
using BCrypt.Net;

namespace Calendario.CONTROLLER
{
    public class AuthController
    {
        private readonly IMongoCollection<ClsUtente> _utentiCollection;
        public static ClsUtente CurrentUser { get; private set; }

        public AuthController()
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["MongoDB"]?.ConnectionString ?? "mongodb://localhost:27017";
                var client = new MongoClient(connectionString);
                var database = client.GetDatabase("CalendarioDB");
                _utentiCollection = database.GetCollection<ClsUtente>("Utenti");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore di connessione a MongoDB: " + ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public bool Register(string username, string email, string password)
        {
            if (_utentiCollection == null) return false;

            // Controlla se l'utente o l'email esistono già
            var exist = _utentiCollection.Find(u => u.Username == username || u.Email == email).FirstOrDefault();
            if (exist != null)
            {
                if (exist.Email == email)
                    MessageBox.Show("Email già in uso. Scegline un'altra.", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show("Username già in uso. Scegline un altro.", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            string hash = BCrypt.Net.BCrypt.HashPassword(password);
            var nuovoUtente = new ClsUtente
            {
                Username = username,
                Email = email,
                PasswordHash = hash
            };

            try
            {
                _utentiCollection.InsertOne(nuovoUtente);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore durante la registrazione: " + ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool Login(string usernameOrEmail, string password, bool rememberMe)
        {
            if (_utentiCollection == null) return false;

            var utente = _utentiCollection.Find(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail).FirstOrDefault();
            if (utente == null || !BCrypt.Net.BCrypt.Verify(password, utente.PasswordHash))
            {
                return false; // Credenziali errate
            }

            CurrentUser = utente;

            if (rememberMe)
            {
                // Genera token
                string token = Guid.NewGuid().ToString("N");
                DateTime expiry = DateTime.Now.AddDays(30);

                var filter = Builders<ClsUtente>.Filter.Eq(u => u.Id, utente.Id);
                var update = Builders<ClsUtente>.Update
                    .Set(u => u.SessionToken, token)
                    .Set(u => u.TokenExpiry, expiry);
                
                _utentiCollection.UpdateOne(filter, update);

                // Salva localmente nei settings
                Properties.Settings.Default.AuthToken = token;
                Properties.Settings.Default.Save();
            }
            else
            {
                // Pulisce vecchio token se non vuole esser ricordato
                PulisciTokenLocale(utente.Id);
            }

            return true;
        }

        public bool CheckSessioneValida()
        {
            string savedToken = Properties.Settings.Default.AuthToken;
            if (string.IsNullOrEmpty(savedToken)) return false;

            if (_utentiCollection == null) return false;

            var utente = _utentiCollection.Find(u => u.SessionToken == savedToken).FirstOrDefault();
            
            if (utente != null && utente.TokenExpiry.HasValue && utente.TokenExpiry.Value > DateTime.Now)
            {
                CurrentUser = utente;
                return true; // Token valido
            }
            
            // Token scaduto o non valido
            if (utente != null) PulisciTokenLocale(utente.Id);
            return false;
        }

        public void Logout()
        {
            CurrentUser = null;
            string savedToken = Properties.Settings.Default.AuthToken;
            if (!string.IsNullOrEmpty(savedToken) && _utentiCollection != null)
            {
                var utente = _utentiCollection.Find(u => u.SessionToken == savedToken).FirstOrDefault();
                if (utente != null)
                {
                    PulisciTokenLocale(utente.Id);
                }
            }
            
            Properties.Settings.Default.AuthToken = string.Empty;
            Properties.Settings.Default.Save();
        }

        private void PulisciTokenLocale(string id)
        {
            var filter = Builders<ClsUtente>.Filter.Eq(u => u.Id, id);
            var update = Builders<ClsUtente>.Update
                .Set(u => u.SessionToken, (string)null)
                .Set(u => u.TokenExpiry, (DateTime?)null);
            _utentiCollection.UpdateOne(filter, update);

            Properties.Settings.Default.AuthToken = string.Empty;
            Properties.Settings.Default.Save();
        }

        public ClsUtente GetCurrentUser()
        {
            string savedToken = Properties.Settings.Default.AuthToken;
            if (string.IsNullOrEmpty(savedToken)) return null;
            if (_utentiCollection == null) return null;
            return _utentiCollection.Find(u => u.SessionToken == savedToken).FirstOrDefault();
        }
    }
}
