using System;
using System.Collections.Generic;
using System.Linq;
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

namespace KawaiCalendar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            InitializeComponent();
            calendar.Date = DateTime.Now.AddMonths(10);



        }

        private void miAdd_Click(object sender, RoutedEventArgs e)
        {

        }

        private void miMoveToDate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void miAbout_Click(object sender, RoutedEventArgs e)
        {
            //abrir github repositorio
        }

        private void calendar_ChangeDate(object sender=null, EventArgs e=null)
        {
            Title = $"{calendar.Date.ToString("MMMM").ToUpper()} de {calendar.Date.Year}";
        }
    }
}
