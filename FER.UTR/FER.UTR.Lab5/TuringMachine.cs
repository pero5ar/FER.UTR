using System;
using System.Collections.Generic;
using System.Linq;

namespace FER.UTR.Lab5
{
    struct TransitionDomain
    {
        string state;
        char tapeSymbol;

        internal TransitionDomain(string stt, char sym)
        {
            state = stt;
            tapeSymbol = sym;
        }
    }

    struct TransitionCodomain
    {
        internal string State;
        internal char TapeSymbol;
        internal int Move;

        internal TransitionCodomain(string stt, char sym, int mv)
        {
            State = stt;
            TapeSymbol = sym;
            Move = mv;
        }
    }

    class TuringMachine
    {
        const int RIGHT = 1;
        const int LEFT = -1;
        const char DELIMITER = '|';
        const char ACCEPTED = '1';
        const char NOT_ACCEPTED = '0';

        static string[] _states;
        static string[] _inputSymbols;
        static string[] _tapeSymbols;
        static string _blankSymbol;
        static char[] _tape;
        static string[] _finalStates;
        static string _initialState;
        static int _headPosition;
        static Dictionary<TransitionDomain, TransitionCodomain> _transitions = new Dictionary<TransitionDomain, TransitionCodomain>();

        static string _currentState;

        static void Main(string[] args)
        {
            Initialize();
            try
            {
                Simulate();
            }
            catch (IndexOutOfRangeException)
            {
                End(false);
            }
        }

        static void Initialize()
        {
            _states = Console.ReadLine().Split(',');
            _inputSymbols = Console.ReadLine().Split(',');
            _tapeSymbols = Console.ReadLine().Split(',');
            _blankSymbol = Console.ReadLine();
            _tape = Console.ReadLine().Trim().ToCharArray();
            _finalStates = Console.ReadLine().Split(',');
            _currentState = _initialState = Console.ReadLine();
            _headPosition = int.Parse(Console.ReadLine());
            string input;
            while (!string.IsNullOrWhiteSpace(input = Console.ReadLine()))
            {
                string state = input.Split(',')[0];
                char tapeSymbol = input.Split(',')[1][0];
                string[] codomain = (input.Split('>')[1]).Split(',');
                int move = codomain[2].Trim().Equals("R") ? RIGHT : LEFT;
                char newTapeSymbol = codomain[1][0];
                _transitions.Add(new TransitionDomain(state, tapeSymbol), new TransitionCodomain(codomain[0], newTapeSymbol, move));
            }
        }

        static void Simulate()
        {
            TransitionCodomain codomain;
            while (_transitions.TryGetValue(new TransitionDomain(_currentState, _tape[_headPosition]), out codomain))
            {
                int newHeadPosition = _headPosition + codomain.Move;
                if (newHeadPosition < 0 || newHeadPosition > _tape.Count()-1)
                {
                    throw new IndexOutOfRangeException();
                }
                _currentState = codomain.State;
                _tape[_headPosition] = codomain.TapeSymbol;
                _headPosition = newHeadPosition;
            }
            End(_finalStates.Contains(_currentState));
        }

        static void End(bool accepted)
        {
            Console.WriteLine(_currentState + DELIMITER + _headPosition + DELIMITER + new string(_tape) + DELIMITER + (accepted ? ACCEPTED : NOT_ACCEPTED));
            Environment.Exit(0);
        }
    }
}
