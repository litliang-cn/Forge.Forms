﻿using System;
using System.Windows;

namespace Forge.Forms.Controls.Internal
{
    // Source: https://stackoverflow.com/questions/817610/wpf-and-initial-focus
    public static class FocusHelper
    {
        public static readonly DependencyProperty InitialFocusProperty =
            DependencyProperty.RegisterAttached(
                "InitialFocus",
                typeof(bool),
                typeof(FocusHelper),
                new PropertyMetadata(false, OnInitialFocusPropertyChanged));

        public static bool GetInitialFocus(FrameworkElement control)
        {
            return (bool)control.GetValue(InitialFocusProperty);
        }

        public static void SetInitialFocus(FrameworkElement control, bool value)
        {
            control.SetValue(InitialFocusProperty, value);
        }

        private static void OnInitialFocusPropertyChanged(
            DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (!(obj is FrameworkElement control))
            {
                return;
            }

            if (args.NewValue is true)
            {
                control.Loaded += HandleFocus;
            }
            else
            {
                control.Loaded -= HandleFocus;
            }
        }

        private static void HandleFocus(object sender, EventArgs e)
        {
            ((FrameworkElement)sender).Focus();
        }
    }
}
