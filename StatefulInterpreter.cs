using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CommandLineCalculator
{
    public sealed class StatefulInterpreter : Interpreter
    {
        private static CultureInfo Culture => CultureInfo.InvariantCulture;

        public override void Run(UserConsole userConsole, Storage storage)
        {
            var stateKeeper = new StateKeeper(storage, Culture);
            while (true)
            {
                stateKeeper.CurrentCommand ??= userConsole.ReadLine().Trim();
                stateKeeper.SaveState();
                switch (stateKeeper.CurrentCommand)
                {
                    case "exit":
                        stateKeeper.Dispose();
                        return;
                    case "add":
                        Add(userConsole, stateKeeper);
                        break;
                    case "median":
                        Median(userConsole, stateKeeper);
                        break;
                    case "help":
                        Help(userConsole, stateKeeper);
                        break;
                    case "rand":
                        Random(userConsole, stateKeeper);
                        break;
                    default:
                        userConsole.WriteLine("Такой команды нет, используйте help для списка команд");
                        break;
                }

                stateKeeper.Clear();
                stateKeeper.SaveState();
            }
        }

        private void Add(UserConsole console, StateKeeper stateKeeper)
        {
            stateKeeper.ArgsCount = 2;
            var args = stateKeeper.GetArgs();
            var a = args[0] == null ? int.Parse(console.ReadLine().Trim(), Culture) : int.Parse(args[0], Culture);
            stateKeeper.Args[0] = a.ToString(Culture);
            stateKeeper.SaveState();
            var b = args[1] == null ? int.Parse(console.ReadLine().Trim(), Culture) : int.Parse(args[1], Culture);
            stateKeeper.Args[1] = b.ToString(Culture);
            stateKeeper.SaveState();
            console.WriteLine((a + b).ToString(Culture));
        }

        private void Median(UserConsole console, StateKeeper stateKeeper)
        {
            stateKeeper.ArgsCount = stateKeeper.ArgsCount == -1
                ? int.Parse(console.ReadLine().Trim(), Culture)
                : stateKeeper.ArgsCount;
            stateKeeper.SaveState();
            var args = stateKeeper.GetArgs();
            for (var i = 0; i < stateKeeper.ArgsCount; i++)
            {
                args[i] = args[i] ?? console.ReadLine().Trim();
                stateKeeper.SaveState();
            }

            var result = CalculateMedian(args.Select(x => int.Parse(x, Culture)).ToList());
            console.WriteLine(result.ToString(Culture));
        }

        private double CalculateMedian(List<int> numbers)
        {
            numbers.Sort();
            var count = numbers.Count;
            if (count == 0)
                return 0;

            if (count % 2 == 1)
                return numbers[count / 2];

            return (numbers[count / 2 - 1] + numbers[count / 2]) / 2.0;
        }

        private void Help(UserConsole console, StateKeeper stateKeeper)
        {
            const string exitMessage = "Чтобы выйти из режима помощи введите end";
            const string commands = "Доступные команды: add, median, rand";

            stateKeeper.ArgsCount = 1;
            if (stateKeeper.ResultsCount == 0)
            {
                var introText = new List<string>
                {
                    "Укажите команду, для которой хотите посмотреть помощь", commands, exitMessage
                };
                PrintHelp(console, stateKeeper, introText);
                stateKeeper.ResultsCount = 1;
                stateKeeper.ShowedResults = 0;
                stateKeeper.SaveState();
            }

            while (true)
            {
                var command = stateKeeper.GetArgs()[0] == null || stateKeeper.GetArgs()[0].Equals("")
                    ? console.ReadLine().Trim()
                    : stateKeeper.GetArgs()[0];
                stateKeeper.Args[0] = command;
                stateKeeper.SaveState();
                switch (command)
                {
                    case "end":
                        return;
                    case "add":
                        var addText = new List<string> {"Вычисляет сумму двух чисел", exitMessage};
                        PrintHelp(console, stateKeeper, addText);
                        break;
                    case "median":
                        var medianText = new List<string> {"Вычисляет медиану списка чисел", exitMessage};
                        PrintHelp(console, stateKeeper, medianText);
                        break;
                    case "rand":
                        var randText = new List<string> {"Генерирует список случайных чисел", exitMessage};
                        PrintHelp(console, stateKeeper, randText);
                        break;
                    default:
                        var defaultText = new List<string> {"Такой команды нет", commands, exitMessage};
                        PrintHelp(console, stateKeeper, defaultText);
                        break;
                }

                stateKeeper.Args[0] = null;
                stateKeeper.ShowedResults = 0;
            }
        }

        private void PrintHelp(UserConsole console, StateKeeper stateKeeper, List<string> helpText)
        {
            for (var i = stateKeeper.ShowedResults; i < helpText.Count; i++)
            {
                console.WriteLine(helpText[i]);
                stateKeeper.ShowedResults = i + 1;
                stateKeeper.SaveState();
            }
        }

        private void Random(UserConsole console, StateKeeper stateKeeper)
        {
            const int a = 16807;
            const int m = 2147483647;
            stateKeeper.ResultsCount = stateKeeper.ResultsCount == 0
                ? int.Parse(console.ReadLine().Trim(), Culture)
                : stateKeeper.ResultsCount;
            stateKeeper.SaveState();
            for (var i = stateKeeper.ShowedResults; i < stateKeeper.ResultsCount; i++)
            {
                console.WriteLine(stateKeeper.x.ToString(Culture));
                stateKeeper.x = a * stateKeeper.x % m;
                stateKeeper.ShowedResults = i + 1;
                stateKeeper.SaveState();
            }
        }
    }

    public class StateKeeper
    {
        /* x
         * command
         * showed results
         * args count
         * results count
         * args
         */

        private readonly Storage storage;
        private readonly CultureInfo culture;
        public long x;
        public string CurrentCommand;
        public int ShowedResults;
        public int ArgsCount = -1;
        public int ResultsCount;
        public string[] Args;

        public StateKeeper(Storage storage, CultureInfo culture)
        {
            this.storage = storage;
            this.culture = culture;
            LoadState();
        }

        public string[] GetArgs() => Args ??= new string[ArgsCount];

        public void LoadState()
        {
            var dump = Encoding.UTF8.GetString(storage.Read());
            if (dump.Equals(""))
            {
                x = 420L;
                return;
            }

            var lines = dump.Split(new[] {"\r\n"}, StringSplitOptions.None);
            x = long.Parse(lines[0], culture);
            CurrentCommand = lines[1].Equals("") ? null : lines[1];
            ShowedResults = int.Parse(lines[2], culture);
            ArgsCount = lines[3].Equals("") ? -1 : int.Parse(lines[3], culture);
            ResultsCount = int.Parse(lines[4], culture);
            if (ArgsCount > 0)
            {
                Args = new string[ArgsCount];
                for (var i = 0; i < lines.Length - 6; i++)
                {
                    Args[i] = lines[i + 5].Equals("") ? null : lines[i + 5];
                }
            }
        }

        public void SaveState()
        {
            var buffer = new StringBuilder();
            buffer.AppendLine(x.ToString(culture));
            buffer.AppendLine(CurrentCommand);
            buffer.AppendLine(ShowedResults.ToString(culture));
            buffer.AppendLine(ArgsCount.ToString(culture));
            buffer.AppendLine(ResultsCount.ToString(culture));
            if (Args != null)
            {
                foreach (var arg in GetArgs())
                {
                    buffer.AppendLine(arg);
                }
            }

            storage.Write(Encoding.UTF8.GetBytes(buffer.ToString()));
        }

        public void Clear()
        {
            CurrentCommand = null;
            ShowedResults = 0;
            ArgsCount = -1;
            Args = null;
            ResultsCount = 0;
        }

        public void Dispose()
        {
            Clear();
            storage.Write(new byte[0]);
        }
    }
}