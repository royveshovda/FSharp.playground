using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using AutoMapper;
using Newtonsoft.Json;

namespace AltRunner
{
    public static class BrainFactory
    {
        static Random random = new Random();
        static PointFinderHelper pointFinderHelper = new PointFinderHelper();

        static BrainFactory()
        {
            Mapper.CreateMap<Parameter, Parameter>();
        }
        public static int CreateGetFirst()
        {
            return 0;
        }

        public static string SaveBrain(Brain2 brain)
        {
            var ser = JsonConvert.SerializeObject(brain);
            var fName = "brain_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm", CultureInfo.InvariantCulture);
            File.WriteAllText(fName + ".json", ser);
            var sb = new StringBuilder();
            int count = 0;
            foreach (var p in brain.Solution)
            {
                count++;
                sb.AppendLine(string.Format("{0:0} {1:00000} {2:00000}", count, p.X, p.Y));
            }
            sb.AppendLine("EOF");
            
            File.WriteAllText(fName + ".path", sb.ToString());
            
            return fName;
        }

        public static Brain2 LoadBrain(string fileName)
        {
            if (File.Exists(fileName))
            {
                var brain = JsonConvert.DeserializeObject<Brain2>(File.ReadAllText(fileName));
                var filters = BrainFactory.Flatten(brain.DecisionTree.Filters);
                foreach (var filter in filters)
                {
                    var origFilter = BrainFactory.Filters.First(w => w.Value.Id == filter.Id).Value;
                    filter.Match = origFilter.Match;
                }

                return brain;
            }
            
            return null;
        }
        
        public static Brain2 CreateNew()
        {
            var res = new Brain2();
           
            res.GetFirst = CreateGetFirst();
            
            var topNode = NewDecisionTree();

            res.DecisionTree = topNode;
            
            for (int i = 0; i < random.Next(2) + 1; i++)
            {
                var filter = CreateNewFilter();
                res.DecisionTree.Filters.Add(filter);
            }
            
            return res;
        }

        private static FilterNode NewDecisionTree()
        {
            var topFilter = new FilterNode
            {
                Match = (sol, rem, par) => true,
                Selector = 0,
                FilterType = FilterType.Or,
                Filters = new List<FilterNode>(),
            };
            return topFilter;
        }

        private static FilterNode CreateNewFilter()
        {
            var filter = Copy(GetRandomValue(Filters));
            filter.Selector = GetRandom(Selectors);
            filter.FilterType = GetRandomValue(FilterTypes);
            filter.Parameter = new Parameter();
            filter.Parameter.CompletedThreshold = random.Next(100)/100.0;

            return filter;

        }

        public static Brain2 Copy(Brain2 brain)
        {
            var res = new Brain2();
            res.GetFirst = brain.GetFirst;
            res.DecisionTree = NewDecisionTree();
            foreach (var filter in brain.DecisionTree.Filters)
            {
                CopyTree("copy",filter, res.DecisionTree.Filters);
            }
            return res;
        }

        public static FilterNode Copy(FilterNode filterNode)
        {
            var dummy = new List<FilterNode>();
            CopyTree("copy FilterNode", filterNode, dummy);
            return dummy.First();
        }

        public static List<FilterNode> Flatten(List<FilterNode> e)
        {
            return e.SelectMany(c => Flatten(c.Filters)).Concat(new List<FilterNode> (e)).ToList();
        }

        public static List<FilterNode> Flatten(FilterNode e)
        {
            var list = e.Filters.SelectMany(c => Flatten(c.Filters)).ToList();
            list.Insert(0,e);
            return list;
        }

