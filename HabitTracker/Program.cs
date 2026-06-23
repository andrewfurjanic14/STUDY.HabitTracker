
using System.Runtime.CompilerServices;

namespace HabitTracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var db = new DataAccess();

            bool menu = true;
            while (menu)
            {
                PrintMenu();
                int.TryParse(Console.ReadLine(), out int userInput);   
                switch (userInput)
                {
                    case 1: // View Habit
                        Habit? chosenHabit = RequestHabit(db);
                        if (chosenHabit is null)
                        {
                            Console.WriteLine("There are currently no habits to choose from");
                            break;
                        }

                        var entries = db.GetHabitEntries(chosenHabit);
                        if (entries.Count <= 0)
                        {
                            Console.WriteLine("Nothing has been logged to this habit yet.");
                            break;
                        }
                        string unit = db.GetEntryMeasurement(entries[0]);

                        foreach (Entry e in entries)
                        {
                            Console.WriteLine($"{e} {unit}");
                        }
                        break;
                    case 2: // Create New Habit
                        // Get Name
                        Console.Write("Enter habit name: ");
                        string? name = Console.ReadLine();
                        while (string.IsNullOrWhiteSpace(name))
                        {
                            Console.WriteLine("Invalid name. Try again.");
                            name = Console.ReadLine();
                        }

                        // Get Measurement
                        Console.Write("Enter the habit's unit of measurement: ");
                        string? measurement = Console.ReadLine();
                        while (string.IsNullOrWhiteSpace(measurement))
                        {
                            Console.WriteLine("Invalid measurement. Try again.");
                            measurement = Console.ReadLine();
                        }
                            
                        db.CreateHabit(new Habit() { Name = name, Measurement = measurement });
                        break;
                    case 3: // Log Entry for Existing Habit
                        Habit? habit = RequestHabit(db);
                        if (habit is null)
                        {
                            Console.WriteLine("There are currently no habits to choose from");
                            break;
                        }
                        Entry entry = new Entry() { HabitId = habit.Id };
                        Console.Write("Choose a quantity to log: ");
                        bool valid = float.TryParse(Console.ReadLine(), out float qty);
                        while (!valid || qty < 0)
                        {
                            Console.WriteLine("Unknown command detected. Try again.");
                            valid = float.TryParse(Console.ReadLine(), out qty);
                        }
                        Console.WriteLine("Choose a date to log, or just press ENTER to use today's date" +
                                          "\n\tFormat as MM/DD/YYYY");
                        string? input = Console.ReadLine();
                        valid = DateTime.TryParse(input, out DateTime date);
                        while (!valid && input != string.Empty)
                        {
                            Console.WriteLine("Unknown command detected. Try again.");
                            input = Console.ReadLine();
                            valid = DateTime.TryParse(input, out date);
                        }
                        entry.Qty = qty;
                        if (input == string.Empty)
                            entry.Date = null; // CreateEntry() will handle null by using today's date
                        else
                            entry.Date = date;

                        db.CreateEntry(entry);
                        break;
                    case 4: // Delete Habit Entry
                        chosenHabit = RequestHabit(db);
                        if (chosenHabit is null)
                        {
                            Console.WriteLine("There are currently no habits to choose from");
                            break;
                        }

                        entries = db.GetHabitEntries(chosenHabit);
                        if (entries.Count <= 0)
                        {
                            Console.WriteLine("Nothing has been logged to this habit yet.");
                            break;
                        }
                        unit = db.GetEntryMeasurement(entries[0]);

                        Console.WriteLine("Choose which entry to delete:");
                        int j = 1;
                        foreach (Entry e in entries)
                        {
                            Console.WriteLine($"{j}. {e} {unit}");
                            j++;
                        }

                        valid = int.TryParse(Console.ReadLine(), out int inputValue);
                        while (!valid || inputValue < 1 || inputValue > entries.Count)
                        {
                            Console.WriteLine("Unknown command. Try again.");
                            valid = int.TryParse(Console.ReadLine(), out inputValue);
                        }
                        db.DeleteEntry(entries[inputValue - 1]);
                        break;
                    case 5: // Update Habit Entry
                        chosenHabit = RequestHabit(db);
                        if (chosenHabit is null)
                        {
                            Console.WriteLine("There are currently no habits to choose from");
                            break;
                        }

                        entries = db.GetHabitEntries(chosenHabit);
                        if (entries.Count <= 0)
                        {
                            Console.WriteLine("Nothing has been logged to this habit yet.");
                            break;
                        }
                        unit = db.GetEntryMeasurement(entries[0]);

                        Console.WriteLine("Choose which entry to update:");
                        j = 1;
                        foreach (Entry e in entries)
                        {
                            Console.WriteLine($"{j}. {e} {unit}");
                            j++;
                        }

                        valid = int.TryParse(Console.ReadLine(), out inputValue);
                        while (!valid || inputValue < 1 || inputValue > entries.Count)
                        {
                            Console.WriteLine("Unknown command. Try again.");
                            valid = int.TryParse(Console.ReadLine(), out inputValue);
                        }
                        Entry oldEntry = entries[inputValue - 1];

                        // Pick new value(s)
                        // New qty
                        Console.Write("Choose a new quantity or press ENTER to keep the old one: ");
                        input = Console.ReadLine();
                        valid = float.TryParse(input, out qty);
                        while ((!valid && input != string.Empty) || qty < 0)
                        {
                            Console.WriteLine("Unknown command detected. Try again.");
                            input = Console.ReadLine();
                            valid = float.TryParse(input, out qty);
                        }
                        float? newQty;
                        if (input == string.Empty)
                            newQty = null;
                        else
                            newQty = qty;

                        // New date
                        Console.WriteLine("Choose a date to log, or just press ENTER to keep the old one" +
                                          "\n\tFormat as MM/DD/YYYY");
                        input = Console.ReadLine();
                        valid = DateTime.TryParse(input, out date);
                        while (!valid && input != string.Empty)
                        {
                            Console.WriteLine("Unknown command detected. Try again.");
                            input = Console.ReadLine();
                            valid = DateTime.TryParse(input, out date);
                        }
                        DateTime? newDate;
                        if (input == string.Empty)
                            newDate = null;
                        else
                            newDate = date;

                        if (newDate is null && newQty is null)
                        {
                            Console.WriteLine("You changed nothing about the entry.");
                            break;
                        }

                        db.UpdateEntry(oldEntry, newDate, newQty);
                        break;
                    case 6:
                        menu = false;
                        break;
                    default:
                        Console.WriteLine("Unknown command detected. Try again.");
                        break;
                }
            }
           
        }
        static void PrintMenu()
        {
            Console.WriteLine("To choose an action, enter its corresponding number:" +
                              "\n\t1. View Habit" +
                              "\n\t2. Create New Habit" +
                              "\n\t3. Log Existing Habit" +
                              "\n\t4. Delete Habit" +
                              "\n\t5. Update Habit" +
                              "\n\t6. Exit");
        }

        // Prints UI to select from current habits, then returns the chosen habit. 
        // Returns null if there are no habits to choose from
        static Habit? RequestHabit(DataAccess db)
        {
            Console.WriteLine("Choose from your existing habits:");
            List<Habit> habits = db.GetHabits();
            if (habits.Count <= 0)
                return null;

            int i = 1;
            foreach (Habit h in habits)
            {
                Console.WriteLine($"{i}. {h}");
                i++;
            }

            int.TryParse(Console.ReadLine(), out int choice);
            while (choice < 1 || choice > habits.Count)
            {
                Console.WriteLine("Unknown command detected. Try again.");
                int.TryParse(Console.ReadLine(), out choice);
            }

            return habits[choice - 1];
        }
    }
}
