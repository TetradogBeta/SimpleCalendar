using System;
using System.Collections.Generic;
using Gabriel.Cat.S.Binaris;

namespace KawaiCalendar.Calendar
{
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
}
