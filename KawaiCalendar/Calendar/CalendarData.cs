﻿using System;
using System.Collections.Generic;
using System.Threading;
using Gabriel.Cat.S.Binaris;
using Gabriel.Cat.S.Extension;

namespace KawaiCalendar.Calendar
{
    public class CalendarData : IElementoBinarioComplejo
    {
        public static ElementoBinario Serializador = ElementoBinario.GetSerializador<CalendarData>();

        Semaphore semaphore;
        public CalendarData()
        {
            SetDate(DateTime.Now);
            DayItems = new SortedList<int, List<CalendarItem>>();
            semaphore = new Semaphore(1,1);

        }
        public SortedList<int, List<CalendarItem>> DayItems { get; set; }

        [IgnoreSerialitzer]
        public DateTime FirstDayMonth { get; set; }
        [IgnoreSerialitzer]
        public int DiasMes { get; set; }
        [IgnoreSerialitzer]
        public bool HasChanges { get; set; }

        ElementoBinario IElementoBinarioComplejo.Serialitzer => Serializador;
        public void WaitUseDic() => semaphore.WaitOne();
        public void ReleaseUseDic() => semaphore.Release();
        public void SetDate(DateTime date)
        {
            FirstDayMonth = new DateTime(date.Year, date.Month, 1);
            DiasMes = FirstDayMonth.AddMonths(1).AddDays(-1).Day;
        }
        public SortedList<int,List<CalendarItem>> GetItemsGroupByYear(int dayOfYear)
        {
            SortedList<int, List<CalendarItem>> items = new SortedList<int, List<CalendarItem>>();
            List<CalendarItem> dayItems;
            if (DayItems.ContainsKey(dayOfYear))
            {
                dayItems = DayItems[dayOfYear];
                for(int i = 0; i < dayItems.Count; i++)
                {
                    if (!items.ContainsKey(dayItems[i].Year))
                        items.Add(dayItems[i].Year, new List<CalendarItem>());
                    items[dayItems[i].Year].Add(dayItems[i]);
                }
            }
            return items;
        }

        public void Remove(DateTime date,CalendarItem calendarItem)
        {
            WaitUseDic();
            DayItems[date.DayOfYear].Remove(calendarItem);

            ReleaseUseDic();
            HasChanges = true;
        }
    }
}
