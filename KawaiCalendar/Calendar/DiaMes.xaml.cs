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

        Semaphore semaphore;
        public DiaMes()
        {
            TicketIdActual = int.MinValue;
            semaphore = new Semaphore(1, 1);
            pos = 0;
            InitializeComponent();
        }
        public DiaMes(DateTime date) : this()
        {

            Date = date;
        }
        bool MouseHover { get; set; }
        int TicketIdActual { get; set; }
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

            await NextPic();
        }

        public async Task NextPic()
        {
            Action act;
            CalendarItem item;
            int ticketId;
            bool encontrado = false;

            TicketIdActual++;
            ticketId = TicketIdActual;


            if (!System.Diagnostics.Debugger.IsAttached)
                for (int i = 0, j = MiRandom.Next(Calendar.TOTALDAYS) * 10; i < j && Equals(ticketId, TicketIdActual) && !MouseHover; i++)
                    await Task.Delay(100);

            if (!Equals(Tag, default))
            {
                while (MouseHover && Equals(ticketId, TicketIdActual)) await Task.Delay(100);

                if (Equals(ticketId, TicketIdActual)) await Task.Delay(900);//asi no es tan repentino el cambio
            }

            if (Equals(ticketId, TicketIdActual))
            {
                act = () =>
               {
                   try
                   {
                       semaphore.WaitOne();
                       if (!Equals(GetItems(), default))
                           for (int i = 0; i < GetItems().Count && !encontrado && Equals(ticketId, TicketIdActual); i++)
                           {
                               encontrado = GetItems()[pos % GetItems().Count].Year <= Date.Year && !Equals(GetItems()[pos % GetItems().Count].Img, default);
                               if (!encontrado) pos++;
                               if (pos == int.MaxValue)
                                   pos = 0;
                           }
                       if (Equals(ticketId, TicketIdActual))
                       {
                           if (encontrado)
                           {
                               item = GetItems()[pos % GetItems().Count];
                               imgDia.SetImage(item.Img);
                               Tag = item;
                           }
                           else
                           {
                               imgDia.SetImage(new Bitmap(1, 1));
                               Tag = default;
                           }
                       }
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




        }

        public DateTime Date
        {
            get => date;
            set
            {
                date = value;
                tbDia.Text = date.Day + "";
                imgDia.SetImage(new Bitmap(1, 1));
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

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {

            if (!Equals(imgDia.Source, default) && !double.IsNaN(imgDia.Source.Width) && imgDia.Source.Width > 10)
                tbDia.Foreground = System.Windows.Media.Brushes.Transparent;
            MouseHover = true;

        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            tbDia.Foreground = System.Windows.Media.Brushes.Black;
            MouseHover = false;
        }

        private void UserControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            AbrirArchivo(Tag);
        }
        public static void AbrirArchivo(object tagFileName)
        {
            FileInfo file;
            Notifications.Wpf.Core.NotificationManager manager;

            if (!Equals(tagFileName, default))
            {
                file = new FileInfo(tagFileName + "");
                if (file.Exists)
                {
                    new PicViewer(tagFileName).Show();
                }
                else
                {
                    manager = new Notifications.Wpf.Core.NotificationManager();
                    manager.ShowAsync(new Notifications.Wpf.Core.NotificationContent()
                    {
                        Title = "Archivo no encontrado",
                        Message = $"'{(tagFileName as CalendarItem).FileName}'",
                        Type = Notifications.Wpf.Core.NotificationType.Information,

                    });
                }

            }
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DiaManagerWindow diaManager = new DiaManagerWindow(this);
            if (!diaManager.IsClosed)
                diaManager.Show();

        }
    }
}
