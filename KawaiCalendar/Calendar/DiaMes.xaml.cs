using System;
using System.Collections.Generic;
using System.Drawing;
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
using Gabriel.Cat.S.Extension;

namespace KawaiCalendar.Calendar
{
    /// <summary>
    /// Lógica de interacción para DiaMEs.xaml
    /// </summary>
    public partial class DiaMes : UserControl
    {
        private CalendarItem item;
        private DateTime date;
        private bool? isSelected;

        public DiaMes()
        {
            InitializeComponent();
        }
        public DiaMes(DateTime date) : this()
        {

            Date = date;
        }

        public CalendarItem Item
        {
            get => item;
            set
            {

                Action act = () =>
                {
                    item = value;
                    if (!Equals(item, default))
                        imgDia.SetImage(new Bitmap(item.FilePic));
                    else imgDia.SetImage(new Bitmap(1, 1));
                };
                Dispatcher.BeginInvoke(act);
            }
        }
        public DateTime Date
        {
            get => date;
            set
            {
                date = value;
                tbDia.Text = date.Day + "";
            }
        }

        public bool? IsSelected { 
            get => isSelected; 
            set { 

                isSelected = value;
                if (!isSelected.HasValue)
                {
                    tbDia.FontWeight = FontWeights.Light;
                }
                else if (isSelected.Value)
                {
                    tbDia.FontWeight = FontWeights.Bold;
                }
                else
                {
                    tbDia.FontWeight = FontWeights.Normal;
                }
            } 
        }
    }
}
