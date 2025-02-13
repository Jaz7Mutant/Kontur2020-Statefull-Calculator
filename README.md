# Калькулятор 

##### Тестовое задание на стажировку в СКБ Контур 2020г.

Андрей разрабатывает приложение для инженерных расчетов. Пока функционал программы невелик. Андрей хочет, чтобы пользователям было удобно работать с приложением. И немудрено, ведь ни один разработчик не хочет, чтобы его продукт отвергали из-за того, что неудобно, пусть даже в нем больше крутых фич.

Самой большой проблемой приложения Андрей считает перезапуск, так как он вынуждает пользователя программы вновь вводить все данные, а этот процесс такой утомительный! Масла в огонь подливает тот факт, что программа может завершиться не только по воле пользователя. Например, никто не застрахован от случайного выдергивания вилки из розетки. Такое положение дел не дает покоя Андрею, поэтому он просит тебя помочь. Сделай так, чтобы после перезапуска приложение запускалось там, где оно было в момент завершения.

Тебе дан интерпретатор команд вводимых с консоли `StatelessInterpreter`, который умеет обрабатывать команды. Особенности их поведения ты можешь узнать из кода. В решении реализована неполная обработка ошибочного ввода, например, можно ввести строку, тогда как программа ждет число, это приведет к падению программы — игнорируй это, считай, что пользователь всегда вводит числа, когда программа этого ждет.

[Скачай проект CommandLineCalculator.](https://ulearn.me/Exercise/StudentZip?courseId=backend-internship-2020&slideId=c6a70e4d-9673-4d02-a50c-fe667a5bd83c) Модифицируй программу так, чтобы при перезапуске она возобновляла работу с того момента, в котором завершилась. Напиши наследника класса Interpreter — `StatefulInterpreter`. Для сохранения состояния программы используй экземпляр класса `Storage`, считай, что он надежно и безопасно сохраняет данные на диск. Ввод осуществляется построчно, поэтому если пользователь напечатал данные в консоль и не нажал Enter, эти данные считаются невведенными и сохраняться не должны.

Более формально требования к `StatefulInterpreter` такие:

- если параллельно запустить две версии программы одну с `StatelessInterpreter`, другую с `StatefulInterpreter`, то при одинаковом вводе программы должны вести себя одинаково;
- если в произвольный момент программу с `StatefulInterpreter` завершить, и снова запустить, то первое условие все равно должно выполняться, как если бы перезапуска не было.

Еще более формальные требования ты можешь узнать из тестов к заданию ;)

`StatelessInterpreter` написан в простом императивном стиле, поведение команд легко усматривается из кода. Постарайся в своем решении сохранить простоту реализации команд, так как Андрей дорожит чистотой кода, и в будущем придется дописать еще не один десяток новых команд. В идеале код `StatefulInterpreter` должен практически совпадать с кодом `StatelessInterpreter`.

#### Особенности проверки

Для того, чтобы проверить корректность твоего решения нам нужно применить специальные практики тестирования и наложить на решение следующие ограничения.

В решении этого задания не используй:

- статические поля;
- статические свойства с set'ером (get only статические свойства использовать можно);
- блоки `try-catch`, `try-finally` и `try-catch-finally`.

Блок `using` можно использовать только для типов стандартной библиотеки т.е. можно писать так

```
using (var stream = new System.IO.MemoryStream())
{
    ...
}
```

но нельзя так

```
using (var disposable = new Calculator.MyDisposableType())
{
    ...
}
```

Эти ограничения нужны для того, чтобы:

- программа сохраняла данные в `Storage`, а не в статические поля. При реальном использовании программы её будут перезапускать, следовательно значения, сохраненные в статических полях, будут теряться;
- мы могли проверить поведение вашей программы при закрытии. Когда мы захотим остановить вашу программу мы бросим исключение и поймаем его в тесте. Использование блоков `try-catch` может исказить результаты тестов.

Тестирующая система не может определить соответствует ли твое решение эти ограничениям. Мы проверим их соблюдение только на втором этапе проверки. Поэтому перед сдачей задачи проверь свое решение на соответствие этим ограничениям.

В ходе тестирования мы будем подавать на вход твоему решения серии команд.

- максимальное количество команд в рамках одного теста - 1000;
- общее количество команд во всех тестах не превосходит 20000;
- максимальное количество строк ввода/вывода в рамках одного теста не превышает 25000;
- общее количество строк ввода/вывода во всех тестах не превосходит 450000;
- максимальное количество чисел в команде `median` - 100;
- максимальное количество чисел в команде `rand` - 100;
- максимальное количество запросов в режиме `help` - 5;
- перезапуск в среднем происходит каждые 3,5 операции ввода/вывода.