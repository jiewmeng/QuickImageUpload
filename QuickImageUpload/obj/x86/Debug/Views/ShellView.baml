   M S B A M L   `   `   `  ����K  GQuickImageUpload, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:QuickImageUpload.ViewModelsQuickImageUpload.ViewModels  5   	   I)clr-namespace:QuickImageUpload.ConvertersQuickImageUpload.Converters  5   	   D @WorkQueue, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null>-clr-namespace:WorkQueueLib;assembly=WorkQueueWorkQueueLib 5   	   &     QuickImageUpload.Views.ShellView   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   8vm)clr-namespace:QuickImageUpload.ViewModels     5   	   :conv)clr-namespace:QuickImageUpload.Converters     5   	   <wi-clr-namespace:WorkQueueLib;assembly=WorkQueue     5   	     � Title$  Quick Image Upload��5   	   $	��500��6$   $	��360��61    � 
ResizeMode$ NoResize=�6=    J� InputBindings 5      �� 5	   
    �� 	Modifiers$
 Ctrlf�6   
 �� Key$ O��6&    �� Command 6.   �� *SelectImagesCommand+�� 5
   
   $
 Ctrlf�6   $ C��6&    6.   �� *CopyImageCommand+�� 5   
    �� Gesture$ 
Ctrl+Alt+C��6    6*   �� *CopyImageDataCommand+�� 5   
   $
 Ctrlf�6   $ V��6&    6.   �� *PasteImageCommand+ � 	Resources 5         OpenIcon  CopyIcon  	PasteIcon  	AboutIcon8  2QuickImageUpload.Converters.StatusToBrushConverter  StatusToBrushConverter0  *QuickImageUpload.Converters.DebugConverter  DebugConverter��%(  &	        &	 1     &	 b     &	 �     &	 �     &	 �     �� 5   
   $��../Images/Open.png��6   6,   �� 5   
   $��../Images/Copy.png��6   6,   �� 5   
   $��../Images/Paste.png��6   6-   �� 5   
   $��../Images/About.png��6   6-     5   
   6&     5   
   6   5      .��5      �  � RowDefinitions 5   
   �� 5      $��20 �6   �� 5      $	��435 �6   �� 5      $��16 �6   .+�5   
   o� $��0��6   .@�5      m� $
��File��6   .@�5      m� $��Select images ...��6   	 m� InputGestureText$	 Ctrl+O��66   
 m� Icon#
 [  6P    m� Command 6q   �� *SelectImagesCommand+m� 5      $��About��6   #
 [ 6*    6L   �� *AboutCommand+5       m� 5!      $
��Edit��6   .@�5"      m� $��Copy Direct Link��6   $	 Ctrl+C��65   #
 [ 6O    6p   �� *CopyImageCommand+m� 5#      $��Copy Image Data��6   $	 
Ctrl+Alt+C��64   #
 [ 6R    6s   �� *CopyImageDataCommand+m� 5$      $��Paste��6   $	 Ctrl+P��6*   #
 [ 6D    6f   �� *PasteImageCommand+5%      5&      �� 5(   
   $��1��6   .+�5)      �� -   5        $	��354��5)   d   $	��452��6p   $ �� IsSynchronizedWithCurrentItem$
 TrueY�6}    J� 	AllowDrop . $��Stretch=�6�   ��6   �� *UploadQueue+ �� SelectedItem 6:   �� *SelectedWorkItem+ � ContextMenu 5*      �� 5+      .@�5,      m� $��Copy Direct Link��6#   $	 Ctrl+C��6=   #
 [ 6W    6x   �� *CopyImageCommand+m� 5-      $��Copy Image Data��6#   $	 
Ctrl+Alt+C��6<   #
 [ 6Z    6{   �� *CopyImageDataCommand+m� 5.      $��Paste��6#   $	 Ctrl+P��62   #
 [ 6L    6n   �� *PasteImageCommand+5/      ��51      �� 52       �� 
TargetType" ��6    �� BasedOn# [B��6>   .�53      -   5        54      ��57      �� 58      .R�59      �  � ColumnDefinitions 5:      �� 5;   "   $	��100 �63   �� 5<   "   $��* �63   .+�5>      �� $	��100��6<   $	��150��6L   $��0��6[   ��6$   �� *Args+6[   �� 5?      $��1��6)   $��10,0,0,0q�69   .+�5@   "   ��  ��Name$ 	txtStatus��6�   ��6,   �� *Status+��6D   �� *Status+ �� 	Converter# [ 6�   � 5A   "    �Name$ progBar��6U   $��10��6I   $y�	Collapsed=�6f    � Value 6.   �� *
Progress+6f   �� 5B   "    �� Orientation$ 
Horizontal=�6-   .+�5C   &   ��  ��Name$ 	btnCancel��6`   $��Cancel��6-   $y�	Collapsed=�6s   ��6>   �� *CancelCommand+6s   �� 5D   &   $ btnRetry��6^   $��Retry��6-   $y�	Collapsed=�6p   ��6=   �� *RetryCommand+6p   �� 5E   &   $ 	btnRemove��6`   $��Remove��6-   $y�	Collapsed=�6s   ��6>   �� *RemoveCommand+6s   5F   #   �� 5G   "   $ txtDirectLink��6T   $y�	Collapsed=�6k   ��6,   ��  �� Path$ Result.DirectLink�6k   �� 5H   "   $ txtLink��6N   $y�	Collapsed=�6_   ��6,   �� $ Result.Link�6_   5I      5J       �� Triggers 5K      �� 5L       �� Value$ Pending��6E    �� Binding 6*   �� *Status+.a�5M   "   ��  �� 
TargetName$ 	btnCancel��6)    �� Property � y� �� Value$ Visible=�6V   5N      �� 5O      $ 	Cancelled��6E    6*   �� *Status+.a�5P   "   �� $ btnRetry��6)    � y�$ Visible=�6U   �� 5Q   "   $ 	btnRemove��6)    � y�$ Visible=�6V   5R      �� 5S      $ Error��6E    6*   �� *Status+.a�5T   "   �� $ 	txtStatus��6)    6X   �� $ Result.Error.Message�# [  � ���� 5U   "   $ btnRetry��6)    � y�$ Visible=�6U   �� 5V   "   $ 	btnRemove��6)    � y�$ Visible=�6V   5W      �� 5X      $ Finished��6E    6*   �� *Status+.a�5Y   "   �� $ txtDirectLink��6)    � y�$ Visible=�6Z   �� 5Z   "   $ txtLink��6)    � y�$ Visible=�6T   �� 5[   "   $ 	btnRemove��6)    � y�$ Visible=�6V   5\      �� 5]      $ 
Processing��6E    6*   �� *Status+.a�5^   "   �� $ progBar��6)    � y�$ Visible=�6T   �� 5_   "   $ 	btnCancel��6)    � y�$ Visible=�6V   5`      5b      ��5e      �� 5f      " ��6   .�5g      ��  � ��
 �����67     �� Triggers  5h      P� 5i      ! P� Property! �@��HasItems" P� Value" .  . �5j   "   ��  � �� 5k   &   �� 5l   *   # �� ImageSource$# ../Images/Usage.gif��65   $ j� 
AlignmentX$$ Center=�6W   % j� 
AlignmentY$% Center=�6k   & j� Stretch$
& None=�6   5n   #   5o      5q      5s      5t      5u      5v      