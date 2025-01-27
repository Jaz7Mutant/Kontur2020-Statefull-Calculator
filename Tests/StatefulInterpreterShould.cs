﻿using FluentAssertions;
using NUnit.Framework;
using static CommandLineCalculator.Tests.TestConsole.Action;

namespace CommandLineCalculator.Tests
{
    public class StatefulInterpreterShould
    {
        public static TestCaseData[] RegularCases => new[]
        {
            new TestCaseData(
                new TestConsole(
                    (Read, "exit")
                )
            ).SetName("exit"),
            new TestCaseData(
                new TestConsole(
                    (Read, "add"),
                    (Read, "15"),
                    (Read, "60"),
                    (Write, "75"),
                    (Read, "exit")
                )
            ).SetName("add"),
            new TestCaseData(
                new TestConsole(
                    (Read, "median"),
                    (Read, "5"),
                    (Read, "17"),
                    (Read, "30"),
                    (Read, "29"),
                    (Read, "23"),
                    (Read, "20"),
                    (Write, "23"),
                    (Read, "exit")
                )
            ).SetName("odd median"),
            new TestCaseData(
                new TestConsole(
                    (Read, "median"),
                    (Read, "6"),
                    (Read, "17"),
                    (Read, "30"),
                    (Read, "29"),
                    (Read, "23"),
                    (Read, "20"),
                    (Read, "24"),
                    (Write, "23.5"),
                    (Read, "exit")
                )
            ).SetName("even median"),
            new TestCaseData(
                new TestConsole(
                    (Read, "rand"),
                    (Read, "1"),
                    (Write, "420"),
                    (Read, "rand"),
                    (Read, "2"),
                    (Write, "7058940"),
                    (Write, "528003995"),
                    (Read, "rand"),
                    (Read, "3"),
                    (Write, "760714561"),
                    (Write, "1359476136"),
                    (Write, "1636897319"),
                    (Read, "exit")
                )
            ).SetName("rand 3x"),
            new TestCaseData(
                new TestConsole(
                    (Read, "ramd"),
                    (Write, "Такой команды нет, используйте help для списка команд"),
                    (Read, "exit")
                )
            ).SetName("unknown command"),
            new TestCaseData(
                new TestConsole(
                    (Read, "help"),
                    (Write, "Укажите команду, для которой хотите посмотреть помощь"),
                    (Write, "Доступные команды: add, median, rand"),
                    (Write, "Чтобы выйти из режима помощи введите end"),
                    (Read, "end"),
                    (Read, "exit")
                )
            ).SetName("empty help"),
            new TestCaseData(
                new TestConsole(
                    (Read, "help"),
                    (Write, "Укажите команду, для которой хотите посмотреть помощь"),
                    (Write, "Доступные команды: add, median, rand"),
                    (Write, "Чтобы выйти из режима помощи введите end"),
                    (Read, "add"),
                    (Write, "Вычисляет сумму двух чисел"),
                    (Write, "Чтобы выйти из режима помощи введите end"),
                    (Read, "end"),
                    (Read, "exit")
                )
            ).SetName("add help"),
            new TestCaseData(
                new TestConsole(
                    (Read, "help"),
                    (Write, "Укажите команду, для которой хотите посмотреть помощь"),
                    (Write, "Доступные команды: add, median, rand"),
                    (Write, "Чтобы выйти из режима помощи введите end"),
                    (Read, "median"),
                    (Write, "Вычисляет медиану списка чисел"),
                    (Write, "Чтобы выйти из режима помощи введите end"),
                    (Read, "end"),
                    (Read, "exit")
                )
            ).SetName("median help"),
            new TestCaseData(
                new TestConsole(
                    (Read, "help"),
                    (Write, "Укажите команду, для которой хотите посмотреть помощь"),
                    (Write, "Доступные команды: add, median, rand"),
                    (Write, "Чтобы выйти из режима помощи введите end"),
                    (Read, "rand"),
                    (Write, "Генерирует список случайных чисел"),
                    (Write, "Чтобы выйти из режима помощи введите end"),
                    (Read, "end"),
                    (Read, "exit")
                )
            ).SetName("rand help"),
            new TestCaseData(
                new TestConsole(
                    (Read, "help"),
                    (Write, "Укажите команду, для которой хотите посмотреть помощь"),
                    (Write, "Доступные команды: add, median, rand"),
                    (Write, "Чтобы выйти из режима помощи введите end"),
                    (Read, "media"),
                    (Write, "Такой команды нет"),
                    (Write, "Доступные команды: add, median, rand"),
                    (Write, "Чтобы выйти из режима помощи введите end"),
                    (Read, "end"),
                    (Read, "exit")
                )
            ).SetName("unknown help"),
            new TestCaseData(
                new TestConsole(
                    (Read, "help"),
                    (Write, "Укажите команду, для которой хотите посмотреть помощь"),
                    (Write, "Доступные команды: add, median, rand"),
                    (Write, "Чтобы выйти из режима помощи введите end"),

                    (Read, "media"),
                    (Write, "Такой команды нет"),
                    (Write, "Доступные команды: add, median, rand"),
                    (Write, "Чтобы выйти из режима помощи введите end"),

                    (Read, "add"),
                    (Write, "Вычисляет сумму двух чисел"),
                    (Write, "Чтобы выйти из режима помощи введите end"),

                    (Read, "rand"),
                    (Write, "Генерирует список случайных чисел"),
                    (Write, "Чтобы выйти из режима помощи введите end"),

                    (Read, "median"),
                    (Write, "Вычисляет медиану списка чисел"),
                    (Write, "Чтобы выйти из режима помощи введите end"),

                    (Read, "end"),
                    (Read, "exit")
                )
            ).SetName("several commands help"),
        };

