using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Verse3.CanvasCore
{
    /// <summary>
    /// Interaction logic for RichCanvasWPFControl.xaml
    /// </summary>
    public partial class RichCanvasWPFControl : UserControl
    {
        public ObservableCollection<Element> Items { get; }
        public ObservableCollection<Element> SelectedItems { get; }
        public RichCanvasWPFControl()
        {
            InitializeComponent();
        }
    }

    public class Element : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void SetProperty<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(property, value))
            {
                property = value;
                OnPropertyChanged(propertyName);
            }
        }
        protected virtual void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
