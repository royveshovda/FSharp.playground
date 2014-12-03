using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace AltRunner
{
    public static class BrainFactory
    {
        static Random random = new Random();
        static AltCalulator helper = new AltCalulator();
        
        public static int CreateGetFirst()
        {
            return 0;
        }

        public static string SaveBrain(Brain2 brain)
        {
            var ser = JsonConvert.SerializeObject(brain);
            var fName = "brain_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm", CultureInfo.InvariantCulture) + ".json";
            File.WriteAllText(fName, ser);
            return fName;
        }

        public static Brain2 LoadBrain(string fileName)
        {
            if (File.Exists(fileName))
            {
                var brain = JsonConvert.DeserializeObject<Brain2>(File.ReadAllText(fileName));
                var filters = BrainFactory.Flatten(brain.DecisionTree);
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
            res.DecisionTree = new List<Filter>();
            
            for (int i = 0; i < random.Next(2) + 1; i++)
            {
                var filter = CreateNewFilter();
                res.DecisionTree.Add(filter);
            }
            
            return res;
        }

        private static Filter CreateNewFilter()
        {
            var filter = GetRandomValue(Filters);
            filter.Selector = GetRandom(Selectors);
            filter.FilterType = GetRandomValue(FilterTypes);
            return filter;

        }

        public static Brain2 Copy(Brain2 brain)
        {
            var res = new Brain2();
            res.GetFirst = brain.GetFirst;
            res.DecisionTree = new List<Filter>();
            foreach (var filter in brain.DecisionTree)
            {
                TraverseTree(filter, res.DecisionTree);
            }
            return res;
        }
        
        static List<Filter> Flatten(List<Filter> e)
        {
            return e.SelectMany(c => Flatten(c.Filters)).Concat(new List<Filter> (e)).ToList();
        }

        public static Brain2 CreateMutant(Brain2 brain)
        {
            var copy = Copy(brain);

            var flattended = Flatten(brain.DecisionTree);
            var decicionVictim = flattended.Skip(random.Next(flattended.Count - 1)).First();
            switch (random.Next(3))
            {
                case 0:
                    decicionVictim.Selector = GetRandom(Selectors);
                    break;
                case 1:
                    decicionVictim.FilterType = GetRandomValue(FilterTypes);
                    break;
                case 2:
                    var newFilter = GetRandomValue(Filters);
                    decicionVictim.FilterType = newFilter.FilterType;
                    decicionVictim.Match = newFilter.Match;
                    break;
                case 3:
                    if (decicionVictim.Filters == null) decicionVictim.Filters = new List<Filter>();
                    decicionVictim.Filters.Add(CreateNewFilter());
                    break;
            }

            return copy;

        }


        public static Brain2 CreateCrossover(Brain2 brain1, Brain2 brain2)
        {
            var copy = Copy(brain1);

            copy.DecisionTree.Clear();
            
            TraverseTree(brain1.DecisionTree.First(), copy.DecisionTree);
            TraverseTree(brain2.DecisionTree.First(), copy.DecisionTree);
            
            return copy;

        }

        private static void TraverseTree(Filter sourceFilter, List<Filter> destParentList)
        {
            var filter = new Filter();
            filter.Id = sourceFilter.Id;
            filter.Match = sourceFilter.Match;
            filter.Selector = sourceFilter.Selector;
            filter.FilterType = sourceFilter.FilterType;
            filter.Filters = new List<Filter>();

            foreach (var filter1 in sourceFilter.Filters)
            {
                TraverseTree(filter1, filter.Filters);
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

      
        public static Dictionary<int, Filter> Filters = new Dictionary<int, Filter>
        {
            {0, new Filter{Id = "under25", Match = (sol, rem) =>
            {
                return ((float)sol.Count / rem.Count) < 0.25;

            }}},

            {1, new Filter{Id = "under50", Match = (sol, rem) =>
            {
                return ((float)sol.Count / rem.Count) < 0.50;

            }}},

            {2, new Filter{Id = "under75", Match = (sol, rem) =>
            {
                return ((float)sol.Count / rem.Count) < 0.75;

            }}},
            {3, new Filter{Id = "over25", Match = (sol, rem) =>
            {
                return ((float)sol.Count / rem.Count) > 0.25;

            }}},

            {4, new Filter{Id = "over50", Match = (sol, rem) =>
            {
                return ((float)sol.Count / rem.Count) > 0.50;

            }}},

            {5, new Filter{Id = "over75", Match = (sol, rem) =>
            {
                return ((float)sol.Count / rem.Count) > 0.75;

            }}}

          

        };  

        public static Dictionary<int, FilterType> FilterTypes = new Dictionary<int, FilterType>
        {
            {0, FilterType.And},
            {1, FilterType.Or}
        }; 
        
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
                Id = "nearest", Definition = (sol, rem) => { return helper.Nearest(sol.Last(),rem); }
            }
            },
            {3, new Selector
            {
                Id = "nearestAndNearHorizontal", Definition = (sol, rem) => { return helper.NearestAndNearHorizontal(sol.Last(),rem); }
            }
            },
            {4, new Selector
            {
                Id = "nearestAndNearVertical", Definition = (sol, rem) => { return helper.NearestAndNearVertical(sol.Last(),rem); }
            }
            },
            {5, new Selector
            {
                Id = "farthest", Definition = (sol, rem) => { return rem.OrderByDescending(m => Calculator.dist(sol.Last(), m)).FirstOrDefault(); }
            }
            },
            {6, new Selector
            {
                Id = "secondNearest", Definition = (sol, rem) => { return helper.SecondNearest(sol.Last(),rem); }
            }
            },
            {7, new Selector
            {
                Id = "thirdNearest", Definition = (sol, rem) => { return helper.ThirdNearest(sol.Last(),rem); }
            }
            },
          
        }; 
    }
}