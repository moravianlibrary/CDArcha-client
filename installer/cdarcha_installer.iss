#define FileVersion GetFileVersion('..\CDArcha_klient\bin\Release\CDArcha_klient.exe')
#define StripVersion(str VerStr) Copy(VerStr, 1, RPos(".", VerStr)-1)
#define ApplicationVersion StripVersion(StripVersion(FileVersion))
;Installer for CDArcha_klientClient
[Setup]
AppVersion={#ApplicationVersion}
OutputBaseFilename=CDArcha_klient_setup
AppPublisher=Moravsk· Zemsk· Knihovna
AppPublisherURL=http://www.mzk.cz/
AppId=CDArcha
DisableDirPage=auto
DisableProgramGroupPage=auto
;need admin rights because of Program Files location and wiaaut.dll for Win XP
PrivilegesRequired=admin
AppName=CDArcha
DefaultDirName={pf32}\CDArcha
DefaultGroupName=CDArcha
UpdateUninstallLogAppName=no
Compression=lzma2
SolidCompression=yes
AllowNoIcons=no
;maximal resolution is 164x314, picture has 163x314, which creates 1 pixel border 
WizardImageFile=wizardImage.bmp
WizardImageStretch=no
;color which creates border
WizardImageBackColor=$A0A0A0
;max resolution 55x58 pixels
WizardSmallImageFile=wizardSmallImage.bmp

[Languages]
Name: "cz"; MessagesFile: "compiler:Languages\Czech.isl"

[Dirs]
Name: "{app}\template"
Name: "{app}\cdarcha_tmp"

[Files]
Source: "..\CDArcha_klient\bin\Release\CDArcha_klient.exe"; DestDir: "{app}"
Source: "..\CDArcha_klient\bin\Release\CroppingAdorner.dll"; DestDir: "{app}"
Source: "..\CDArcha_klient\bin\Release\Interop.WIA.dll"; DestDir: "{app}"
Source: "..\CDArcha_klient\bin\Release\Zoom.Net.dll"; DestDir: "{app}"
Source: "..\CDArcha_klient\bin\Release\Zoom.Net.YazSharp.dll"; DestDir: "{app}"
Source: "..\CDArcha_klient\bin\Release\Newtonsoft.Json.dll"; DestDir: "{app}"
Source: "..\CDArcha_klient\bin\Release\SobekCM_Marc_Library.dll"; DestDir: "{app}"
Source: "..\CDArcha_klient\bin\Release\CDArcha_klient.exe.config"; DestDir: "{app}"
Source: "..\CDArcha_klient\bin\Release\CDArcha_klient.exe.manifest"; DestDir: "{app}"
Source: "..\CDArcha_klient\bin\Release\template\MARC21slim2MODS.xsl"; DestDir: "{app}\template"
Source: "..\CDArcha_klient\bin\Release\cdarcha_tmp\marcxml.xml"; DestDir: "{app}\cdarcha_tmp"
Source: "..\CDArcha_klient\bin\Release\cdarcha_tmp\mods.xml"; DestDir: "{app}\cdarcha_tmp"
Source: "..\CDArcha_klient\WIA-DLL\wiaaut.dll"; DestDir: "{sys}"; Flags: onlyifdoesntexist 32bit
Source: "..\SobekCM_Marc_Library\DLLs\Zoom.net\libxml2.dll"; DestDir: "{app}"
Source: "..\SobekCM_Marc_Library\DLLs\Zoom.net\libxslt.dll"; DestDir: "{app}"
Source: "..\SobekCM_Marc_Library\DLLs\Zoom.net\zlib1.dll"; DestDir: "{app}"
Source: "..\SobekCM_Marc_Library\DLLs\Zoom.net\yaz.dll"; DestDir: "{app}"
Source: "..\SobekCM_Marc_Library\DLLs\Zoom.net\iconv.dll"; DestDir: "{app}"

[Icons]
Name: "{group}\CDArcha_klient"; Filename: "{app}\CDArcha_klient.exe"
Name: "{group}\Uninstall CDArcha_klient"; Filename: "{app}\unins000.exe"
Name: "{commondesktop}\CDArcha_klient"; Filename: "{app}\CDArcha_klient.exe"

[UninstallDelete]
Type: files; Name: "{app}\yaz.log"

[Run]
Filename: "{sys}\regsvr32"; Parameters: "/s {sys}\wiaaut.dll"

[Code]
{Detect .NET version}
function IsDotNetDetected(version: string; service: cardinal): boolean;
// version -- Specify one of these strings for the required .NET Framework version:
//    'v1.1.4322'     .NET Framework 1.1
//    'v2.0.50727'    .NET Framework 2.0
//    'v3.0'          .NET Framework 3.0
//    'v3.5'          .NET Framework 3.5
//    'v4\Client'     .NET Framework 4.0 Client Profile
//    'v4\Full'       .NET Framework 4.0 Full Installation
//
// service -- Specify any non-negative integer for the required service pack level:
//    0               No service packs required
//    1, 2, etc.      Service pack 1, 2, etc. required
var
    key: string;
    install, serviceCount: cardinal;
    success: boolean;
begin
    key := 'SOFTWARE\Microsoft\NET Framework Setup\NDP\' + version;
    // .NET 3.0 uses value InstallSuccess in subkey Setup
    if Pos('v3.0', version) = 1 then begin
        success := RegQueryDWordValue(HKLM, key + '\Setup', 'InstallSuccess', install);
    end else begin
        success := RegQueryDWordValue(HKLM, key, 'Install', install);
    end;
    // .NET 4.0 uses value Servicing instead of SP
    if Pos('v4', version) = 1 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Servicing', serviceCount);
    end else begin
        success := success and RegQueryDWordValue(HKLM, key, 'SP', serviceCount);
    end;
    result := success and (install = 1) and (serviceCount >= service);
end;

procedure InitializeWizard();
var ResultCode:Integer; Version: TWindowsVersion;
begin
    {Check previously installed version}
    if RegKeyExists(HKEY_CURRENT_USER, 'Software\Microsoft\Windows\CurrentVersion\Uninstall\CDArcha_klient_is1') then
    begin
      MsgBox('Nespr·vn˝ typ instalace:' #13#13 
             'Naöla se p¯edchozÌ instalace programu CDArcha_klient, kter· m· typ uûivatelsk· instalace.'
           + ' St·hnÏte uûivatelskou verzi nebo odinstalujte souËasnou verzi a spusùte instalaci znovu.',
             mbCriticalError, MB_OK);
      Abort();
    end;
    GetWindowsVersionEx(Version);
    if((not Version.NTPlatform) or (Version.Major < 5) or 
           ((Version.Major = 5) and (Version.Minor = 0))) then
    begin
      MsgBox('Nepodporovan˝ OS:' #13#13 
             'Tato verze je funkËnÌ jen na Windows XP a vyööÌch OS.',
             mbCriticalError, MB_OK);
      Abort();
    end;

    if not IsDotNetDetected('v4\Client', 0) then
    begin
        MsgBox('Aplikace vyûaduje .NET Framework 4.0 Client Profile.'#13#13
               'TeÔ zaËne jeho instalace.', mbInformation, MB_OK);
        ExtractTemporaryFile('dotNetFx40_Client_setup.exe');
        
        Exec(ExpandConstant('{tmp}\dotNetFx40_Client_setup.exe'),'',
             '', SW_SHOWNORMAL, ewWaitUntilTerminated, ResultCode);
        if not (ResultCode = 0) then
        begin
          MsgBox('Instalace se nezda¯ila:' #13#13 
                 'Nezda¯ila se instalace .Net Framework 4.0 Client Profile. '
                 + 'Skuste ho nainstalovat ruËnÏ.', mbError, MB_OK);
          Abort();
        end;
    end;
end;
