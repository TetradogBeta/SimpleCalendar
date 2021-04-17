using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        public const int TOTALDAYS = DIASSEMANA * 6;
        const int CICLO = TOTALDAYS * 1000;//cada 42 segundos porque mola :D vale no xD
        public static CalendarData DataBase { get; set; }
        public static readonly string[] FormatosValidos = { ".jpeg", ".gif", ".jpg", ".png", ".bmp" };

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
            date = DateTime.Now.AddYears(-2);

            if (Equals(DataBase, default))
                DataBase = new CalendarData();

            for (int i = 0; i < DIAS.Length; i++)
            {
                view = new Viewbox();
                view.Child = new TextBlock { Text = DIAS[i] + "", Foreground = System.Windows.Media.Brushes.Gray };
                ugMes.Children.Add(view);
            }
            for (int i = 0; i < TOTALDAYS; i++)
            {
                ugMes.Children.Add(new DiaMes(GetDate(i)));
            }

            cambioImgs = new Timer(CambioImagenes);
            if (!System.Diagnostics.Debugger.IsAttached)
                cambioImgs.Change(1000, CICLO);
            else cambioImgs.Change(1000, 2000);

            Date = Date;

        }


        public bool HasChanges { get; set; }
        public DateTime Date
        {
            get => date;
            set
            {
                DiaMes dia;
                int dayOfWeek;
                int dayOfYear;
                bool hasChanges = value.Month != date.Month || value.Year != date.Year;
                date = value;
                DataBase.SetDate(date);

                dayOfWeek = (int)DataBase.FirstDayMonth.DayOfWeek - 1;
                if (dayOfWeek < 0)
                    dayOfWeek = 6;//domingo
                dayOfYear = DataBase.FirstDayMonth.DayOfYear;
                //pongo el dia
                for (int i = 0; i < TOTALDAYS; i++)
                {
                    dia = ugMes.Children[i + DIASSEMANA] as DiaMes;
                    dia.Date = DataBase.FirstDayMonth.AddDays(i - dayOfWeek);
                    if (hasChanges)
                    {
                        dia.SetItems(GetList(dia.Date.DayOfYear));
                        if (!Equals(dia.Tag, default))
                            dia.NextPic();
                    }
                    dia.IsSelected = null;
                }
                for (int i = dayOfWeek, j = 0; j < DataBase.DiasMes; j++, i++)
                {
                    dia = ugMes.Children[i + DIASSEMANA] as DiaMes;
                    dia.Date = DataBase.FirstDayMonth.AddDays(i - dayOfWeek);
                    dia.IsSelected = false;
                }
                //selecciono el dia del mes
                (ugMes.Children[DIASSEMANA + dayOfWeek + date.Day - 1] as DiaMes).IsSelected = true;

                if (ChangeDate != null)
                    ChangeDate(this, new EventArgs());
            }
        }


        private void CambioImagenes(object state = null)
        {

            int diaACambiar;
            DateTime dateAux;
            Action act;
            List<int> posiciones = new List<int>(System.Linq.Enumerable.Range(0, TOTALDAYS));
            int cambios = MiRandom.Next(TOTALDAYS);


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
                        (ugMes.Children[diaACambiar + DIASSEMANA] as DiaMes).NextPic();
                    }
                    cambiando = false;
                };
                Dispatcher.BeginInvoke(act);
            }
        }

        private DateTime GetDate(int posMonth)
        {

            posMonth -= (int)DataBase.FirstDayMonth.DayOfWeek - 1;
            return DataBase.FirstDayMonth.AddDays(posMonth);
        }

        private List<CalendarItem> GetList(int dayOfYear)
        {
            List<CalendarItem> items;

            if (!DataBase.DayItems.ContainsKey(dayOfYear))
                items = default;
            else items = DataBase.DayItems[dayOfYear];

            return items;
        }
        public void Add(DateTime date, params string[] items)
        {
            Notifications.Wpf.Core.NotificationManager manager = new Notifications.Wpf.Core.NotificationManager();

            if (!DataBase.DayItems.ContainsKey(date.DayOfYear))
                DataBase.DayItems.Add(date.DayOfYear, new List<CalendarItem>());
            DataBase.DayItems[date.DayOfYear].AddRange(items.Filtra(s =>
            {

                bool result = FormatosValidos.Contains(new FileInfo(s).Extension);
                if (!result)
                {

                    manager.ShowAsync(new Notifications.Wpf.Core.NotificationContent()
                    {
                        Title = "Incompatible file",
                        Message = System.IO.Path.GetFileName(s),
                        Type = Notifications.Wpf.Core.NotificationType.Error
                    });
                }
                return result;
            }).Convert((img) => new CalendarItem { FilePic = img, Year = date.Year }));
            HasChanges = true;
        }
    }
    public class CalendarData : IElementoBinarioComplejo
    {
        public static ElementoBinario Serializador = ElementoBinario.GetSerializador<CalendarData>();

        public CalendarData()
        {
            SetDate(DateTime.Now);

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
    public class CalendarItem : IElementoBinarioComplejo,ISaveAndLoad, IComparable, IComparable<CalendarItem>
    {
        public const string EXTENSION = ".jpeg";
        public static readonly System.Drawing.Imaging.ImageFormat Formato = System.Drawing.Imaging.ImageFormat.Jpeg;

        public static ElementoBinario Serializador = ElementoBinario.GetSerializador<CalendarItem>();
        public static string Directory;
        private Bitmap img;

        static CalendarItem()
        {
            Directory = new DirectoryInfo("Thumbnails").FullName;
            if (!System.IO.Directory.Exists(Directory))
                System.IO.Directory.CreateDirectory(Directory);
        }

        public int Year { get; set; }
        public string FilePic { get; set; }
        public string FileName => System.IO.Path.GetFileName(FilePic);

        public string Hash { get; set; }
        public string IdRapido { get; set; }
        [IgnoreSerialitzer]
        public Bitmap Img
        {
            get
            {
                Bitmap bmp;
                string path;
                int proporcion;
    

                if (Equals(img, default))
                {
                    if (Equals(Hash, default))
                    {
                        bmp = new Bitmap(FilePic);

                        Hash = bmp.GetBytes().Hash();
                        IdRapido = new FileInfo(FilePic).IdUnicoRapido();

                        if (bmp.Width < bmp.Height)
                            proporcion = bmp.Height/ bmp.Width;
                        else proporcion = bmp.Width/ bmp.Height;

                        img = bmp.Escala(proporcion);
                        img.Save(System.IO.Path.Combine(Directory, IdRapido + EXTENSION),Formato);
                    }
                    else
                    {
                        path = System.IO.Path.Combine(Directory, IdRapido + EXTENSION);
                        if (File.Exists(path))
                        {
                            img = new Bitmap(path);
                        }else if (File.Exists(FilePic))
                        {
                            Hash = default;
                            img = Img;
                        }
                    }
                }
                return img;
            }
            set
            {
                img = value;
            }
        }

        ElementoBinario IElementoBinarioComplejo.Serialitzer => Serializador;

        public int CompareTo(object obj)
        {
            return CompareTo(obj as CalendarItem);
        }

        public int CompareTo([AllowNull] CalendarItem other)
        {
            int compareTo = Equals(other, default) ? -1 : 0;
            if (compareTo == 0)
                compareTo = Year.CompareTo(other.Year);
            return compareTo;
        }
        public override string ToString()
        {
            return FilePic;
        }

        void ISaveAndLoad.Load()
        {

        }

        void ISaveAndLoad.Save()
        {
            Img = Img;
        }
    }
}
