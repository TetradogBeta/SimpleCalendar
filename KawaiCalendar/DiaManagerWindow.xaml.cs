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
        public DiaManagerWindow(CalendarData dataBase, DiaMes diaMes) : this()
        {
            SortedList<int, List<CalendarItem>> items = dataBase.GetItemsGroupByYear(new DateDay(diaMes.Date));
            Title = $"Dia {diaMes.Date.ToString().Split(' ')[0]} ";
            Date = diaMes.Date;
            DataBase = dataBase;
            foreach (KeyValuePair<int, List<CalendarItem>> item in items)
            {
                stkYears.Children.Add(new YearViewer(new DateTime(item.Key,Date.Month,Date.Day), DataBase, item));

            }
        }
        CalendarData DataBase { get; set; }
        DateTime Date { get; set; }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Action act;
            DateTime date=MainWindow.AddFiles(Date);
            bool reloaded = false;
            if (date.Day == Date.Day && date.Month == Date.Month)
            {
                act = () =>
                {
                    YearViewer yearViewer;
                    foreach (var year in stkYears.Children)
                    {
                        yearViewer = year as YearViewer;
                        if (yearViewer.Date.Year == date.Year)
                        {
                            yearViewer.Reload();
                            reloaded = true;
                        }
                    }
                    if (!reloaded)
                    {
                        stkYears.Children.Add(new YearViewer(date, DataBase, default));
                    }
                };
                Dispatcher.BeginInvoke(act);
            }
        }
    }
}
