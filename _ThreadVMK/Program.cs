using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _ThreadVMK
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\nПолучение статистики о текущем потоке");
            Prog1();
            Console.WriteLine("\nMy thread");
            Prog2();
            Console.WriteLine("\nДелегат ParametrizedThreadStart");
            Prog3();
            Console.WriteLine("\nСинхронизация потоков. Класс AutoResetEvent");
            Prog4();
            Console.WriteLine("\nОператор Lock");
            Prog5();
            Console.WriteLine("\nЧасы");
            Prog6();
            Console.ReadKey();
        }

        public class MyThread
        {
            public void ThreadNumbers()
            {
                // Информация о потоке
                Console.WriteLine("{0} поток использует метод ThreadNumbers", Thread.CurrentThread.Name);
                // Выводим числа
                Console.Write("Числа: ");
                for (int i = 0; i < 10; i++)
                {
                    Console.Write(i + ", ");
                    // Используем задержку
                    Thread.Sleep(3000);
                }
                Console.WriteLine();
            }
        }

        public class Params
        {
            public int a, b;
            public Params(int a, int b)
            {
                this.a = a;
                this.b = b;
            }
        }

        static void Add(object obj)
        {
            if (obj is Params)
            {
                Console.WriteLine("ID потока метода Add(): " + Thread.CurrentThread.ManagedThreadId);
                Params pr = (Params)obj;
                Console.WriteLine("{0} + {1} = {2}", pr.a, pr.b, pr.a + pr.b);
                waitHandle.Set();
            }
        }

        static void Prog1()
        {
            Console.Title = "Информация о потоке!!";
            Thread thread = Thread.CurrentThread;
            Console.WriteLine(@"        ID: {0}                
                Запущен поток: {1}
                Приоритет потока: {2}
                Состояние потока: {3}", Thread.CurrentContext.ContextID, thread.IsAlive,
                thread.Priority, thread.ThreadState);
        }

        static void Prog2()
        {
            Console.Write("Сколько использовать потоков (1 или 2)?");
            string number = Console.ReadLine();
            Thread mythread = Thread.CurrentThread;
            mythread.Name = "Первичный";
            // Выводим информацию о потоке
            Console.WriteLine("--> {0} главный поток", Thread.CurrentThread.Name);
            MyThread mt = new MyThread();
            switch (number)
            {
                case "1":
                    mt.ThreadNumbers();
                    break;
                case "2":
                    // Создаем поток
                    Thread backgroundThread = new Thread(new ThreadStart(mt.ThreadNumbers));
                    backgroundThread.Name = "Вторичный";
                    backgroundThread.Start();
                    break;
                default:
                    Console.WriteLine("использую 1 поток");
                    goto case "1";
            }
            MessageBox.Show("Сообщение ...", "Работа в другом потоке");
        }

        static void Prog3()
        {
            Console.WriteLine("Главный поток. ID: " + Thread.CurrentThread.ManagedThreadId);
            Params pm = new Params(10, 10);
            Thread t = new Thread(new ParameterizedThreadStart(Add));
            t.Start(pm);

            // Задержка
            Thread.Sleep(5);
        }

        private static AutoResetEvent waitHandle = new AutoResetEvent(false);

        static void Prog4()
        {
            Console.WriteLine("Главный поток. ID: " + Thread.CurrentThread.ManagedThreadId);
            Params pm = new Params(10, 10);
            Thread t = new Thread(new ParameterizedThreadStart(Add));
            t.Start(pm);
            // Задержка
            // Thread.Sleep(5);
            // Ждем уведомления
            waitHandle.WaitOne();
            Console.WriteLine("Все потоки завершились");
        }

        static int x = 0;

        static void Prog5()
        {
            for (int i = 0; i < 5; i++)
            {
                Thread myThread = new Thread(Count);
                myThread.Name = "Поток " + i.ToString();
                myThread.Start();
            }
        }

        public static void Count()
        {
            x = 1;
            for (int i = 1; i < 9; i++)
            {
                Console.WriteLine("{0}: {1}", Thread.CurrentThread.Name, x);
                x++;
                Thread.Sleep(100);
            }
        }
        public class TickTock
        {
            private object lockOn = new object();
            public void Tick(bool running)
            {
                lock (lockOn)
                {
                    if (!running)
                    {
                        // Остановить часы
                        Monitor.Pulse(lockOn);
                        return;
                    }
                    Console.Write("Тик ");
                    Monitor.Pulse(lockOn); Monitor.Wait(lockOn);
                }
            }
            public void Tock(bool running)
            {
                lock (lockOn)
                {
                    if (!running)
                    {
                        Monitor.Pulse(lockOn);
                        return;
                    }
                    Console.WriteLine("так");
                    Monitor.Pulse(lockOn);
                    Monitor.Wait(lockOn);
                }
            }
        }
        class MyThread2
        {
            public Thread thrd;
            TickTock ttobj;

            // Новый поток
            public MyThread2(string name, TickTock tt)
            {
                thrd = new Thread(this.Run);
                ttobj = tt;
                thrd.Name = name;
                thrd.Start();
            }

            void Run()
            {
                if (thrd.Name == "Tick")
                {
                    for (int i = 0; i < 5; i++)
                        ttobj.Tick(true);
                    ttobj.Tick(false);
                }
                else
                {
                    for (int i = 0; i < 5; i++)
                        ttobj.Tock(true);
                    ttobj.Tock(false);
                }
            }
        }

        static void Prog6()
        {
            TickTock tt = new TickTock();
            MyThread2 mt1 = new MyThread2("Tick", tt);
            MyThread2 mt2 = new MyThread2("Tock", tt);
            mt1.thrd.Join();
            mt2.thrd.Join();
            Console.WriteLine("Часы остановлены");
        }



    }
}
