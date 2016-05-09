using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FER.UTR.Lab3
{
    struct TransitionDomain
    {
        string state;
        string inputSymbol;
        string stackSymbol;

        internal TransitionDomain(string stt, string input, string stack)
        {
            state = stt;
            inputSymbol = input;
            stackSymbol = stack;
        }
    }

    struct TransitionCodomain
    {
        internal string State;
        internal List<string> StackSymbols;

        internal TransitionCodomain(string stt, string symbols)
        {
            State = stt;
            StackSymbols = new List<string>();
            foreach (char c in symbols)
            {
                StackSymbols.Add(c.ToString()); 
            }
        }
    }

    class DPDA
    {
        const char EPSILON = '$';
        const char DELIMITER = '|';
        const char STACK_TOP = '#';
        const char ACCEPTED = '1';
        const char NOT_ACCEPTED = '0'; 
        const string TRANSITION_FAIL = "fail";

        static string[] _inputArrays;
        static string[] _states;
        static string[] _inputSymbols;
        static string[] _stackSymbols;
        static string[] _finalStates;
        static string _initialState;
        static string _initialStackSymbol;
        static Dictionary<TransitionDomain, TransitionCodomain> _transitions = new Dictionary<TransitionDomain, TransitionCodomain>();

        static bool _accepted;
        static string _currentState;
        static Stack<string> _stack = new Stack<string>();

        static void Main()
        {
            Initialize();
            foreach (string input in _inputArrays)
            {
                Console.WriteLine(Test(input.Split(',')));
            }
        }

        static void Initialize()
        {
            _inputArrays = Console.ReadLine().Split(DELIMITER);
            _states = Console.ReadLine().Split(',');
            _inputSymbols = Console.ReadLine().Split(',');
            _stackSymbols = Console.ReadLine().Split(',');
            _finalStates = Console.ReadLine().Split(',');
            _initialState = Console.ReadLine();
            _initialStackSymbol = Console.ReadLine();
            string input;
            while (!string.IsNullOrWhiteSpace(input = Console.ReadLine()))
            {
                string state = input.Split(',')[0];
                string inputSymbol = input.Split(',')[1];
                string stackSymbol = (input.Split(',')[2]).Split('-')[0];
                string[] codomain = (input.Split('>')[1]).Split(',');
                _transitions.Add(new TransitionDomain(state, inputSymbol, stackSymbol), new TransitionCodomain(codomain[0], codomain[1]));
            }
        }

        static string Test(string[] input)
        {
            StringBuilder output = new StringBuilder();
            _accepted = true;
            _currentState = _initialState;
            _stack.Push(_initialStackSymbol);
            output.Append(PrintStatus());

            foreach (string c in input)
            {
                if (!CheckSymbol(c, ref output))
                {
                    break;
                }
            }
            if (_accepted)
            {
                while ((_stack.Count > 0) && !(_finalStates.Contains(_currentState)))
                {
                    if(Transition(EPSILON.ToString()))
                    {
                        output.Append(PrintStatus());
                    }
                    else
                    {
                        break;
                    }
                }
                if (_stack.Count > 0)
                {
                    if (!(_finalStates.Contains(_currentState)))
                    {
                        _accepted = false;
                    }
                }
            }
            output.Append(_accepted ? ACCEPTED : NOT_ACCEPTED);

            _stack.Clear();
            return output.ToString();
        }

        static bool CheckSymbol(string symbol, ref StringBuilder output)
        {
            if (Transition(symbol.ToString()))
            {
                output.Append(PrintStatus());
                return true;
            }
            else if (Transition(EPSILON.ToString()))
            {
                output.Append(PrintStatus());
                return CheckSymbol(symbol, ref output);
            }
            else
            {
                output.Append(TRANSITION_FAIL + DELIMITER);
                _accepted = false;
                return false;
            }
        }

        static string PrintStatus()
        {
            if (_stack.Count > 0)
            {
                return _currentState + STACK_TOP + PrintStack() + DELIMITER;
            }
            return _currentState + STACK_TOP + EPSILON + DELIMITER;
        }

        static bool Transition(string symbol)
        {
            if (_stack.Count == 0)
            {
                return false;
            }
            string stackTop = _stack.Pop();
            TransitionCodomain codomain;
            if (_transitions.TryGetValue(new TransitionDomain(_currentState, symbol, stackTop), out codomain))
            {
                _currentState = codomain.State;
                if (!codomain.StackSymbols.First().Equals(EPSILON.ToString()))
                {
                    foreach (string item in codomain.StackSymbols.Reverse<string>())
                    {
                        _stack.Push(item);
                    }
                }
                return true;
            }
            _stack.Push(stackTop);
            return false;
        }

        static string PrintStack()
        {
            StringBuilder stackBuilder = new StringBuilder();
            foreach (string item in _stack)
            {
                stackBuilder.Append(item);
            }
            return stackBuilder.ToString();
        }
    }
}
