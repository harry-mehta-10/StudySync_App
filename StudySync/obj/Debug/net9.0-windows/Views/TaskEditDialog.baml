   M S B A M L   `   `   `  ����D  @StudySync, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:StudySync.ViewsStudySync.Views  5   	   ;"clr-namespace:StudySync.ViewModelsStudySync.ViewModels  5   	   $    StudySync.Views.TaskEditDialog   5   	   W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   /localclr-namespace:StudySync.Views     5   	   1vm"clr-namespace:StudySync.ViewModels     5   	     � Title$  	Edit Task��5	   	   $	��450��5
   	   $	��500��5   	    � WindowStartupLocation$ CenterOwner=�5   	    � 
ResizeMode$ NoResize=�5   	    � ShowInTaskbar$ False��5   	   .��5      � $��20q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6    � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5#   
   �� $��0��5$      $��0��5%      $��Title:��5&      $��Center=�5'      $��	0,0,10,15q�5(      �� 5*   
   $��0��5+      $��1��5,      $��0,0,0,15q�5.      ��5-      �� *Title+ �� UpdateSourceTrigger$ PropertyChanged=�5.      �� 51   
   $��1��52      $��0��53      $��Description:��54      $	��Top=�55      $��	0,0,10,15q�56      �� 58   
   $��1��59      $��1��5:       �� TextWrapping$
 Wrap=�5<       �� AcceptsReturn$
 True��5=      $��60��5>      "	 �� VerticalScrollBarVisibility$
	 Auto=�5?      $��0,0,0,15q�5@      ��5;      �� *Description+5@      �� 5C   
   $��2��5D      $��0��5E      $��	Due Date:��5F      $��Center=�5G      $��	0,0,10,15q�5H      (  "System.Windows.Controls.DatePicker  5J   
   $��2��5K      $��1��5L      $��0,0,0,15q�5N      
   SelectedDate
 5M      �� *	DueDate+5N      �� 5Q   
   $��3��5R      $��0��5S      $��Subject:��5T      $��Center=�5U      $��	0,0,10,15q�5V      �� 5X   
   $��3��5Y      $��1��5Z       �� 
IsEditable$
 True��5]      $��0,0,0,15q�5^      ��5[      �� *AvailableSubjects+ �� SelectedValue 5\      �� *	Subject+5^      �� 5a   
   $��4��5b      $��0��5c      $��	Priority:��5d      $��Center=�5e      $��	0,0,10,15q�5f      �� 5h   
   $��4��5i      $��1��5j      $��0,0,0,15q�5m      ��5k      �� *AvailablePriorities+ 5l      �� *TaskPriority+5m      �� 5p   
   $��5��5q      $��0��5r      $��Estimated Time:��5s      $��Center=�5t      $��	0,0,10,15q�5u      � 5w   
   $��5��5x      $��1��5y      $��0,0,0,15q�5z       5{      �� 5|      $
��Auto �6#   �� 5}      $
��Auto �6#   �� 5~      $
��Auto �6#   �� 5      $
��Auto �6#   .+�5�      �� $��0��5�      $��50��5�      ��5�      �� *EstimatedHours+5�      �� 5�      $��1��5�      $�� hours ��5�      $��Center=�5�      $	��5,0q�5�      �� 5�      $��2��5�      $��50��5�      ��5�      �� *EstimatedMinutes+5�      �� 5�      $��3��5�      $�� minutes��5�      $��Center=�5�      $	��5,0q�5�      5�      �� 5�   
   $��6��5�      $��0��5�      $��Status:��5�      $��Center=�5�      $��	0,0,10,15q�5�      �� 5�   
   $��6��5�      $��1��5�      $��	Completed��5�      $��Center=�5�      $��0,0,0,15q�5�       d� 	IsChecked 5�      �� *IsCompleted+5�      �� 5�   
   $��8��5�      $��0��5�      $��2��5�       �� Orientation$ 
Horizontal=�5�      $��Right=�5�      .+�5�      �� -   5        $��Cancel��5�         SecondaryButtonStyle#��[  5�      $��0,0,10,0q�5�      ��5�      �� *CancelCommand+5�      �� 5�      -   5        $
��Save��5�        PrimaryButtonStyle#��[ 5�      ��5�      �� *SaveCommand+5�      5�      5�      5�      