        public static TestCaseData[] InterruptionCases => new[]
        {
            new TestCaseData(
                new TestConsole(
                    (Read, "help"),
                    (Write, "Укажите команду, для которой хотите посмотреть помощь"),
                    (Write, "Доступные команды: add, median, rand"),
                    (Write, "Чтобы выйти из режима помощи введите end"),

                    (Read, "media"),
                    (Write, "Такой команды нет"),
                    (Write, "Доступные команды: add, median, rand"),
                    (Write, "Чтобы выйти из режима помощи введите end"),

                    (Read, "add"),
                    (Write, "Вычисляет сумму двух чисел"),
                    (Write, "Чтобы выйти из режима помощи введите end"),

                    (Read, "rand"),
                    (Write, "Генерирует список случайных чисел"),
                    (Write, "Чтобы выйти из режима помощи введите end"),

                    (Read, "median"),
                    (Write, "Вычисляет медиану списка чисел"),
                    (Write, "Чтобы выйти из режима помощи введите end"),

                    (Read, "end"),

                    (Read, "rand"),
                    (Read, "1"),
                    (Write, "420"),
                    (Read, "rand"),
                    (Read, "2"),
                    (Write, "7058940"),
                    (Write, "528003995"),
                    (Read, "rand"),
                    (Read, "3"),
                    (Write, "760714561"),
                    (Write, "1359476136"),
                    (Write, "1636897319"),

                    (Read, "median"),
                    (Read, "6"),
                    (Read, "17"),
                    (Read, "30"),
                    (Read, "29"),
                    (Read, "23"),
                    (Read, "20"),
                    (Read, "24"),
                    (Write, "23.5"),

                    (Read, "median"),
                    (Read, "5"),
                    (Read, "17"),
                    (Read, "30"),
                    (Read, "29"),
                    (Read, "23"),
                    (Read, "20"),
                    (Write, "23"),

                    (Read, "add"),
                    (Read, "15"),
                    (Read, "60"),
                    (Write, "75"),
                    (Read, "exit")
                ),
                new[] {0, 3, 5, 8, 11, 12, 13, 
                    14, 16, 19, 21, 24, 29, 31, 32, 35, 36, 39, 43, 44, 50, 51

                }
            ).SetName("full random"),


            new TestCaseData(
                new TestConsole(
                    (Read, "median"),
                    (Read, "6"),
                    (Read, "17"),
                    (Read, "30"),
                    (Read, "29"),
                    (Read, "23"),
                    (Read, "20"),
                    (Read, "24"),
                    (Write, "23.5"),

                    (Read, "rand"),
                    (Read, "1"),
                    (Write, "420"),

                    (Read, "help"),
                    (Write, "Укажите команду, для которой хотите посмотреть помощь"),
                    (Write, "Доступные команды: add, median, rand"),
                    (Write, "Чтобы выйти из режима помощи введите end"),

                    (Read, ""),
                    (Write, "Такой команды нет"),
                    (Write, "Доступные команды: add, median, rand"),
                    (Write, "Чтобы выйти из режима помощи введите end"),

                    (Read, "add"),
                    (Write, "Вычисляет сумму двух чисел"),
                    (Write, "Чтобы выйти из режима помощи введите end"),

                    (Read, "rand"),
                    (Write, "Генерирует список случайных чисел"),
                    (Write, "Чтобы выйти из режима помощи введите end"),

                    (Read, "median"),
                    (Write, "Вычисляет медиану списка чисел"),
                    (Write, "Чтобы выйти из режима помощи введите end"),

                    (Read, "end"),
                    (Read, "rand"),
                    (Read, "2"),
                    (Write, "7058940"),
                    (Write, "528003995"),
                    (Read, "ramd"),
                    (Write, "Такой команды нет, используйте help для списка команд"),
                    (Read, "rand"),
                    (Read, "3"),
                    (Write, "760714561"),
                    (Write, "1359476136"),
                    (Write, "1636897319"),
                    (Read, "exit")
                ),
                new[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 10, 11, 12, 15, 16, 19, 23, 25}
                ).SetName("long random"),

            new TestCaseData(
                new TestConsole(
                    (Read, "help"),
                    (Write, "Укажите команду, для которой хотите посмотреть помощь"),
                    (Write, "Доступные команды: add, median, rand"),
                    (Write, "Чтобы выйти из режима помощи введите end"),

                    (Read, "media"),
                    (Write, "Такой команды нет"),
                    (Write, "Доступные команды: add, median, rand"),
                    (Write, "Чтобы выйти из режима помощи введите end"),

                    (Read, "add"),
                    (Write, "Вычисляет сумму двух чисел"),
                    (Write, "Чтобы выйти из режима помощи введите end"),

                    (Read, "rand"),
                    (Write, "Генерирует список случайных чисел"),
                    (Write, "Чтобы выйти из режима помощи введите end"),

                    (Read, "median"),
                    (Write, "Вычисляет медиану списка чисел"),
                    (Write, "Чтобы выйти из режима помощи введите end"),

                    (Read, "end"),

                    (Read, "rand"),
                    (Read, "1"),
                    (Write, "420"),
                    (Read, "rand"),
                    (Read, "2"),
                    (Write, "7058940"),
                    (Write, "528003995"),
                    (Read, "rand"),
                    (Read, "3"),
                    (Write, "760714561"),
                    (Write, "1359476136"),
                    (Write, "1636897319"),

                    (Read, "median"),
                    (Read, "6"),
                    (Read, "17"),
                    (Read, "30"),
                    (Read, "29"),
                    (Read, "23"),
                    (Read, "20"),
                    (Read, "24"),
                    (Write, "23.5"),

                    (Read, "median"),
                    (Read, "5"),
                    (Read, "17"),
                    (Read, "30"),
                    (Read, "29"),
                    (Read, "23"),
                    (Read, "20"),
                    (Write, "23"),

                    (Read, "add"),
                    (Read, "15"),
                    (Read, "60"),
                    (Write, "75"),

                    (Read, "input"),
                    (Write, "Такой команды нет, используйте help для списка команд"),

                    (Read, "exit")
                ),
                new[] {
                    0,3,5,8,11,
                    12,13,14,16,19,21,24,29,31,32,35,
                    36,39,43,44,50,53,55,59,60,62,63,64,69,70,73,74,75,85,87

                }
            ).SetName("full random 2"),


            new TestCaseData(
                new TestConsole(
                    (Read, "add"),
                    (Read, "15"),
                    (Read, "60"),
                    (Write, "75"),
                    (Read, "exit")
                ),
                new[] {2}
            ).SetName("add"),
            new TestCaseData(
                new TestConsole(
                    (Read, "median"),
                    (Read, "3"),
                    (Read, "60"),
                    (Read, "50"),
                    (Read, "41"),
                    (Write, "50"),
                    (Read, "exit")
                ),
                new[] {1, 4}
            ).SetName("median"),
            new TestCaseData(
                new TestConsole(
                    (Read, "rand"),
                    (Read, "2"),
                    (Write, "420"),
                    (Write, "7058940"),
                    (Read, "exit")
                ),
                new[] {1}
            ).SetName("rand")
        };