        public static Brain2 CreateMutant(Brain2 brain)
        {
            var copy = Copy(brain);

            var flattended = Flatten(brain.DecisionTree.Filters);
            var decicionVictim = flattended.Skip(random.Next(flattended.Count - 1)).First();
            switch (random.Next(3))
            {
                //case 0:
                //    decicionVictim.History.Add("Changed selector");
                //    decicionVictim.Selector = GetRandom(Selectors);
                //    break;
                //case 1:
                //    decicionVictim.History.Add("Changed FilterNode type");
                //    decicionVictim.FilterType = GetRandomValue(FilterTypes);
                //    break;
                //case 2:
                //    decicionVictim.History.Add("Changed match logic");
                //    var newFilter = GetRandomValue(Filters);
                //    decicionVictim.FilterType = newFilter.FilterType;
                //    decicionVictim.Match = newFilter.Match;
                //    break;
                case 0:
                    decicionVictim.History.Add("Added new FilterNode");
                    if (decicionVictim.Filters == null) decicionVictim.Filters = new List<FilterNode>();
                    decicionVictim.Filters.Add(CreateNewFilter());
                    break;
                case 1:
                    decicionVictim.History.Add("Increased completed threshold");
                    decicionVictim.Parameter.CompletedThreshold += 0.05;
                    break;
                case 2:
                    decicionVictim.History.Add("Decreased completed threshold");
                    decicionVictim.Parameter.CompletedThreshold -= 0.05;
                    break;
            }
            
            return copy;

        }


        //TODO better crossover
        public static Brain2 CreateCrossoverOLD(Brain2 brain1, Brain2 brain2)
        {
            var copy = Copy(brain1);

            copy.DecisionTree.Filters.Clear();
            var node1 = brain1.DecisionTree.Filters.First();
            var node2 = brain2.DecisionTree.Filters.First();
            node1.History.Add("Crossover");
            node2.History.Add("Crossover");
            CopyTree("crossover", node1, copy.DecisionTree.Filters);
            CopyTree("crossover", node2, copy.DecisionTree.Filters);
            
            return copy;

        }

        public static Tuple<Brain2, Brain2> CreateCrossover(Brain2 brain1, Brain2 brain2)
        {
            var list1 = Flatten(brain1.DecisionTree);
            var list2 = Flatten(brain2.DecisionTree);

            var splitPoint1 = 0;
            if (list1.Count > 2)
                splitPoint1 = random.Next(1, list1.Count - 2);

            var splitPoint2 = 0;
            if(list2.Count > 2)
                splitPoint2 = random.Next(1, list2.Count-2);

            var node1 = list1[splitPoint1];
            var node2 = list2[splitPoint2];

            var tempPoint1 = node1.Clone();
            var tempPoint2 = node2.Clone();

            node1.Id = tempPoint2.Id;
            node1.History = tempPoint2.History;
            node1.FilterType = tempPoint2.FilterType;
            node1.Filters = tempPoint2.Filters;
            node1.Parameter = tempPoint2.Parameter;
            node1.History.Add("Crossover");

            node2.Id = tempPoint1.Id;
            node2.History = tempPoint1.History;
            node2.FilterType = tempPoint1.FilterType;
            node2.Filters = tempPoint1.Filters;
            node2.Parameter = tempPoint1.Parameter;
            node2.History.Add("Crossover");

            return new Tuple<Brain2, Brain2>(brain1, brain2);
        }

        
        private static void CopyTree(string source, FilterNode sourceFilterNode, List<FilterNode> destParentList)
        {
            //foreach (var h in sourceFilterNode.History)
            //{
            //    Console.WriteLine(h);
            //}
            //Console.WriteLine(source);
            
            var filter = new FilterNode();
            filter.Id = sourceFilterNode.Id;
            filter.Match = sourceFilterNode.Match;
            filter.Selector = sourceFilterNode.Selector;
            filter.FilterType = sourceFilterNode.FilterType;
            filter.History = sourceFilterNode.History.ToList();
            
            filter.Parameter = new Parameter();
            if (sourceFilterNode.Parameter != null)
            {
                filter.Parameter.CompletedThreshold = sourceFilterNode.Parameter.CompletedThreshold;
            }

            filter.Filters = new List<FilterNode>();

            foreach (var filter1 in sourceFilterNode.Filters)
            {
                CopyTree(source, filter1, filter.Filters);
            }

            destParentList.Add(filter);

        }

