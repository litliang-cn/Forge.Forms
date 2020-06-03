﻿using Forge.Forms.Annotations;

namespace Forge.Forms
{
    [Form(Mode = DefaultFields.None)]
    [Title("{Binding Title}", IsVisible = "{Binding Title|IsNotEmpty}")]
    [Text("{Binding Message}", IsVisible = "{Binding Message|IsNotEmpty}")]
    [Action("negative", "{Binding NegativeAction}",
        IsCancel = true,
        ClosesDialog = true,
        IsVisible = "{Binding NegativeAction|IsNotEmpty}",
        Icon = "{Binding NegativeActionIcon}")]
    [Action("positive", "{Binding PositiveAction}",
        IsDefault = true,
        ClosesDialog = true,
        Validates = true,
        IsVisible = "{Binding PositiveAction|IsNotEmpty}",
        Icon = "{Binding PositiveActionIcon}")]
    public sealed class Prompt<T> : DialogBase
    {
        private object icon;
        private string name;
        private string toolTip;
        private T value;

        [Field(Name = "{Binding Name}",
            ToolTip = "{Binding ToolTip}",
            Icon = "{Binding Icon}")]
        public T Value
        {
            get => value;
            set
            {
                if (Equals(value, this.value))
                {
                    return;
                }

                this.value = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => name;
            set
            {
                if (value == name)
                {
                    return;
                }

                name = value;
                OnPropertyChanged();
            }
        }

        public string ToolTip
        {
            get => toolTip;
            set
            {
                if (value == toolTip)
                {
                    return;
                }

                toolTip = value;
                OnPropertyChanged();
            }
        }

        public object Icon
        {
            get => icon;
            set
            {
                if (value == icon)
                {
                    return;
                }

                icon = value;
                OnPropertyChanged();
            }
        }
    }
}
