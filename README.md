# Flaschenpost

## Android-App

die android-app befindet sich im verzeichnis **FlaschenpostToDo**  
suchen sie im hauptverzeichnis nach der datei *Settings.cs*  
* ApiAddress  
die ip-adresse des rechners im lokalen netzwerk

**Protocol** und **Port** kann bei bedarf angepasst werden.  
android-app kompilieren und auf ein virtuelles- oder ein echtes gerät deployen (ab android 11 'Red Velvet Cake' / api level 30)

sobald die app auf das gerät deployed wurde, kann das projekt geschlossen werden.

## ASP.Net Core API Server

der API-Server befindet sich im verzeichnis **FlaschenpostApi**  
suchen sie im projekt *FlaschenpostApi/Properties* nach der datei *launchSettings.json*  
der server ist so konfiguriert, dass er auf alle eingehenden verbindungsanfragen lauscht.  
der port sollte der gleiche wie bei der android-app sein.  

starten sie den api-server mit dem *http* protokoll.  
öffnen sie ihren webbrowser und rufen sie die seite *http://localhost:5274* auf.  
sie sollten jetzt eine swagger open api dokumentation sehen.

sobald der server läuft, öffnen sie die datei *FlaschenpostApi.http*.  
hier gibt es ein POST request, welches ein paar demo-daten an den server sendet.  
mit dem GET request kann geprüft werden, ob der server die daten erhalten hat.

---

## Testen

starten sie die android-app. bei einem tap auf *synchronize* werden die auf dem server gespeicherten daten an die app gesendet.  
sobald die einträge angekommen sind, sind folgende interaktionen möglich:  

tab *todo*
* anklicken  
zeigt ein paar details
* von links nach rechts wischen  
todo wird als *erledigt* markiert

tab *done*
* anklicken  
zeigt details und wann das todo als *erledigt* markiert wurde
* von rechts nach links wischen  
todo wird als *unerledigt* markiert
* von links nach rechts wischen  
todo kann nach bestätigung endgültig gelöscht werden (wird auch vom server gelöscht!)

die daten in der android-app werden nicht dauerhaft gespeichert, d.h. sobald die app beendet wird, gehen nicht versendete änderungen (z.b. wenn das gerät offline ist) verloren.

sobald die app neu gestartet und synchronisiert wird, überträgt der server den zuletzt gespeicherten stand von todo und done.

---

## xUnit

das api-backend kann über das projekt *ApiTest* via xUnit-Tests getestet werden. 