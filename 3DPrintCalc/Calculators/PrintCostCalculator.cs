using Calculator.Infra;
using Calculator.Infra.Attributes;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Calculator.Calculators
{
    [Display(Name = "Custos de Impressão")]
    public class PrintCostCalculator : ICalculator
    {
        [Display(Name = "Preço total do filamento")]
        [Unit(Prefix = "R$")]
        public double TotalFilamentCost { get; set; }

        [Display(Name = "Total de tempo")]
        [DefaultValue("2h 3m")]
        public TimeSpan TotalTime { get; set; }

        [Display(Name = "Preço por hora")]
        [Unit(Prefix = "R$", Suffix = "/h")]
        [DefaultValue(2)]
        public double CostPerHour { get; set; }

        [Display(Name = "Média de KW que a impressora consome")]
        [Unit(Suffix = "KW")]
        [DefaultValue(.23)]
        public double AverageKW { get; set; }

        [Display(Name = "Preço do KWh")]
        [Unit(Suffix = "KW/h")]
        [DefaultValue(.9)]
        public double CostPerKWH { get; set; }

        [Display(Name = "Multiplicador")]
        [DefaultValue(1)]
        public int Quantity { get; set; }



        [Display(Name = "Custo total das horas")]
        [Unit(Prefix = "R$")]
        public double TotalHoursCost => TotalTime.TotalHours * CostPerHour;

        [Display(Name = "Total de energia")]
        [Unit(Suffix = "KW/h")]
        public double TotalKWh => TotalTime.TotalHours * AverageKW;

        [Display(Name = "Custo total de energia")]
        [Unit(Prefix = "R$")]
        public double TotalEnergyCost => CostPerKWH * TotalKWh;

        [Unit(Prefix = "R$")]
        public double Total => (TotalEnergyCost + TotalHoursCost + TotalFilamentCost) * Quantity;
    }
}
