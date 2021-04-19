using Gabriel.Cat.S.Binaris;
using System;
using System.Collections.Generic;
using System.Text;

namespace KawaiCalendar.Calendar
{
    public class DateDay : IComparable<DateDay>, IComparable, IElementoBinarioComplejo
    {
        public static ElementoBinario Serializador = ElementoBinario.GetSerializador<DateDay>();

        public DateDay() { }
        public DateDay(DateTime date)
        {
            Day = date.Day;
            Month = date.Month;
        }
        public int Day { get; set; }
        public int Month { get; set; }

        ElementoBinario IElementoBinarioComplejo.Serialitzer => Serializador;

        int IComparable<DateDay>.CompareTo(DateDay other)
        {
            return ICompareTo(other);
        }

        int IComparable.CompareTo(object obj)
        {
            return ICompareTo(obj as DateDay);
        }

        private int ICompareTo(DateDay other)
        {
            int compareTo = Equals(other, default) ? -1 : 0;
            if (compareTo == 0)
                compareTo = Day.CompareTo(other.Day);
            if (compareTo == 0)
                compareTo = Month.CompareTo(other.Month);
            return compareTo;
        }
        public static implicit  operator DateDay(DateTime date)=> new DateDay(date);
    }
}
