using System;

namespace ConsoleApp6;

internal class Sick
{
    public Sick(string name = "Безымянный", int conditionNumber = 0)
    {
        Name = name;
        Condition = (Condition)conditionNumber;
    }

    public string Name { get; set; }
    public Condition Condition { get; set; }

    public string TalkWith(Visitor visitor)
    {
        return $"{visitor.Name} поговорил с {Name}";
    }
}
internal class Visitor
{
    public Visitor(string name)
    {
        Name = name;
    }

    public string Name { get; set; }

}

internal class Doctor
{
    public Doctor(string name = "Доктор", int freePlace = 15)
    {
        Name = name;
        FreePlace = freePlace;
    }

    readonly string Name;

    public int FreePlace;
    /// <summary>
    /// Проверяет может ли посетитель встретить пациента
    /// </summary>
    /// <param name="sick"></param>
    /// <returns>bool</returns>
    public bool CanVisitSick(Sick sick)
    {
        return (sick.Condition == Condition.good || sick.Condition == Condition.notGood) && FreePlace > 0;
    }
    public void TakePlace()
    {
        FreePlace--;
    }
    public void ReleasePlace()
    {
        FreePlace++;
    }

}
internal class Procedures
{

    public void takeHeal(Sick sick)
    {

        Console.WriteLine($"{sick.Name} принимает процедуры");

    }
}