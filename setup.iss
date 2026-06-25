[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
AppId={{8A7B6C5D-4E3F-2A1B-0C9D-8E7F6A5B4C3D}
AppName=Calendario Prenotazioni
AppVersion=2.1
AppPublisher=Gestione Booking
DefaultDirName={autopf}\Calendario Prenotazioni
DefaultGroupName=Calendario Prenotazioni
; Cartella di output dove verrà salvato l'installer (.exe)
OutputDir=Installer
OutputBaseFilename=Calendario_Setup_v2.1
SetupIconFile=Calendario\IMG\Icona.ico
Compression=lzma2
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64
PrivilegesRequired=admin

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Dirs]
; IMPORTANTE: Siccome il programma salva i file .json nella stessa cartella dell'eseguibile,
; dobbiamo dare i permessi di scrittura agli utenti standard se viene installato in Program Files.
Name: "{app}"; Permissions: users-modify

[Files]
; Prende tutti i file dalla cartella Release del progetto, MA ESCLUDE i file .json
; In questo modo l'aggiornamento non sovrascriverà mai i dati di salvataggio dell'utente
Source: "Calendario\bin\Release\*"; DestDir: "{app}"; Excludes: "*.json"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Calendario Prenotazioni"; Filename: "{app}\Calendario.exe"
Name: "{group}\{cm:UninstallProgram,Calendario Prenotazioni}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\Calendario Prenotazioni"; Filename: "{app}\Calendario.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\Calendario.exe"; Description: "{cm:LaunchProgram,Calendario Prenotazioni}"; Flags: nowait postinstall skipifsilent

