   M S B A M L   `   `   `  ����   CDArcha_klient#    CDArcha_klient.SettingsWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  
Nastavení��$	��564��$	��600�� � 
ResizeMode$ NoResize=� � WindowStartupLocation$ CenterScreen=�.��� ����  j� Viewport$ 0,0,218,218� j� ViewportUnits$ Absolute=� j� Stretch$
 None=� j� TileMode$
 Tile=� �� ImageSource$? 9/CDArcha_klient;component/Images/ok-background-center.png��.+���  � 	Resources 	 �� 
TargetType
 �� Property �� Value �� 
StartPoint �� EndPoint � Color � Offset �� 
TargetType ��Name �� Triggers P� Property P� Value �� 
TargetName��%�  '��       '�� �     �� "	 ��.��� 
 � ��$ 20���� 
 � ��$ 0q��� 
 � �� �� $ 0.5,0&�$ 0.5,1&�.��� $ #404143��$	 0.0Q�� $ #2F2F2F��$	 1.0Q��� "	 ��.��� 
 � �� �#B6B6B6�� 
 � ��$ 20���� 
 � �� �� $ 0.5,0&�$ 0.5,1&�.��� $ #404143��$	 0.0Q�� $ #2F2F2F��$	 1.0Q��� 
 � �� �� " ��.R�� .+��� $��Border�����#373737$��1,1,1,1q�._��� $ ContentSite��$��Center=�$��Center=�$��Header��$��	12,2,12,2q���.  P�  �@��
IsSelected . . ��� $ Border��
 � �� �#1C1C1C�� $ Border��
 � ��$ 1,1,1,0q�.@��� $��Připojení��.��� ���� $ 0,0,218,218�$ Absolute=�$
 None=�$
 Tile=�$? 9/CDArcha_klient;component/Images/ok-background-center.png�� � ColumnDefinitions �� $��* ��� $��* �.+��� $��
Nastavení��$��2��$��20(�$
��Bold"����#5e5e5e$��Arial)�$��Center=�$	��Top=��� $��Sigla Knihovny:��$��28��$
��Left=�$��
30,101,0,0q�$��Arial)����#5e5e5e$	��Top=��� -   $��siglaTextBox��$��23��$
��Left=�$��145,101,0,0q�$��Arial)����#5e5e5e$	��Top=�$	��120���� $��Báze záznamu:��$��28��$
��Left=�$��
21,101,0,0q�$��Arial)����#5e5e5e$	��Top=�$��1���� -   $��baseTextBox��$��23��$
��Left=�$��129,101,0,0q�$��Arial)����#5e5e5e$	��Top=�$	��120��$��1�� � ToolTip$6 0Báze bude použita u vlastního identifikátoru��	� -   $��xServerRadioButton��$��Používat X-Server:��$��17��$
��Left=�$��Arial)����#5e5e5e$��
21,147,0,0q�$	��Top=�$��1���� $��X-Server URL:��$��28��$
��Left=�$��
21,174,0,0q�$��Arial)����#5e5e5e$	��Top=�$��1���� $��X-Server Databáze:��$��28��$
��Left=�$��
21,203,0,0q�$��Arial)����#5e5e5e$	��Top=�$��1���� -   $��xServerUrlTextBox��$��23��$
��Left=�$��129,174,0,0q�$��Arial)����#5e5e5e$	��Top=�$	��120��$��1���� -   $��xServerDatabaseTextBox��$��23��$
��Left=�$��129,203,0,0q�$��Arial)����#5e5e5e$	��Top=�$	��120��$��1��	� -   $��z39ServerRadioButton��$��Používat Z39.50:��$��17��$
��Left=�$��
30,147,0,0q�$��Arial)����#5e5e5e$	��Top=��� $��Z39.50 Server URL:��$��28��$
��Left=�$��
30,174,0,0q�$��Arial)����#5e5e5e$	��Top=��� $��Z39.50 Server Port:��$��28��$
��Left=�$��
30,203,0,0q�$��Arial)����#5e5e5e$	��Top=��� $��Z39.50 Databáze:��$��28��$
��Left=�$��
30,232,0,0q�$��Arial)����#5e5e5e$	��Top=��� $��Z39.50 Kódování:��$��28��$
��Left=�$��
30,261,0,0q�$��Arial)����#5e5e5e$	��Top=��� $��Z39.50 Už. jméno:��$��28��$
��Left=�$��
30,289,0,0q�$��Arial)����#5e5e5e$	��Top=��� $��Z39.50 Heslo:��$��28��$
��Left=�$��
30,318,0,0q�$��Arial)����#5e5e5e$	��Top=��� -   $��z39ServerTextBox��$��23��$
��Left=�$��145,174,0,0q�$��Arial)����#5e5e5e$	��Top=�$	��120���� -   $��z39PortTextBox��$��23��$
��Left=�$��145,203,0,0q�$��Arial)����#5e5e5e$	��Top=�$	��120���� -	   $��z39DatabaseTextBox��$��23��$
��Left=�$��145,232,0,0q�$��Arial)����#5e5e5e$	��Top=�$	��120���� -
   $��z39UserNameTextBox��$��23��$
��Left=�$��145,289,0,0q�$��Arial)����#5e5e5e$	��Top=�$	��120���� -   $��z39PasswordTextBox��$��23��$
��Left=�$��145,318,0,0q�$��Arial)����#5e5e5e$	��Top=�$	��120���� -   $��z39EncodingComboBox��$��23��$
��Left=�$��145,261,0,0q�$��Arial)����#5e5e5e$	��Top=�$	��120�� �� 
IsReadOnly .  �� 
IsEditable .  {�.  .@��� $��UTF-8 (Unicode)�� �� 
IsSelected . �� $��Windows-1250���� $��Marc-8���� -   $��	groupBox1��$�� Vyhledávací atributy ��$��98��$
��Left=�$��
30,350,0,0q�$	��Top=�$	��503��$��2��.��� .+��� $��Čárový kód:��$��Arial)����#5e5e5e$��28��$
��Left=�$��10,7,0,0q�$	��Top=��� -   $��z39BarcodeField��$��Arial)����#5e5e5e$��23��$
��Left=�$��	109,8,0,0q�$	��Top=�$	��120���� $��ČNB:��$��Arial)����#5e5e5e$��28��$
��Left=�$��	272,7,0,0q�$	��Top=��� -   $��z39CnbField��$��Arial)����#5e5e5e$��23��$
��Left=�$��	354,9,0,0q�$	��Top=�$	��120���� $��ISMN:��$��Arial)����#5e5e5e$��28��$
��Left=�$��
272,36,0,0q�$	��Top=��� -   $��z39IsmnField��$��Arial)����#5e5e5e$��23��$
��Left=�$��
354,38,0,0q�$	��Top=�$	��120���� $
��EAN:��$��Arial)����#5e5e5e$��28��$
��Left=�$��	10,36,0,0q�$	��Top=��� -   $��z39EanField��$��Arial)����#5e5e5e$��23��$
��Left=�$��
109,38,0,0q�$	��Top=�$	��120���� -    ��Name$ apiUrlTextBox��$��23��$
��Left=�$��
145,41,0,0q�$��Arial)����#5e5e5e$	��Top=�$	��396��$��2���� $��API URL:��$��28��$
��Left=�$��	30,41,0,0q�$��Arial)����#5e5e5e$	��Top=��� $��Přizpůsobení��.��� ���� $ 0,0,218,218�$ Absolute=�$
 None=�$
 Tile=�$? 9/CDArcha_klient;component/Images/ok-background-center.png�� �� $��* ��� $��* �.+��� $ ��Přizpůsobení nastavení��$��2��$��20(�$
��Bold"����#5e5e5e$��Arial)�$��Center=�$	��Top=��� -   $(��"disableClosingConfirmationCheckBox��$*��$Neptát se při vypínání aplikace��$��0��$��28��$
��Left=�$��	21,43,0,0q�$��Arial)����#5e5e5e$	��Top=�$V PPři vypínání se neobjeví potvrzovací dialog, ale rovnou se vypne aplikace.���� -   $#��alwaysDownloadUpdatesCheckBox��$&�� Automaticky stahovat aktualizace��$��0��$��28��$
��Left=�$��	21,68,0,0q�$��Arial)����#5e5e5e$	��Top=�$: 4Při dostupnosti nové verzi ji automaticky stáhne.���� -   $"��neverDownloadUpdatesCheckBox��$"��Nikdy nestahovat aktualizace��$��0��$��28��$
��Left=�$��	21,93,0,0q�$��Arial)����#5e5e5e$	��Top=�$Q KNebude se ptát na stažení nové verze, pokud je současná podporovaná.���� -   $2��,disableMissingAuthorYearNotificationCheckBox��$7��1Ignorovat chybějícího autora nebo rok vydání��$��1��$��28��$
��Left=�$��	10,43,0,0q�$��Arial)����#5e5e5e$	��Top=�$j dPři odesílání záznamu bez autora nebo roku vydání se nebude ptát, ale rovnou záznam odešle���� -   $-��'disableWithoutCoverNotificationCheckBox��$$��Ignorovat chybějící obálku��$��28��$
��Left=�$��
21,117,0,0q�$��Arial)����#5e5e5e$	��Top=�$X RPři odesílání záznamu bez obálky se nebude ptát, ale rovnou záznam odešle��$y�Hidden=��� -   $+��%disableWithoutTocNotificationCheckBox��$"��Ignorovat chybějící obsah��$��28��$
��Left=�$��
21,142,0,0q�$��Arial)����#5e5e5e$	��Top=�$W QPři odesílání záznamu bez obsahu se nebude ptát, ale rovnou záznam odešle��$y�Hidden=��� -   $.��(disableCoverDeletionNotificationCheckBox��$,��&Neptat se při odstraňování obálky��$��28��$
��Left=�$��
21,167,0,0q�$��Arial)����#5e5e5e$	��Top=�$Z TPři odstraňování obálky se nebude ptát na potvrzení, ale rovnou ji odstrání��$y�Hidden=��� -   $,��&disableTocDeletionNotificationCheckBox��$+��%Neptat se při odstraňování obsahu��$��28��$
��Left=�$��
21,192,0,0q�$��Arial)����#5e5e5e$	��Top=�$Y SPři odstraňování obsahu se nebude ptát na potvrzení, ale rovnou ho odstrání��$y�Hidden=��� -   $1��+disableCustomIdentifierNotificationCheckBox��$4��.Neupozorňovat při vlastním identifikátoru ��$��1��$��28��$
��Left=�$��	10,68,0,0q�$��Arial)����#5e5e5e$	��Top=�$p jPři použití vlastního identifikátoru nebude na nic upozorňovat a zobrazovat finální identifikátor���� -   $&�� disableResolveIdentifierCheckBox��$'��!Nerozeznávat typ identifikátoru��$��Arial)����#5e5e5e$��35��$
��Left=�$��	10,93,0,0q�$p jPři použití vlastního identifikátoru nebude na nic upozorňovat a zobrazovat finální identifikátor��$	��Top=�$��1��$	��257���� -   $��label1��$5��/(v dialogovém okně pro zakládání záznamu)��$��1��$��34��$
��Left=�$��
23,103,0,0q�$	��Top=�$	��255�����	#FF909090�� -   $��enableLocalImageCopy��$5��/Ukládat lokální kopie skenovaných obrázků��$��Arial)����#5e5e5e$��24��$
��Left=�$��
21,216,0,0q�$Q KNebude se ptát na stažení nové verze, pokud je současná podporovaná.��$	��Top=�$��2��$	��425��$y�Hidden=��� -   $��
rectangle1��$��2��$��2��$
��Left=�$��
14,237,0,0q����	#FFCECECE$	��Top=�$	��537���� -    $��scanOutputDir��$��23��$��21,314,134,0q�$	��Top=�$��2�� �� 
IsReadOnly . $y�Hidden=��� -!   $��chooseScanOutputDir��$��Zvolit adresář��$��1��$��23��$
��Left=�$��151,313,0,0q�$	��Top=�$	��109��$y�Hidden=��� -"   $��tmpDir��$��2��$��23�� . $��21,284,134,0q�$	��Top=��� -#   $��	tmpDirBtn��$��Zvolit adresář��$��1��$��23��$
��Left=�$��151,283,0,0q�$	��Top=�$	��109���� -$   $��label6��$M��GAdresář pro ukládání dočasných souborů počas archivace média:��$��28��$
��Left=�$��
18,260,0,0q�$	��Top=�$��2��$	��347���� $��Kompatibilita��$y�Hidden=�.��� ���� $? 9/CDArcha_klient;component/Images/ok-background-center.png��$
 None=�$
 Tile=�$ 0,0,218,218�$ Absolute=� �� $	��291 ��� $	��281 �.+��� $��Nastavení skenování��$��Arial)�$��20(�$
��Bold"����#5e5e5e$��2��$��Center=�$	��Top=��� -%   $��scanLowDataFlowCheckBox��$-��'Snížit datový tok mezi skenerem a PC��$��Arial)����#5e5e5e$��28��$
��Left=�$��
21,100,0,0q�$Y SVhodné zapnout v případě, že skenování po odskenování pár stránek spadne��$	��Top=��� -&   $��scanLowResCheckBox��$��Snížit rozlíšení��$��Arial)����#5e5e5e$��28��$
��Left=�$��
21,147,0,0q�$V PVhodné zapnout v případě, že Váš skener čeká /chladí/ příliš často��$	��Top=��� -'   $��label2��$[��U(vhodné zapnout v případě, že skenování po odskenování pár stránek spadne)�����	#FF909090$��34��$��
36,110,0,0q�$	��Top=�$
��Left=�$	��518��$��2���� -(   $��label3��$^��XTato záložka slouží k doladění nastavení v případě problémů se seknováním.�����	#FF909090$��34��$
��Left=�$��	18,34,0,0q�$	��Top=�$	��517��$��2���� -)   $��
rectangle2��$��2��$��2��$
��Left=�$��	17,85,0,0q����	#FFCECECE$	��Top=�$	��537���� -*   $��label4��$g��aPokud nepociťujete problémy v průběhu skenování, doporučujeme všechna nastavení vypnout.�����	#FF909090$��2��$��34��$
��Left=�$��	19,52,0,0q�$	��Top=�$	��517���� -+   $��label5��$X��R(vhodné zapnout v případě, že Váš skener čeká /chladí/ příliš často)�����	#FF909090$��2��$��34��$
��Left=�$��
36,157,0,0q�$	��Top=�$	��518��� $��Center=�$��180,413,189,0q�$	��209�� �� $
��123* ��� $	��77* �.+��� -,   $��
saveButton��$��Uložit nastavení��$��25��$	��Top=�   MyButton#��[  ���#CECECE���#656565$��1q� � Cursor$
 Hand��$��	-4,78,0,0q�$
��Left=�$	��133������ $ 0.5,0&�$ 0.5,1&�.��� $ #535353��$	 0.0Q�� $ #3d3d3d��$	 1.0Q��� $��Zrušit��$��25��$��Right=�$	��Top=�#��[   �� IsCancel . ���#CECECE���#656565$��1q�$
 Hand��$��1��$��	0,78,-9,0q����� $ 0.5,0&�$ 0.5,1&�.��� $ #535353��$	 0.0Q�� $ #3d3d3d��$	 1.0Q��� $g��aURL adresa serverové části CDArcha např. https://cdarcha.mzk.cz, http://127.0.0.1:1337, apod.��$��28��$
��Left=�$��
146,89,0,0q�$��Arial)����	#B2363636$	��Top=�$	��417��$��9(