using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator.Infra
{
    public interface IGenericCalculator
    {
        ICalculator Run();
    }
}
