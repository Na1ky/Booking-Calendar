using Calendario.MODEL;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows.Forms;

namespace Calendario.CONTROLLER
{
    public class PrenotazioneController
    {
        private readonly IMongoCollection<ClsPrenotazione> _prenotazioniCollection;

        public List<ClsPrenotazione> getPrenotazione 
        { 
            get 
            {
                if (_prenotazioniCollection == null) return new List<ClsPrenotazione>();
                return _prenotazioniCollection.Find(_ => true).ToList(); 
            } 
        }

        public PrenotazioneController()
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["MongoDB"]?.ConnectionString ?? "mongodb://localhost:27017";
                var client = new MongoClient(connectionString);
                var database = client.GetDatabase("CalendarioDB");
                _prenotazioniCollection = database.GetCollection<ClsPrenotazione>("Prenotazioni");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore di connessione a MongoDB: " + ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void CaricaDaFile(string fileName)
        {
            // Metodo obsoleto: Mantenuto per compatibilità con eventuali vecchie chiamate, 
            // ma ora i dati sono caricati direttamente da MongoDB tramite getPrenotazione
        }

        public void SalvaSuJson(string nomeFIle)
        {
            // Metodo obsoleto
        }

        public List<ClsPrenotazione> GetPrenotazioniPerMese(int year, int month)
        {
            if (_prenotazioniCollection == null) return new List<ClsPrenotazione>();

            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            DateTime lastDayOfMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            var builder = Builders<ClsPrenotazione>.Filter;
            var filter = builder.Lte(p => p.DataInizio, lastDayOfMonth) & builder.Gte(p => p.DataFine, firstDayOfMonth);

            return _prenotazioniCollection.Find(filter).ToList();
        }

        public void AddPrenotazione(ClsPrenotazione p)
        {
            if (_prenotazioniCollection != null)
            {
                var filter = Builders<ClsPrenotazione>.Filter.Where(x => x.DataInizio < p.DataFine && x.DataFine > p.DataInizio);
                var sovrapposizione = _prenotazioniCollection.Find(filter).FirstOrDefault();
                if (sovrapposizione != null)
                {
                    throw new ArgumentException($"Le date si sovrappongono con la prenotazione di {sovrapposizione.Nome} {sovrapposizione.Cognome}.");
                }
            }

            if (string.IsNullOrEmpty(p.Id))
            {
                p.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            }
            _prenotazioniCollection?.InsertOne(p);
        }

        internal void stampaInformazioniPrenotazione(string id)
        {
            var prenotazione = _prenotazioniCollection?.Find(x => x.Id == id).FirstOrDefault();
            if (prenotazione != null)
            {
                MessageBox.Show(prenotazione.ToString(), "INFORMAZIONI", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        internal void ModificaPrenotazioni(string idPrenotazione, string nome, string cognome, double versamento, bool tipologia, DateTime dataInizio, DateTime dataFine, double spese, double acconto, bool famigliaDominici = false, System.Drawing.Color? colore = null)
        {
            if (_prenotazioniCollection != null)
            {
                var filterOverlap = Builders<ClsPrenotazione>.Filter.Where(x => x.Id != idPrenotazione && x.DataInizio < dataFine && x.DataFine > dataInizio);
                var sovrapposizione = _prenotazioniCollection.Find(filterOverlap).FirstOrDefault();
                if (sovrapposizione != null)
                {
                    throw new ArgumentException($"Modifica non possibile: le nuove date si sovrappongono con la prenotazione di {sovrapposizione.Nome} {sovrapposizione.Cognome}.");
                }
            }

            var colorToSave = colore ?? (tipologia ? System.Drawing.Color.LightGreen : System.Drawing.Color.Yellow);
            var colorHex = System.Drawing.ColorTranslator.ToHtml(colorToSave);

            var filter = Builders<ClsPrenotazione>.Filter.Eq(x => x.Id, idPrenotazione);
            var update = Builders<ClsPrenotazione>.Update
                .Set(p => p.Nome, nome)
                .Set(p => p.Cognome, cognome)
                .Set(p => p.Versamento, versamento)
                .Set(p => p.TipoPrenotazione, tipologia)
                .Set(p => p.DataInizio, dataInizio)
                .Set(p => p.DataFine, dataFine)
                .Set(p => p.SpesePulizia, spese)
                .Set(p => p.Acconto, acconto)
                .Set(p => p.FamigliaDominici, famigliaDominici)
                .Set(p => p.ColoreCellaHex, colorHex);

            try
            {
                _prenotazioniCollection?.UpdateOne(filter, update);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore durante il salvataggio dei dati in MongoDB: " + ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        internal void eliminaPrenotazione(string id)
        {
            var filter = Builders<ClsPrenotazione>.Filter.Eq(x => x.Id, id);
            _prenotazioniCollection?.DeleteOne(filter);
        }

        public List<ClsPrenotazione> GetPrenotazioniPerAnno(int anno)
        {
            try
            {
                if (_prenotazioniCollection == null) return new List<ClsPrenotazione>();

                // Filtra le prenotazioni per l'anno selezionato (dove l'anno di DataInizio è uguale all'anno richiesto)
                // Usiamo un LINQ ToList per semplicità, oppure un filtro basato su DateTime
                DateTime inizioAnno = new DateTime(anno, 1, 1);
                DateTime fineAnno = new DateTime(anno, 12, 31, 23, 59, 59);

                var filter = Builders<ClsPrenotazione>.Filter.Gte(p => p.DataInizio, inizioAnno) & Builders<ClsPrenotazione>.Filter.Lte(p => p.DataInizio, fineAnno);
                return _prenotazioniCollection.Find(filter).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nel recupero delle prenotazioni: " + ex.Message);
            }
        }

        internal void eliminaTutteLePrenotazioni()
        {
            _prenotazioniCollection?.DeleteMany(Builders<ClsPrenotazione>.Filter.Empty);
        }
    }
}
