using Gabriel.Cat.S.Extension;
using KawaiCalendar.Calendar;
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
using System.Linq;
namespace KawaiCalendar
{
    /// <summary>
    /// Lógica de interacción para DiaManagerWindow.xaml
    /// </summary>
    public partial class DiaManagerWindow : Window
    {
        static SortedList<DateDay, DiaManagerWindow> DicDiasManager = new SortedList<DateDay, DiaManagerWindow>();
        public DiaManagerWindow()
        {
            InitializeComponent();
        }
        public DiaManagerWindow(DiaMes diaMes) : this()
        {
            if (!DicDiasManager.ContainsKey(diaMes.Date))
            {
                DicDiasManager.Add(diaMes.Date, this);
                Title = $"Dia {diaMes.Date.ToString().Split(' ')[0]} ";
                Date = diaMes.Date;
                Update();
                KeyDown += (s, e) => { if (e.Key == Key.F5) Update(); };
                Closing += (s, e) =>{if (DicDiasManager.ContainsKey(Date)) DicDiasManager.Remove(Date);};

            }
            else
            {
                DicDiasManager[diaMes.Date].Focus();
                IsClosed = true;
                this.Close();
            }

        }
        public bool IsClosed { get; set; }
        DateTime Date { get; set; }
        public void Update()
        {
            SortedList<int, List<CalendarItem>> items;
            Action act = () =>
            {

                items = CalendarData.DataBase.GetItemsGroupByYear(new DateDay(Date));
                stkYears.Children.Clear();
                foreach (KeyValuePair<int, List<CalendarItem>> item in items)
                {
                    stkYears.Children.Add(new YearViewer(new DateTime(item.Key, Date.Month, Date.Day), item));

                }
            };
            Dispatcher.BeginInvoke(act);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
           MainWindow.AddFiles(Date);
 
        }
        public static void UpdateDay(DateTime day)
        {
            Action act;
            YearViewer yearViewer;
            DiaManagerWindow diaManager;
            bool reloaded = false;
            //si al final se añade a este dia lo recargo
            if (DicDiasManager.ContainsKey(day))
            {
                diaManager = DicDiasManager[day];
                act = () =>
                {
                   diaManager.stkYears.Children.ToArray().Any((y) =>
                    {
                        yearViewer = y as YearViewer;
                        reloaded = yearViewer.Date.Year == day.Year;
                        if (reloaded)
                            yearViewer.Reload();
                        return reloaded;
                    });
                    if (!reloaded)
                    {
                        diaManager.stkYears.Children.Add(new YearViewer(day));
                        diaManager.stkYears.Children.Sort();
                    }
                };
                diaManager.Dispatcher.BeginInvoke(act);
            }
        }
    }
}
