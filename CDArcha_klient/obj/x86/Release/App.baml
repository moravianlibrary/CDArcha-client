   M S B A M L   `   `   `  ����   CDArcha_klient    CDArcha_klient.App   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       �� 	Resources     CustomThumbForSlider �� 
TargetType �� Property �� Value �� 
TargetType  SliderRepeatButtonStyle % !HorizontalSliderTrackNormalBorder �� EndPoint �� 
StartPoint � Color � Offset  MyCustomStyleForSlider	 � RowDefinitions
 m�Name m� Fill m� 	Placement ��Name �� CornerRadius ��Name ]�Name ]� DecreaseRepeatButton �� DecreaseLarge ]� IncreaseRepeatButton �� IncreaseLarge ]� Thumb n�Name  MyButton J� RenderTransformOrigin �� Padding ��Name �� Triggers P� Property P� Value �� 
TargetName  MyButtonNoPadding  MLB_MenuItem �� Path �� RelativeSource  �� AncestorType! � ColumnDefinitions" ��Name# z� SharedSizeGroup$ �Name% F�Name& �Name' � IsSharedSizeScope( K� IsItemsHost-  'System.Windows.Input.KeyboardNavigation)   DirectionalNavigation��%�  &	        &	 y     &	     &	 \    7  7  7  7   &	 �    &	 �    &	 �    �� " n�.���  �@�OverridesDefaultStyle . ��  � �� �� " n�.R��� #��z��#��z��$��18��$��5���� " ��.���  �@�OverridesDefaultStyle . ��  � �� .  ��  � |� .  ��  � �� �� " ��.R��� 
������ �� $	 0,1&�$	 0,0&�.��� $ #8A8A8A��$	 0.5Q�� $ #BFBFBF��$	 0.7Q��� " ��.���  � �� �� " ��.R��� #��z��#��z��#��z��._�� 	 �� $
��Auto ��� $
��Auto �#��z���� $
��Auto �.+�m� $
 TopTick��$y�Visible=�# z��$	 Top=�$��4��$��0��m� $
 
BottomTick��$y�Visible=�# z��$ Bottom=�$��4��$��0���� $ TrackBackground��#��z��8��  $��1q�$ 1��$	��5,0q�$��Center=�$	��4.0��$��1��._��� $��-6,-1q�.+��� $ PART_SelectionRange��$y�Hidden=�$	��4.0��#��� ��#��� ��$	��1.0��]� $ 
PART_Track��$��1�� �� #��Z 8��  �� #��Z 8��  n� $ Thumb��8�� 
���   ��� " ��.���  � �� �� " ��.R��� $ ButtonBorder��$ 5��#��z��#��z��#��z��$ 0.5,0.5&�$
 20,0q�._��� $ ButtonContentPresenter��$��Center=�$��Center=� P�  �@J�IsMouseOver . . ��� $ ButtonBorder�� �@J�Opacity$	 0.9Q��� " ��.���  � �� �� " ��.R��� $ ButtonBorder��$ 5��#��z��#��z��#��z��$ 0.5,0.5&�$	 0,0q�._��� $ ButtonContentPresenter��$��Center=�$��Center=� P�  �@J�IsMouseOver . . ��� $ ButtonBorder�� �@J�Opacity$	 0.9Q��� " m�.���  � �� �� $ 
Foreground� �� "  o���  � �� �� " m�.R��� $ Border��#��z��#��z��#��z��._�� ! �� $
" Col0��$��17��$
��Auto �$# MenuItemIconColumnGroup���� $
��Auto �$# MenuTextColumnGroup���� $
��Auto �$# MenuItemIGTColumnGroup���� $
" Col3��$��14 �.+��� $
 Icon��$��0��$��4,0,6,0q�$��Center=�$
��Icon��� $$ 
GlyphPanel��$��0��$y�Hidden=�$��4,0,6,0q�$��Center=�.+�F� $% GlyphPanelpath��$��Center=�#��z��=����    �� �     >I Ѡ%& @�p �`Vl ��* �`Vl     Ѡ%& �0F 	$��LeftToRight=��� $ 
HeaderHost��$��1��#��z����. $��Header���� $ IGTHost��$��2��$��8,1,8,1q�$��InputGestureText��$��Center=�� $$ 
ArrowPanel��$��3��$��4,0,6,0q�$��Center=�.+�F� $% ArrowPanelPath��$��Right=�$��Center=�#��z��"����        �     z � 	=  	= 	� $& SubMenuPopup��$��Right=�|�.  #��� -����� $ IsSubmenuOpen� �� *TemplatedParent+.���� $ SubMenuBorder�����#2e2e2e$��1q�._�� $$ SubMenu��' . ���#2e2e2e.+��� ( . $) Cycle=� P�  �@m�Role$ TopLevelHeader=�. ���  � ��$ 6,1,6,1q��� $ SubMenuPopup�� � ��$ Bottom=��� $
 Col0�� � ��$ 0���� $
 Col3�� � ��$
 Auto ��� $
 Icon�� � y�$ 	Collapsed=��� $ 
GlyphPanel�� � y�$ 	Collapsed=��� $ IGTHost�� � y�$ 	Collapsed=��� $ 
ArrowPanel�� � y�$ 	Collapsed=�P�  �@m�Role$ TopLevelItem=�. ���  � ��$ 6,1,6,1q��� $ 
ArrowPanel�� � y�$ 	Collapsed=�P�  �@m�Role$ SubmenuHeader=�. ���  � ��$	 Top=���  � ��$ 0,2,0,2q�P�  �@m�Role$ SubmenuItem=�. ���  � ��$	 Top=���  � ��$ 0,2,0,2q��� $ 
ArrowPanel�� � y�$ 	Collapsed=�P� " �@m�IsSuspendingPopupAnimation . . ��� $ SubMenuPopup�� � ��$
 None=�P�  Z�  �@m�Icon. ��� $
 Icon�� � y�$ 	Collapsed=�P�  �@m�	IsChecked . . ��� $ 
GlyphPanel�� � y�$ Visible=��� $
 Icon�� � y�$ 	Collapsed=�P�  �@m�IsHighlighted . . ��� $ Border�� � �� �#4a4a4a��  � ��
 �   �P�  � {� .  . ���  � ��
 �����