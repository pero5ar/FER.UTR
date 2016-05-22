using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FER.UTR.Lab4
{
    /// <summary>
    /// LL1 parser grammar:
    ///     S -> aAB | bBA
    ///     A -> bC | a
    ///     B -> ccSbc | $
    ///     C -> AA
    /// </summary>
    class LL1ParserExample
    {
        static InputCharacter _inputCharacter;
        static StringBuilder _printBuffer = new StringBuilder();

        static void Main()
        {
            _inputCharacter = new InputCharacter(Console.ReadLine());
            try
            {
                _printBuffer.Append("S");
                S(_inputCharacter.Get());
                End(_inputCharacter.HasFinished());
            }
            catch (IndexOutOfRangeException)
            {
                End(false);
            }
        }

        static void S(char c)
        {
            switch(c)
            {
                case 'a':
                    {
                        _printBuffer.Append("A");
                        A(_inputCharacter.Next());
                        _printBuffer.Append("B");
                        B(_inputCharacter.HandledNext());
                        break;
                    }
                case 'b':
                    {
                        _printBuffer.Append("B");
                        B(_inputCharacter.HandledNext());
                        _printBuffer.Append("A");
                        A(_inputCharacter.Next());
                        break;
                    }
                default:
                    {
                        End(false);
                        break;
                    }
            }
        }

        static void A(char c)
        {
            switch (c)
            {
                case 'b':
                    {
                        _printBuffer.Append("C");
                        C(_inputCharacter.Get());
                        break;
                    }
                case 'a':
                    {
                        break;
                    }
                default:
                    {
                        End(false);
                        break;
                    }
            }
        }

        static void B(char c)
        {
            switch (c)
            {
                case InputCharacter.END:
                    {
                        End(true);
                        break;
                    }
                case 'c':
                    {
                        if(_inputCharacter.Next() != 'c')
                        {
                            End(false);
                        }
                        _printBuffer.Append("S");
                        S(_inputCharacter.Next());
                        if(_inputCharacter.Next() != 'b')
                        {
                            End(false);
                        }
                        if(_inputCharacter.Next() != 'c')
                        {
                            End(false);
                        }
                        break;
                    }
                default:
                    {
                        _inputCharacter.Back();
                        break;
                    }
            }
        }

        static void C(char c)
        {
            _printBuffer.Append("A");
            A(_inputCharacter.Next());
            _printBuffer.Append("A");
            A(_inputCharacter.Next());
        }

        static void End(bool accepted)
        {
            Console.WriteLine(_printBuffer);
            if (accepted)
            {
                Console.WriteLine("DA");
            }
            else
            {
                Console.WriteLine("NE");
            }
            Environment.Exit(0);
        }
    }

    class InputCharacter
    {
        internal const char END = '$';

        string _input;
        int _inputPosition;

        internal InputCharacter(string input)
        {
            _input = input;
        }

        internal char Get()
        {
            return _input[_inputPosition];
        }

        internal char Next()
        {
            return _input[++_inputPosition];
        }

        internal void Back()
        {
            _inputPosition--;
        }

        internal char HandledNext()
        {
            try
            {
                return Next();
            }
            catch (IndexOutOfRangeException)
            {
                return END;
            }
        }

        internal bool HasFinished()
        {
            return _input.Count() == (_inputPosition + 1);
        }
    }
}
