using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
        private IList<CalendarItem> items;
        private DateTime date;
        private bool? isSelected;
        private int pos;

        public DiaMes()
        {

            pos = 0;
            InitializeComponent();
        }
        public DiaMes(DateTime date) : this()
        {

            Date = date;
        }

        public IList<CalendarItem> Items
        {
            get => items;
            set
            {
                items = value;
                NextPic();
            }
        }

        private void NextPic()
        {
            if (!Equals(Items, default) && File.Exists(Items[pos % Items.Count].FilePic))
                imgDia.SetImage(new Bitmap(Items[pos % Items.Count].FilePic));
            else imgDia.SetImage(new Bitmap(1, 1));


            pos++;
            if (pos == int.MaxValue)
                pos = 0;
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

        public bool? IsSelected
        {
            get => isSelected;
            set
            {
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
