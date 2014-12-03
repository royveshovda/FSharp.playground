using System;
using System.Collections.Generic;
using System.Linq;

namespace AltRunner
{
    public class Colony
    {
        private readonly Calculator.Point[] solution;
        public List<Brain2> Lemmings { get; set; }
        private int Generation = 0;

        public List<Calculator.Point> BestResult { get
        {
            if (BestBrain != null)
            {
                return BestBrain.Solution;
            }
            return null;
            
        } }

        public Brain2 BestBrain { get { return Lemmings.Where(w => w.Failed == false && w.Solution.Count == solution.Count()).OrderBy(d => d.SolutionDistance).FirstOrDefault(); } }

        public void RunAll()
        {
            //Parallel.ForEach(Lemmings, l => l.Think(solution.ToList()));
            for (int i = 0; i < Lemmings.Count; i++)
            {
                Lemmings[i].Think(solution.ToList());
            }
        }

        public void Evolve()
        {
            var numberOfIndividuals = Lemmings.Count;
            
            //TODO change with generation number

            var keepPercent = 10;

            // of keepPercent
            var mutatePercentage = 100;
            var crossoverPercentage = 100;


            var bestToWorst = Lemmings.Where(w=>w.Failed == false).OrderBy(o => o.SolutionDistance).ToList();

            
            var offsprings = new List<Brain2>();
            
            foreach (var lemming in bestToWorst.Take( (numberOfIndividuals / keepPercent) * (100 / mutatePercentage)))
            {
                offsprings.Add(BrainFactory.CreateMutant(lemming));
            
            }

            Brain2 last = null;
            foreach (var lemming in bestToWorst.Take((numberOfIndividuals / keepPercent) * (100 / crossoverPercentage)))
            {
                if (last != null)
                {
                    offsprings.Add(BrainFactory.CreateCrossover(last, lemming));
                    offsprings.Add(BrainFactory.CreateCrossover(lemming, last));
                }
                last = lemming;

            }


            var missing = numberOfIndividuals - (bestToWorst.Take(numberOfIndividuals / keepPercent).Count() + offsprings.Count);

            if (missing < Lemmings.Count)
            {
                for (int i = 0; i < missing; i++)
                {
                    offsprings.Add(BrainFactory.CreateNew());
                }
            }

            Lemmings = bestToWorst.Take(numberOfIndividuals / keepPercent).Concat(offsprings).ToList();
            
            var top3 = bestToWorst.Take(1);
            Console.WriteLine("Best of generation " + Generation);

            foreach (var lemming in top3)
            {
                Console.WriteLine(lemming.SolutionDistance);
            }
            Console.WriteLine("=====================\n\n\n");
            
            Generation++;
        }

        public Colony(Calculator.Point[] solution)
        {
            this.solution = solution.ToArray();
            Lemmings = new List<Brain2>();
        }
    }
}