   M S B A M L   `   `   `  ���� N  JCDArcha_klient, Version=1.6.8131.494, Culture=neutral, PublicKeyToken=null/clr-namespace:CDArcha_klientCDArcha_klient       CDArcha_klient.TabsControl   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     ,srcclr-namespace:CDArcha_klient     -   $��TabsControlUserControl�����#363636$y�Visible=�.���� -   $��
tabControl��$��0q�$��0q�  � 	Resources   �� 
TargetType �� Property �� Value �� 
StartPoint �� EndPoint � Color � Offset �� 
TargetType	 ��Name
 �� Triggers P� Property P� Value �� 
TargetName��%�  '��       '�� �     �� " ��.���  � ��$ 26����  � ��$ 0q���  � �� �� $ 0.5,0&�$ 0.5,1&�.��� $ #404143��$	 0.0Q�� $ #2F2F2F��$	 1.0Q��� " ��.���  � �� �#B6B6B6��  � ��$ 26����  � �� �� $ 0.5,0&�$ 0.5,1&�.��� $ #404143��$	 0.0Q�� $ #2F2F2F��$	 1.0Q���  � �� �� " ��.R�� .+��� $��Border�����#373737$��1,1,1,1q�._��� $	 ContentSite��$��Center=�$��Center=�$��Header��$��	12,2,12,2q���. 
 P�  �@��
IsSelected . . ��� $ Border�� � �� �#1C1C1C�� $ Border�� � ��$ 1,1,1,0q�.@��� -    ��Name$ metadataTabItem��$��Metadata��.��� ����  j� Viewport$ 0,0,218,218� j� ViewportUnits$ Absolute=� j� Stretch$
 None=� j� TileMode$
 Tile=� �� ImageSource$? 9/CDArcha_klient;component/Images/ok-background-center.png�� � ColumnDefinitions �� -    ��Name$ 
metadataCD��$
��Auto ��� $
��Auto ��� $	��300��.+��� -    ��Name$ image1��$��2��$
��Fill=�$��Bottom=�$	��250��$��Images/CD-icon.png��$	��250��$��Right=�$��0,0,-40,-60q��� $��Center=�$��Stretch=�$��1��$��5�� �� ResizeBehavior$ PreviousAndNext=����� $ 0,0,218,218�$ Absolute=�$
 None=�$
 Tile=�$? 9/CDArcha_klient;component/Images/ok-background-center.png���� $��0��$��Hidden=�.��� -    �Name$ 
recordGrid�� �� ���� $ 0,0,204,204�$ Absolute=�$
 None=�$
 Tile=�$= 7/CDArcha_klient;component/Images/ok-background-left.png��.+�� -   $ partRecordGrid��$	��230�� �� $��2* �.+��� $��Informace o části��$��Arial)����#858585$
��Left=�$��13,5,0,0q�$��14(�$
��Bold"�$	��Top=��� $��Název��$��Arial)����#858585$
��Left=�$��	13,25,0,0q�$	��Top=��� $��Autoři��$��Arial)����#858585$
��Left=�$��	13,69,0,0q�$	��Top=��� -    ��Name$ partYearLabel��$	��Rok��$��Arial)����#858585$
��Left=�$��
13,113,0,0q�$	��Top=��� -	   $ partNumberLabel��$��Číslo části��$��Arial)����#858585$
��Left=�$��
13,199,0,0q�$	��Top=��� -
   $ partVolumeLabel��$��Ročník��$��Arial)����#858585$
��Left=�$��
13,155,0,0q�$	��Top=��� -   $ partNameLabel��$��Název části��$��Arial)����#858585$
��Left=�$��
13,245,0,0q�$	��Top=��� $��Identifikátory��$��Arial)����#858585$��14(�$
��Left=�$��
13,292,0,0q�$	��Top=�$
��Bold"� � ToolTip$4 .Je nutné uvést alespoň jeden identifikátor���� -   $ partIsbnLabel��$��ISBN / EAN / UPC��$��Arial)����#858585$
��Left=�$��
13,312,0,0q�$	��Top=�$y�	Collapsed=�{�.  �� -   $ partIssnLabel��$
��ISSN��$��Arial)����#858585$
��Left=�$��
13,312,0,0q�$	��Top=�$y�	Collapsed=�{�.  �� -   $ partCnbLabel��$
��ČNB��$��Arial)����#858585$
��Left=�$��
13,356,0,0q�$	��Top=��� -   $ partOclcLabel��$
��OCLC��$��Arial)����#858585$
��Left=�$��
13,400,0,0q�$	��Top=��� -   $ partIsmnLabel��$
��ISMN��$��Arial)����#858585$
��Left=�$��
13,444,0,0q�$	��Top=��� -   $ partUrnNbnLabel��$��URN:NBN��$��Arial)����#858585$
��Left=�$��
13,488,0,0q�$	��Top=��� $��Vlastní��$��Arial)����#858585$
��Left=�$��
13,532,0,0q�$	��Top=��� -    ��Name$ partTitleTextBox��$��22��$��0,2,0,0q�$��
16,45,36,0q�$��Arial)����#363636���#c0c0c0$	��Top=����#111111$ Název díla���� -   $ partAuthorTextBox��$��22��$��0,2,0,0q�$��
16,89,36,0q�$��Arial)����#363636���#c0c0c0$	��Top=����#111111$&  Seznam autorů oddělen čárkou���� -   $ partYearTextBox��$��22��$��0,2,0,0q�$��16,133,36,0q�$��Arial)����#363636���#CECECE$	��Top=����#111111$! Rok vydání díla (svazku)���� -   $ partNumberTextBox��$��22��$��0,2,0,0q�$��16,219,36,0q�$��Arial)����	#FF626262���#CECECE$	��Top=����#111111$ Číslo díla (svazku)���� -   $ partVolumeTextBox��$��22��$��0,2,0,0q�$��16,175,36,0q�$��Arial)����	#FF626262���#CECECE$	��Top=����#111111$ Ročník díla (svazku)���� -   $ partNameTextBox��$��22��$��0,2,0,0q�$��16,265,36,0q�$��Arial)����	#FF626262���#CECECE$	��Top=����#111111$- 'Poznámky týkající se díla (svazku)���� -   $ partIsbnTextBox��$��22��$��0,2,0,0q�$��16,332,36,0q�$��Arial)����#363636���#CECECE$	��Top=����#111111$) #ISBN číslo díla (pokud existuje)��{�.  $y�	Collapsed=��� -   $ partIssnTextBox��$��22��$��0,2,0,0q�$��16,332,36,0q�$��Arial)����#363636���#CECECE$	��Top=����#111111$) #ISSN číslo díla (pokud existuje)��{�.  $y�	Collapsed=��� -   $ partCnbTextBox��$��22��$��0,2,0,0q�$��16,376,36,0q�$��Arial)����#363636���#CECECE$	��Top=����#111111$0 *ČNB identifikátor díla (pokud existuje)���� -   $ partOclcTextBox��$��22��$��0,2,0,0q�$��16,420,36,0q�$��Arial)����#363636���#CECECE$	��Top=����#111111$) #OCLC číslo díla (pokud existuje)���� -   $ partIsmnTextBox��$��22��$��0,2,0,0q�$��16,464,36,0q�$��Arial)����#363636���#CECECE$	��Top=����#111111$) #ISMN číslo díla (pokud existuje)���� -   $ partUrnNbnTextBox��$��22��$��0,2,0,0q�$��16,508,36,0q�$��Arial)����#363636���#CECECE$	��Top=����#111111$3 -URN:NBN identifikátor díla (pokud existuje)���� -   $ partSiglaTextBox��$��22��$��0,2,0,0q�$��16,552,36,0q�$��Arial)����#363636���#CECECE$	��Top=����#111111$� �Váš unikátní identifikátor díla, například systémové číslo nebo čárový kód (vyplňte jen pokud neexistuje jiný identifikátor)���� -   $ partYearWarning��$��
0,135,10,0q�$��Uniform=�$	��Top=�$y�Hidden=�$��18��$:��4/CDArcha_klient;component/Images/ok-icon-warning.png��$��Right=��� -    $ partNumberWarning��$��
0,221,10,0q�$��Uniform=�$	��Top=�$y�Hidden=�$��18��$:��4/CDArcha_klient;component/Images/ok-icon-warning.png��$��Right=��� -!   $ partVolumeWarning��$��
0,176,10,0q�$��Uniform=�$	��Top=�$y�Hidden=�$��18��$:��4/CDArcha_klient;component/Images/ok-icon-warning.png��$��Right=��� -"   $ partIsbnWarning��$��
0,334,10,0q�$��Uniform=�$	��Top=�$y�Hidden=�$��18��$:��4/CDArcha_klient;component/Images/ok-icon-warning.png��$��Right=��� -#   $ partIssnWarning��$��
0,334,10,0q�$��Uniform=�$	��Top=�$y�Hidden=�$��18��$:��4/CDArcha_klient;component/Images/ok-icon-warning.png��$��Right=��� -$   $ partCnbWarning��$��
0,378,10,0q�$��Uniform=�$	��Top=�$y�Hidden=�$��18��$:��4/CDArcha_klient;component/Images/ok-icon-warning.png��$��Right=��� -%   $ partOclcWarning��$��
0,422,10,0q�$��Uniform=�$	��Top=�$y�Hidden=�$��18��$:��4/CDArcha_klient;component/Images/ok-icon-warning.png��$��Right=��� -&   $ partIsmnWarning��$��
0,466,10,0q�$��Uniform=�$	��Top=�$y�Hidden=�$��18��$:��4/CDArcha_klient;component/Images/ok-icon-warning.png��$��Right=��� -'   $ partUrnWarning��$��
0,510,10,0q�$��Uniform=�$	��Top=�$y�Hidden=�$��18��$:��4/CDArcha_klient;component/Images/ok-icon-warning.png��$��Right=��� -(   $ partSiglaWarning��$��
0,554,10,0q�$��Uniform=�$	��Top=�$y�Hidden=�$��18��$:��4/CDArcha_klient;component/Images/ok-icon-warning.png��$��Right=��� -)    ��Name$ downloadMetadataButton��$��Right=�$��0,6,33,0q����#b4b4b4$��30�����#353535 � Cursor$
 Hand��$	��Top=�$��30��$��1q�$@ :Opakovaně stáhnout informace z katalogu a z Obálek Knih��   MyButtonNoPadding#��[  ���� $ 0.5,0&�$ 0.5,1&�.��� $ #484848��$	 0.0Q�� $ #363636��$	 1.0Q�.���� $��25��$��25��$��Center=�$��Center=�$��Uniform=�$7��1/CDArcha_klient;component/Images/refresh-icon.png���� -*   $ addUnionButton��$!��Připojit souborný záznam��$��30��$��Center=�$��16,164,34,0q����#b4b4b4���#353535$
 Hand��$	��Top=�$	��180��$��1q�#��[  $? 9Vyhledat souborný záznam v katalogu Souborném katalogu��$y�Hidden=����� $ 0.5,0&�$ 0.5,1&�.��� $ 	#FF9B4848��$	 0.0Q�� $ 	#FF512013��$	 1.0Q��� -+   $ optionalAtributesLink��$(��"Další nepovinné identifikátory��$
 Hand��$��Arial)����#858585$
��Left=�$��
13,356,0,0q�$	��Top=�$y�Hidden=�$	��167��� -,   $ collectionRecordGrid��$	��230��$��1��$y�	Collapsed=�.+��� $��Informace SZ��$��Arial)����#858585$
��Left=�$��13,5,0,0q�$��14(�$
��Bold"�$	��Top=��� $��Název��$��Arial)����#858585$
��Left=�$��	13,25,0,0q�$	��Top=��� $��Autoři��$��Arial)����#858585$
��Left=�$��	13,69,0,0q�$	��Top=��� $��Rok vydání��$��Arial)����#858585$��
13,113,0,0q�$	��Top=�$
��Left=��� $��Identifikátory SZ��$��Arial)����#858585$��14(�$
��Left=�$��
13,292,0,0q�$	��Top=�$
��Bold"�$4 .Je nutné uvést alespoň jeden identifikátor���� --   $ 	isbnLabel��$��ISBN / EAN / UPC��$��Arial)����#858585$
��Left=�$��
13,312,0,0q�$	��Top=�$y�	Collapsed=�{�.  �� -.   $ cnbTextLabel��$
��ČNB��$��Arial)����#858585$
��Left=�$��
13,356,0,0q�$	��Top=��� -/   $ oclcTextLabel��$
��OCLC��$��Arial)����#858585$
��Left=�$��
13,400,0,0q�$	��Top=��� -0   $ ismnTextLabel��$
��ISMN��$��Arial)����#858585$
��Left=�$��
13,444,0,0q�$	��Top=��� -1   $ urnNbnTextLabel��$��URN:NBN��$��Arial)����#858585$
��Left=�$��
13,488,0,0q�$	��Top=��� -2   $ siglaTextLabel��$��Vlastní��$��Arial)����#858585$
��Left=�$��
13,532,0,0q�$	��Top=��� -3   $ titleTextBox��$��22��$��0,2,0,0q�$��
16,45,36,0q�$��Arial)����#363636���#c0c0c0$	��Top=����#111111$ Název díla���� -4   $ authorTextBox��$��22��$��0,2,0,0q�$��
16,89,36,0q�$��Arial)����#363636���#c0c0c0$	��Top=����#111111$&  Seznam autorů oddělen čárkou���� -5   $ yearTextBox��$��22��$��0,2,0,0q�$��16,133,36,0q�$��Arial)����#363636���#CECECE$	��Top=����#111111$ Rok vydání���� -6   $ isbnTextBox��$��22��$��0,2,0,0q�$��16,332,36,0q�$��Arial)����#363636���#CECECE$	��Top=����#111111$) #ISBN číslo díla (pokud existuje)���� -7   $ 
cnbTextBox��$��22��$��0,2,0,0q�$��16,376,36,0q�$��Arial)����#363636���#CECECE$	��Top=����#111111$0 *ČNB identifikátor díla (pokud existuje)���� -8   $ oclcTextBox��$��22��$��0,2,0,0q�$��16,420,36,0q�$��Arial)����#363636���#CECECE$	��Top=����#111111$) #OCLC číslo díla (pokud existuje)���� -9   $ ismnTextBox��$��22��$��0,2,0,0q�$��16,464,36,0q�$��Arial)����#363636���#CECECE$	��Top=����#111111$) #ISMN číslo díla (pokud existuje)���� -:   $ urnNbnTextBox��$��22��$��0,2,0,0q�$��16,508,36,0q�$��Arial)����#363636���#CECECE$	��Top=����#111111$3 -URN:NBN identifikátor díla (pokud existuje)���� -;   $ siglaTextBox��$��22��$��0,2,0,0q�$��16,552,36,0q�$��Arial)����#363636���#CECECE$	��Top=����#111111$� �Váš unikátní identifikátor díla, například systémové číslo nebo čárový kód (vyplňte jen pokud neexistuje jiný identifikátor)���� -<   $ yearWarning��$��
0,135,10,0q�$��Uniform=�$	��Top=�$y�Hidden=�$��18��$:��4/CDArcha_klient;component/Images/ok-icon-warning.png��$��Right=��� -=   $ isbnWarning��$��
0,334,10,0q�$��Uniform=�$	��Top=�$y�Hidden=�$��18��$:��4/CDArcha_klient;component/Images/ok-icon-warning.png��$��Right=��� ->   $ 
cnbWarning��$��
0,378,10,0q�$��Uniform=�$	��Top=�$y�Hidden=�$��18��$:��4/CDArcha_klient;component/Images/ok-icon-warning.png��$��Right=��� -?   $ oclcWarning��$��
0,422,10,0q�$��Uniform=�$	��Top=�$y�Hidden=�$��18��$:��4/CDArcha_klient;component/Images/ok-icon-warning.png��$��Right=��� -@   $ ismnWarning��$��
0,466,10,0q�$��Uniform=�$	��Top=�$y�Hidden=�$��18��$:��4/CDArcha_klient;component/Images/ok-icon-warning.png��$��Right=��� -A   $ partnoPartnameWarning��$��85��$
��Left=�$
��Fill=�$	��Top=�$	��181��$@��:/CDArcha_klient;component/Images/arrow_partno_partname.png��$��
16,206,0,0q��� -B   $  showCompleteMetadataButton��$"��Zobrazit metadatový záznam��{�.  $��30��$��Center=�$��16,584,34,0q����#b4b4b4���#353535$
 Hand��$	��Top=�$	��180��$��1q�#��[  $? 9Zobrazit úplný metadatový záznam stažený z katalogu��$��2������ $ 0.5,0&�$ 0.5,1&�.��� $ #484848��$	 0.0Q�� $ #363636��$	 1.0Q��� $��2��.���� $��15q�.+�9� -C    9�Name$ expanderStep0��$��Doplňkové informace�����	#19000000.��� -D   $ 	gridStep0��$	��210��.+�� -E   $ gridIdentifiers��$	��185��$��Center=�$��Center=�$��477.75�� � UseLayoutRounding .  ���	#FF790000  � OverridesDefaultStyle  .  $y�Visible=�! K� ZIndex$	! 100��$��203.25,12.5,235,12.5q� �� $	��22* ��� $
��541* ��� $	��21* �.+��� -F   $ label1��$:��4Metadatový záznam obsahuje více identifikátorů.��$
��Left=�$��	14,4,0,91q�
�������$��Light"�$��20(�$��Normal&�$��2��	� -G   " 	�Name$" radioMultipartMonography��$ ��Existuje souborný záznam��$��20��$
��Left=�$��0,44,0,0q�$	��Top=�
�������# 	� 	GroupName$# radioMultipart��$��Normal"�$��14(�{�. $ d� 	IsChecked$
$ TrueY�$��1��	� -H   $" radioMultipartCoedition��$��Bez souborného záznamu��
�������$# radioMultipart��$��20��$��Right=�$��0,44,0,0q�$	��Top=�$��Normal"�$��14(�$��1���� -I   % ��Name$% multipartIdentifierOwn��$��33��$
��Left=�$��
167,94,0,0q�$	��Top=�$	��283��$��14(�& �� SelectedIndex$& -1��$
��10,6q�$��1��$��2���� -J   $! multipartIdentifierOwnLabel��$��Skenovaný dokument:��$��28��$
��Left=�$��	14,94,0,0q�$	��Top=�
�������$��14(�$��2���� -K   $% multipartIdentifierUnion��$��14(�$��33��$��Right=�$��
0,130,10,0q�$	��Top=�$	��283��$
��10,6q�$��1��$��2���� -L   $# multipartIdentifierUnionLabel��$��Souborný záznam:��$��14(�
�������$��28��$
��Left=�$��
14,132,0,0q�$	��Top=�{�. $��2���� -M   ' ��Name$' checkboxMinorPartName��$$��Část s nevýznamným názvem��$��20��$
��Left=�$��0,68,0,0q�$	��Top=�$��14(�
�������$��1���� $[��UNejsou potřebné žádné doplňkové informace k tomuto bibliografickému záznamu.��$��Center=�$��Center=�9� -N   $ expanderStep2��$N��HKrok 1: Existující archivy médií k tomuto bibliografickému záznamu�����	#19000000( 9� 
IsExpanded( . .��� -O   $ 	gridStep2��$	��220��.+��� -P   $ loadArchiveList�����#353535$��1q�$��Obnovit seznam��$
 Hand�����#b4b4b4$��30��$
��Left=�#��[  $ Načíst obsah média��$	��Top=�$	��168��$��
550,10,0,0q����� $ 0.5,1&�$ 0.5,0&�.��� $ #484848��$	 0.0Q�� $ #363636��$	 1.0Q�R� -Q   ) R�Name$) archiveTree��$	��200��$	��Top=����	#66FFFFFF���	#FFA2A2A2���	#FF464646$	��500��$��	25,10,0,0q�$
��Left=��� -R   $ showArchiveItem�����#353535$��1q�$��Zobrazit obsah��$
 Hand�����#b4b4b4$��30��$
��Left=�#��[  $ Načíst obsah média��$	��Top=�$	��168��$��
550,45,0,0q����� $ 0.5,1&�$ 0.5,0&�.��� $ #484848��$	 0.0Q�� $ #363636��$	 1.0Q��� -S   $ archiveStateLabel��$��Stav archivu:��$
��Left=�$	��Top=�$��
550,89,0,0q��� -T   $ archiveState��$�� ��$	��Top=�$��
623,89,0,0q�$
��Bold"��� -U   $ 	archiveOp��$�� ��$	��Top=�$��550,112,0,0q��� -V   $ scanArchiveItem�����#353535$��1q�$��Skenovat��$
 Hand�����#b4b4b4$��30��$
��Left=�#��[  $ Načíst obsah média��$	��Top=�$	��268��$��550,180,0,0q����� $ 0.5,1&�$ 0.5,0&�.��� $ #484848��$	 0.0Q�� $ #363636��$	 1.0Q��� -W   $ closeArchiveItem�����#353535$��1q�$��Uzavřít archiv��$
 Hand�����#b4b4b4$��30��$
��Left=�#��[  $ Načíst obsah média��$	��Top=�$	��168��$��550,145,0,0q����� $ 0.5,1&�$ 0.5,0&�.��� $ #484848��$	 0.0Q�� $ #363636��$	 1.0Q�9� -X   $ expanderStep3��$*��$Krok 2: Načtení média z mechaniky�����	#19000000( . .��� -Y   $ 	gridStep3��$��20,0,0,0q�$	��175��.+��� -Z   $ labelDiscName��$��Název média:��$��28��$
��Left=�$��0,60,0,0q�$	��Top=��� -[   $ labelDiscVolno��$��Číslo svazku:��$��28��$
��Left=�$��0,96,0,0q�$	��Top=��� -\   $ labelDiscSize��$��	Velikost:��$��28��$
��Left=�$��	0,116,0,0q�$	��Top=��� -]   $ discName��$��28��$
��Left=�$��	75,60,0,0q�$	��Top=�$
��Bold"�$��není vložené���� -^   $ 	discVolno��$��28��$
��Left=�$��	75,96,0,0q�$	��Top=��� -_   $ discSize��$��28��$
��Left=�$��
75,116,0,0q�$	��Top=��� -`   $% 	driveList��$��23��$��	71,20,0,0q�$	��Top=�$
��Left=�$��50���� -a   $ labelDriveLetter��$��
Mechanika:��$��28��$��1,20,0,0q�$	��Top=�$
��Left=�$��65���� -b   $ 	loadMedia�����#353535$��1q�$��Načíst��$
 Hand�����#b4b4b4$��30��$
