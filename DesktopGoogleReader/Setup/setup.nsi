!include "MUI2.nsh"
!include "checkDotNet3.nsh"

!define MIN_FRA_MAJOR "3"
!define MIN_FRA_MINOR "5"
!define MIN_FRA_BUILD "*"


; The name of the installer
Name "Desktop Google Reader"

; The file to write
OutFile "Setup-DesktopGoogleReader.exe"





; The default installation directory
InstallDir "$PROGRAMFILES\Desktop Google Reader"

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\DesktopGoogleReader" "Install_Dir"

; Request application privileges for Windows Vista
RequestExecutionLevel admin


 


;--------------------------------

  !define MUI_ABORTWARNING



!define MUI_HEADERIMAGE
!define MUI_HEADERIMAGE_BITMAP "logoSetupSmall.bmp"
!define MUI_WELCOMEFINISHPAGE_BITMAP "logoSetupBig.bmp"
!define MUI_WELCOMEPAGE_TITLE "Desktop Google Reader"
!define MUI_WELCOMEPAGE_TEXT "Desktop Google Reader is a client for the popular service by Google.$\r$\n$\r$\nPlease stop any instance of Desktop Google Reader prior to installing this version."
!define MUI_STARTMENUPAGE_DEFAULTFOLDER "Desktop Google Reader"
!define MUI_ICON "DesktopGoogleReader.ico"
!define MUI_UNICON "uninstall.ico"


Var StartMenuFolder
; Pages

  !insertmacro MUI_PAGE_WELCOME
  !insertmacro MUI_PAGE_LICENSE "License.txt"
  !insertmacro MUI_PAGE_COMPONENTS
  !insertmacro MUI_PAGE_DIRECTORY

  !define MUI_STARTMENUPAGE_REGISTRY_ROOT "HKCU" 
  !define MUI_STARTMENUPAGE_REGISTRY_KEY "Software\DesktopGoogleReader" 
  !define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "Start Menu Folder"
  !insertmacro MUI_PAGE_STARTMENU Application $StartMenuFolder

  !insertmacro MUI_PAGE_INSTFILES
  !define MUI_FINISHPAGE_RUN "DesktopGoogleReader.exe"
  !insertmacro MUI_PAGE_FINISH




  !insertmacro MUI_UNPAGE_WELCOME
  !insertmacro MUI_UNPAGE_CONFIRM
  !insertmacro MUI_UNPAGE_INSTFILES
  !insertmacro MUI_UNPAGE_FINISH





;--------------------------------




!insertmacro MUI_LANGUAGE "English"

; LoadLanguageFile "${NSISDIR}\Contrib\Language files\English.nlf"
;--------------------------------
;Version Information

  VIProductVersion "1.4.0.0"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "ProductName" "Desktop Google Reader"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "CompanyName" "lI' Ghun"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "LegalCopyright" "© 2010 - 2011"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "FileDescription" "Client to Google Reader"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "FileVersion" "1.4"







Function un.UninstallDirs
    Exch $R0 ;input string
    Exch
    Exch $R1 ;maximum number of dirs to check for
    Push $R2
    Push $R3
    Push $R4
    Push $R5
       IfFileExists "$R0\*.*" 0 +2
       RMDir "$R0"
     StrCpy $R5 0
    top:
     StrCpy $R2 0
     StrLen $R4 $R0
    loop:
     IntOp $R2 $R2 + 1
      StrCpy $R3 $R0 1 -$R2
     StrCmp $R2 $R4 exit
     StrCmp $R3 "\" 0 loop
      StrCpy $R0 $R0 -$R2
       IfFileExists "$R0\*.*" 0 +2
       RMDir "$R0"
     IntOp $R5 $R5 + 1
     StrCmp $R5 $R1 exit top
    exit:
    Pop $R5
    Pop $R4
    Pop $R3
    Pop $R2
    Pop $R1
    Pop $R0
FunctionEnd









