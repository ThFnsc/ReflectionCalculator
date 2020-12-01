using Calculator.Infra;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Calculator.Calculators
{
    [Display(Name = "Calculadora de Bhaskara")]
    public class BhaskaraCalculator : ICalculator
    {
        [Display(Name = "A")]
        public double A { get; set; }
        
        [Display(Name = "B")]
        public double B { get; set; }
        
        [Display(Name = "C")]
        public double C { get; set; }

        [Display(Name = "Delta")]
        public double Delta => Math.Sqrt(Math.Pow(B, 2) - 4 * A * C);

        [Display(Name = "X1")]
        public double X1 => (-B + Delta) / 2 * A;

        [Display(Name = "X2")]
        public double X2 => (-B - Delta) / 2 * A;
    }
}