��Left=�$��
137,16,0,0q�#��[  $ Načíst obsah média��$	��Top=�$��80������ $ 0.5,1&�$ 0.5,0&�.��� $ #484848��$	 0.0Q�� $ #363636��$	 1.0Q�� -c   $ mediaInfoNew��$	��100��$��Right=�$��0,38.5,29,38.5q�$��Center=�$	��220�����	#2600FF00$y�Hidden=�.+�A� ���	#FFF4F4F5$
��Left=�$��40��$��	22,29,0,0q�$	��Top=�$��40���� -d   $ mediaInfoNewImg��$��48��$��
18,25,0,27q�$��Uniform=�$��Images/ok-icon-done.png��$
��Left=�$��48���� -e   * ��Name$* mediaInfoNewCaption��$��42��$
��Left=�$��	76,33,0,0q�$*��$Médium ještě nebylo archivováno.��$	��Top=�
���   �$	��115��$
��Wrap=�� -f   $ mediaInfoExists�����	#3FFFFB00$	��158��$��0,9.5,29,9.5q�$��Center=�$��Right=�$	��220��$y�Hidden=� �� $
��143* ��� $	��77* �.+��� -g   $ mediaInfoLabel1��$��Datum archivace:��$��28��$
��Left=�$��0,10,0,0q�$	��Top=��� -h   $ mediaInfoLabel2��$��Titul:��$
��Left=�$��0,33,0,0q�$��28��$	��Top=��� -i   $ mediaInfoLabel3��$��Autor:��$��28��$
��Left=�$��0,55,0,0q�$	��Top=��� -j   $ mediaInfoLabel4��$��Vydáno:��$��28��$
��Left=�$��0,77,0,0q�$	��Top=��� -k   $ mediaInfoLabel6��$��Poškozené médium:��$��28��$
��Left=�$��	0,123,0,0q�$	��Top=��� -l   $ mediaInfoDateUpdate��$��28��$
��Left=�$��	91,10,0,0q�$	��Top=�$��2��$	��127���� -m   $ mediaInfoTitle��$��28��$
��Left=�$��	31,33,0,0q�$	��Top=�$��2��$	��188��$
��Bold"��� -n   $ mediaInfoAuthors��$��28��$
��Left=�$��	37,55,0,0q�$	��Top=�$��2��$	��183��$
��Bold"��� -o   $ mediaInfoYear��$��28��$
��Left=�$��	46,77,0,0q�$	��Top=�$��2��$	��174��$
��Bold"��� -p   $ mediaInfoReadProblem��$��28��$
��Left=�$��103,123,0,0q�$	��Top=�$��2��$	��116���� -q   $ mediaInfoLabel5��$��	Velikost:��$��28��$
��Left=�$��	0,100,0,0q�$	��Top=��� -r   $ mediaInfoSize��$��28��$
��Left=�$��
48,100,0,0q�$	��Top=�$��2��$	��170��9� -s   $ expanderStep4��$��Krok 3: Archivace�����	#19000000( . .��� -t   $ 	gridStep4��$��77��.+��� -u   $ 	btnCreate��$��1q�$��Spustit archivaci��$
 Hand��
