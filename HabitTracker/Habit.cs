using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTracker
{
    internal class Habit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Measurement { get; set; }

        public override string ToString()
        {
            return $"{Name} - {Measurement}";
        }
    }

    internal class Entry
    {
        public int HabitId { get; set; }
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public float Qty { get; set; }

        public override string ToString()
        {
            DateTime cleanDt = (DateTime)Date;
            DateOnly date = DateOnly.FromDateTime(cleanDt);
            return $"{date} - {Qty}";
        }
    }
}
