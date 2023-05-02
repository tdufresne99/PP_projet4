@echo off


rem // INSTRUCTIONS /////////////////////////////////////////////////////////////////

rem // Le ZippeurUniTim est un script conçu pour zipper facilement un projet Unity.

rem // Prérequis: le logiciel 7-zip doit être installé.

rem // Utilisation: le fichier .bat doit être placé dans le dossier du projet Unity
rem // (à côté des dossiers assets, etc.), puis il faut l'exécuter (double-clic).


rem // CONFIGURATION AVANCÉE ////////////////////////////////////////////////////////

rem ExclureLibrary:    1 permet d'exclure le dossier Library (0 pour l'inclure)*
set ExclureLibrary=1;

rem ExclureGit:        1 permet d'exclure le dossier .git (0 pour l'inclure)*
set ExclureGit=1;

rem ExclureZip:        1 permet d'exclure les fichiers .zip (0 pour l'inclure)*
set ExclureZip=1;

rem *Les exclusions créent des archives plus légères et plus rapides!

rem NiveauCompression: permet de choisir le niveau de compression (1, 3, 5, 7, 9)
rem                    1 (Fastest), 3 (Fast), 5 (Normal), 7 (Maximum), 9 (Ultra)
set NiveauCompression=7


rem // DÉBUT DU PROGRAMME ///////////////////////////////////////////////////////////

rem Sauvegarde de l'encodage original:
for /f "tokens=2 delims=:." %%x in ('chcp') do set cp=%%x
rem Changement de l'encodage:
chcp 1252>nul

rem Variables pour la mise en forme du texte: 
set "Gras=[1m"
set "Norm=[0m"

:PAGETITRE
echo @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
echo @@@@@@@@@@@@@@@@@@@@@@@@@@@         @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
echo @@@@@@@@@@@@@@@@@@@@@                     @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
echo @@@@@@  @@@@@@@@@@      @@@@@@@@@@  @@@@     @@@@@@@@@@  @@@@@@@             @@@
echo @@@@@  @@@  @@@@    @@@@@@@@@@@@@@@@@@@@@@@    @@@   @@@  @@@@@@             @@@
echo @@@@  @@@   @@@   @@@@@@@@@@@@@@@@@@@@@@@@@@@   @@@   @@   @@@@@   %Gras%ZIPPEUR%Norm%   @@@
echo @@@   @@   @@@   @@@@@@@@@@@@@@@@@@@@@@@@@@@@@   @@@  @@@  @@@@@   %Gras%UNITY  %Norm%   @@@
echo @@@  @@@  @@@   @@@@@@@@@@@@@    @@@              @@   @@   @@@@   %Gras%TIM    %Norm%   @@@
echo @@@  @@@  @@@   @@@@@   @@@@@@@ @  @@@@@@@@@@@@   @@@  @@@  @@@@             @@@
echo @@@  @@@  @@@   @@@@@ @@@@@@@@@                   @@   @@@  @@@@   CSTJ      @@@
echo @@@  @@@   @@   @@@@@              @  @          @@@   @@   @@@@   V0.94     @@@
echo @@@@  @@@  @@@@@@ @@@@            @     @       @@@   @@@  @@@@@   H2022     @@@
echo @@@@   @@@  @@@@   @@@@                 @      @@@   @@@  @@@@@@   CC3.0     @@@
echo @@@@@   @@@@@@@@@     @@@@            @@     @@@@@@@@@@  @@@@@@@             @@@
echo @@@@@@@@@@@@@@@@@@@@      @@@@@@@@@@@      @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
echo @@@@@@@@@@@@@@@@@@@@@@@@               @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
echo @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
echo.

rem Variable pour indiquer les exclusions:
set "DossiersExclus=Logs, Temp."

rem Détection des exclusions et ajustement des espacements:
if %ExclureLibrary% neq 0 (
	set "DossiersExclus=Librairie, %DossiersExclus%"
)
if %ExclureGit% neq 0 (
	set "DossiersExclus=.git, %DossiersExclus%"
)
if %ExclureZip% neq 0 (
	set "DossiersExclus=*.zip, %DossiersExclus%"
)

rem Variable pour afficher la ligne des exclusions:
set "DossiersExclus=  (Exclusions : %DossiersExclus%)"

echo   Permet de créer un fichier zip optimisé, à partir d'un dossier projet Unity.
echo %DossiersExclus%

rem Ajustement du dossier actuel (utile en contexte admin):
setlocal enableextensions
cd /d "%~dp0"

rem Identification du nom du dossier:
for %%I in (.) do set NomD=%%~nxI

