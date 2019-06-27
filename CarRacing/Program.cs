using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;

namespace CarRacing
{
    class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetConsoleWindow();                                          // Функция получения дискриптора консоли
        static Graphics consoleGraphics = Graphics.FromHwnd(GetConsoleWindow());          // Создаём графический контейнер для рисования в консоли
        delegate void MoveCar();                                                          // делегат перемещения
        delegate void PosCar(int Position);                                               // установка позиции
        delegate void PosOut();                                                           // Вывод позиции
        delegate void FinishCar(object obj);                                              // Машина приехала к финишу
        abstract class Car
        {
            public virtual event FinishCar Finish;                                        // Автомобиль приехал к финишу
            public string Name;
            int pos;
            public static Random rnd = new Random();

            public void MoveTo(int Position)                                               // Установка позиции
            {
                this.Position = Position;
            }
            public int Position
            {
                get { return pos; }
                set
                {
                    pos = value;
                    Console.WriteLine(this + " на позиции " + pos);
                }
            }
            public override string ToString()
            {
                return Name;
            }
            public abstract void Move();                                                   // Движение автомобиля
            public abstract void Out();                                                    // Вывод позиции
        }
        class Sport : Car                                                                  //Спортивная
        {
            public override event FinishCar Finish;
            Image ImageSport;
            public Sport()
            {
                try
                {
                    ImageSport = Image.FromFile("Спортивный.png");
                }
                catch(Exception ex)
                {
                    Game.IsError = true;
                    Console.WriteLine(ex.Message);
                }
                Name = "Спортивный автомобиль";
            }
            public override void Out()
            {
                consoleGraphics.DrawImage(ImageSport, Position * 5, 150, 30, 30);
            }
            public override void Move() 
            {
                Position += rnd.Next(50, 120) / 50;                                        // Алгоритм изменения скорости (можно усложнить)
                if (Position >= 100)
                    if (Finish != null) Finish(this);
            }
        }
        class Bus : Car                                                                     //Автобус
        {
            public override event FinishCar Finish;
            Image ImageBus;
            public Bus()
            {
                try
                {
                    ImageBus = Image.FromFile("Автобус.png");
                }
                catch (Exception ex)
                {
                    Game.IsError = true;
                    Console.WriteLine(ex.Message);
                }
        Name = "Автобус";
            }
            public override void Out()
            {
                consoleGraphics.DrawImage(ImageBus, Position * 5, 200, 30, 30);
            }
            public override void Move()
            {
                Position += rnd.Next(40, 80) / 40;
                if (Position >= 100)
                    if (Finish != null) Finish(this); 
            }
        }
        class FreightCar : Car                                                             //Грузовик
        {
            public override event FinishCar Finish;
            Image ImageFreightCar;
            public FreightCar()
            {
                try
                {
                    ImageFreightCar = Image.FromFile("Грузовик.png");
                }
                catch (Exception ex)
                {
                    Game.IsError = true;
                    Console.WriteLine(ex.Message);
                }
                Name = "Грузовик";
            }
            public override void Out()
            {
                consoleGraphics.DrawImage(ImageFreightCar, Position * 5, 100, 30, 30);
            }
            public override void Move()
            {
                Position += rnd.Next(60, 90) / 60;
                if (Position >= 100)
                    if (Finish != null) Finish(this);
            }
        }
        class PassengerCar : Car                                                           //Легковая
        {
            public override event FinishCar Finish;
            Image ImagePassengerCar;
            public PassengerCar()
            {
                try
                {
                    ImagePassengerCar = Image.FromFile("Легковой.png");
                }
                catch(Exception ex)
                {
                    Game.IsError = true;
                    Console.WriteLine(ex.Message);
                }
        Name = "Легковой автомобиль";
            }
            public override void Out()
            {
                consoleGraphics.DrawImage(ImagePassengerCar, Position * 5, 250, 30, 30);
            }
            public override void Move()
            {
                Position += rnd.Next(50, 110) / 50;
                if (Position >= 100)
                    if (Finish != null) Finish(this);
            }
        }
        class Game
        {
            bool isGame;
            public MoveCar Move;
            public PosCar MoveTo;
            public PosOut Out;
            public object Winner;
            public static bool IsError = false;
            Pen whitePen;
            Font font10, font30;
            int start;
            public Game()
            {
                font10 = new Font("verdana", 10);
                font30 = new Font("verdana", 30);
                whitePen = new Pen(Color.White);
                isGame = true;
                start = 5;
            }
            public void Run()
            {
                if (!IsError)
                {
                    MoveTo(0);                                                         // перемещаем всех на старт
                    while (start != 0)                                                 // Обратный отсчет
                    {
                        consoleGraphics.Clear(Color.Black);
                        consoleGraphics.DrawString("Старт", font10, Brushes.White, 40, 80);
                        consoleGraphics.DrawLine(whitePen, new Point(30, 0), new Point(30, 300));
                        consoleGraphics.DrawString(start.ToString(), font30, Brushes.White, 150, 160);
                        Out();
                        start--;
                        System.Threading.Thread.Sleep(1000);
                    }
                }
                else
                {
                    Console.WriteLine("Файл(ы) Не удалось загрузить, функциональность программы будет ограничена");
                    MoveTo(0);                                                         // перемещаем всех на старт
                    while (start != 0)
                    {
                        Console.SetCursorPosition(0, 10);
                        Console.WriteLine("До старта осталось: " + start.ToString());
                        start--;
                        System.Threading.Thread.Sleep(1000);
                    }
                }
                while (isGame)                                                        // Толкаем всех вперед пока кто-нибудь не приедет к финишу
                {
                    Console.Clear();
                    if (!IsError)
                    {
                        consoleGraphics.Clear(Color.Black);
                        consoleGraphics.DrawString("Финиш", font10, Brushes.White, 540, 80);
                        consoleGraphics.DrawLine(whitePen, new Point(530, 0), new Point(530, 300));
                        Move();
                        Out();
                    }
                    else
                    {
                        Move();
                    }
                    System.Threading.Thread.Sleep(100);
                }
            }
            public void OnFinish(object Winner)                                      // кто-то приехал к финишу
            {
                isGame = false;
                this.Winner = Winner;
            }
            public void JoinGame(params Car[] cars)                                  // Подписываемся на игру
            {
                foreach(var car in cars)
                {
                    Move += car.Move;
                    MoveTo += car.MoveTo;
                    car.Finish += OnFinish;
                    Out += car.Out;
                }
            }
        }
        static void Main(string[] args)
        {
            // Создаем игру
            Game game = new Game();

            // Создаем машинки
            Car car1 = new Bus();
            Car car2 = new Sport();
            Car car3 = new FreightCar();
            Car car4 = new PassengerCar();

            // Подписываемся на участие в игре
            game.JoinGame(car1, car2, car3, car4);

            // Запуск игры
            game.Run();

            // Печатаем победителя
            Console.WriteLine("\nГонку выиграл: " + game.Winner);
            Console.ReadLine();
        }     
    }
}
