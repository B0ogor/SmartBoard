   M S B A M L   `   `   `  ����E  ASmartBoard, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null?$clr-namespace:SmartBoard.Views.modelSmartBoard.Views.model  5      )    #SmartBoard.Views.Windows.MainWindow   5      W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      6local$clr-namespace:SmartBoard.Views.model     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      M IWpf.Ui, Version=4.0.0.0, Culture=neutral, PublicKeyToken=11f9f5cc97b3ffd6/ui&http://schemas.lepo.co/wpfui/2022/xaml  5      $
��1100��5
      $	��650��5      "  Wpf.Ui.Controls.FluentWindow!    ExtendsContentIntoTitleBar$
  True��5         WindowBackdropType$
 Mica=�5         WindowCornerPreference$ Round=�5       � WindowStartupLocation$ CenterScreen=�5       � Title 5	      �� *ViewModel.ApplicationTitle+ �� Mode$ OneWay=� J� InputBindings 5      �� 5   
   
 �� Key$ F��5       �� 	Modifiers$ Controlf�5      	 �� Command	 5      �� 
 �� ElementName$
 AutoSuggestBox�� �� Path$ FocusCommand�5      .��5      � .+�5   
     Wpf.Ui.Controls.TitleBar  -   5          Name$ TitleBar��5      $��0��5      %   CloseWindowByDoubleClickOnIcon$
 True��5          Title 5      �� *ViewModel.ApplicationTitle+   Icon 5      '  !Wpf.Ui.Markup.SymbolIconExtension     Symbol$ Apps24=�5!      $  Wpf.Ui.Controls.NavigationView  5#   
   -   5          Name$ RootNavigation��5$      $��	42,0,42,0q�5%         FrameMargin$ 0q�5(         IsBackButtonVisible$ Visible=�5)         IsPaneToggleVisible$
 True��5*         OpenPaneLength$	 310Q�5,         PaneDisplayMode$
 Left=�5-         BreadcrumbBar 5&      �� $
 BreadcrumbBar��   FooterMenuItemsSource 5'      �� *ViewModel.FooterMenuItems+$ OneWay=�   MenuItemsSource 5+      �� *ViewModel.MenuItems+$ OneWay=�   TitleBar 5.      �� $
 TitleBar��$ OneWay=�   AutoSuggestBox 5/      $  Wpf.Ui.Controls.AutoSuggestBox  50      -   5          Name$ AutoSuggestBox��50   $      PlaceholderText$ Search��6<      Icon 51      '  !Wpf.Ui.Controls.IconSourceElement  52         
IconSource. 53      &   Wpf.Ui.Controls.SymbolIconSource      Symbol$  Search24=�62   54      56      !   	MenuItems! 58      (  "Wpf.Ui.Controls.NavigationViewItem  59      $��Задачи��6(   "   Icon" 5:       	  Wpf.Ui.Controls.SymbolIcon	  5;      # 	  Symbol$# GlanceDefault12=�6(   $   	MenuItems$ 5=      �� 5>      ��6'   �� *ViewModel.TaskTemplates+��5?      �� 5@   "   .R�5A   &   �� % �� Orientation$% Vertical=�61   $	��276��6H   5B   #   ��5D      �� 5E   "   .R�5F   &   %
   SmartBoard.Views.model.TaskCard
  %   SmartBoard.Views.model.TaskCard&   MouseLeftButtonDown & TaskCard_MouseLeftButtonDown5M      ' 
  Title' 5G      �� *Title+( 
  Description( 5H      �� *Description+) 
  TaskType) 5I      �� *
TaskType+* 
  Priority* 5J      �� *
Priority++ 
  Assignee+ 5K      �� *
Assignee+, 
  Deadline, 5L      �� *
Deadline+5M      5N   #   5P      5R      5U      5W      5X      