        [Test]
        [TestCaseSource(nameof(RegularCases))]
        public void Run_As_Expected(TestConsole console)
        {
            var storage = new MemoryStorage();
            var interpreter = new StatefulInterpreter();
            interpreter.Run(console, storage);
            console.AtEnd.Should().BeTrue();
        }

        [Test]
        [TestCaseSource(nameof(InterruptionCases))]
        public void Run_With_Interruptions(
            TestConsole console,
            int[] failureSchedule)
        {
            var storage = new MemoryStorage();
            var brokenConsole = new BrokenConsole(console, failureSchedule);
            for (var i = 0; i < failureSchedule.Length; i++)
            {
                var exception = Assert.Throws<TestException>(() =>
                {
                    var interpreter = new StatefulInterpreter();
                    interpreter.Run(brokenConsole, storage);
                });
                exception.Type.Should().Be(TestException.ExceptionType.InducedFailure);
            }

            var finalInterpreter = new StatefulInterpreter();
            finalInterpreter.Run(brokenConsole, storage);

            console.AtEnd.Should().BeTrue();
        }

        [Test]
        public void Clear_Storage_On_Exit()
        {
            var console = new TestConsole(
                (Read, "rand"),
                (Read, "1"),
                (Write, "420"),
                (Read, "exit")
            );

            var interpreter = new StatefulInterpreter();
            var storage = new MemoryStorage();
            interpreter.Run(console, storage);

            var secondConsole = new TestConsole(
                (Read, "rand"),
                (Read, "1"),
                (Write, "420"),
                (Read, "exit")
            );

            interpreter = new StatefulInterpreter();
            interpreter.Run(secondConsole, storage);
            secondConsole.AtEnd.Should().BeTrue();
        }
    }
}