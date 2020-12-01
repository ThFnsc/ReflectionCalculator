using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator.Infra.Attributes
{
    public class UnitAttribute : Attribute
    {
        public string Prefix { get; set; }

        public string Suffix { get; set; }

        public string Format(object value) =>
            $"{Prefix}{value}{Suffix}";

        public override string ToString() =>
            Prefix + Suffix;
    }
}
