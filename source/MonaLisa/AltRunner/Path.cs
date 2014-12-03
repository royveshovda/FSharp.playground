using System.Linq;

namespace AltRunner
{
    public class Path
    {
        public Calculator.Point P1 { get; set; }
        public Calculator.Point P2 { get; set; }

        public bool Intersects(Path[] anotherPath)
        {
            return anotherPath.Any(a => a.Intersects(this));
        }

        bool Intersects(Path anotherPath)
        {
            return CrossProduct(P1, P2, anotherPath.P1) !=
                   CrossProduct(P1, P2, anotherPath.P2) ||
                   CrossProduct(anotherPath.P1, anotherPath.P2, P1) !=
                   CrossProduct(anotherPath.P1, anotherPath.P2, P2);
        }

        public static double CrossProduct(Calculator.Point p1, Calculator.Point p2, Calculator.Point p3)
        {
            return (p2.X - p1.X) * (p3.Y - p1.Y) - (p3.X - p1.X) * (p2.Y - p1.Y);
        }
    }
}