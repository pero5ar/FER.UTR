using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FER.UTR.Lab1
{
    struct Transition
    {
        string state;
        string symbol;

        internal Transition(string stt, string symb)
        {
            state = stt;
            symbol = symb;
        }
    }

    class eNFA
    {
        const char EPSILON = '$';
        const char EMPTY = '#';
        const char DELIMITER = '|';

        static string[] _inputArrays;
        static string[] _states;
        static string[] _symbols;
        static string[] _finalStates;
        static string _initialState;
        static Dictionary<Transition, string[]> _transitions = new Dictionary<Transition, string[]>();
        static SortedSet<string> _currentStates = new SortedSet<string>();

        static void Main()
        {
            Initialize();
            foreach (string input in _inputArrays)
            {
                Console.WriteLine(Test(input));
            }
        }

        static void Initialize()
        {
            _inputArrays = Console.ReadLine().Split(DELIMITER);
            _states = Console.ReadLine().Split(',');
            _symbols = Console.ReadLine().Split(',');
            Array.Resize(ref _symbols, _symbols.Length + 1);
            _symbols[_symbols.Length - 1] = EPSILON.ToString();
            _finalStates = Console.ReadLine().Split(',');
            _initialState = Console.ReadLine();
            string input;
            while (!string.IsNullOrWhiteSpace(input = Console.ReadLine()))
            {
                if (!input.Contains(EMPTY))
                {
                    string state = input.Split(',')[0];
                    string symbol = (input.Split(',')[1]).Split('-')[0];
                    string[] nextStates = (input.Split('>')[1]).Split(',');
                    if (_states.Contains(state) && _symbols.Contains(symbol))
                    {
                        _transitions.Add(new Transition(state, symbol), nextStates);
                    }
                }
            }
        }

        static string Test(string inputString)
        {
            StringBuilder output = new StringBuilder();
            _currentStates.Add(_initialState);
            string[] inputArray = inputString.Split(',');

            EpsilonClosure();
            output.Append(PrintCurrentStates());
            foreach (string input in inputArray)
            {
                if (_symbols.Contains(input))
                {
                    Clousure(input);
                    EpsilonClosure();
                }
                else
                {
                    _currentStates.Clear();
                }
                output.Append(DELIMITER.ToString() + PrintCurrentStates());
            }

            _currentStates.Clear();
            return output.ToString();
        }

        static void Clousure(string symbol)
        {
            SortedSet<string> newCurrentStates = new SortedSet<string>();
            GetStates(ref newCurrentStates, symbol);
            _currentStates = newCurrentStates;
        }

        static void EpsilonClosure()
        {
            SortedSet<string> newCurrentStates = new SortedSet<string>(_currentStates);
            GetStates(ref newCurrentStates, EPSILON.ToString()); 
            if (_currentStates.Count < newCurrentStates.Count)
            {
                _currentStates = newCurrentStates;
                EpsilonClosure();
            }
        }

        static void GetStates(ref SortedSet<string> newCurrentStates, string symbol)
        {
            foreach (string state in _currentStates)
            {
                Transition transition = new Transition(state, symbol);
                string[] newStates;
                if (_transitions.TryGetValue(transition, out newStates))
                {
                    foreach (string newState in newStates)
                    {
                        newCurrentStates.Add(newState);
                    }
                }
            }
        }

        static string PrintCurrentStates()
        {
            if (!_currentStates.Any())
            {
                return EMPTY.ToString();
            }
            StringBuilder print = new StringBuilder();
            foreach (string state in _currentStates)
            {
                print.Append(state + ",");
            }
            print.Length--;
            return print.ToString();
        }
    }
}
