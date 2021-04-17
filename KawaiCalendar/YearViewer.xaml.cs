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
        public YearViewer(DateTime date,CalendarData dataBase,KeyValuePair<int, List<CalendarItem>> items) : this()
        {
            Image img;
            DataBase = dataBase;
            Date = date;
            tbYear.Text = items.Key+"";
            for(int i=0;i<items.Value.Count;i++)
            {
                img = new Image();
                img.Stretch = Stretch.UniformToFill;
                img.SetImage(items.Value[i].GetImgOrInvertida());
                img.Tag = items.Value[i];
                img.MouseLeftButtonDown += (s, e) => {
                    Action act;
                    if (MessageBox.Show("¿Quieres quitarlo del dia?", "Atención", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        DataBase.Remove(Date, (s as Image).Tag as CalendarItem);
                        act = () => ugItemsYear.Children.Remove(s as UIElement);
                        Dispatcher.BeginInvoke(act);
                    }
                
                };
                ugItemsYear.Children.Add(img);
            }
        }
        CalendarData DataBase { get; set; }
        DateTime Date { get; set; }
    }
}
