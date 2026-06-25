<div align="center">

<img src="https://res.cloudinary.com/dxudggkln/image/upload/v1782149842/Gemini_Generated_Image_1t48c51t48c51t48_gjoas4.png" alt="Booking Calendar Banner" width="600"/>

# 📅 Booking Calendar

### Sistema di gestione prenotazioni per strutture ricettive — Dark Modern UI

[![Release](https://img.shields.io/github/v/release/Na1ky/Booking-Calendar?style=for-the-badge&color=7C3AED&label=Latest)](https://github.com/Na1ky/Booking-Calendar/releases)
[![GitHub](https://img.shields.io/badge/Source-GitHub-181717?style=for-the-badge&logo=github)](https://github.com/Na1ky/Booking-Calendar)

![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=csharp&logoColor=white)
![.NET WinForms](https://img.shields.io/badge/.NET_WinForms-512BD4?style=flat&logo=dotnet&logoColor=white)
![MongoDB](https://img.shields.io/badge/MongoDB_Atlas-47A248?style=flat&logo=mongodb&logoColor=white)
![Guna UI2](https://img.shields.io/badge/Guna.UI2-7C3AED?style=flat)
![BCrypt](https://img.shields.io/badge/BCrypt-CC2927?style=flat)

</div>

---

## 📖 Overview

**Booking Calendar** è un'applicazione desktop in **C# WinForms** per la gestione avanzata delle prenotazioni di strutture ricettive (B&B, appartamenti, case vacanza). Nonostante utilizzi il framework classico WinForms, il progetto dispone di un'interfaccia **Dark Mode** completamente personalizzata — angoli arrotondati, effetti glow sul focus e layout fluido — dimostrando come sia possibile spingere le capacità grafiche del framework nativo senza librerie di terze parti pesanti o a pagamento.

I dati delle prenotazioni sono sincronizzati in cloud tramite **MongoDB Atlas** con accesso protetto da hash BCrypt.

---

## ✨ Funzionalità Principali

| | Feature | Descrizione |
|---|---|---|
| 📅 | **Calendario Visuale** | Griglia interattiva con giorni liberi/occupati a colpo d'occhio |
| 📝 | **Gestione Prenotazioni** | Aggiungi, modifica ed elimina prenotazioni (Nome, Cognome, Caparra, Pulizie, Pagamento, Tipo) |
| 💶 | **Calcolatore Prezzi** | Prezzi medi mensili configurabili + calcolo automatico del costo totale per un soggiorno |
| 📄 | **Export Word** | Generazione documenti `.docx` di riepilogo direttamente dall'app |
| 🔒 | **Cloud Sync & Auth** | Dati su MongoDB Atlas con password hashate via BCrypt |
| 🎨 | **UI Dark Moderna (v2.0)** | Powered by `Guna.UI2.WinForms` — corner arrotondati, animazioni fluide, estetica dark-indigo |

---

## 🧱 Stack Tecnologico

| Tecnologia | Ruolo |
|---|---|
| **C# / .NET WinForms** | Linguaggio e framework desktop |
| **MongoDB Atlas** | Database cloud per prenotazioni e utenti |
| **Guna.UI2.WinForms** | Componenti UI moderni (rounded, animated) |
| **BCrypt.Net** | Hashing sicuro delle password |
| **DocumentFormat.OpenXml** | Generazione file `.docx` |
| **MongoDB.Driver** | Client MongoDB per .NET |
| **Newtonsoft.Json** | Configurazioni JSON legacy/locali |

---

## 🏗️ Architettura (MVC 3-Tier)

```
VIEW (Forms)
├── FrmCalendario.cs          ← Schermata principale con griglia e sidebar
├── FrmAggiungiPrenotazione.cs
├── FrmModificaPrenotazione.cs
├── FrmGestionePrezzo.cs      ← Modulo calcolo prezzi
├── FrmSalvaIlFile.cs         ← Export Word
├── FrmLogin.cs / FrmRegister.cs
└── ... altri form

CONTROLLER
└── PrenotazioneController.cs ← Business logic e data passing

MODEL
└── Prenotazione.cs           ← Struttura dati
```

---

## 🎨 Design Notes

Palette **Dark-Indigo / Purple**:

- Sfondi profondi: `rgb(10, 13, 28)`
- Pannelli "Glass/Card" elevati: `rgb(22, 28, 62)`
- Accenti indigo/viola: `#7C3AED` per bordi e focus (`ModernInputContainer`)
- `MonthCalendar` personalizzato per essere coerente con i colori dell'applicazione

---

## 🚀 Come Eseguire

<<<<<<< HEAD
This project is distributed "as is" for educational or personal management purposes.

=======
**Requisiti:** Visual Studio 2022, .NET SDK, connessione internet (MongoDB Atlas)

```bash
# 1. Clona il repository
git clone https://github.com/Na1ky/Booking-Calendar.git

# 2. Apri la soluzione in Visual Studio
#    Doppio click su "Calendario.sln"

# 3. Ripristina i pacchetti NuGet
#    Visual Studio lo fa automaticamente, oppure:
#    Tools → NuGet Package Manager → Restore

# 4. Configura la stringa di connessione MongoDB Atlas
#    nelle impostazioni / file di configurazione

# 5. Premi F5 o "Avvia" per eseguire l'applicazione
```

---

## 📦 Release

Scarica il **setup installer** già compilato dalla sezione [**Releases**](https://github.com/Na1ky/Booking-Calendar/releases).

Il pacchetto di installazione è generato con **Inno Setup** (`setup.iss`).

| Versione | Data | Note |
|---|---|---|
| v2.0 ⭐ | Giu 2026 | UI completamente rinnovata con Guna.UI2, Cloud Sync |
| v1.x | — | Versioni precedenti con UI base |

---

## 📝 Licenza

Distribuito "as is" per scopi educativi o di gestione personale.

---

## 👤 Autore

**Nicolas Dominici** — [nicolas-dominici.it](https://nicolas-dominici.it) · [GitHub @Na1ky](https://github.com/Na1ky)
>>>>>>> a31fe5b9b8a4dbe411e73a320b1f651824b5e010
