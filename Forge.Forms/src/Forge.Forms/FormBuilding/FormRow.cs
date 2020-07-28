﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Forge.Forms.Controls.Internal;

namespace Forge.Forms.FormBuilding
{
    public class FormRow
    {
        public FormRow()
            : this(true, 1)
        {
        }

        public FormRow(bool startsNewRow, int rowSpan)
        {
            StartsNewRow = startsNewRow;
            RowSpan = rowSpan;
            Elements = new List<FormElementContainer>();
        }

        public bool StartsNewRow { get; }

        public int RowSpan { get; }

        public List<FormElementContainer> Elements { get; }
    }

    public class FormElementContainer
    {
        public FormElementContainer(int column, int columnSpan, FormElement element)
            : this(column, columnSpan, new List<FormElement> { element })
        {
        }

        public FormElementContainer(int column, int columnSpan, List<FormElement> elements)
        {
            Column = column;
            ColumnSpan = columnSpan;
            Elements = elements;
        }

        internal FormElementContainer(int column, int columnSpan, ILayout layout)
        {
            Column = column;
            ColumnSpan = columnSpan;
            Elements = layout.GetElements().ToList();
            Layout = layout;
        }

        public int Column { get; }

        public int ColumnSpan { get; }

        public List<FormElement> Elements { get; }

        // This is not ready for public API
        public ILayout Layout { get; }
    }

    /// <summary>
    /// Supports custom layout of form elements.
    /// </summary>
    public interface ILayout
    {
        IEnumerable<FormElement> GetElements();

        FrameworkElement Build(Func<FormElement, FrameworkElement> elementBuilder);
    }

    public class GridLayout : ILayout
    {
        public GridLayout(IEnumerable<GridColumnLayout> columns, double top, double bottom)
        {
            Columns = columns?.ToList() ?? new List<GridColumnLayout>(0);
            Top = top;
            Bottom = bottom;
        }

        public List<GridColumnLayout> Columns { get; }

        public double Top { get; }

        public double Bottom { get; }

        public IEnumerable<FormElement> GetElements() => Columns.SelectMany(c => c.GetElements());

        public FrameworkElement Build(Func<FormElement, FrameworkElement> elementBuilder)
        {
            ColumnDefinition GetDefinition(double size)
            {
                if (size > 0d)
                {
                    return new ColumnDefinition
                    {
                        Width = new GridLength(size, GridUnitType.Star)
                    };
                }

                if (size < 0d)
                {
                    return new ColumnDefinition
                    {
                        Width = new GridLength(-size, GridUnitType.Pixel)
                    };
                }

                return null;
            }

            var grid = new Grid
            {
                Margin = new Thickness(0d, Top, 0d, Bottom)
            };

            var colnum = 0;
            foreach (var column in Columns)
            {
                if (column.Width == 0d)
                {
                    continue;
                }

                var gridColumn = GetDefinition(column.Left);
                if (gridColumn != null)
                {
                    grid.ColumnDefinitions.Add(gridColumn);
                    colnum++;
                }

                grid.ColumnDefinitions.Add(GetDefinition(column.Width));
                var child = column.Build(elementBuilder);
                Grid.SetColumn(child, colnum);
                grid.Children.Add(child);
                colnum++;
                gridColumn = GetDefinition(column.Right);
                if (gridColumn != null)
                {
                    grid.ColumnDefinitions.Add(gridColumn);
                    colnum++;
                }
            }

            return grid;
        }
    }

    public class Layout : ILayout
    {
        public Layout(IEnumerable<ILayout> children)
            : this(children, new Thickness(), VerticalAlignment.Stretch, HorizontalAlignment.Stretch)
        {
        }

        public Layout(IEnumerable<ILayout> children, Thickness margin, VerticalAlignment verticalAlignment, HorizontalAlignment horizontalAlignment)
        {
            Children = children?.ToList() ?? new List<ILayout>(0);
            Margin = margin;
            VerticalAlignment = verticalAlignment;
            HorizontalAlignment = horizontalAlignment;
        }

        public List<ILayout> Children { get; }

        public Thickness Margin { get; set; }

        public VerticalAlignment VerticalAlignment { get; }

        public HorizontalAlignment HorizontalAlignment { get; }

        public IEnumerable<FormElement> GetElements() => Children.SelectMany(c => c.GetElements());

        public FrameworkElement Build(Func<FormElement, FrameworkElement> elementBuilder)
        {
            var panel = new StackPanel
            {
                Margin = Margin,
                VerticalAlignment = VerticalAlignment,
                HorizontalAlignment = HorizontalAlignment
            };

            foreach (var child in Children)
            {
                panel.Children.Add(child.Build(elementBuilder));
            }

            return panel;
        }
    }

    public class GridColumnLayout : ILayout
    {
        public GridColumnLayout(ILayout child, double width, double left, double right)
        {
            Child = child;
            Width = width;
            Left = left;
            Right = right;
        }

        public ILayout Child { get; }

        public double Width { get; }

        public double Left { get; }

        public double Right { get; }

        public IEnumerable<FormElement> GetElements() => Child.GetElements();

        public FrameworkElement Build(Func<FormElement, FrameworkElement> elementBuilder)
        {
            return Child.Build(elementBuilder);
        }
    }

    public class FormElementLayout : ILayout
    {
        public FormElementLayout(FormElement element)
        {
            Element = element;
        }

        public FormElement Element { get; }

        public IEnumerable<FormElement> GetElements() => new[] { Element };

        public FrameworkElement Build(Func<FormElement, FrameworkElement> elementBuilder)
        {
            return elementBuilder(Element);
        }
    }

    public class InlineLayout : ILayout
    {
        public InlineLayout(IEnumerable<ILayout> elements, double top, double bottom)
        {
            Elements = elements?.ToList() ?? new List<ILayout>(0);
            Top = top;
            Bottom = bottom;
        }

        public List<ILayout> Elements { get; }

        public double Top { get; }

        public double Bottom { get; }

        public IEnumerable<FormElement> GetElements() => Elements.SelectMany(e => e.GetElements());

        public FrameworkElement Build(Func<FormElement, FrameworkElement> elementBuilder)
        {
            var panel = new ActionPanel
            {
                Margin = new Thickness(0d, Top, 0d, Bottom)
            };

            foreach (var element in Elements)
            {
                if (element is FormElementLayout formElementLayout)
                {
                    var contentPresenter = elementBuilder(formElementLayout.Element);
                    ActionPanel.SetPosition(contentPresenter, formElementLayout.Element.LinePosition);
                    panel.Children.Add(contentPresenter);
                }
                else
                {
                    panel.Children.Add(element.Build(elementBuilder));
                }
            }

            return panel;
        }
    }
}
