using System;
using System.Windows;
using System.Windows.Controls;

namespace OpenStreetApp
{
    public class ListBoxHelper
    {
        #region SelectedItemStyle

        /// <summary>
        /// SelectedItemStyle Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty SelectedItemStyleProperty =
            DependencyProperty.RegisterAttached("SelectedItemStyle", typeof(Style), typeof(ListBoxHelper),
                new PropertyMetadata((Style)null));

        /// <summary>
        /// Gets the SelectedItemStyle property. This dependency property 
        /// indicates what style is to be used on a selected item.
        /// </summary>
        public static Style GetSelectedItemStyle(DependencyObject d)
        {
            return (Style)d.GetValue(SelectedItemStyleProperty);
        }

        /// <summary>
        /// Sets the SelectedItemStyle property. This dependency property 
        /// indicates what style is to be used on a selected item.
        /// </summary>
        public static void SetSelectedItemStyle(DependencyObject d, Style value)
        {
            if (!(d is ListBox))
                throw new InvalidCastException(d.ToString() + " is no ListBox");

            var lb = (ListBox)d;

            if (GetSelectedItemStyle(d) == null)
                lb.SelectionChanged += new SelectionChangedEventHandler(lb_SelectionChanged);
            else if (value == null)
                lb.SelectionChanged -= new SelectionChangedEventHandler(lb_SelectionChanged);

            d.SetValue(SelectedItemStyleProperty, value);
        }

        static void lb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lb = (ListBox)sender;

            foreach (var item in e.AddedItems)
            {
                ListBoxItem listItem = lb.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                listItem.Style = GetSelectedItemStyle(lb);
            }

            foreach (var item in e.RemovedItems)
            {
                ListBoxItem listItem = lb.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                listItem.Style = null;
            }
        }

        #endregion


    }
}
