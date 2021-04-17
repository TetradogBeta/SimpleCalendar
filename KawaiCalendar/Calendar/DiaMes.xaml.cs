using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
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
using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Utilitats;

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
        Task next;
        Semaphore semaphore;
        public DiaMes()
        {
            semaphore = new Semaphore(1, 1);
            pos = 0;
            InitializeComponent();
            next = Task.Delay(1);
        }
        public DiaMes(DateTime date) : this()
        {

            Date = date;
        }

        public IList<CalendarItem> GetItems()
        {
            return items;
        }

        public async Task SetItems(IList<CalendarItem> value)
        {
            try
            {
                semaphore.WaitOne();
                items = value;
            }
            catch { }
            finally
            {
                semaphore.Release();
            }

            next = NextPic();

            await next;
        }

        public async Task NextPic()
        {
            Action act;
            bool encontrado = false;

            if (!System.Diagnostics.Debugger.IsAttached)
                await Task.Delay(MiRandom.Next(Calendar.TOTALDAYS) * 1000);

            act = () =>
            {
                try
                {
                    semaphore.WaitOne();
                    if (!Equals(GetItems(), default))
                        for (int i = 0; i < GetItems().Count && !encontrado; i++)
                        {
                            encontrado = GetItems()[pos % GetItems().Count].Year <= Date.Year && File.Exists(GetItems()[pos % GetItems().Count].FilePic);
                            if (!encontrado) pos++;
                            if (pos == int.MaxValue)
                                pos = 0;
                        }
                    if (encontrado)
                        imgDia.SetImage(new Bitmap(GetItems()[pos % GetItems().Count].FilePic));
                    else imgDia.SetImage(new Bitmap(1, 1));
                }
                catch { }
                finally
                {
                    semaphore.Release();

                    pos++;
                    if (pos == int.MaxValue)
                        pos = 0;


                }
            };
           await Dispatcher.BeginInvoke(act);





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
