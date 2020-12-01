using Calculator.Calculators;
using Calculator.Infra;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Calculator
{
    class Program
    {
        static void Main()
        {
            Type calculator = null;
            for (; ; )
            {
                var calculators = Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => t.GetInterfaces().Contains(typeof(ICalculator)))
                    .ToList();
                if (calculators.Count > 1 || calculator != null)
                {
                    calculators.ForEach((type, i) =>
                        Console.WriteLine($"{i}. {type.GetCustomAttribute<DisplayAttribute>()?.Name ?? type.Name}"));
                    Console.WriteLine($"{calculators.Count}. Sair");
                    string input;
                    int index;
                    do
                    {
                        Console.Write("> ");
                        input = Console.ReadLine();
                    }
                    while (!int.TryParse(input, out index) || index < 0 || index > calculators.Count);
                    if (index == calculators.Count)
                        return;
                    calculator = calculators[index];
                }
                else
                    calculator = calculators.First();
                var calculatorInstance = (IGenericCalculator)Activator.CreateInstance(typeof(GenericCalculator<>).MakeGenericType(calculator));
                calculatorInstance.Run();
                Console.WriteLine();
            }
        }
    }
}
