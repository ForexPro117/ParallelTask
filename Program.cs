using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp6;

enum Condition
{
    good,
    notGood,
    bad,
    veryBad
}

public static class Program
{
    static void GetProcedure(Sick[] sicks)
    {
        Parallel.For(0, sicks.Length, (i) =>
        {
            Console.WriteLine($"{sicks[i].Name} ожидает начало процедуры TASK:{Task.CurrentId}, THREAD:{Thread.CurrentThread.ManagedThreadId}");
            try
            {
                Monitor.Enter(procedure);
                Thread.Sleep(1);
                procedure.takeHeal(sicks[i]);
                Task.Run(() => FileWriter($"*|*{sicks[i].Name} принимает процедуры TASK:{Task.CurrentId}, THREAD:{Thread.CurrentThread.ManagedThreadId}*|*"));
                Console.WriteLine($"{sicks[i].Name} пошел отдыхать  TASK:{Task.CurrentId}, THREAD:{Thread.CurrentThread.ManagedThreadId}");

            }
            catch (Exception)
            {
                Console.WriteLine($"{sicks[i].Name} не смог принят процедуру  TASK:{Task.CurrentId}, THREAD:{Thread.CurrentThread.ManagedThreadId}*************************");
            }
            finally
            {
                Monitor.Exit(procedure);

            }

        });
    }
    static void Check(Visitor[] visitors, Sick[] sicks)
    {
        Parallel.For(0, visitors.Length, (i) =>
        {
            Console.WriteLine($"{visitors[i].Name} Хочет спросить у доктора о возможности посетить {sicks[i].Name} TASK:{Task.CurrentId}, THREAD:{Thread.CurrentThread.ManagedThreadId}");
            if (Monitor.TryEnter(doctor, 15))
            {
                try
                {
                    if (doctor.CanVisitSick(sicks[i]))
                    {
                        Console.WriteLine($"//////////////******{visitors[i].Name}*******////////////// Сободных мест: " + doctor.FreePlace);
                        doctor.TakePlace();
                        Task.Run(() => Visit(sicks[i], visitors[i]));
                        Task.Run(() => FileWriter($"*{visitors[i].Name} может навести пациента TASK:{Task.CurrentId}, THREAD:{Thread.CurrentThread.ManagedThreadId}*"));
                    }
                    else
                    {
                        Console.WriteLine($"//////////////******{visitors[i].Name}*******////////////// Сободных мест: " + doctor.FreePlace);
                        Task.Run(() => FileWriter($"***{visitors[i].Name} не может навести пациента и уходит домой TASK:{Task.CurrentId}, THREAD:{Thread.CurrentThread.ManagedThreadId}***"));
                    }
                    Thread.Sleep(1);
                }
                finally
                {
                    Monitor.Exit(doctor);
                }
            }
            else
            {
                Task.Run(() => FileWriter($"**XXX**{visitors[i].Name} устал ждать и ушел домой TASK:{Task.CurrentId}, THREAD:{Thread.CurrentThread.ManagedThreadId}**XXX**"));
                Console.WriteLine($"**XXX**{visitors[i].Name} устал ждать и ушел домой TASK:{Task.CurrentId}, THREAD:{Thread.CurrentThread.ManagedThreadId}**XXX**");
            }
        });
    }
    static void Visit(Sick sick, Visitor visitor)
    {
        Monitor.Enter(sick);
        Console.WriteLine(sick.TalkWith(visitor));
        Task.Run(() => FileWriter($"****{visitor.Name} поговорил с {sick.Name} TASK:{Task.CurrentId}, THREAD:{Thread.CurrentThread.ManagedThreadId}****"));
        Thread.Sleep(50);
        doctor.ReleasePlace();
        Monitor.Exit(sick);

    }
    static void FileWriter(string text)
    {
        try
        {
            Monitor.Enter(sw);
            Console.WriteLine($"Запись логов в файл TASK:{Task.CurrentId}, THREAD:{Thread.CurrentThread.ManagedThreadId}");
            sw.WriteLine(text);
            Console.WriteLine($"Конец записи в файл TASK:{Task.CurrentId}, THREAD:{Thread.CurrentThread.ManagedThreadId}");
        }
        catch (Exception)
        {
            Console.WriteLine($"ОШИБКА ПРИ ЗАПИСИ  TASK:{Task.CurrentId}, THREAD:{Thread.CurrentThread.ManagedThreadId}");

        }
        finally
        {
            Monitor.Exit(sw);
        }
    }

    static Doctor doctor = new Doctor("Доктор", 3);
    static StreamWriter sw = StreamWriter.Null;
    static Procedures procedure = new Procedures();

    public static void Main()
    {
        int Number = 10;



        Sick[] sicks = new Sick[Number];
        Visitor[] visitors = new Visitor[Number];

        for (int i = 0; i < Number; i++)
        {
            sicks[i] = new Sick("Пациент " + (i + 1), i % 4);
        }
        for (int i = 0; i < Number; i++)
        {
            visitors[i] = new Visitor("Посетитель " + (i + 1));
        }
        try
        {
            sw = new StreamWriter("text.txt");
            Parallel.Invoke(
            () => GetProcedure(sicks),
            () => Check(visitors, sicks));
        }
        catch (Exception)
        {


        }
        finally
        {
            Thread.Sleep(2000);
            sw.Close();
            Console.WriteLine("*****************************************");
        }
    }
}
