using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;

namespace AltRunner
{
    public class PointFinderHelper
    {
        public static Random r =new Random();

        public static double Length(IEnumerable<Calculator.Point> path)
        {
            var p = path.ToList();
            if (p.Any(a => a.X > 90 || a.X < -90 || a.Y > 180 || a.Y < -180))
            {
                return Calculator.length(p.ToArray());
            }
            return GeoDistance(p);
        }

        private static double GeoDistance(IEnumerable<Calculator.Point> path)
        {
            double sum = 0;
            var p = path.ToList();
            Calculator.Point? firstPoint = null;
            for (int i = 0; i < p.Count; i++)
            {
                var point = p[i];
                if (firstPoint == null)
                {
                    firstPoint = point;
                }
                if (i == p.Count - 1)
                {
                    sum +=
                        new GeoCoordinate(point.X, point.Y).GetDistanceTo(new GeoCoordinate(firstPoint.Value.X,
                            firstPoint.Value.Y));
                    break;
                }

                sum += new GeoCoordinate(point.X, point.Y).GetDistanceTo(new GeoCoordinate(p[i + 1].X, p[i + 1].Y));
            }

            return sum/1000;

        }

        public List<Calculator.Point> Remaining(IEnumerable<Calculator.Point> solution, IEnumerable<Calculator.Point> taken)
        {
            return solution.Except(taken).ToList();
        } 

        public Calculator.Point? Nearest(Calculator.Point p, IEnumerable<Calculator.Point> remaining)
        {
            return remaining.OrderBy(m => Calculator.dist(p, m)).FirstOrDefault();
        }

        public Calculator.Point? SecondNearest(Calculator.Point p, IEnumerable<Calculator.Point> remaining)
        {
            return remaining.OrderBy(m => Calculator.dist(p, m)).Skip(1).FirstOrDefault();
        }

        public Calculator.Point? ThirdNearest(Calculator.Point p, IEnumerable<Calculator.Point> remaining)
        {
            return remaining.OrderBy(m => Calculator.dist(p, m)).Skip(2).FirstOrDefault();
        }

        public Calculator.Point? NearestAndNearHorizontal(Calculator.Point p, IEnumerable<Calculator.Point> remaining)
        {
            var nearest = remaining.OrderBy(m => Calculator.dist(p, m));
            return nearest.Take(10).OrderBy(o =>Math.Abs(p.X - o.X)).FirstOrDefault();
        }

        public Calculator.Point? NearestAndNearVertical(Calculator.Point p, IEnumerable<Calculator.Point> remaining)
        {
            var nearest = remaining.OrderBy(m => Calculator.dist(p, m));
            return nearest.Take(10).OrderBy(o => Math.Abs(p.Y - o.Y)).FirstOrDefault();
        }
        

        public Calculator.Point? Farthest(Calculator.Point p, IEnumerable<Calculator.Point> remaining)
        {
            return remaining.OrderByDescending(m => Calculator.dist(p, m)).FirstOrDefault();
        }

        public Calculator.Point? Random(Calculator.Point p, IEnumerable<Calculator.Point> remaining)
        {
            var tmp = remaining.ToList();
            return tmp.Skip(r.Next(tmp.Count() - 1)).FirstOrDefault();
        }

        public Calculator.Point? First(IEnumerable<Calculator.Point> solution)
        {
            return solution.OrderByDescending(o => o.X*o.Y).FirstOrDefault();
        }

        public Calculator.Point? Last(IEnumerable<Calculator.Point> solution)
        {
            return solution.OrderBy(o => o.X * o.Y).FirstOrDefault();
        }

        public Calculator.Point? Middle(IEnumerable<Calculator.Point> solution)
        {
            return solution.OrderBy(o => o.X * o.Y).Take(solution.Count() / 2).FirstOrDefault();
        }


    }
}