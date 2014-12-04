using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Text;


namespace AltRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            var t = new Test(args.Any() && File.Exists(args.First()), args.FirstOrDefault());
            
        }
    }


    public class Test 
    {
        private double bestDistance;

        public Test(bool realTest, string fileName)
        {
            //Proper test
            if (realTest)
            {
                var monaLisa = global::Program.readFile("..\\..\\..\\mona-lisa100K.tsp", 100000, 6);
           //     var monaLisa = global::Program.readFile("..\\..\\..\\burma14.tsp", 14, 8);
           
                var winnerBrain = BrainFactory.LoadBrain(fileName);
                winnerBrain.Think(monaLisa.ToList());

                if (winnerBrain.Solution.Count != monaLisa.Count())
                {
                    Console.WriteLine("Winner brain failed");
                }
                else
                {
                    Console.WriteLine("Distance: " + winnerBrain.SolutionDistance);
                }

                var resString = new StringBuilder();
                var count = 0;
                foreach (var point in winnerBrain.Solution)
                {
                    count++;
                    resString.AppendLine(count + " " + point.X + " " + point.Y);
                }

                File.WriteAllText("winnerPath.txt", resString.ToString());
            }



            //var t = global::Program.readFile("..\\..\\..\\burma14.tsp", 14, 8);
          //  var t = global::Program.readFile("..\\..\\..\\t2.txt", 14, 8);
             var t = global::Program.readFile("..\\..\\..\\uruguay.tsp", 734, 7);
            var test = PointFinderHelper.Length(t);
            var col = new Generation(t);
      
            for (int i = 0; i < 100; i++)
            {
                col.Lemmings.Add(BrainFactory.CreateNew());
            }

            var bestRes = new List<Calculator.Point>();
            Brain2 bestBrain = null;
            for (int i = 0; i < 100; i++)
            {
                col.RunAll();
                if (col.BestResult != null && (bestRes.Any() == false  || col.BestBrain.SolutionDistance < bestDistance))
                {
                    //bestRes = col.BestResult.ToList();
                    //bestDistance = col.BestBrain.SolutionDistance;
                    //bestBrain = BrainFactory.Copy(col.BestBrain);
                    
                
                }
                col.Evolve();
            }

            bestRes = col.BestResult.ToList();
            bestDistance = col.BestBrain.SolutionDistance;
            bestBrain = BrainFactory.Copy(col.BestBrain);
                    
            Console.WriteLine("Winner algorithm:\n====================================");
            Console.WriteLine("Winner brain saved at " + BrainFactory.SaveBrain(col.BestBrain));

            Console.WriteLine(bestDistance);
//            File.WriteAllText("Winner.txt", winnerAlgorithm);

            
            for (int i = 0; i < bestRes.Count; i++)
            {
                Console.WriteLine("{0:0} {1:00000} {2:00000}", i, bestRes[i].X, bestRes[i].Y);
            }
            
            if (bestBrain != null) bestBrain.Think(t.ToList());
            
            
            Console.ReadLine();
        }
    }
}
