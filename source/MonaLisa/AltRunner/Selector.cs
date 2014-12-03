using System;
using System.Collections.Generic;

namespace AltRunner
{
    public class Selector
    {
        public string Id { get; set; }
        public Func<List<Calculator.Point>, List<Calculator.Point>, Calculator.Point?> Definition { get; set; }
        public Calculator.Point? Run(List<Calculator.Point> solution, List<Calculator.Point> remaining)
        {
            return Definition(solution, remaining);
        }
    }
}