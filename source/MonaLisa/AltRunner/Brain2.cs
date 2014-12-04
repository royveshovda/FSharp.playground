using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace AltRunner
{
    public class Brain2
    {
        private List<Calculator.Point> solution = new List<Calculator.Point>();
        public List<Filter> DecisionTree { get; set; }
        
        public int GetFirst { get; set; }
        
        [JsonIgnore]
        public bool Failed { get; set; }
        
        [JsonIgnore]
        public double SolutionDistance { get { return PointFinderHelper.Length(Solution); } }
        
        [JsonIgnore]
        public List<Calculator.Point> Solution
        {
            get { return solution; }
            set { solution = value; }
        }

        public void Think(List<Calculator.Point> problem)
        {
            var topFilter = new Filter
            {
                Match = (sol, rem, par) => true,
                Selector = 0,
                Filters = DecisionTree,
            };

            var remaining = problem.ToList();
            Solution.Clear();
            Failed = false;

            var run = BrainFactory.Selectors[GetFirst].Run(Solution, remaining);
            if (run != null)
            {
                AddToSolution(remaining, run.Value);
            }

            while (solution.Count < problem.Count)
            {
                var selector = topFilter.Run(Solution, remaining);
                if (selector == null)
                {
                    Failed = true;
                    break;
                }
              
                var point = BrainFactory.Selectors[selector.Value].Run(solution, remaining);
               
                if (point == null)
                {
                    Failed = true;
                    break;
                }

                AddToSolution(remaining, point.Value);
            }
        }

        

        private void AddToSolution(List<Calculator.Point> remaining, Calculator.Point point)
        {
            Solution.Add(point);
            remaining.Remove(point);
        }
        
    }
}