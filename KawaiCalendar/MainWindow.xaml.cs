using Gabriel.Cat.S.Extension;
using System;
using System.Collections.Generic;
using System.IO;
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
        string DataBasePath = "database.bin";
        public MainWindow()
        {

            InitializeComponent();



            Height = HeightWin;
            Width = WidthWin;
            Top = PosY;
            Left = PosX;

            if (File.Exists(DataBasePath))
            {
                calendar.DataBase = Calendar.CalendarData.Serializador.GetObject(File.ReadAllBytes(DataBasePath)) as Calendar.CalendarData;
            }

            calendar_ChangeDate();
            Closing += (s, e) =>
            {
                Save();
            };

        }
        public string DataBase
        {
            get
            {
                return Properties.Settings.Default.DataBase;
            }
            set
            {
                Properties.Settings.Default.DataBase = value;
                Properties.Settings.Default.Save();
            }
        }
        public double PosX
        {
            get
            {
                return Properties.Settings.Default.PosX;
            }
            set
            {
                Properties.Settings.Default.PosX = value;
                Properties.Settings.Default.Save();
            }
        }
        public double PosY
        {
            get
            {
                return Properties.Settings.Default.PosY;
            }
            set
            {
                Properties.Settings.Default.PosY = value;
                Properties.Settings.Default.Save();
            }
        }
        public double HeightWin
        {
            get
            {
                return Properties.Settings.Default.Height;
            }
            set
            {
                Properties.Settings.Default.Height = value;
                Properties.Settings.Default.Save();
            }
        }

        public double WidthWin
        {
            get
            {
                return Properties.Settings.Default.Width;
            }
            set
            {
                Properties.Settings.Default.Width = value;
                Properties.Settings.Default.Save();
            }
        }
        public void Save()
        {
            Title = "Saving";
            PosY = Top;
            PosX = Left;
            HeightWin = Height;
            WidthWin = Width;
            Calendar.CalendarData.Serializador.GetBytes(calendar.DataBase).Save(DataBasePath);
            calendar_ChangeDate();
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

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F12)
                Save();
        }
    }
}
