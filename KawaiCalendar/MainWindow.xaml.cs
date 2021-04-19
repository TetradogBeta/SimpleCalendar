using Gabriel.Cat.S.Extension;
using KawaiCalendar.Calendar;
using Microsoft.Win32;
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
        public const string VERSION = "KawaiCalendar V1.6";
        static MainWindow Main { get; set; }
        Uri ulrGithub = new Uri("https://github.com/TetradogBeta/SimpleCalendar");
        string DataBasePath = "database.bin";
        public MainWindow()
        {
            Main = this;
            if (File.Exists(DataBasePath))
            {
                Calendar.CalendarData.DataBase = Calendar.CalendarData.Serializador.GetObject(File.ReadAllBytes(DataBasePath)) as Calendar.CalendarData;
            }
            InitializeComponent();



            Height = HeightWin;
            Width = WidthWin;
            Top = PosY;
            Left = PosX;



            calendar.Date = DateTime.Now;
            
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
            if (Calendar.CalendarData.DataBase.HasChanges)
            {
                Calendar.CalendarData.Serializador.GetBytes(Calendar.CalendarData.DataBase).Save(DataBasePath);
                Calendar.CalendarData.DataBase.HasChanges = false;
            }
            calendar_ChangeDate();
        }

        private void miAdd_Click(object sender, RoutedEventArgs e)
        {
            AddFilesDialog(DateTime.Now);
        }
        private DateTime AddFilesDialog(DateTime date)
        {
            DateTime dateResult=default;
            OpenFileDialog opn = new OpenFileDialog() { Multiselect = true };
            if (opn.ShowDialog().GetValueOrDefault())
               dateResult= Add(date, opn.FileNames);
            return dateResult;
        }

        private void miMoveToDate_Click(object sender, RoutedEventArgs e)
        {
            SelectorDeFecha selector = new SelectorDeFecha();
            selector.ShowDialog();
            calendar.Date = selector.Date.Value;

        }

        private void miAbout_Click(object sender, RoutedEventArgs e)
        {
            //abrir github repositorio
            if (MessageBox.Show("Este programa está dedicado a Sangus103 que me lo solicitó hace tiempo, por cierto ¿quieres ver el código fuente?", "Información", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                this.ulrGithub.Abrir();

        }

        private void calendar_ChangeDate(object sender = null, EventArgs e = null)
        {
            Title = $"{VERSION}  {calendar.Date.ToString("MMMM").ToUpper()} de {calendar.Date.Year}";
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Action act=default;
            int multiplicador = 1;
            //if (e.Key.HasFlag(Key.LeftCtrl) || e.Key.HasFlag(Key.RightCtrl))
            //    multiplicador = 3;
            //if(e.Key.HasFlag(Key.RightAlt))
            //    multiplicador *= 2;

            switch (e.Key)
            {
                case Key.F12:
                case Key.G:
                case Key.S:

                    Save();
                    break;
                case Key.Left:
                case Key.NumPad4:
                    act = () => calendar.Date = calendar.Date.AddMonths(-1* multiplicador);
                    break;
                case Key.Up:
                case Key.NumPad8:
                    act = () => calendar.Date = calendar.Date.AddYears(1 * multiplicador);
                    break;
                case Key.Right:
                case Key.NumPad6:
                    act = () => calendar.Date = calendar.Date.AddMonths(1 * multiplicador);
                    break;
                case Key.Down:
                case Key.NumPad2:
                    act = () => calendar.Date = calendar.Date.AddYears(-1 * multiplicador);
                    break;
                case Key.F5:
                    act = () => calendar.Date = DateTime.Now;
                    break;

            }
            if (!Equals(act, default))
            {
               Dispatcher.BeginInvoke(act);
            }

        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            Add(DateTime.Now,e.Data.GetData(DataFormats.FileDrop) as string[]);
        }
        private DateTime Add(DateTime date,string[] files)
        {
            SelectorDeFecha selector = new SelectorDeFecha();
            selector.Date = date;
            selector.ShowDialog();
            CalendarData.DataBase.Add(selector.Date.Value, files);
            DiaManagerWindow.UpdateDay(selector.Date.Value);
            return selector.Date.Value;
        }

        private void Window_DragOver(object sender, DragEventArgs e)
        {


            e.Effects = DragDropEffects.None;

            // If the DataObject contains string data, extract it.
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {

                e.Effects = DragDropEffects.Copy;

            }
        }
        public static DateTime AddFiles(DateTime startDate)
        {
            return Main.AddFilesDialog(startDate);
        }
    }
}
