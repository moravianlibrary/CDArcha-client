   M S B A M L   `   `   `  ����  CDArcha_klient    CDArcha_klient.HelpWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Návod��5   	   $	��500��6   $	��500��6$   $	��300��60    � WindowStartupLocation$ CenterScreen=�6?    � 
ResizeMode$ CanResizeWithGrip=�6d   .��5      � ��5   
   �� 5       j� Viewport$ 0,0,218,218�6    j� ViewportUnits$ Absolute=�60    j� Stretch$
 None=�6I    j� TileMode$
 Tile=�6X    �� ImageSource$? 9/CDArcha_klient;component/Images/ok-background-center.png��5      .+�5
   
   �� .��5      �� .@�5      �� $��Návod��6   $��Arial)�6(   $��20(�6;   $
��Bold"�6I   ���#5e5e5e$��Center=�5      $	��Top=�65   �� 5      $��Přihlašovací údaje��6   $��14(�65   $
��Bold"�6C   $��Center=�5      $	��Top=�65   $��Arial)�6M   ���#5e5e5e6`   �� 5      2  ,System.Windows.Markup.XmlAttributeProperties  XmlSpace preserve5   -   
������ $��0q�5   3   	 �� 
IsReadOnly	 . 
 �� TextWrapping$

 Wrap=�6Y   $��Arial)�5      ���#5e5e5e.��6B   ��
Uživ. jméno: Přihlašovací jméno do systému ObálkyKnih.cz
Registrace na http://obalkyknih.cz/signup

Heslo: Heslo do systému ObálkyKnih.cz, které jste zadali při registraci
                5      �� 5      $��
Nastavení��6   $��14(�6,   $
��Bold"�6:   $��Center=�6L   $	��Top=�6i   $��Arial)�5      ���#5e5e5e6+   �� 5       preserve5   -   
������ $��0q�5   3   	 . $

 Wrap=�6Y   $��Arial)�5      ���#5e5e5e.��6B   ��
Sigla: Sigla, která byla přidělena vaší knihovně (tvoří ji 3 písmena a 3 čísla)

Grafický editor: Pokud chcete používat na úpravu obrázků externí grafický editor, je nutné nastavit cestu k němu (k jeho .exe souboru)

Dále je nutné vybrat zda se mají metadata stahovat přes Z39.50 nebo X-Server.

Z39.50 Server URL: URL adresa Z39.50 serveru

Z39.50 Port: Port, na kterém je Z39.50 server dostupný

Z39.50 Databáze: Databáze, ve které se nachází hledaný záznam

Z39.50 Kódování: Kódování, ve kterém jsou poskytovány záznamy v Z39.50

Vyhledávací atribut: Číslo, pod kterým je možné vyhledávat podle čárového kódu, mělo by to být 1063, ale někde se to může lišit.

Z39.50 Už. jméno: Uživatelské jméno k Z39.50 serveru pokud je nějaké, většinou žádné není nastaveno

Z39.50 Heslo: Heslo z Z39.50 serveru pokud je nějaké, většinou není nastaveno

X-Server URL: URL adresa X-Serveru

X-Server Databáze: Databáze, ve které se nachází hledaný záznam
                52      �� 53      $��Aktualizace��6   $��14(�6.   $
��Bold"�6<   $��Center=�6N   $	��Top=�6k   $��Arial)�54      ���#5e5e5e6+   �� 55       preserve56   -   
������ $��0q�55   3   	 . $

 Wrap=�6Y   $��Arial)�56      ���#5e5e5e.��6B   ��
Při každém spuštění aplikace a před každým odesláním na server je kontrolována verze proti serveru obalkyknih.cz, proto je nutné pro požívání aplikace mít stálé internetové připojení.
Aplikace se dá aktualizovat vždy po zapnutí z úvodní obrazovky, nebo taky kdykoliv z menu (Soubor -> Aktualizovat)
Po stažení aktualizace ji není nutné instalovat okamžitě, je možné nainstalovat ji kdykoliv dvojitým poklepáním na zelený progress bar v stavovém řádku na spodku aplikace.
                    5:      �� 5;      $��Klávesové zkratky��6   $��14(�64   $
��Bold"�6B   $��Center=�6T   $	��Top=�6q   $��Arial)�5<      ���#5e5e5e6+   �� 5=       preserve5>   -   
������ $��0q�5=   3   	 . $

 Wrap=�6Y   $��Arial)�5>      ���#5e5e5e.��6B   ��
Ctrl + N = Založení nové jednotky (Menu -> Soubor ->Nový)

Ctrl + Šipka doprava = Rotace vybraného obrázku o 90° vpravo

Ctrl + Šipka doleva = Rotace vybraného obrázku o 90° vlevo

Ctrl + R = Rotace vybraného obrázku o 180°

Ctrl + H = Překlopení vybraného obrázku horizontálně

Ctrl + C = Ořezaní nastaveného výběru v obrázku

Ctrl + D = Narovnání obrázku podle sklonu textu

Ctrl + Tabulátor = Pohyb mezi záložkami

Ctrl + Z = Vrátí poslední změnu

Ctrl + Y = Zopakuje poslední změnu
                5R      �� 5S      $��Postup práce��6   $��14(�6/   $
��Bold"�6=   $��Center=�6O   $	��Top=�6l   $��Arial)�5T      ���#5e5e5e6+   �� 5U       preserve5V   -   
������ $��0q�5U   3   	 . $

 Wrap=�6Y   $��Arial)�5V      ���#5e5e5e.��6B   ��
Proces začíná založením nového záznamu (Soubor -> Nový [Ctrl + N]), objeví se nové okno, do kterého zadáte nebo naskenujete čárový kód záznamu (skenujte čárový kód vydaný vaší institucí, ne oficiální ISBN/EAN čárový kód.)

Po zadání čárového kódu a potvrzení se automaticky začnou stahovat metadata přes Z39.50 nebo X-Server a též obrázky obálky a obsahu ze serveru obálek knih. Počas stahování je ve stavovém řádku informace, že probíhá stahování. Takhle jednoduše vidíte, zda je nebo není daný záznam v systému obálek knih.

Dvojitým poklepáním na stažený náhled obsahu se začne stahovat pdf obsahu a otevře se v předvoleném pdf prohlížeči.

Je nutné mít vyplněný Název a alespoň jeden identifikátor. Pokud záznam nemá ISBN, ISSN, ČNB, OCLC, EAN ani URN:NBN, je vždy možné zadat jednoznačný identifikátor ve vašem systému (například systémové číslo), k tomuto identifikátoru se pak automaticky připojí sigla knihovny.

Další fází je naskenovaní obálky, její ořezání, případné otočení a úprava a kontrastu, pokud je to nezbytné. Obdobně funguje taky skenování obsahu.

Když je všechno naskenované a upravené je nutné vybrat zda se má na serveru poslat obsah na OCR. Pak už je možné odeslat všechno na obálky knih.

Když odeslání skončí správně uvidíte na kontrolní záložce obrázky obálky a obsahu.
                5d      5e      5f      5g      5h      