�������$��40��#��[  $ Načíst obsah média��$��Center=�$��Right=�$	��180��$��0,18.5,50,18.5q����� $ 0.5,1&�$ 0.5,0&�.��� $ 	#FF9B4848��$	 0.0Q�� $ 	#FF512013��$	 1.0Q��� -v   $ btnCreateWarning��$��Médium není načtené��$��25��$��Right=�$��Center=�
���  ��$
��Bold"�$��0,26,240,26q��� -w   $ btnCreateWarningIcon��$��24��$��Right=�$
��Fill=�$��Center=�$��24��$ ��Images/ok-icon-caution.png��$��0,26.5,370,26.5q��� -x   $ labelMediaNo��$��Pořadové číslo disku:��$
��Left=�$��Center=�$��
20,27,0,27q��� -y   $ mediaNoTextBox��$��32��$��5,0,0,0q�$��
137,24,0,0q�$��Arial)�
���   �$	��Top=����	#FF5D5D5D$ Název díla��$��2q�$��12(����	#FFF7F7F7$��Center=�$��75��$
��Left=��� -z   $ controlTabItem��$��Zpracování��{�.  .���  �� $��* �$	��230���� $
��Auto ��� $	��30* �$	��300��.+��� $��Center=�$��Stretch=�$��1��$��5��$ PreviousAndNext=����� $ 0,0,218,218�$ Absolute=�$
 None=�$
 Tile=�$? 9/CDArcha_klient;component/Images/ok-background-center.png��� $��0������ $ 0,0,204,204�$ Absolute=�$
 None=�$
 Tile=�$= 7/CDArcha_klient;component/Images/ok-background-left.png��.+�� $	��Top=�$
��Left=�.+��� $��Název�����#858585$��Arial)�$��	10,20,0,0q�$	��Top=�$
��Left=��� -{   $ controlTitle�����#CECECE$��Arial)�$��	80,20,0,0q�$	��Top=�$
��Left=��� $��Autoři�����#858585$��Arial)�$��	10,40,0,0q�$	��Top=�$
��Left=��� -|   $ controlAuthors�����#CECECE$��Arial)�$��	80,40,0,0q�$	��Top=�$
��Left=��� $��Rok vydání�����#858585$��Arial)�$��	10,60,0,0q�$	��Top=�$
��Left=��� -}   $ controlYear�����#CECECE$��Arial)�$��	80,60,0,0q�$	��Top=�$
��Left=��� $��Identifikátory�����#858585$��Arial)�$��
10,100,0,0q�$	��Top=�$
��Left=�� -~   $ controlIdentifiersGrid��$��
10,120,0,0q��� -   $ controlTabDoneIcon��$<��6/CDArcha_klient;component/Images/ok-icon-hourglass.png��$��Uniform=�$	��Top=�$��Center=�$��90��$��70,196,70,0q��� -�   $ controlNewUnitButton��$��Archivace nového titulu��$��35��$
 Hand��$
��Left=�$��
21,466,0,0q����#5E5E5E���#CECECE$	��Top=�$	��188��$��0q�  MyButton#��[ + �� 	ClickMode$+ Press=�$P JUkončit archivaci aktuálního titulu. Archivovat médium jiného titulu.���� -�   $ controlSameUnitButton�����#5E5E5E$��0q�$+ Press=�$ ��Archivovat další médium��$
 Hand�����#CECECE$��35��$
��Left=�$��
22,354,0,0q�#��[ $\ VPokračovat v archivaci stejného titulu. Archivovat další médium stejného titulu.��$	��Top=�$	��188���� -�   $ zobrazitVysledek�����#b7da86$��1q�$��ZOBRAZIT VÝSLEDEK��$
 Hand��
�������$
��Left=�#��[ $' !Odeslat všechno na obalkyknih.cz��$	��188��$��35��$	��Top=�$��
22,543,0,0q����� $ 0.5,1&�$ 0.5,0&�.��� $ #89c23d��$	 0.0Q�� $ #68a328��$	 1.0Q��� $ ��Archivovat další médium��$
��Left=�$��
21,308,0,0q�$	��Top=����	#FF898989$	��188��$��Center=��� $'��!stejného titulu (bib. záznamu).��$
��Left=�$��
21,326,0,0q�$	��Top=����	#FF898989$	��188��$��Center=��� $
��Left=�$��24��$��
22,396,0,0q�$	��Top=�$	��187��, J� Opacity$	, 0.2Q��� $.��(Ukončit archivaci současného titulu a��$
��Left=�$��
10,422,0,0q�$	��Top=����	#FF898989$	��210��$��Center=��� $)��#vyhledat další titul k archivaci.��$
��Left=�$��
22,440,0,0q�$	��Top=����	#FF898989$	��188��$��Center=��� $
��Left=�$��24��$��
23,509,0,0q�$	��Top=�$	��187��$	, 0.2Q��� -�   $  controlNewUnitButton_Copy1��$#��Ukončit archivaci i aplikaci��$��35��$
 Hand��$
��Left=�$��21,591,0,10q����	#FF6C3131���#CECECE$��Bottom=�$	��188��$��0q�#��[ $+ Press=�$P JUkončit archivaci aktuálního titulu. Archivovat médium jiného titulu.��� -�   $ tab3FinishGrid��$��2��$��	200,0,0,0q�$y�Visible=����� $ 0,0,218,218�$ Absolute=�$
 None=�$
 Tile=�$? 9/CDArcha_klient;component/Images/ok-background-center.png�� �� $��* ��� $��* ��� $��* ��� $��2������ $ 0,0,218,218�$ Absolute=�$
 None=�$
 Tile=�$? 9/CDArcha_klient;component/Images/ok-background-center.png��.��� -�   $ progressGrid������ $? 9/CDArcha_klient;component/Images/ok-background-center.png��$
 None=�$
 Tile=�$ 0,0,218,218�$ Absolute=� �� .+�� -�   - �Name$- copyProgress��$��24��$��Stretch=�$��18,205,10,0q�$	��Top=�. � SmallChange$. 1Q��� -�   $ copyTimeLabel��$��Uplynulý čas:��$
��Left=�$��
14,246,0,0q�$��28��$	��Top=��� -�   $ copySizeLabel��$��Zkopírováno:��$��28��$
��Left=�$��
14,266,0,0q�$	��Top=�$��74���� -�   $ copyTime��$��28��$
��Left=�$��
89,246,0,0q�$	��Top=��� -�   $ copySize��$��28��$
��Left=�$��
89,266,0,0q�$	��Top=��� -�   $ copyPercent��$��LeftToRight=�$��28��$��Right=�$��Right=�$��
0,246,10,0q�$	��Top=�$��40���� -�   $ step1��$5��/1) Příprava       (Zaslaní identifikátorů)��$��28��$
��Left=�$��	52,30,0,0q�$	��Top=��� -�   $ step2��$?��92) Stahování     (Kopírování média na lokální PC)��$��28��$
��Left=�$��	52,54,0,0q�$	��Top=��� -�   $ step3��$?��93) Kontrola       (Výpočet kontrolního součtu média)��$��28��$
��Left=�$��	52,77,0,0q�$	��Top=��� -�   $ step4��$<��64) Zasílání        (Kopírování média na server)��$
��Left=�$��
52,100,0,0q�$��28��$	��Top=��� -�   $ step5��$>��85) Kontrola       (Kontrola zaslaných dat a ukončení)��$��28��$
��Left=�$��
52,123,0,0q�$	��Top=��� -�   $ 	stepIcon1��$
��Left=�$��	23,33,0,0q�$<��6/CDArcha_klient;component/Images/ok-icon-hourglass.png��$��Uniform=�$	��Top=�$��19��$��19��$y�Visible=��� -�   $ 	stepIcon2��$
��Left=�$��	23,57,0,0q�$<��6/CDArcha_klient;component/Images/ok-icon-hourglass.png��$��Uniform=�$��19��$y�Hidden=�$��19��$	��Top=��� -�   $ 	stepIcon3��$
��Left=�$��	23,80,0,0q�$<��6/CDArcha_klient;component/Images/ok-icon-hourglass.png��$��Uniform=�$��19��$y�Hidden=�$��19��$	��Top=��� -�   $ 	stepIcon4��$��19��$
��Left=�$��
23,103,0,0q�$<��6/CDArcha_klient;component/Images/ok-icon-hourglass.png��$��Uniform=�$	��Top=�$��19��$y�Hidden=��� -�   $ 	stepIcon5��$��19��$
��Left=�$��
23,126,0,0q�$<��6/CDArcha_klient;component/Images/ok-icon-hourglass.png��$��Uniform=�$	��Top=�$��19��$y�Hidden=��� -�   $ step5Finish��$6��0Čekám na dokončení na straně serveru: 01:00��$��28��$
��Left=�$��329,123,0,0q�$	��Top=��� / J� RenderTransformOrigin$/ -0.27,-0.13&�$��20,300,15,0q�.+�9� -�   $ expanderArchStep0��$��Log procesu archivace�����	#19000000.��� -�   $ gridArchStep0��$	��210��.+��� -�   0 ��Name$0 
logTextBox��$	��850�����	#7FFFFFFF���	#33000000
���   �$
��Left=�$��24,0,0,0q�. �/� .T�J� .*�.*��� $� ��9� -�   $ expanderArchStep1��$��Skenování�����	#19000000$��Verdana)�.��� -�   $ gridArchStep1��$	��280��.+�� -�   $ gridAuth��$	��Top=�$��Right=�$��
10,0,15,50q�$��2��$y�Hidden=�� -�   $ gridToc��$
��Left=�$	��Top=�$��	240,0,0,0q�.+��� -�   $ originalTocImage��$
 Hand��$��Center=�{�.  $��	0,10,20,0q�$	��300��$	��200��$��Images/ok-empty.png��$��Uniform=�$	��Top=�� -�   $ 	gridCover��$	��302��$	��Top=�$
��Left=�$	��206��$��14,0,0,0q�.+��� -�   $ originalCoverImage��$
��Left=�$��6,10,0,0q�$	��300��$	��200��$��Images/ok-empty.png��$��Uniform=�$	��255��$	��Top=��� -�   $ controlNewUnitButton_Copy��$"��Opakovaně načíst obrázky��$��35��$
 Hand��$
��Left=����#5E5E5E���#CECECE$��Bottom=�$	��188��$��0q�#��[ $+ Press=�$P JUkončit archivaci aktuálního titulu. Archivovat médium jiného titulu.��$��
500,0,0,22q��� -�   $ scanningStartButton��$��SKENOVAT��$��35��$
 Hand��$
��Left=�$��
500,15,0,0q����	#FF6C3131
�������$	��Top=�$	��188��$��0q�#��[ $+ Press=�$P JUkončit archivaci aktuálního titulu. Archivovat médium jiného titulu.���� -�   $ scanningTabDoneIcon��$"��Images/ok-icon-hourglass.png��$��Uniform=�$	��Top=�$
��Left=�$��90��$��
550,90,0,0q��� -�   $ scanningTabItem��$��Skenování��$y�Visible=�.��� -�   $ scanningGrid�� �� $��* �$	��230��$	��500���� $
��Auto ��� $	��30* �$	��230���� $	��233 �$	��233��$	��233��.+�� -�   $ leftScanningGrid��$��0������ $ 0,0,204,204�$ Absolute=�$
 None=�$
 Tile=�$= 7/CDArcha_klient;component/Images/ok-background-left.png��.+��� -�   $ thumbnailsTitleLabel��$
��Left=�$��	10,10,0,0q�$	��Top=�$��Arial)����#CECECE#   CDArcha_klient.MyScrollViewer  $��Disabled=�$��Hidden=�$��0,40,0,0q�.��� 1 � RowDefinitions1 �� $
��Auto ��� $
��Auto ��� $
��327* �.+��� $��Médium�����#CECECE$��Center=�$	��Top=�$��25��$��0���� $��4q�$	��Top=�$��Center=�$��0��$��50,30q�._��� -�   $ coverThumbnail��$	��Top=�$��Uniform=�$7��1/CDArcha_klient;component/Images/default-icon.png��$
 Hand��{�.  �� -�   $ deleteCoverIcon��$9��3/CDArcha_klient;component/Images/ok-icon-delete.png��$��18��$��Uniform=�$	��Top=�$��Right=�$��26,30q�$y�Hidden=�$
 Hand���� $��Booklet�����#CECECE$��Center=�$��25��$��Arial)�$��1��$��
92,0,92,57q�$	��Top=��� -�   $ tocPagesNumber��$��0 stran�����#B6B6B6$
��0,15q�$��Center=�$	��Top=�$��25��$��Arial)�$��1��   CDArcha_klient.MyListView  -�   2  Name$2 tocImagesList��$	��Top=�$
��0,40q�$��1��$��Disabled=�
������ $��0q�  ��%�   (��       * SystemColors.HighlightBrushKey+)(��      *SystemColors.ControlBrushKey+)��Transparent����Transparent��� $��2������ $ 0,0,218,218�$ Absolute=�$
 None=�$
 Tile=�$? 9/CDArcha_klient;component/Images/ok-background-center.png��.+��� -�   $ selectedImage��$��Center=�$��20q�$��Center=�$7��1/CDArcha_klient;component/Images/default-icon.png��$��Uniform=�� $��3��.+�  $��Disabled=�$��Hidden=�.��� ���� $ 0,0,218,218�$ Absolute=�$
 None=�$
 Tile=�$> 8/CDArcha_klient;component/Images/ok-background-right.png��.+��� $��18��$
