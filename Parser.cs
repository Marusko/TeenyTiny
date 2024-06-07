using System.Runtime.InteropServices;

namespace Teeny_Tiny
{
    public class Parser
    {
        public Token CurToken { get; set; }
        public Token PeekToken { get; set; }
        public Lexer Lexer { get; set; }

        public List<string> Symbols { get; set; } = new();
        public List<string> LabelsDeclared { get; set; } = new();
        public List<string> LabelsGotoed { get; set; } = new();

        public Parser(Lexer lexer)
        {
            Lexer = lexer;
            NextToken();
            NextToken();
        }

        // Return true if the current token matches.
        public bool CheckToken(TokenType kind)
        {
            return kind == CurToken.Kind;
        }

        // Return true if the next token matches.
        public bool CheckPeek(TokenType kind)
        {
            return kind == PeekToken.Kind;
        }

        // Try to match current token. If not, error. Advances the current token.
        public void Match(TokenType kind)
        {
            if (!CheckToken(kind))
            {
                Abort($"Expected {kind}, got {CurToken.Kind}");
            }
            NextToken();
        }

        // Advances the current token.
        public void NextToken()
        {
            CurToken = PeekToken;
            PeekToken = Lexer.GetToken();
        }

        public void Abort(string message)
        {
            Console.WriteLine($"Error. {message}");
            Environment.Exit(1);
        }

        public void Program()
        {
            Console.WriteLine("PROGRAM");
            while (CheckToken(TokenType.NEWLINE))
            {
                NextToken();
            }
            while (!CheckToken(TokenType.EOF))
            {
                Statement();
            }

            foreach (var label in LabelsGotoed)
            {
                if (!LabelsDeclared.Contains(label))
                {
                    Abort($"Attempting to GOTO to undeclared label: {label}");
                }
            }
        }

        public void Statement()
        {
            if (CheckToken(TokenType.PRINT))
            {
                Console.WriteLine("STATEMENT-PRINT");
                NextToken();
                if (CheckToken(TokenType.STRING))
                {
                    NextToken();
                }
                else
                {
                    Expression();
                }
            }
            else if (CheckToken(TokenType.IF))
            {
                Console.WriteLine("STATEMENT-IF");
                NextToken();
                Comparison();
                Match(TokenType.THEN);
                Nl();
                while (!CheckToken(TokenType.ENDIF))
                {
                    Statement();
                }
                Match(TokenType.ENDIF);
            }
            else if (CheckToken(TokenType.WHILE))
            {
                Console.WriteLine("STATEMENT-WHILE");
                NextToken();
                Comparison();
                Match(TokenType.REPEAT);
                Nl();
                while (!CheckToken(TokenType.ENDWHILE))
                {
                    Statement();
                }
                Match(TokenType.ENDWHILE);
            }
            else if (CheckToken(TokenType.LABEL))
            {
                Console.WriteLine("STATEMENT-LABEL");
                NextToken();
                if (LabelsDeclared.Contains(CurToken.Text))
                {
                    Abort($"Label already exists: {CurToken.Text}");
                }
                LabelsDeclared.Add(CurToken.Text);
                Match(TokenType.IDENT);
            } 
            else if (CheckToken(TokenType.GOTO))
            {
                Console.WriteLine("STATEMENT-GOTO");
                NextToken();
                LabelsGotoed.Add(CurToken.Text);
                Match(TokenType.IDENT);
            }
            else if (CheckToken(TokenType.LET))
            {
                Console.WriteLine("STATEMENT-LET");
                NextToken();
                if (!Symbols.Contains(CurToken.Text))
                {
                    Symbols.Add(CurToken.Text);
                }
                Match(TokenType.IDENT);
                Match(TokenType.EQ);
                Expression();
            }
            else if (CheckToken(TokenType.INPUT))
            {
                Console.WriteLine("STATEMENT-INPUT");
                NextToken();
                if (!Symbols.Contains(CurToken.Text))
                {
                    Symbols.Add(CurToken.Text);
                }
                Match(TokenType.IDENT);
            }
            else
            {
                Abort($"Invalid statement at {CurToken.Text} ({CurToken.Kind})");
            }

            Nl();
        }

        public void Comparison()
        {
            Console.WriteLine("COMPARISON");
            Expression();
            if (IsComparisonOperator())
            {
                NextToken();
                Expression();
            }
            else
            {
                Abort($"Expected comparison operator at: {CurToken.Text}");
            }

            while (IsComparisonOperator())
            {
                NextToken();
                Expression();
            }
        }

        public void Expression()
        {
            Console.WriteLine("EXPRESSION");
            Term();
            while (CheckToken(TokenType.PLUS) || CheckToken(TokenType.MINUS))
            {
                NextToken();
                Term();
            }
        }

        public void Term()
        {
            Console.WriteLine("TERM");
            Unary();
            while (CheckToken(TokenType.ASTERISK) || CheckToken(TokenType.SLASH))
            {
                NextToken();
                Unary();
            }
        }

        public void Unary()
        {
            Console.WriteLine("UNARY");
            if (CheckToken(TokenType.PLUS) || CheckToken(TokenType.MINUS))
            {
                NextToken();
            }

            Primary();
        }

        public void Primary()
        {
            Console.WriteLine($"PRIMARY ({CurToken.Text})");
            if (CheckToken(TokenType.NUMBER))
            {
                NextToken();
            }
            else if (CheckToken(TokenType.IDENT))
            {
                if (!Symbols.Contains(CurToken.Text))
                {
                    Abort($"Referencing variable before assignment: {CurToken.Text}");
                }
                NextToken();    
            }
            else
            {
                Abort($"Unexpected token at {CurToken.Text}");
            }
        }

        public bool IsComparisonOperator()
        {
            return CheckToken(TokenType.GT)
                   || CheckToken(TokenType.GTEQ)
                   || CheckToken(TokenType.LT)
                   || CheckToken(TokenType.LTEQ)
                   || CheckToken(TokenType.EQEQ)
                   || CheckToken(TokenType.NOTEQ);
        }

        public void Nl()
        {
            Console.WriteLine("NEWLINE");
            Match(TokenType.NEWLINE);
            while (CheckToken(TokenType.NEWLINE))
            {
                NextToken();
            }
        }
    }
}
