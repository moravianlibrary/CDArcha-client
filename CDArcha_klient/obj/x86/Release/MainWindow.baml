   M S B A M L   `   `   `  ����   CDArcha_klient    CDArcha_klient.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     � Title$  CDArcha_klient��$	��670��$
��1000�� � WindowStartupLocation$ CenterScreen=�$
��1000��$	��670�� � WindowState$ 	Maximized=�.��]� -   $��	dockPanel��.+�o� $	��Top=����#363636���#CECECE.@�m� $��_Soubor�����#363636���#CECECE   MLB_MenuItem#��[  .@�m� -   $��newMenuItem��$��_Nový�� m� InputGestureText$ Ctrl+N�����#363636���#CECECE#��[  m� -   $��updateMenuItem��$��_Aktualizovat�����#363636���#CECECE#��[  m� -   $��closeMenuItem��$��_Konec��$ Alt+F4�����#363636���#CECECE#��[  m� $��_Nastavení�����#363636���#CECECE#��[  .@�m� -   $��credentialsMenuItem��$��_Přihlašovací údaje�����#363636���#CECECE#��[  m� -   $��settingsMenuItem��$��_Další nastavení�����#363636���#CECECE#��[  m� $��_Pomoc�����#363636���#CECECE#��[  .@�m� -   $��aboutMenuItem��$��_O Aplikaci�����#363636���#CECECE#��[  m� -	   $��helpMenuItem��$��_Rychlý návod�����#363636���#CECECE#��[  $y�Hidden=�m� -
   $��onlineHelpMenuItem��$��_Online nápověda�����#363636���#CECECE#��[  $y�Hidden=�m� $��_Krok zpět�����#363636���#CECECE#��[  $y�Hidden=�.@�m� -   $��undoMenuItem��$��Vrátit poslední úpravu��$ Ctrl+Z��{�.  ���#363636���#CECECE#��[  m� -   $��redoMenuItem��$ ��Opakovat poslední úpravu��$ Ctrl+Y��{�.  ���#363636���#CECECE#��[  �� -   $��	statusBar��$��Bottom=�$��25�����#363636.@��� -   $��updateDownloadTextBox��$
��10,0q�$y�	Collapsed=����#cecece$��Arial)�Stahování aktualizace� -   $��updateDownloadProgressBar��$��14��$��60��$y�	Collapsed=� � Value$ 0Q��� -   $��metadataDownloadTextBox��$
��10,0q�$y�	Collapsed=����#cecece$��Arial)�� -   $��mainGrid������  j� Viewport$ 0,0,218,218� j� ViewportUnits$ Absolute=� j� Stretch$
 None=� j� TileMode$
 Tile=�	 �� ImageSource$?	 9/CDArcha_klient;component/Images/ok-background-center.png��.+�� $��	0,100,0,0q�$	��Top=�$��Center=�$	��300��.+��� -   $��Nová archivace��$��50��$��Center=�$��	0,110,0,0q�$��Arial)����#FCF5FC$��16(�$	��Top=�$	��200��$y�Visible=�  MyButton#��[ 
 � Cursor$

 Hand�����#bb5b5d$��1q�����  �� 
StartPoint$ 0.5,0&� �� EndPoint$ 0.5,1&�.���  � Color$ #9e0b0f�� � Offset$	 0.0Q�� $ #82070a��$	 1.0Q��� -   $��downloadUpdateButton��$��Stáhnout novou verzi��$��30��$��Center=�$��	0,380,0,0q�$	��Top=�$	��160��$y�Hidden=�#��[ $

 Hand�����#b7da86$��1q�$��Arial)�
����������� $ 0.5,0&�$ 0.5,1&�.��� $ #89c23d��$	 0.0Q�� $ #68a328��$	 1.0Q��� $��Vaše současná verze:�����#4C4B49$��Arial)�$	��Top=�$
��Left=�$��
40,200,0,0q��� -   $��programVersionLabel��$
��Left=�$	��Top=�$��180,200,0,0q�$��26�����#4C4B49$��Arial)��� $��Podpora:�����#4C4B49$��Arial)�$	��Top=�$
��Left=�$��
40,220,0,0q��� -   $��programSupportLabel��$
��Left=�$	��Top=�$��180,220,0,0q�$��24��$��89�����#4C4B49$��Arial)��� -   $��versionStateLabel��$��Verze není aktuální��$
��Left=�$��
40,250,0,0q�$	��Top=�$��Arial)��� -   $��versionStateImage��$:��4/CDArcha_klient;component/Images/ok-icon-caution.png��$��Uniform=�$��150,250,0,0q�$	��Top=�$
��Left=�$��23��$y�Hidden=��� 	 �� X1$ 40��	 �� Y1$	 290��	 �� X2$	 260��	 �� Y2$	 290�����#4C4B49$��1��$��	0,0,0,-74q��� $��Nejnovější verze:�����#4C4B49$��Arial)�$	��Top=�$
��Left=�$��
40,300,0,0q��� -   $��latestVersionLabel��$�� ��$
��Left=�$	��Top=�$��180,300,0,0q����#4C4B49$��Arial)��� $��Datum:�����#4C4B49$��Arial)�$	��Top=�$
��Left=�$��
40,320,0,0q��� -   $��latestDateLabel��$�� ��$
��Left=�$	��Top=�$��180,320,0,0q����#4C4B49$��Arial)��� -   $��label1��$&�� Online nápověda a seznam změn��$��28��$
��Left=�$��
40,350,0,0q�$	��Top=�
����  �$

 Hand���� -   $��showTmpFolderBtn�����#b7da86$��1q�$��Zobrazit složku��$

 Hand��$��Arial)�
�������$��30��$��
70,55,70,0q�#��[ $	��Top=�$��Center=�$y�Hidden=����� $ 0.5,1&�$ 0.5,0&�.��� $ #89c23d��$	 0.0Q�� $ #68a328��$	 1.0Q��� -   $��offlineModeLabel��$!��Používáte OFFLINE REŽIM��$��28��$��	51,0,49,0q�$	��Top=�$��14(�
���  ��$
��Bold"�$��Tahoma)�$y�Hidden=��� -   $��tmpFolderLabel��$?��9Ve vašem dočasném adresáři jsou archivované média.��$��28��$
��Left=�$��1,25,0,0q�$	��Top=�$	��299��$��Center=�$y�Hidden=