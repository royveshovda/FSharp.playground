using System;
using System.Collections.Generic;
using System.Linq;
using AltRunner;

public class SomeBrains
{


    public Calculator.Point? GetNext(Calculator.Point? start, Calculator.Point[] sol, IEnumerable<Calculator.Point> res, PointFinderHelper pointFinderHelper)
    {
        if (start == null) { start = GetStartPoint(sol, pointFinderHelper); }
        Calculator.Point? point = null;
        point = pointFinderHelper.Nearest(start.Value, pointFinderHelper.Remaining(sol, res));
        if (point.Value.X < start.Value.Y)
        {
            return point;
        }
        if (point.Value.Y < start.Value.X)
        {
            point = pointFinderHelper.Farthest(point.Value, pointFinderHelper.Remaining(sol, res));
        }
        return point;
    }



    public Calculator.Point? GetStartPoint(Calculator.Point[] solution, PointFinderHelper pointFinderHelper)
    {
        return solution[9];
    }


}