using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FER.UTR.Lab2
{
    struct TransitionDomain
    {
        string state;
        string symbol;

        internal TransitionDomain(string stt, string symb)
        {
            state = stt;
            symbol = symb;
        }
    }

    class DFAMinimization
    {
        const int UNDEFINED = -1;

        static string[] _states;
        static string[] _symbols;
        static string[] _finalStates;
        static string _initialState;
        static Dictionary<TransitionDomain, string> _transitionsByDomain = new Dictionary<TransitionDomain, string>();

        static SortedSet<string> _reachableStates = new SortedSet<string>();
        static SortedSet<string> _minimizedStates;
        static SortedSet<string> _minimizedFinalStates;
        static SortedSet<string> _minimizedNonFinalStates;
        static Dictionary<TransitionDomain, string> _minimizedTransitionsByDomain;

        static List<SortedSet<string>> _statePartitions = new List<SortedSet<string>>();
        static Dictionary<string, List<int>> _statesByTransitionsPartition = new Dictionary<string, List<int>>();

        static void Main()
        {
            Initialize();
            ObtainReachableStates(_initialState);
            Minimization();
            Output();
        }

        static void Initialize()
        {
            _states = Console.ReadLine().Split(',');
            _symbols = Console.ReadLine().Split(',');
            _finalStates = Console.ReadLine().Split(',');
            _initialState = Console.ReadLine();
            string input;
            while (!string.IsNullOrWhiteSpace(input = Console.ReadLine()))
            {
                string state = input.Split(',')[0];
                string symbol = (input.Split(',')[1]).Split('-')[0];
                string nextState = input.Split('>')[1];
                if (_states.Contains(state) && _symbols.Contains(symbol) && _states.Contains(nextState))
                {
                    _transitionsByDomain.Add(new TransitionDomain(state, symbol), nextState);
                }
            }
        }

        static void ObtainReachableStates(string state)
        {
            if (_reachableStates.Add(state))
            {
                foreach (string symbol in _symbols)
                {
                    string nextState;
                    if (_transitionsByDomain.TryGetValue(new TransitionDomain(state, symbol), out nextState))
                    {
                        ObtainReachableStates(nextState);
                    }
                }
            }
        }

        static void Minimization()
        {
            _minimizedFinalStates = new SortedSet<string>();
            _minimizedNonFinalStates = new SortedSet<string>();
            foreach (string state in _reachableStates)
            {
                if (_finalStates.Contains(state))
                {
                    _minimizedFinalStates.Add(state);
                }
                else
                {
                    _minimizedNonFinalStates.Add(state);
                }
            }
            if (_minimizedNonFinalStates.Any())
            {
                _statePartitions.Add(_minimizedNonFinalStates);
            }
            if (_minimizedFinalStates.Any())
            {
                _statePartitions.Add(_minimizedFinalStates);
            }
            PartitionRefinement(ref _statePartitions);
            UpdateData();
        }

        static void PartitionRefinement(ref List<SortedSet<string>> partitionsList)
        {
            DefineStatesByTransitionsPartition();
            List<SortedSet<string>> newPartitionsList = new List<SortedSet<string>>();
            foreach (SortedSet<string> partition in partitionsList)
            {
                if (partition.Count == 1)
                {
                    newPartitionsList.Add(partition);
                    continue;
                }
                SortedSet<string> reducedPartition = new SortedSet<string>(partition);
                SortedSet<string> newPartition = Partition(ref reducedPartition);
                if (newPartition != null)
                {
                    newPartitionsList.Add(reducedPartition);
                    newPartitionsList.Add(newPartition);
                }
                else
                {
                    newPartitionsList.Add(partition);
                }
            }
            if (newPartitionsList.Count > partitionsList.Count)
            {
                partitionsList = newPartitionsList;
                PartitionRefinement(ref partitionsList);
            }
        }

        static SortedSet<string> Partition(ref SortedSet<string> partition)
        {
            List<int> firstStateTransitionsPartition = _statesByTransitionsPartition[partition.First()];
            SortedSet<string> newPartition = new SortedSet<string>();
            SortedSet<string> oldPartition = new SortedSet<string>(partition);
            foreach (string state in partition)
            {
                if (!firstStateTransitionsPartition.SequenceEqual(_statesByTransitionsPartition[state]))
                {
                    oldPartition.Remove(state);
                    newPartition.Add(state);
                }
            }
            partition = oldPartition;
            return newPartition.Any() ? newPartition : null;
        }

        static void DefineStatesByTransitionsPartition()
        {
            _statesByTransitionsPartition.Clear();
            foreach (string state in _reachableStates)
            {
                List<int> transitionsPartition = new List<int>();
                foreach (string symbol in _symbols)
                {
                    string nextState;
                    if (_transitionsByDomain.TryGetValue(new TransitionDomain(state, symbol), out nextState))
                    {
                        transitionsPartition.Add(GetStatePartition(nextState));
                    }
                    else
                    {
                        transitionsPartition.Add(UNDEFINED);
                    }
                }
                _statesByTransitionsPartition.Add(state, transitionsPartition);
            }
        }

        static int GetStatePartition(string state)
        {
            foreach (SortedSet<string> partition in _statePartitions)
            {
                if (partition.Contains(state))
                {
                    return _statePartitions.IndexOf(partition);
                }
            }
            return UNDEFINED;
        }

        static void UpdateData()
        {
            _initialState = _statePartitions[GetStatePartition(_initialState)].First();
            UpdateSet(ref _minimizedFinalStates);
            UpdateSet(ref _minimizedNonFinalStates);
            _minimizedStates = new SortedSet<string>(_minimizedNonFinalStates.Union(_minimizedFinalStates));
            UpdateTransitions();
        }

        static void UpdateSet(ref SortedSet<string> set)
        {
            SortedSet<string> updatedSet = new SortedSet<string>();
            foreach (string state in set)
            {
                int partitionIndex = GetStatePartition(state);
                if (partitionIndex != UNDEFINED)
                {
                    updatedSet.Add(_statePartitions[partitionIndex].First());
                }
            }
            set = updatedSet;
        }

        static void UpdateTransitions()
        {
            _minimizedTransitionsByDomain = new Dictionary<TransitionDomain, string>();
            foreach (string state in _minimizedStates)
            {
                foreach (string symbol in _symbols)
                {
                    string nextState;
                    if (_transitionsByDomain.TryGetValue(new TransitionDomain(state, symbol), out nextState))
                    {
                        if (!_minimizedStates.Contains(nextState))
                        {
                            nextState = _statePartitions[GetStatePartition(nextState)].First();
                        }
                        _minimizedTransitionsByDomain.Add(new TransitionDomain(state, symbol), nextState);
                    }
                }
            }
        }

        static void Output()
        {
            Console.WriteLine(PrintSet(_minimizedStates));
            Console.WriteLine(PrintSet(_symbols));
            Console.WriteLine(PrintSet(_minimizedFinalStates));
            Console.WriteLine(_initialState);
            PrintTransitions();
        }

        static string PrintSet(IEnumerable<string> set)
        {
            if (!set.Any())
            {
                return null;
            }
            StringBuilder print = new StringBuilder();
            foreach (string element in set)
            {
                print.Append(element + ",");
            }
            print.Length--;
            return print.ToString();
        }

        static void PrintTransitions()
        {
            foreach (string state in _minimizedStates)
            {
                foreach (string symbol in _symbols)
                {
                    string nextState;
                    if (_minimizedTransitionsByDomain.TryGetValue(new TransitionDomain(state, symbol), out nextState))
                    {
                        Console.WriteLine(state + "," + symbol + "->" + nextState);
                    }
                }
            }
        }
    }
}
