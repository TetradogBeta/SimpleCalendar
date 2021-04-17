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
        public DiaManagerWindow(CalendarData dataBase,DiaMes diaMes) : this()
        {
            SortedList<int, List<CalendarItem>> items = dataBase.GetItemsGroupByYear(diaMes.Date.DayOfYear);
            Title = $"Dia {diaMes.Date.ToString().Split(' ')[0]} ";
            Date = diaMes.Date;
            foreach (KeyValuePair<int, List<CalendarItem>> item in items)
            {
                stkYears.Children.Add(new YearViewer(Date,dataBase,item));

            }
        }
        DateTime Date { get; set; }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.AddFiles(Date);
        }
    }
}
