﻿using System.Windows.Input;
using MahApps.Metro.Controls;

namespace Forge.Forms.Extension.Controls
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class DialogWindow : MetroWindow
    {
        public DialogWindow(object model, object context, WindowOptions options)
        {
            DataContext = options;
            InitializeComponent();
            Loaded += (sender, e) => MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            Form.Environment.Add(options.EnvironmentFlags);
            Form.Context = context;
            Form.Model = model;
        }

        private void CloseDialogCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DialogResult = e.Parameter as bool?;
            Close();
        }

        private void CloseDialogCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}
