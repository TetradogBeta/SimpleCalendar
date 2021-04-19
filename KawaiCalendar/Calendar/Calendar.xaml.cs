using System;
using System.Collections.Generic;
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
using System.Windows.Threading;
using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Utilitats;

namespace KawaiCalendar.Calendar
{
    /// <summary>
    /// Lógica de interacción para Calendar.xaml
    /// </summary>
    public partial class Calendar : UserControl
    {


        const int DIASSEMANA = 7;
        public const int TOTALDAYS = DIASSEMANA * 6;
        const int CICLO = TOTALDAYS * 1000;//cada 42 segundos porque mola :D vale no xD



        bool cambiando;
        DateTime date;
        Timer cambioImgs;


        public event EventHandler ChangeDate;
        public Calendar()
        {
            const string DIAS = "LMXJVSD";

            Viewbox view;
            DiaMes diaMes;

            InitializeComponent();

            cambiando = false;
            date = DateTime.Now.AddYears(-2);


            for (int i = 0; i < DIAS.Length; i++)
            {
                view = new Viewbox();
                view.Child = new TextBlock { Text = DIAS[i] + "", Foreground = System.Windows.Media.Brushes.Gray };
                ugMes.Children.Add(view);
            }
            for (int i = 0; i < TOTALDAYS; i++)
            {
                diaMes = new DiaMes(GetDate(i));
                ugMes.Children.Add(diaMes);
            }

            cambioImgs = new Timer(CambioImagenes);
            if (!System.Diagnostics.Debugger.IsAttached)
                cambioImgs.Change(1000, CICLO);
            else cambioImgs.Change(1000, 2000);

            UpdateDays();
            Task.Delay(150).ContinueWith(t =>
            {
                CambioImagenes();
            });


        }



        public DateTime Date
        {
            get => date;
            set
            {

                bool hasChanges = value.Month != date.Month || value.Year != date.Year;
                date = value;
                CalendarData.DataBase.SetDate(date);
                UpdateDays(hasChanges);


                if (ChangeDate != null)
                    ChangeDate(this, new EventArgs());
            }
        }
        private void UpdateDays(bool hasChanges = false)
        {
            DiaMes dia;
            int dayOfWeek;

            dayOfWeek = (int)CalendarData.DataBase.FirstDayMonth.DayOfWeek - 1;
            if (dayOfWeek < 0)
                dayOfWeek = 6;//domingo
            //pongo el dia
            for (int i = 0; i < TOTALDAYS; i++)
            {
                dia = ugMes.Children[i + DIASSEMANA] as DiaMes;
                dia.Date = CalendarData.DataBase.FirstDayMonth.AddDays(i - dayOfWeek);
                if (hasChanges)
                {
                    _ = dia.SetItems(CalendarData.DataBase.GetList(new DateDay(dia.Date)));
                    if (!Equals(dia.Tag, default))
                        _ = dia.NextPic();
                }
                dia.IsSelected = null;
            }
            for (int i = dayOfWeek, j = 0; j < CalendarData.DataBase.DiasMes; j++, i++)
            {
                dia = ugMes.Children[i + DIASSEMANA] as DiaMes;
                dia.Date = CalendarData.DataBase.FirstDayMonth.AddDays(i - dayOfWeek);
                dia.IsSelected = false;
            }
            //selecciono el dia del mes
            (ugMes.Children[DIASSEMANA + dayOfWeek + date.Day - 1] as DiaMes).IsSelected = true;
        }

        private void CambioImagenes(object state = null)
        {

            int diaACambiar;
            DateTime dateAux;
            Action act;
            List<int> posiciones = default;
            int cambios;
            DispatcherOperation dispatcher;
            act = () =>
            {
                posiciones = new List<int>(ugMes.Children.ToArray().Filtra((d) =>
                 {
                     DiaMes dia = d as DiaMes;
                     bool correcto = !Equals(dia, default);
                     if (correcto)
                         correcto = !Equals(dia.GetItems(), default) && dia.GetItems().Count > 0;
                     return correcto;


                 }).Convert((d) => ugMes.Children.IndexOf(d as UIElement)));
            };
            dispatcher = Dispatcher.BeginInvoke(act);
            while (dispatcher.Status != DispatcherOperationStatus.Completed) Thread.Sleep(100);
            cambios = MiRandom.Next(posiciones.Count);


            if (!cambiando)
            {
                cambiando = true;
                act = () =>
                {
                    for (int i = 0; i < cambios; i++)
                    {
                        diaACambiar = posiciones[MiRandom.Next(posiciones.Count)];
                        posiciones.Remove(diaACambiar);

                        dateAux = GetDate(diaACambiar);
                        _ = (ugMes.Children[diaACambiar + DIASSEMANA] as DiaMes).NextPic();
                    }
                    cambiando = false;
                };
                Dispatcher.BeginInvoke(act);
            }
        }

        private DateTime GetDate(int posMonth)
        {

            posMonth -= (int)CalendarData.DataBase.FirstDayMonth.DayOfWeek - 1;
            return CalendarData.DataBase.FirstDayMonth.AddDays(posMonth);
        }



    }
}
