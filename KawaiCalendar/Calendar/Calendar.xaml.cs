using System;
using System.Collections.Generic;
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
using Gabriel.Cat.S.Binaris;
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
        const int TOTALDAYS = DIASSEMANA * 6;
        const int CICLO = TOTALDAYS * 1000;//cada 42 segundos porque mola :D vale no xD

        bool cambiando;
        DateTime date;
        Timer cambioImgs;

        public event EventHandler ChangeDate;
        public Calendar()
        {
            const string DIAS = "LMXJVSD";
            Viewbox view;
            InitializeComponent();

            cambiando = false;

            date = DateTime.Now;
            Data = new CalendarData();
            for (int i = 0; i < DIAS.Length; i++)
            {
                view = new Viewbox();
                view.Child = new TextBlock { Text = DIAS[i] + "", Foreground=Brushes.Gray };
                ugMes.Children.Add(view);
            }
            for (int i = 0; i < TOTALDAYS; i++)
            {
                ugMes.Children.Add(new DiaMes(GetDate(i)));
            }

            cambioImgs = new Timer(CambioImagenes);
            cambioImgs.Change(CICLO, CICLO);
            CambioImagenes();

        }



        public DateTime Date
        {
            get => date;
            set
            {
                DiaMes dia;
                int dayOfWeek = (int)Data.FirstDayMonth.DayOfWeek - 1;
                date = value;
                Data.SetDate(date);

                //pongo el dia
                for (int i = 0; i < TOTALDAYS; i++)
                {
                    dia =ugMes.Children[i + DIASSEMANA] as DiaMes;
                    dia.Date = Data.FirstDayMonth.AddDays(i - dayOfWeek);
                    dia.IsSelected = null;
                }
                for (int i = dayOfWeek,j=0 ; j < Data.DiasMes;j++, i++)
                {
                    dia = ugMes.Children[i + DIASSEMANA] as DiaMes;
                    dia.Date = Data.FirstDayMonth.AddDays(i - dayOfWeek);
                    dia.IsSelected = false;
                }
                //selecciono el dia del mes
                (ugMes.Children[DIASSEMANA + dayOfWeek+date.Day-1] as DiaMes).IsSelected=true;

                CambioImagenes();
                if (ChangeDate != null)
                    ChangeDate(this, new EventArgs());
            }
        }
        public CalendarData Data { get; set; }

        private void CambioImagenes(object state = null)
        {

            int diaACambiar;
            DateTime dateAux;
            Action act;

            int cambios = MiRandom.Next(TOTALDAYS);
            if (!cambiando)
            {
                cambiando = true;
                act = () =>
                {
                    for (int i = 0; i < cambios; i++)
                    {
                        diaACambiar = MiRandom.Next(TOTALDAYS);
                        dateAux = GetDate(diaACambiar);
                        (ugMes.Children[diaACambiar + DIASSEMANA] as DiaMes).Item = GetRandom(dateAux.DayOfYear);
                    }
                    cambiando = false;
                };
                Dispatcher.BeginInvoke(act);
            }
        }

        private DateTime GetDate(int posMonth)
        {

            posMonth -= (int)Data.FirstDayMonth.DayOfWeek -1;
            return Data.FirstDayMonth.AddDays(posMonth);
        }

        private CalendarItem GetRandom(int dayOfYear)
        {
            CalendarItem item;

            if (!Data.DayItems.ContainsKey(dayOfYear))
                item = default;
            else item = Data.DayItems[dayOfYear][MiRandom.Next(Data.DayItems[dayOfYear].Count)];

            return item;
        }
        public void Add(DateTime date, params string[] items)
        {
            if (!Data.DayItems.ContainsKey(date.DayOfYear))
                Data.DayItems.Add(date.DayOfYear, new List<CalendarItem>());
            Data.DayItems[date.DayOfYear].AddRange(items.Convert((i) => new CalendarItem { FilePic = i, Year = date.Year }));
        }
    }
    public class CalendarData : IElementoBinarioComplejo
    {
        public static ElementoBinario Serializador = ElementoBinario.GetSerializador<CalendarData>();

        public CalendarData()
        {
           DateTime date = DateTime.Now;
            FirstDayMonth = new DateTime(date.Year, date.Month, 1);
            DiasMes = FirstDayMonth.AddMonths(1).AddDays(-1).Day;

        }
        public SortedList<int, List<CalendarItem>> DayItems { get; set; } = new SortedList<int, List<CalendarItem>>();

        [IgnoreSerialitzer]
        public DateTime FirstDayMonth { get; set; }
        [IgnoreSerialitzer]
        public int DiasMes { get; set; }

        ElementoBinario IElementoBinarioComplejo.Serialitzer => Serializador;
        public void SetDate(DateTime date)
        {
            FirstDayMonth = new DateTime(date.Year, date.Month, 1);
            DiasMes = FirstDayMonth.AddMonths(1).AddDays(-1).Day;
        }
    }
    public class CalendarItem : IElementoBinarioComplejo
    {
        public static ElementoBinario Serializador = ElementoBinario.GetSerializador<CalendarItem>();


        public int Year { get; set; }
        public string FilePic { get; set; }

        ElementoBinario IElementoBinarioComplejo.Serialitzer => Serializador;
    }
}
