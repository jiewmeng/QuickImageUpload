   M S B A M L   `   `   `  ����   QuickImageUpload&     QuickImageUpload.Views.ShellView   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008       J� InputBindings  �� 
 �� Key$ C�� �� 	Modifiers$
 Ctrlf� �� Command �� *CopyImageCommand+�� $ V��$
 Ctrlf� �� *PasteImageCommand+.���  � RowDefinitions �� $��24 ��� $��* �.+�o� $��0��.@�m� $
��File��.@�m� $��Select images ...�� m� Command �� *SelectImageCommand+m� $
��Help��m� $��About��m� $
��Exit��m� $
��Edit��.@�m� $
��Copy�� �� *CopyImageCommand+m� $��Paste�� �� *PasteImageCommand+�� $��1��.+��� -   $	��400��$ �� IsSynchronizedWithCurrentItem$
 TrueY� J� 	AllowDrop . ���� *UploadedImages+ � ContextMenu �� .@�m� $
��Copy�� �� *CopyImageCommand+m� $��Paste�� �� *PasteImageCommand+���� .R�� 	 � ColumnDefinitions	 �� $	��100 ��� $��* �.+��� $	��100��$	��100��$��0������ *
DirectLink+�� $��1��$��10,0,0,0q�.+��� ���� *
DirectLink+�� ���� *Link+�� ���� *Notification+