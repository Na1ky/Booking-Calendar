using Calendario.MODEL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calendario.CONTROLLER
{
    public class PrenotazioneController
    {
        List<ClsPrenotazione> lstPrenotazioni { get; set; }
        public List<ClsPrenotazione> getPrenotazione { get { return lstPrenotazioni; } }

        public PrenotazioneController()
        {
            lstPrenotazioni = new List<ClsPrenotazione>();
        }

        public void CaricaDaFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                string jsonData = File.ReadAllText(fileName);
                if (!string.IsNullOrWhiteSpace(jsonData))
                    lstPrenotazioni = JsonConvert.DeserializeObject<List<ClsPrenotazione>>(jsonData) ?? new List<ClsPrenotazione>();
            }
            else
            {
                // Se il file non esiste (primo avvio), inizializza lista vuota invece di crashare
                lstPrenotazioni = new List<ClsPrenotazione>();
            }
        }

        public void SalvaSuJson(string nomeFIle)
        {
            string jsonData = JsonConvert.SerializeObject(lstPrenotazioni, Formatting.Indented);
            File.WriteAllText(nomeFIle, jsonData);
        }

        public List<ClsPrenotazione> GetPrenotazioniPerMese(int year, int month)
        {
            if (lstPrenotazioni == null || !lstPrenotazioni.Any())
            {
                return new List<ClsPrenotazione>(); // Restituisce una lista vuota se non ci sono prenotazioni
            }

            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            DateTime lastDayOfMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            return lstPrenotazioni.Where(p =>
                p.DataInizio <= lastDayOfMonth && p.DataFine >= firstDayOfMonth
            ).ToList();
        }

        public void AddPrenotazione(ClsPrenotazione p)
        {
            lstPrenotazioni.Add(p);
        }

        internal void stampaInformazioniPrenotazione(string id)
        {
            MessageBox.Show(lstPrenotazioni.Find(x => x.Id == id).ToString(), "INFORMAZIONI", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        internal void ModificaPrenotazioni(string idPrenotazione, string nome, string cognome, double versamento, bool tipologia, DateTime dataInizio, DateTime dataFine, double spese, double acconto)
        {
            ClsPrenotazione prenotazioneDaModificare = lstPrenotazioni.Find(x => x.Id == idPrenotazione);

            // Controllo se la prenotazione esiste
            if (prenotazioneDaModificare == null)
            {
                MessageBox.Show("Errore: Prenotazione non trovata.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Aggiorna i dati della prenotazione
            prenotazioneDaModificare.Nome = nome;
            prenotazioneDaModificare.Cognome = cognome;
            prenotazioneDaModificare.Versamento = versamento;
            prenotazioneDaModificare.TipoPrenotazione = tipologia;
            prenotazioneDaModificare.DataInizio = dataInizio;
            prenotazioneDaModificare.DataFine = dataFine;
            prenotazioneDaModificare.SpesePulizia = spese;
            prenotazioneDaModificare.Acconto = acconto;

            // Prova a salvare su JSON
            try
            {
                SalvaSuJson(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "prenotazioni.json"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore durante il salvataggio dei dati: " + ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        internal void eliminaPrenotazione(string id)
        {
            lstPrenotazioni.RemoveAll(x => x.Id == id);
            SalvaSuJson(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "prenotazioni.json"));
        }

        public List<ClsPrenotazione> GetPrenotazioniPerAnno(int anno)
        {
            try
            {
                // Filtra le prenotazioni per l'anno selezionato
                var prenotazioniAnno = lstPrenotazioni.Where(p => p.DataInizio.Year == anno).ToList();

                // Se non ci sono prenotazioni, ritorna una lista vuota
                return prenotazioniAnno;
            }
            catch (Exception ex)
            {
                // Gestione degli errori: mostra il messaggio di errore
                throw new Exception("Errore nel recupero delle prenotazioni: " + ex.Message);
            }
        }

        internal void eliminaTutteLePrenotazioni()
        {
            lstPrenotazioni.Clear();
            SalvaSuJson(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "prenotazioni.json"));
        }
    }
}
