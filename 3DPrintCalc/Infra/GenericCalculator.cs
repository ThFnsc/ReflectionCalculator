using Calculator.Infra.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Calculator.Infra
{
    public class GenericCalculator<TCalculator> : IGenericCalculator where TCalculator : ICalculator, new()
    {
        public ICalculator Run()
        {
            var evaluator = new DataTable();
            var calculator = new TCalculator();
            var type = calculator.GetType();
            var properties = type.GetProperties();

            var gets = properties.Where(p => p.CanRead).ToList();
            var sets = properties.Where(p => p.CanWrite).ToList();

            foreach (var property in sets)
                property.SetValue(calculator, AskValue(property, evaluator));

            foreach (var group in gets.GroupBy(g => g.CanWrite).OrderByDescending(g => g.Key))
            {
                Console.WriteLine();
                foreach (var property in group)
                    PrintProperty(property, calculator);
            }

            return calculator;
        }

        private void PrintProperty(PropertyInfo property, ICalculator calculator)
        {
            var unit = property.GetCustomAttribute<UnitAttribute>();
            var displayName = property.GetCustomAttribute<DisplayAttribute>()?.Name ?? property.Name;
            var value = property.GetValue(calculator);
            if (property.PropertyType == typeof(TimeSpan))
                value = ((TimeSpan)value).Format();
            else if (IsNumber(property.PropertyType))
                value = Convert.ToDecimal(value).ToString("0.##");
            value = unit?.Format(value) ?? value;
            Console.WriteLine($"{displayName}: {value}");
        }

        private object AskValue(PropertyInfo property, DataTable evaluator)
        {
            string input;
            do
            {
                Prompt(property);
                input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                    input = property.GetCustomAttribute<DefaultValueAttribute>()?.Value?.ToString();
            }
            while (input == null);

            return ParseInput(property.PropertyType, evaluator, input);
        }

        private object ParseInput(Type propertyType, DataTable evaluator, string input)
        {
            if (propertyType == typeof(TimeSpan))
                return TimeSpanExtensions.TryParseNatural(input) ?? TimeSpan.Parse(input);
            else if (propertyType == typeof(string))
                return input;
            else if (TryAsNumber(input, propertyType, evaluator, out object number))
                return number;
            throw new NotImplementedException();
        }

        private void Prompt(PropertyInfo property)
        {
            var defaultValue = property.GetCustomAttribute<DefaultValueAttribute>()?.Value;
            var unit = property.GetCustomAttribute<UnitAttribute>();
            var displayName = property.GetCustomAttribute<DisplayAttribute>()?.Name ?? property.Name;

            string hint = null;
            if (defaultValue != null)
                if (unit == null)
                    hint = defaultValue.ToString();
                else
                    hint = unit.Format(defaultValue);
            else if (unit != null)
                hint = unit.ToString();

            Console.Write($"{displayName}{(hint == null ? "" : $" ({hint})")}: ");
        }

        private static bool IsNumber(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        private static bool TryAsNumber(string input, Type target, DataTable evaluator, out object number)
        {
            number = null;
            if (IsNumber(target))
            {
                number = evaluator.Compute(input, "");
                var evaluatedType = number.GetType();
                if (evaluatedType == target.GetType())
                    return true;
                var methods = typeof(Convert)
                    .GetMethods()
                    .Where(m => m.IsStatic && m.ReturnType == target)
                    .Where(m => { var pars = m.GetParameters(); return pars.Length == 1 && pars.Single().ParameterType == evaluatedType; });
                number = methods.First().Invoke(null, new object[] { number });
                return true;
            }
            return false;
        }
    }
}