# Booking-Calendar (Reservation Management System)

![.NET](https://img.shields.io/badge/.NET-WinForms-512BD4?style=flat&logo=.net)
![C#](https://img.shields.io/badge/C%23-Modern_UI-239120?style=flat&logo=c-sharp)

A modern Desktop application developed in **C# Windows Forms (WinForms)** for the advanced management of reservations for an accommodation facility (B&B, apartment, vacation home).

Despite using the classic WinForms framework, the project features a fully customized **Dark Mode** User Interface (UI), with modern components, rounded borders, glowing focus effects, and a fluid layout. This demonstrates how it is possible to push the graphical capabilities of the native framework to the limit without using heavy or paid third-party libraries.

---

## ✨ Main Features

*   **📅 Visual Calendar:** A clear grid interface that allows you to see occupied and free days at a glance.
*   **📝 Reservation Management:** Quickly add, edit, and delete reservations, storing data such as Name, Surname, Deposit, Cleaning Fees, Payment, and Type (Safe/Unsafe).
*   **💶 Price Calculator:** An integrated module to input the average prices for each month of the year and calculate the total estimated cost for a stay based on the selected dates with one click.
*   **📄 Word Export:** Generate and export recap documents directly into `.docx` Word files dynamically.
*   **🔒 Secure Cloud Sync & Auth:** Reservation data is stored in the cloud using MongoDB Atlas, and access is secured via BCrypt password hashing.
*   **🎨 Custom Modern UI (v2.0):** The project now features an overhauled modern UI powered by `Guna.UI2.WinForms`, delivering sleek rounded corners, smooth animations, and a polished dark aesthetic.

---

## 🛠️ Technologies Used

*   **Language:** C#
*   **Framework:** .NET / Windows Forms (WinForms)
*   **Architecture:** Simplified Model-View-Controller (MVC) (3-tier pattern: `MODEL`, `VIEW`, `CONTROLLER`).
*   **Database:** MongoDB Atlas (Cloud)
*   **External Libraries:** 
    * `Guna.UI2.WinForms` (for modern UI components)
    * `MongoDB.Driver` (for database communication)
    * `BCrypt.Net` (for secure password hashing)
    * `DocumentFormat.OpenXml` (for `.docx` generation)
    * `Newtonsoft.Json` (for legacy/local JSON configurations).

---

## 📂 Project Structure

The project is logically structured into folders to separate architectural concepts:

*   `/CONTROLLER`: Contains business logic and data passing (e.g., `PrenotazioneController.cs`).
*   `/MODEL`: Contains data structure definitions (e.g., `Prenotazione.cs`).
*   `/VIEW`: Contains all application windows (Forms) and the custom UI library.
    *   `FrmCalendario.cs` (Main screen with the grid and sidebar)
    *   `FrmAggiungiPrenotazione.cs`
    *   `FrmModificaPrenotazione.cs`
    *   `FrmGestionePrezzo.cs`
    *   `FrmSalvaIlFile.cs` (Word export module)
    *   `FrmLogin.cs` & `FrmRegister.cs` (Authentication screens)
    *   ... and other forms for saving or deleting.

---

## 🚀 How to Run the Project

1. **Clone or download the repository:**
   ```bash
   git clone https://github.com/your-username/Booking-Calendar.git
   ```
2. **Open the solution in Visual Studio:**
   Double-click the `Calendario.sln` file to open the entire project in Microsoft Visual Studio 2022.
3. **Restore NuGet Packages:**
   Make sure to restore the `Newtonsoft.Json` package if the IDE doesn't do it automatically.
4. **Build and Run:**
   Press `F5` or click "Start" to launch the program (Debug/Release Configuration).

---

## 🎨 Design Notes

The interface is based on a **Dark-Indigo/Purple** palette:
*   Deep backgrounds (e.g., `10, 13, 28`).
*   Elevated panels in "Glass/Card" design (`22, 28, 62`).
*   Bright indigo and purple accents (`124, 58, 237` / `#7C3AED`) for borders and input focus (`ModernInputContainer`).
*   Integration with single multiple-selections on `MonthCalendar` made consistent with the application's colors, overcoming WinForms' rigidities.

---

## 📝 License

This project is distributed "as is" for educational or personal management purposes.