        private static int GetRandom<T>(Dictionary<int, T> input)
        {
            return random.Next(input.Count);
        }

        private static T GetRandomValue<T>(Dictionary<int, T> input)
        {
            return input[random.Next(input.Count)];
        }

      
        public static Dictionary<int, FilterNode> Filters = new Dictionary<int, FilterNode>
        {
            {0, new FilterNode{Id = "underFilter", Match = (sol, rem, par) =>
            {
                if (par.CompletedThreshold != null)
                {
                    return ((float)sol.Count / rem.Count) < par.CompletedThreshold;
                }
                return false;
            }}},

            {1, new FilterNode{Id = "overFilter", Match = (sol, rem, par) =>
            {
                if (par.CompletedThreshold != null)
                {
                    return ((float)sol.Count / rem.Count) > par.CompletedThreshold;
                }
                return false;

            }}},

          

        };  

        public static Dictionary<int, FilterType> FilterTypes = new Dictionary<int, FilterType>
        {
            {0, FilterType.And},
            {1, FilterType.Or}
        };

        private static bool IsPointSet(Calculator.Point point)
        {
            return (Math.Abs(point.X - 0.0) > 0 && Math.Abs(point.Y - 0.0) > 0);
        }
        public static Dictionary<int,Selector> Selectors = new Dictionary<int, Selector>
        {
            {0, new Selector
            {
                Id = "first", Definition = (sol, rem) => { return rem.First(); }
            }
            },
            {1, new Selector
            {
                Id = "last", Definition = (sol, rem) => { return rem.Last(); }
            }
            },
            {2, new Selector
            {
                Id = "nearest", Definition = (sol, rem) => { return pointFinderHelper.Nearest(sol.Last(),rem); }
            }
            },
            {3, new Selector
            {
                Id = "nearestAndNearHorizontal", Definition = (sol, rem) => { return pointFinderHelper.NearestAndNearHorizontal(sol.Last(),rem); }
            }
            },
            {4, new Selector
            {
                Id = "nearestAndNearVertical", Definition = (sol, rem) => { return pointFinderHelper.NearestAndNearVertical(sol.Last(),rem); }
            }
            },
            {5, new Selector
            {
                Id = "farthest", Definition = (sol, rem) => { return rem.OrderByDescending(m => Calculator.dist(sol.Last(), m)).FirstOrDefault(); }
            }
            },
            {6, new Selector
            {
                Id = "secondNearest", Definition = (sol, rem) => { return pointFinderHelper.SecondNearest(sol.Last(),rem); }
            }
            },
            {7, new Selector
            {
                Id = "thirdNearest", Definition = (sol, rem) => { return pointFinderHelper.ThirdNearest(sol.Last(),rem); }
            }
            },
            {8, new Selector
            {
                Id = "keepToleft", Definition = (sol, rem) =>
                {
                    var last = sol.Last();
                    var orderedByDistance = rem.OrderBy(m => Calculator.dist(last, m)).Take(10).ToList();
                    
                    var toTheLeft = orderedByDistance.TakeWhile(p => (p.X - last.X) < 0).LastOrDefault();
                    if (IsPointSet(toTheLeft) == false)
                    {
                        toTheLeft = orderedByDistance.TakeWhile(p => (p.Y - last.Y) < 0).LastOrDefault();
                        
                    }
                    if (IsPointSet(toTheLeft) == false)
                    {
                        toTheLeft = orderedByDistance.SkipWhile(p => (p.X - last.X) <= 0).FirstOrDefault();

                    }
                    if (IsPointSet(toTheLeft) == false)
                    {
                        toTheLeft = orderedByDistance.SkipWhile(p => (p.Y - last.Y) <= 0).FirstOrDefault();
                    }

                    if (IsPointSet(toTheLeft))
                    {
                        return toTheLeft;
                    }
                    
                    return null;
                }
            }
            },
          
        }; 
    }
}