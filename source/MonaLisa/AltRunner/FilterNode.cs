using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace AltRunner
{
    public class FilterNode
    {
        private List<FilterNode> filters = new List<FilterNode>();
        private List<string> history = new List<string>();
        public string Id { get; set; }
        public FilterType FilterType { get; set; }
        public Parameter Parameter { get; set; }

        public List<string> History
        {
            get { return history; }
            set { history = value; }
        }

        public List<FilterNode> Filters
        {
            get { return filters; }
            set { filters = value; }
        }

        [JsonIgnore]
        public Func<List<Calculator.Point>, List<Calculator.Point>, Parameter, bool> Match { get; set; }
        
        public int Selector { get; set; }
      
        public int? Run(List<Calculator.Point> solution, List<Calculator.Point> remaining)
        {
            int? res = null;
            if (Filters == null || Filters.Any() == false)
            {
                if (Match(solution, remaining, Parameter))
                {
                    return Selector;
                }
            }
            else
            {   
                foreach (var filterGroup in Filters.GroupBy(g => g.FilterType))
                {
                    if (filterGroup.Key == FilterType.Or)
                    {
                        foreach (var filter in filterGroup.ToList())
                        {
                            res = filter.Run(solution, remaining);
                            if (res != null)
                            {
                                return res;
                            }
                        }
                    }
                    else
                    {
                        foreach (var filter in filterGroup.ToList())
                        {
                            res = filter.Run(solution, remaining);
                            if (res == null)
                            {
                                return null;
                            }
                        }
                    }
                }
            }
            return res;
        }
    }

    public class Parameter
    {
        public double? CompletedThreshold { get; set; }
    }
}