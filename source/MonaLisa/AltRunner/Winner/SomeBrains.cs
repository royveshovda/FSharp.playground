using System;
using System.Collections.Generic;
using System.Linq;
using AltRunner;

public class SomeBrains
{


    public Calculator.Point? GetNext(Calculator.Point? start, Calculator.Point[] sol, IEnumerable<Calculator.Point> res, AltCalulator helper)
    {
        if (start == null) { start = GetStartPoint(sol, helper); }
        Calculator.Point? point = null;
        point = helper.Nearest(start.Value, helper.Remaining(sol, res));
        if (point.Value.X < start.Value.Y)
        {
            return point;
        }
        if (point.Value.Y < start.Value.X)
        {
            point = helper.Farthest(point.Value, helper.Remaining(sol, res));
        }
        return point;
    }



    public Calculator.Point? GetStartPoint(Calculator.Point[] solution, AltCalulator helper)
    {
        return solution[9];
    }


}