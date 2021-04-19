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
        public DiaManagerWindow()
        {
            InitializeComponent();
        }
        public DiaManagerWindow(DiaMes diaMes) : this()
        {

            Title = $"Dia {diaMes.Date.ToString().Split(' ')[0]} ";
            Date = diaMes.Date;
            Update();
            KeyDown += (s, e) => { if (e.Key == Key.F5) Update(); };
        }

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
            Action act;
            YearViewer yearViewer;
            DateTime date=MainWindow.AddFiles(Date);
            bool reloaded = false;
            //si al final se añade a este dia lo recargo
            if (date.Day == Date.Day && date.Month == Date.Month)
            {
                act = () =>
                {
                    stkYears.Children.ToArray().Any((y) =>
                    {
                        yearViewer = y as YearViewer;
                        reloaded = yearViewer.Date.Year == date.Year;
                        if (reloaded)
                            yearViewer.Reload();
                        return reloaded;
                    });
                    if (!reloaded)
                    {
                        stkYears.Children.Add(new YearViewer(date));
                        stkYears.Children.Sort();
                    }
                };
                Dispatcher.BeginInvoke(act);
            }
        }
    }
}
