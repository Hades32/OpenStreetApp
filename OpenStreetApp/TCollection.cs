using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace OpenStreetApp
{
    public class TCollection<T>:Collection<T>
    {
        public void addSynchronized(T item)
        {
            lock (this)
            {
                this.Add(item);
            }
        }

        public void removeSynchronized(T item)
        {
            lock (this)
            {
                this.Remove(item);
            }
        }

        public bool containsSynchronized(T item)
        {
            lock (this)
            {
                return this.Contains(item);
            }
        }
    }
}
