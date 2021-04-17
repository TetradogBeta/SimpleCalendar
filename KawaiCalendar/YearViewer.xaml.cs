using Gabriel.Cat.S.Extension;
using KawaiCalendar.Calendar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Lógica de interacción para YearViewer.xaml
    /// </summary>
    public partial class YearViewer : UserControl
    {
        public YearViewer()
        {
            InitializeComponent();
        }
        public YearViewer(DateTime date, CalendarData dataBase, KeyValuePair<int, List<Calendar.CalendarItem>> item) : this()
        {

            DataBase = dataBase;
            Date = date;
            Year =date.Year;
            Reload(item);

        }
        CalendarData DataBase { get; set; }
        DateTime Date { get; set; }
        public int Year { get; private set; }
        public void Reload(KeyValuePair<int, List<Calendar.CalendarItem>> item=default)
        {
            Image img;
            List<Calendar.CalendarItem> items =Equals(item.Value,default)? DataBase.GetItemsGroupByYear(Date.DayOfYear,Year)[Year]:item.Value;
            tbYear.Text = Year + "";
            for (int i = 0; i < items.Count; i++)
            {
                img = new Image();
                img.Stretch = Stretch.UniformToFill;
                img.SetImage(items[i].GetImgOrInvertida());
                img.Tag = items[i];
                img.MouseLeftButtonDown += (s, e) =>
                {
                    Action act;
                    if (MessageBox.Show("¿Quieres quitarlo del dia?", "Atención", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        DataBase.Remove(Date, (s as Image).Tag as Calendar.CalendarItem);
                        act = () =>
                        {
                            ugItemsYear.Children.Remove(s as UIElement);
                            if (ugItemsYear.Children.Count == 0)
                                (Parent as StackPanel).Children.Remove(this);
                        };
                        Dispatcher.BeginInvoke(act);
                    }

                };
                ugItemsYear.Children.Add(img);
            }
        }
    }
}
