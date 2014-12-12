using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AltRunner
{
    public class Generation
    {
        private readonly Calculator.Point[] solution;
        public List<Brain2> Lemmings { get; set; }
        private int generationNumber;

        public List<Calculator.Point> BestResult { get
        {
            if (BestBrain != null)
            {
                return BestBrain.Solution;
            }
            return null;
            
        } }

        public Brain2 BestBrain { get { return Lemmings.Where(w => w.Failed == false && w.Solution.Count == solution.Count())
                                                .OrderBy(d => d.SolutionDistance)
                                                .ThenBy(o => BrainFactory.Flatten(o.DecisionTree.Filters).Count).FirstOrDefault(); } }

        public void RunAll()
        {
            //Parallel.ForEach(Lemmings, l => l.Think(solution.ToList()));
            for (int i = 0; i < Lemmings.Count; i++)
            {
                Lemmings[i].Think(solution.ToList());
            }
        }

        private void CalculateFitnessForAll(int lengthOfProblem)
        {
            foreach (var lemming in Lemmings)
            {
                var distance = 0.0;
                if (!lemming.Failed)
                {
                    distance = lemming.SolutionDistance;
                }
                var missingPoint = lengthOfProblem - lemming.Solution.Count;

                //TODO: Finish this one

            }
        }

        public void Evolve(double completed, int lengthOfProblem)
        {
            var numberOfIndividuals = Lemmings.Count;
            
            //TODO: 1: Include number of nodes in fitness, so that lemmings not able to complete will get a rating
            //TODO: 2: Evolve: Only from previous generation
            //TODO: Consider to change number of mutations and crossovers based on generation-number



            // 10% keep, 45% mutated, 25% crossover. The rest (20%) are new guys
            var keepPercent = 10; 
            var mutatePercentage = (45 - completed);
            var crossoverPercentage = (25 - completed) / 2;

            CalculateFitnessForAll(lengthOfProblem);
            //TODO: Get fitnessNumber


            var bestToWorst = Lemmings.Where(w=>w.Failed == false).OrderBy(o => o.SolutionDistance).ToList();

            
            var offsprings = new List<Brain2>();
            
            foreach (var lemming in bestToWorst.Take(Convert.ToInt32(numberOfIndividuals * (mutatePercentage / 100))))
            {
                offsprings.Add(BrainFactory.CreateMutant(lemming));
            
            }

            Brain2 last = null;
            foreach (var lemming in bestToWorst.Take(Convert.ToInt32(numberOfIndividuals * (crossoverPercentage / 100))))
            {
                if (last != null)
                {
                    Tuple<Brain2, Brain2> crossovers = BrainFactory.CreateCrossover(last, lemming);
                    offsprings.Add(crossovers.Item1);
                    offsprings.Add(crossovers.Item2);
                }
                last = lemming;

            }


            var missing = numberOfIndividuals - (bestToWorst.Take(numberOfIndividuals / keepPercent).Count() + offsprings.Count);

            if (missing > 0)
            {
                for (int i = 0; i < missing; i++)
                {
                    offsprings.Add(BrainFactory.CreateNew());
                }
            }

            Lemmings = bestToWorst.Take(numberOfIndividuals / keepPercent).Concat(offsprings).ToList();
            
            var top3 = bestToWorst.Take(1);
            Console.WriteLine("Best of generation " + generationNumber);

            foreach (var lemming in top3)
            {
                Console.WriteLine(lemming.SolutionDistance);
            }
            Console.WriteLine("=====================\n\n\n");
            
            generationNumber++;
        }

        public Generation(Calculator.Point[] solution)
        {
            this.solution = solution.ToArray();
            Lemmings = new List<Brain2>();
        }
    }
}