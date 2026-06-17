using System;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Calendario.MODEL
{
    public class ClsPrenotazione
    {
        private string id;
        private string nome;
        private string cognome;
        private double versamento;
        private DateTime dataInizio;
        private DateTime dataFine;
        private bool tipoPrenotazione;
        private Color coloreCella;
        private double spesePulizia;
        private double acconto;
        private bool famigliaDominici;

        public double Acconto
        {
            get => acconto;
            set
            {
                if (value < 0)
                    throw new ArgumentException("L'acconto non può essere negativo.");
                acconto = value;
            }
        }
        public double SpesePulizia
        {
            get => spesePulizia;
            set => spesePulizia = value;
        }

        public string Id
        {
            get => id;
            set => id = value;
        }

        public string Nome
        {
            get => nome;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length < 3 || !Regex.IsMatch(value, @"^[a-zA-Zàèéìòù ]+$"))
                    throw new ArgumentException("Il nome deve contenere almeno 3 lettere e non avere numeri o simboli.");
                nome = value.Trim();
            }
        }

        public string Cognome
        {
            get => cognome;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length < 3 || !Regex.IsMatch(value, @"^[a-zA-Zàèéìòù ]+$"))
                    throw new ArgumentException("Il cognome deve contenere almeno 3 lettere e non avere numeri o simboli.");
                cognome = value.Trim();
            }
        }

        public double Versamento
        {
            get => versamento;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Il versamento non può essere negativo.");
                versamento = value;
            }
        }

        public DateTime DataInizio
        {
            get => dataInizio;
            set
            {
                dataInizio = value;
            }
        }

        public DateTime DataFine
        {
            get => dataFine;
            set
            {
                if (value <= dataInizio)
                    throw new ArgumentException("La data di fine deve essere successiva alla data di inizio.");
                dataFine = value;
            }
        }

        public bool TipoPrenotazione
        {
            get => tipoPrenotazione;
            set => tipoPrenotazione = value;
        }

        public Color ColoreCella
        {
            get => coloreCella;
            set
            {
                if (value == Color.Transparent)
                    throw new ArgumentException("Il colore della cella non può essere trasparente.");
                coloreCella = value;
            }
        }

        public bool FamigliaDominici { get => famigliaDominici; set => famigliaDominici = value; }

        // Metodo per ottenere i dettagli della prenotazione in stringa
        public override string ToString()
        {
            return $"Prenotazione di {Nome} {Cognome}\ndal {DataInizio:dd/MM/yyyy} al {DataFine:dd/MM/yyyy}\nImporto: {Versamento}€\nTipo: {(TipoPrenotazione ? "Sicura" : "Insicura")}\nSpese di pulizia: {SpesePulizia}\nAcconto : {Acconto}";
        }

        // Costruttore
        public ClsPrenotazione(string id, string nome, string cognome, double versamento, DateTime dataInizio, DateTime dataFine, bool tipoPrenotazione, Color coloreCella, double spesePulizia, double acconto, bool famigliaDominici)
        {
            Id = id;
            Nome = nome;
            Cognome = cognome;
            Versamento = versamento;
            DataInizio = dataInizio;
            DataFine = dataFine;
            TipoPrenotazione = tipoPrenotazione;
            ColoreCella = coloreCella;
            SpesePulizia = spesePulizia;
            Acconto = acconto;
            FamigliaDominici = famigliaDominici;
        }
    }
}
