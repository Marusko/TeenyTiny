using System.Transactions;

namespace Teeny_Tiny
{
    public class Lexer
    {
        public string Source { get; set; }
        public char CurrentChar { get; set; }
        public int CurrentPos { get; set; }

        public Lexer(string source)
        {
            Source = source + "\n";
            CurrentChar = ' ';
            CurrentPos = -1;
            NextChar();
        }

        // Process the next character.
        public void NextChar()
        {
            CurrentPos += 1;
            CurrentChar = CurrentPos >= Source.Length ? '\0' : Source[CurrentPos];
        }

        // Return the lookahead character.
        public char Peek()
        {
            var tmp = CurrentPos;
            return tmp++ >= Source.Length ? '\0' : Source[tmp];
        }

        // Invalid token found, print error message and exit.
        public void Abort(string message)
        {
            Console.WriteLine($"Lexing error. {message}");
            Environment.Exit(1);
        }

        // Skip whitespace except newlines, which we will use to indicate the end of a statement.
        public void SkipWhitespace()
        {
            while (CurrentChar == ' ' || CurrentChar == '\t' || CurrentChar == '\r')
            {
                NextChar();
            }
        }

        // Skip comments in the code.
        public void SkipComment()
        {
            if (CurrentChar == '#')
            {
                while (CurrentChar != '\n')
                {
                    NextChar();
                }
            }
        }

        // Return the next token.
        public Token GetToken()
        {
            SkipWhitespace();
            SkipComment();
            Token token = null;
            if (CurrentChar == '+')
            {
                token = new Token(CurrentChar.ToString(), TokenType.PLUS);
            } else if (CurrentChar == '-')
            {
                token = new Token(CurrentChar.ToString(), TokenType.MINUS);
            } else if (CurrentChar == '*')
            {
                token = new Token(CurrentChar.ToString(), TokenType.ASTERISK);
            }
            else if (CurrentChar == '/')
            {
                token = new Token(CurrentChar.ToString(), TokenType.SLASH);
            }
            else if (CurrentChar == '=')
            {
                if (Peek() == '=')
                {
                    var last = CurrentChar;
                    NextChar();
                    token = new Token(last.ToString() + CurrentChar, TokenType.EQEQ);
                }
                else
                {
                    token = new Token(CurrentChar.ToString(), TokenType.EQ);
                }
            }
            else if (CurrentChar == '>')
            {
                if (Peek() == '=')
                {
                    var last = CurrentChar;
                    NextChar();
                    token = new Token(last.ToString() + CurrentChar, TokenType.GTEQ);
                }
                else
                {
                    token = new Token(CurrentChar.ToString(), TokenType.GT);
                }
            }
            else if (CurrentChar == '<')
            {
                if (Peek() == '=')
                {
                    var last = CurrentChar;
                    NextChar();
                    token = new Token(last.ToString() + CurrentChar, TokenType.LTEQ);
                }
                else
                {
                    token = new Token(CurrentChar.ToString(), TokenType.LT);
                }
            }
            else if (CurrentChar == '!')
            {
                if (Peek() == '=')
                {
                    var last = CurrentChar;
                    NextChar();
                    token = new Token(last.ToString() + CurrentChar, TokenType.NOTEQ);
                }
                else
                {
                    var c = Peek();
                    Abort($"Expected !=, got !{c}");
                }
            }
            else if (CurrentChar == '\"')
            {
                NextChar();
                var start = CurrentPos;
                while (CurrentChar != '\"')
                {
                    if (CurrentChar == '\r' || CurrentChar == '\n'
                                            || CurrentChar == '\t' || CurrentChar == '\\'
                                            || CurrentChar == '%')
                    {
                        Abort("Illegal character in string.");
                    }
                    NextChar();
                }
                var tokText = Source.Substring(start, CurrentPos - start);
                token = new Token(tokText, TokenType.STRING);
            }
            else if (char.IsNumber(CurrentChar))
            {
                var start = CurrentPos;
                while (char.IsNumber(Peek()))
                {
                    NextChar();
                }
                if (Peek() == '.')
                {
                    NextChar();
                    if (!char.IsNumber(Peek()))
                    {
                        Abort("Illegal character in number.");
                    }
                    while (char.IsNumber(Peek()))
                    {
                        NextChar();
                    }
                }
                var tokText = Source.Substring(start, CurrentPos - start + 1);
                token = new Token(tokText, TokenType.NUMBER);
            }
            else if (char.IsLetter(CurrentChar))
            {
                var start = CurrentPos;
                while (char.IsLetterOrDigit(Peek()))
                {
                    NextChar();
                }
                var tokText = Source.Substring(start, CurrentPos - start + 1);
                var keyword = Token.CheckIfKeyword(tokText);
                if (keyword == null)
                {
                    token = new Token(tokText, TokenType.IDENT);
                } 
                else
                {
                    token = new Token(tokText, keyword);
                }
            }
            else if (CurrentChar == '\n')
            {
                token = new Token(CurrentChar.ToString(), TokenType.NEWLINE);
            }
            else if (CurrentChar == '\0')
            {
                token = new Token(' '.ToString(), TokenType.EOF);
            }
            else
            {
                Abort($"Unknown token: {CurrentChar}");
            }

            NextChar();
            return token;
        }
    }
}
