Je desktopová aplikace pro účely archivace datových médií CD, DVD, USB disků a disket v prostředí knihoven.

'''Instalátor uživatelské verze''' (pro uživatele bez admin práv na instalovaný PC):
https://cdarcha.mzk.cz/cdarcha_klient/CDArcha_klient_setupNoAdmin.exe

'''Instalátor administrátorské verze''' (pro uživatele s admin právy na instalovaný PC):
https://cdarcha.mzk.cz/cdarcha_klient/CDArcha_klient_setup.exe


Tuto desktopovou aplikaci je možné provozovat:
* '''V režimu online''' - kdy archivováná média jsou zasíláná na serverovou část CDarcha server.
* '''V režimu offline''' - kdy archivováná média jsou ukládáná ve formě ISO souborů lokálně na PC.

Je možná archivace medií:
* Optických datových médií CD/DVD - archivační klient vytvoří obraz (jeden .iso soubor) celého média se zachovaním originálního systému souborů (i kombinací ISO 9660 + Joliet + UDF).
* Pružných médií a USB disků - archivační klient vytvoří jede .iso soubor, který bude obsahovat soubory originálního média. Nevytváří se obraz. Pokud je archivován např. 8GB flash disk a obsahuje 10MB dat, bude výsledný .iso soubor velikosti 10MB.

Společne s .iso archivem je archivován i MARC metadatový záznam daného titulu (platí pro online režim). Pracovní postup zkráceně:
* Vyhledání titulu pomocí Z39.50, nebo X-Server, kterého médium se bude archivovat.
* Načtení média. Součastí procesu načítání je i kontrola už v minulost proběhlé archivace stejného média.
* Archivace - vytváření .iso archivu.
* Přenos na server. Součástí přenosu je i následná kontrola na serveru a klientovi pomocí kontrolního součtu a porovnání.
* Skenování - vytvoření reprezentatívní foto média, obalu a bookletů.

Další info viz. wiki stránka projektu.
