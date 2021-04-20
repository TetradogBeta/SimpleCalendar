using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Gabriel.Cat.S.Binaris;
using Gabriel.Cat.S.Extension;

namespace KawaiCalendar.Calendar
{
    public class CalendarData : IElementoBinarioComplejo,ISaveAndLoad
    {
        public static ElementoBinario Serializador = ElementoBinario.GetSerializador<CalendarData>();

        [IgnoreSerialitzer]
        public static readonly string[] FormatosValidos = { ".jpeg", ".gif", ".jpg", ".png", ".bmp" };
        [IgnoreSerialitzer]
        public static CalendarData DataBase { get; set; } = new CalendarData();
        public CalendarData()
        {
            DayItems = new SortedList<DateDay, List<CalendarItem>>();
            SetDate(DateTime.Now);
        }
        public SortedList<DateDay, List<CalendarItem>> DayItems { get; set; }

        [IgnoreSerialitzer]
        public DateTime FirstDayMonth { get; set; }
        [IgnoreSerialitzer]
        public int DiasMes { get; set; }
        [IgnoreSerialitzer]
        public bool HasChanges { get; set; }

        ElementoBinario IElementoBinarioComplejo.Serialitzer => Serializador;

        public void SetDate(DateTime date)
        {
            FirstDayMonth = new DateTime(date.Year, date.Month, 1);
            DiasMes = FirstDayMonth.AddMonths(1).AddDays(-1).Day;
        }
        public SortedList<int,List<CalendarItem>> GetItemsGroupByYear(DateDay dayOfYear, int year=-1)
        {
            SortedList<int, List<CalendarItem>> items = new SortedList<int, List<CalendarItem>>();
            List<CalendarItem> dayItems;
            if (DayItems.ContainsKey(dayOfYear))
            {
                dayItems = DayItems[dayOfYear];
                for(int i = 0; i < dayItems.Count; i++)
                {
                    if (year!=-1 && year==dayItems[i].Year && !items.ContainsKey(dayItems[i].Year) || !items.ContainsKey(dayItems[i].Year))
                        items.Add(dayItems[i].Year, new List<CalendarItem>());
                    if(items.ContainsKey(dayItems[i].Year))
                       items[dayItems[i].Year].Add(dayItems[i]);
                }
            }
            return items;
        }

        public void Remove(DateTime date,CalendarItem calendarItem)
        {

            DayItems[new DateDay(date)].Remove(calendarItem);

            HasChanges = true;
        }
        public List<CalendarItem> GetList(DateDay dayOfYear)
        {
            List<CalendarItem> items;

            if (!this.DayItems.ContainsKey(dayOfYear))
                items = default;
            else items = this.DayItems[dayOfYear];

            return items;
        }
        public void Add(DateTime date, params string[] items)
        {
            Notifications.Wpf.Core.NotificationManager manager = new Notifications.Wpf.Core.NotificationManager();
            DateDay dateDay = new DateDay(date);
            if (!this.DayItems.ContainsKey(dateDay))
                this.DayItems.Add(dateDay, new List<CalendarItem>());
          
                this.DayItems[dateDay].AddRange(items.Filtra(s =>
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
                }).Convert((img) => new CalendarItem { FilePic = img, Year = date.Year,Date= dateDay }));
         
            HasChanges = true;
        }

        void ISaveAndLoad.Save()
        {

        }

        void ISaveAndLoad.Load()
        {
           foreach(var items in DayItems)
            {
                for (int i = 0; i < items.Value.Count; i++)
                    items.Value[i].Date = items.Key;
            }
        }
    }
}
