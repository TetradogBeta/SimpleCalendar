using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KawaiCalendar
{
    /// <summary>
    /// Lógica de interacción para SelectorDeFecha.xaml
    /// </summary>
    public partial class SelectorDeFecha : Window
    {
        public SelectorDeFecha()
        {
            InitializeComponent();
            Date = DateTime.Now;
        }
        public DateTime? Date
        {
            get => picker.SelectedDate;
            set => picker.SelectedDate = value;
        }
    }
}
