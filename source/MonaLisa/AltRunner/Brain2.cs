using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace AltRunner
{
    public class Brain2
    {
        private List<Calculator.Point> solution = new List<Calculator.Point>();
        
        
        public FilterNode DecisionTree { get; set; }
        
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

        [JsonIgnore]
        public double FitnessValue { get; set; }

        public void Think(List<Calculator.Point> problem)
        {
            var remaining = problem.ToList();
            Solution.Clear();
            Failed = false;

            
            // Add first node to solution
            var run = BrainFactory.Selectors[GetFirst].Run(Solution, remaining);
            if (run != null)
            {
                AddToSolution(remaining, run.Value);
            }

            // while solution is not finished
            while (solution.Count < problem.Count)
            {
                var selector = DecisionTree.Run(Solution, remaining);
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