# HabitTracker
Stores logs of habits using an SQLite database through ADO.NET. 
The database consists of two tables:
  1. Habits
  3. Entries
The Habits table contains the "habit name" and "unit of measurement"
The Entries table contains the "date of log," "quantity recorded," and the corresponding habit-id
UI control flow is handled by a basic switch-case loop in the main method, and some helpers defined in the same Program.cs file