rem Variable du nom du fichier archive:
set Fichier=%NomD%.zip

rem Ajout du chemin de 7-Zip:
set path7z="C:\Program Files\7-Zip"
set PATH=%PATH%;%path7z%

rem Recherche le dossier de 7zip:
if NOT exist %path7z% goto ERREUR7ZINSTAL

echo.
echo   Le contenu du dossier suivant va être archivé :
echo   %Gras%%cd%%Norm%

rem Recherche du sous-dossier Assets:
if NOT exist "%cd%\Assets" goto ERREURUNITY

rem Recherche d'une archive du même nom:
echo.
if exist "%cd%\%Fichier%" (
    echo   Attention, le fichié %Gras%%Fichier%%Norm% sera remplacé.
) else (
    echo   Le fichié %Gras%%Fichier%%Norm% sera créé.
)
echo.

:QUESTION
set /p mode=  Souhaitez-vous continuer? (%Gras%[O]%Norm%, N) 

rem Si c'est 3, on passe à l'étiquette ARRET
if /i "%mode%" == "N" goto ARRET
if /i "%mode%" == "Non" goto ARRET

rem Par défaut, c'est le mode automatique...

:AUTOMATIQUE
rem Retour temporaire à l'encodage initial (pour les messages d'erreur de 7zip):
chcp %cp%>nul

rem Supression du fichier si existant:
if exist "%cd%\%Fichier%" (
	del "%cd%\%Fichier%"
	rem Test pour détecter une erreur de suppression:
	if exist "%cd%\%Fichier%" goto ERREURDEL
)

rem La commande de base 7-zip (ajoute les fichiers dans l'archive, sauf Logs et Temp):
set "Commande=7z a -tzip ^"%Fichier%^" ^"%cd%^" -mx%NiveauCompression% -xr!Logs -xr!Temp"

rem Réduction du verbillage de 7-zip lors de la compression:
set "Commande=%Commande% -bb0 -bso0"

rem Ajout des exclusions, selon les réglages:
if %ExclureLibrary% neq 0 set "Commande=%Commande% -xr!Library"
if %ExclureGit% neq 0 set "Commande=%Commande% -xr!.git"
if %ExclureZip% neq 0 set "Commande=%Commande% -xr!*.zip"

rem Exécution de la commande 7-zip:
%Commande%

rem Mémorisation du code d'erreur:
set MemErreur=%ERRORLEVEL%

rem Si une erreur survient:
if %MemErreur% neq 0 goto ERREUR7ZA

rem Retour à l'encodage pour nos messages en français:
chcp 1252>nul

echo.
echo @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
echo.
echo   %Gras%SUCCÈS ! %Norm%
echo   Fichié résultant: %Gras%%Fichier%%Norm%
echo   Merci d'avoir utilisé le ZippeurUniTim !
echo.
echo @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
goto FIN

:ARRET
echo.
echo @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
echo.
echo   %Gras%INTERRUPTION ! %Norm%
echo   Arrêt du script. Aucun fichier modifié.
echo.
echo @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
goto FIN

:ERREURUNITY
echo.
echo @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
echo.
echo   %Gras%ERREUR ! %Norm%
echo   Le dossier ne semble pas être un projet Unity (pas de sous-dossier Assets).
echo   Indice: %Gras%Le fichier .BAT doit être à l'intérieur du dossier du projet Unity.%Norm% 
echo.
echo @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
goto FIN

:ERREURDEL
rem Retour à l'encodage pour nos messages en français:
chcp 1252>nul
echo.
echo @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
echo.
echo   %Gras%ÉCHEC ! %Norm%
echo   Erreur lors de la suppression du fichier (utilisé par une autre application?)
echo   Suggestion: Supprimez manuellement le fichier %Gras%%Fichier%%Norm%
echo.
echo @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
goto FIN

:ERREUR7ZINSTAL
echo.
echo @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
echo.
echo   %Gras%ERREUR ! %Norm%
echo   Pour utiliser ce script, %Gras%7-zip doit être installé%Norm% dans le dossier :
echo   %path7z%
echo.
echo @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
goto FIN

:ERREUR7ZA
rem Retour à l'encodage pour nos messages en français:
chcp 1252>nul
echo.
echo @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
echo.
echo   %Gras%ÉCHEC ! %Norm%
echo   Erreur 7-zip #%MemErreur% lors de la compression.
echo   Indice: Certains problèmes surviennent quand le projet est ouvert dans Unity.
echo.
echo @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
goto FIN

:FIN
echo.
rem Retour à l'encodage initial:
chcp %cp%>nul
pause