��Left=�$	��Top=�$��25,12q�$��Uniform=�$��18��$<��6/CDArcha_klient;component/Images/ok-icon-transform.png���� $��Transformace��$
��45,9q�$	��Top=�$
��Left=�$��Arial)����#5E5E5E� $��0,40,0,0q�$	��Top=�$��Center=�.+��� -�   $ rotateLeftIcon��$��55��$
��Left=�$	��Top=�$��0,0,0,0q�$��Uniform=�$��55��$9��3/CDArcha_klient;component/Images/ok-icon-90left.png��{�.  $7 1Otočit obrázek o 90° vlevo (Ctrl+šipka vlevo)��$
 Hand���� -�   $ rotateRightIcon��$��55��$
��Left=�$	��Top=�$��70,0,0,0q�$��Uniform=�$��55��$:��4/CDArcha_klient;component/Images/ok-icon-90right.png��{�.  $9 3Otočit obrázek o 90° vpravo (Ctrl+šipka vpravo)��$
 Hand���� -�   $ rotate180Icon��$��55��$
��Left=�$	��Top=�$��	140,0,0,0q�$��Uniform=�$��55��$6��0/CDArcha_klient;component/Images/ok-icon-180.png��{�.  $' !Otočit obrázek o 180° (Ctrl+R)��$
 Hand���� -�   $ 
deskewIcon��$��55��$
��Left=�$	��Top=�$��0,60,0,0q�$��Uniform=�$��55��$8��2/CDArcha_klient;component/Images/ok-icon-align.png��{�.  $0 *Narovnat mírnou rotaci podle sklonu textu��$
 Hand���� -�   $ flipIcon��$��55��$
��Left=�$	��Top=�$��	70,60,0,0q�$��Uniform=�$��55��$9��3/CDArcha_klient;component/Images/ok-icon-mirror.png��{�.  $- 'Zrcadlově překlopit obrázek (Ctrl+H)��$
 Hand���� -�   $ cropIcon��$��55��$
��Left=�$	��Top=�$��
140,60,0,0q�$��Uniform=�$��55��$7��1/CDArcha_klient;component/Images/ok-icon-crop.png��{�.  $ Ořezat obrázek (Ctrl+C)��$
 Hand���� -�   $ saveSelectedButton��$��55��$
��Left=�$	��Top=�$��140,102,0,0q�$��Uniform=�$��55��$;��5/CDArcha_klient;component/Images/ok-icon-crop-new.png��$&  Uložit vybranou část (Ctrl+H)��$
 Hand��� $��	0,193,0,0q�$	��Top=�$��Center=�.+��� $��18��$
��Left=�$	��Top=�$��Uniform=�$��18��$=��7/CDArcha_klient;component/Images/ok-icon-brightness.png���� $	��Jas��$��20,0,0,0q�$��Arial)����#5E5E5E$	��Top=�$
��Left=��� -�   3 ��Name$3 brightnessSlider��  MyCustomStyleForSlider#��[ $	��160��$��25�����#5E5E5E$	��Top=�$��	0,20,40,0q�4 � Minimum$
4 -255Q�5 � Maximum$	5 255Q�6 � Value$6 0Q�7 �� Ticks$7 0R�$. 5Q�8 �� SelectionStart$
8 -255Q�9 �� SelectionEnd$	9 255Q�{�.  : �� AutoToolTipPrecision$: 0��$
 Hand��; �� AutoToolTipPlacement$; TopLeft=�< �� IsSnapToTickEnabled< .  $8 2Nastavte jas a kontrast a potvrďte tlačítkem OK���� $��18��$
��Left=�$	��Top=�$��Uniform=�$��18��$��0,45,0,0q�$;��5/CDArcha_klient;component/Images/ok-icon-contrast.png���� $��Kontrast��$��	20,45,0,0q�$��Arial)����#5E5E5E$	��Top=�$
��Left=��� -�   $3 contrastSlider��#��[ $	��160��$��25�����#5E5E5E$	��Top=�$��	0,65,40,0q�$
 Hand��$
4 -100Q�$	5 100Q�$6 0Q�$7 0R�$. 5Q�$
8 -100Q�$	9 100Q�{�.  $: 0��$; TopLeft=�< .  $8 2Nastavte jas a kontrast a potvrďte tlačítkem OK���� $��18��$
��Left=�$	��Top=�$��Uniform=�$��18��$��0,90,0,0q�$8��2/CDArcha_klient;component/Images/ok-icon-gamma.png���� $��Gamma��$��	20,90,0,0q�$��Arial)����#5E5E5E$	��Top=�$
��Left=��� -�   $3 gammaSlider��#��[ $	��160��$��25�����#5E5E5E$	��Top=�$��
0,110,40,0q�$
 Hand��$4 0Q�$5 2Q�$6 1Q�$7 1R�$. 1Q�{�.  $: 1��$; TopLeft=�< .  $7 1Nastavte hodnotu gamma a potvrďte tlačítkem OK���� -�   $ sliderConfirmButton��$��40��$��30��$��OK��$
 Hand��{�.  $��Right=�$��0,62,0,0q����#CECECE���#656565$	��Top=�$��1q�#��[ $&  Potvrdit změnu jasu a kontrastu������ $ 0.5,0&�$ 0.5,1&�.��� $ #535353��$	 0.0Q�� $ #3d3d3d��$	 1.0Q�� $��34,343,34,0q�$	��Top=�$��Center=�$	��214��.+��� -�   $ scanCoverButton��$��Skenovat médium��$��30��$��Center=�$��0,0,0,0q����#CECECE���#656565$	��Top=�$	��165��$��1q�#��[ $
 Hand��$&  Naskenovat obrázek disku/média������ $ 0.5,0&�$ 0.5,1&�.��� $ #535353��$	 0.0Q�� $ #3d3d3d��$	 1.0Q��� -�   $ scanTocButton��$��Skenovat booklet��$��30��$��Center=�$��0,64,0,0q����#CECECE���#656565$	��Top=�$	��165��$��1q�#��[ $
 Hand��$" Naskenovat obrázky bookletu������ $ 0.5,0&�$ 0.5,1&�.��� $ #535353��$	 0.0Q�� $ #3d3d3d��$	 1.0Q��� -�   $ loadFromFileLabel��$��Načíst ze souboru��$��Arial)����	#FF1E1E1E$��
37,0,29,62q�$��Center=�$
 Hand��$_ YNačíst obrázek ze souboru, po kliknutí vyberte zda chcete načíst obálku nebo obsah��$��24��$��Bottom=��� -�   $ scanCoverA5��$��A5��$��28��$
��Left=�$��	28,30,0,0q�$	��Top=����	#FF5E5E5E$
 Hand���� -�   $ scanCoverA4��$��A4�����	#FF5E5E5E$��28��$��Right=�$��	0,30,72,0q�$	��Top=�$
 Hand���� -�   $ scanCoverA3��$��A3�����	#FF5E5E5E$��28��$
��Left=�$��
116,30,0,0q�$	��Top=�$
 Hand���� -�   $ 	scanTocA5��$��A5�����	#FF5E5E5E$��28��$
��Left=�$��	28,94,0,0q�$	��Top=�$
 Hand���� -�   $ 	scanTocA4��$��A4�����	#FF5E5E5E$��28��$
��Left=�$��	69,94,0,0q�$	��Top=�$
 Hand���� -�   $ 	scanTocA3��$��A3�����	#FF5E5E5E$��28��$
��Left=�$��
116,94,0,0q�$	��Top=�$
 Hand���� -�   $ 
sendButton��$��Odeslat��$��30�����#b7da86$
 Hand��$��Center=�$��	0,178,0,0q�
�������$' !Odeslat všechno na obalkyknih.cz��$	��Top=�$	��165��$��1q�#��[ ���� $ 0.5,0&�$ 0.5,1&�.��� $ #89c23d��$	 0.0Q�� $ #68a328��$	 1.0Q�� $��34,571,34,0q�$��Bottom=�$��Center=�.+��� -�   $' ocrCheckBox��$��Použít OCR��$��0,40,0,0q�$��Arial)����#5E5E5E$	��Top=�$��Center=�$% Použít OCR na odeslaný obsah��$y�Hidden=�$
$ TrueY��� -�   = ��Name$= 
separator3��$��5��$
��Left=�$��
36,466,0,0q�$	��Top=�$	��163���� -�   $ saveSelecteModeButton��$��Výsekový režim��$��21��$��Center=�$��19,159,95,0q����#CECECE���#656565$	��Top=�$	��119��$��1q�#��[ $
 Hand��$ Zapnout výsekový režim������ $ 0.5,0&�$ 0.5,1&�.��� $ #535353��$	 0.0Q�� $ #3d3d3d��$	 1.0Q