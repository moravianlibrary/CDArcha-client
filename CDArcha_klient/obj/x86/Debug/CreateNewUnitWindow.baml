   M S B A M L   `   `   `  ����  CDArcha_klient(    "CDArcha_klient.CreateNewUnitWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Založit novou jednotku��5   	   $	��200��6(   $	��300��65    � WindowStartupLocation$ CenterScreen=�6A    � 
ResizeMode$ NoResize=�6f   .��5      �� -   5        .@�5   
   �� -   5        $��monographTabItem��5   &   $��
Monografie��6   $��Arial)�6>   ���#5e5e5e$��0��6f   .��5   
   � ��5   
   �� 5	       j� Viewport$ 0,0,218,218�6    j� ViewportUnits$ Absolute=�60    j� Stretch$
 None=�6I    j� TileMode$
 Tile=�6X    �� ImageSource$? 9/CDArcha_klient;component/Images/ok-background-center.png��5
      .+�5   
   � .+�5      �� $��Založit novou monografii��6   $��Center=�67   ���#5e5e5e$
��Bold"�6i   $��Arial)�5      $��20(�6'   $��0,6,0,0q�65   $	��Top=�6F   �� 5      -   5        $!��monographIdentifierComboBox��5   Q   $��32��6   $
��Left=�6#   $��	10,47,0,0q�6>   $��Arial)�5      ���#5e5e5e$	��Top=�6?   $	��100��6W   $	��6,0q�6c   $��Center=�6q   .@�5      �� $��Čárový kód��6    �� 
IsSelected . 64   �� 5      $
��ISBN��6   �� 5      $
��ISMN��6'   �� 5      $
��ČNB��6'   �� 5      $	��EAN��6   �� 5      $
��OCLC��6   �� 5      $��URN:NBN��6   5      �� 5      -   5        $ ��monographIdentifierTextBox��5   Q   $��32��6   $
��Left=�6"   $��
116,47,0,0q�6=   $	��Top=�6s   $	��142��6�   $��Center=�5   :   $	��6,0q�6\   6j   �� 5      -   5        $��monographNewUnitButton��5      $��Založit��5      $��Center=�6'   $	��Top=�6D   $��0,90,0,0q�6\      MyButton#��[  6n   $��80��5   3   $��25��6>   ���#CECECE���#656565$��1q�5   !   	 � Cursor$
	 Hand��65   
 � ToolTip$
 Založit novou jednotku��6C   ��5      �� 5       �� 
StartPoint$ 0.5,0&�6*    �� EndPoint$ 0.5,1&�6=   .��5       �  � Color$ #535353��6'    � Offset$	 0.0Q�67   � 5!      $ #3d3d3d��6'   $	 1.0Q�67   5"      5$      5%      5&      5'      �� 5(   
   -   5        $��periodicalTabItem��5(   &   $��
Periodikum��6   $��Arial)�6?   ���#5e5e5e$��1��6g   .��5)      � ��5*      �� 5+      $ 0,0,218,218�6!   $ Absolute=�68   $
 None=�6Q   $
 Tile=�6`   $? 9/CDArcha_klient;component/Images/ok-background-center.png��5,      .+�5.      � .+�5/      �� $��Založit nové periodikum��6   $��Center=�6>   ���#5e5e5e$
��Bold"�6p   $��Arial)�50      $��20(�6'   $��0,6,0,0q�65   $	��Top=�6F   �� 51      -   5        $"��periodicalIdentifierComboBox��51   Y   $��32��6   $
��Left=�6+   $��	10,47,0,0q�6F   $��Arial)�52      ���#5e5e5e$	��Top=�6?   $��99��6W   $	��6,5q�6b   $��Center=�6p   .@�53      �� $��Čárový kód��6'    . 6<   �� 54      $
��ISSN��6'   �� 55      $
��ISBN��6'   �� 56      $
��ISMN��6'   �� 57      $
��ČNB��6'   �� 58      $	��EAN��6'   �� 59      $
��OCLC��6'   �� 5:      $��URN:NBN��6'   5;      �� 5<      -   5        $!��periodicalIdentifierTextBox��5<   Y   $��32��6   $
��Left=�6*   $��
115,47,0,0q�6E   $	��Top=�6|   $	��138��6�   $��Center=�5=   :   6\   �� 5>      -	   5        $��periodicalNewUnitButton��5?      $��Založit��5>      $��Center=�6/   $	��Top=�6L   $��0,90,0,0q�6d   #��[  6v   $��80��5?   4   $��25��6?   ���#CECECE���#656565$��1q�5@   !   $
	 Hand��65   $
 Založit novou jednotku��6C   ��5A      �� 5B      $ 0.5,0&�62   $ 0.5,1&�6E   .��5C   "   � $ #535353��6/   $	 0.0Q�6?   � 5D   "   $ #3d3d3d��6/   $	 1.0Q�6?   5E      5G      5H      5I      5J      5K      5L      