; The stuff to install
Section "Desktop Google Reader"

  SectionIn RO
  
  SetOutPath "$INSTDIR\\Resources\\Images"
  File "..\Resources\Images\Feed.png"
  File "..\Resources\Images\tray.ico"
  File "..\Resources\Images\trayUnreadItems.ico"
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  !insertmacro AbortIfBadFramework

  ; Put file there
  File "Documentation.URL"
  File "DesktopGoogleReader.ico"
  File "..\DesktopGoogleReader.exe"
  File "..\DesktopGoogleReader.pdb"
  File "..\DesktopGoogleReader.application"
  File "..\DesktopGoogleReader.exe.config"
  File "..\DesktopGoogleReader.exe.manifest"
  File "..\GoogleReaderAPI.dll"
  File "..\GoogleReaderAPI.pdb"  
  ; File "..\GoogleReaderAPI.dll.config"
  File "LICENSE.txt"
  File "Documentation.ico"
  File "..\Winkle.dll"
  File "..\Winkle.pdb"
  
  ;Webkit
  File /r "..\CFLite.resources"
  File /r "..\JavaScriptCore.resources"
  File /r "..\WebKit.resources"
  File "..\CFLite.dll"
  File "..\icudt40.dll"
  File "..\icuin40.dll"
  File "..\icuuc40.dll"
  File "..\JavaScriptCore.dll"
  File "..\lib*"
  File "..\objc.dll"
  File "..\pthreadVC2.dll"
  File "..\SQLite3.dll"
  File "..\ssleay32.dll"
  File "..\WebKit*"
  File "..\vcredist_x86.exe"

  
  File "..\ExternalServices\Twitter\Dimebrain.TweetSharp.dll"
  File "..\ExternalServices\Twitter\Dimebrain.TweetSharp.xml"
  File "..\ExternalServices\Twitter\Newtonsoft.Json.dll"
  File "..\ExternalServices\Facebook.dll"
  
  ; Write the installation path into the registry
  WriteRegStr HKLM SOFTWARE\DesktopGoogleReader "Install_Dir" "$INSTDIR"
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\DesktopGoogleReader" "DisplayName" "Desktop Google Reader"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\DesktopGoogleReader" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\DesktopGoogleReader" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\DesktopGoogleReader" "NoRepair" 1
  WriteUninstaller "uninstall.exe"

    Push $R0
   ClearErrors
   ReadRegDword $R0 HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{9A25302D-30C0-39D9-BD6F-21E6EC160475}" "Version"

   ; if VS redist SP1 not installed, install it
   IfErrors 0 VSRedistInstalled
   ExecWait '"$INSTDIR\vcredist_x86.exe" /qb'
   StrCpy $R0 "-1"

VSRedistInstalled:

  
SectionEnd

; Optional section (can be disabled by the user)
Section "Start Menu Shortcuts"

!insertmacro MUI_STARTMENU_WRITE_BEGIN Application

  CreateDirectory "$SMPROGRAMS\$StartMenuFolder"
  CreateShortCut "$SMPROGRAMS\$StartMenuFolder\\Desktop Google Reader.lnk" "$INSTDIR\DesktopGoogleReader.exe" "" "$INSTDIR\DesktopGoogleReader.ico" 0
  CreateShortCut "$SMPROGRAMS\$StartMenuFolder\\Documentation.lnk" "$INSTDIR\Documentation.URL" "" $INSTDIR\Documentation.ico" 0
  CreateShortCut "$SMPROGRAMS\$StartMenuFolder\\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  
!insertmacro MUI_STARTMENU_WRITE_END

  
SectionEnd


;--------------------------------

; Uninstaller

Section "Uninstall"

  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\DesktopGoogleReader"
  DeleteRegKey HKLM "Software\DesktopGoogleReader"
  ; Remove files and uninstaller
  Delete $INSTDIR\*.*

  ; Remove shortcuts, if any
  !insertmacro MUI_STARTMENU_GETFOLDER Application $StartMenuFolder
    
  Delete "$SMPROGRAMS\$StartMenuFolder\\*.*"
  


  DeleteRegKey HKCU "Software\DesktopGoogleReader"


  ; Remove directories used
   ; RMDir "$SMPROGRAMS\$StartMenuFolder"
Push 10 #maximum amount of directories to remove
  Push "$SMPROGRAMS\$StartMenuFolder" #input string
    Call un.UninstallDirs

   
  ; RMDir "$INSTDIR"
  
  Push 10 #maximum amount of directories to remove
  Push $INSTDIR #input string
    Call un.UninstallDirs


SectionEnd
