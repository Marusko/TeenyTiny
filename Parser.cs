namespace Teeny_Tiny
{
    public class Parser
    {
        public Token CurToken { get; set; }
        public Token PeekToken { get; set; }
        public Lexer Lexer { get; set; }
        public Emitter Emitter { get; set; }

        public List<string> Symbols { get; set; } = new();
        public List<string> LabelsDeclared { get; set; } = new();
        public List<string> LabelsGotoed { get; set; } = new();

        public Parser(Lexer lexer, Emitter emitter)
        {
            Lexer = lexer;
            Emitter = emitter;
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
            Emitter.HeaderLine("#include <stdio.h>");
            Emitter.HeaderLine("int main(void){");
            while (CheckToken(TokenType.NEWLINE))
            {
                NextToken();
            }
            while (!CheckToken(TokenType.EOF))
            {
                Statement();
            }

            Emitter.EmitLine("return 0;");
            Emitter.EmitLine("}");

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
                NextToken();
                if (CheckToken(TokenType.STRING))
                {
                    Emitter.EmitLine($"printf(\"{CurToken.Text}\\n\");");
                    NextToken();
                }
                else
                {
                    Emitter.Emit("printf(\"%.2f\\n\", (float)(");
                    Expression();
                    Emitter.EmitLine("));");
                }
            }
            else if (CheckToken(TokenType.IF))
            {
                NextToken();
                Emitter.Emit("if(");
                Comparison();
                Match(TokenType.THEN);
                Nl();
                Emitter.EmitLine("){");
                while (!CheckToken(TokenType.ENDIF))
                {
                    Statement();
                }
                Match(TokenType.ENDIF);
                Emitter.EmitLine("}");
            }
            else if (CheckToken(TokenType.WHILE))
            {
                NextToken();
                Emitter.Emit("while(");
                Comparison();
                Match(TokenType.REPEAT);
                Nl();
                Emitter.EmitLine("){");
                while (!CheckToken(TokenType.ENDWHILE))
                {
                    Statement();
                }
                Match(TokenType.ENDWHILE);
                Emitter.EmitLine("}");
            }
            else if (CheckToken(TokenType.LABEL))
            {
                NextToken();
                if (LabelsDeclared.Contains(CurToken.Text))
                {
                    Abort($"Label already exists: {CurToken.Text}");
                }
                LabelsDeclared.Add(CurToken.Text);
                Emitter.EmitLine($"{CurToken.Text}:");
                Match(TokenType.IDENT);
            } 
            else if (CheckToken(TokenType.GOTO))
            {
                NextToken();
                LabelsGotoed.Add(CurToken.Text);
                Emitter.EmitLine($"goto {CurToken.Text};");
                Match(TokenType.IDENT);
            }
            else if (CheckToken(TokenType.LET))
            {
                NextToken();
                if (!Symbols.Contains(CurToken.Text))
                {
                    Symbols.Add(CurToken.Text);
                    Emitter.HeaderLine($"float {CurToken.Text};");
                }
                Emitter.Emit($"{CurToken.Text} = ");
                Match(TokenType.IDENT);
                Match(TokenType.EQ);
                Expression();
                Emitter.EmitLine(";");
            }
            else if (CheckToken(TokenType.INPUT))
            {
                NextToken();
                if (!Symbols.Contains(CurToken.Text))
                {
                    Symbols.Add(CurToken.Text);
                    Emitter.HeaderLine($"float {CurToken.Text};");
                }
                Emitter.EmitLine($"if(0 == scanf(\"%f\", &{CurToken.Text})) {{");
                Emitter.EmitLine($"{CurToken.Text} = 0;");
                Emitter.Emit("scanf(\"%");
                Emitter.EmitLine("*s\");");
                Emitter.EmitLine("}");
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
            Expression();
            if (IsComparisonOperator())
            {
                Emitter.Emit(CurToken.Text);
                NextToken();
                Expression();
            }
            else
            {
                Abort($"Expected comparison operator at: {CurToken.Text}");
            }

            while (IsComparisonOperator())
            {
                Emitter.Emit(CurToken.Text);
                NextToken();
                Expression();
            }
        }

        public void Expression()
        {
            Term();
            while (CheckToken(TokenType.PLUS) || CheckToken(TokenType.MINUS))
            {
                Emitter.Emit(CurToken.Text);
                NextToken();
                Term();
            }
        }

        public void Term()
        {
            Unary();
            while (CheckToken(TokenType.ASTERISK) || CheckToken(TokenType.SLASH))
            {
                Emitter.Emit(CurToken.Text);
                NextToken();
                Unary();
            }
        }

        public void Unary()
        {
            if (CheckToken(TokenType.PLUS) || CheckToken(TokenType.MINUS))
            {
                Emitter.Emit(CurToken.Text);
                NextToken();
            }

            Primary();
        }

        public void Primary()
        {
            if (CheckToken(TokenType.NUMBER))
            {
                Emitter.Emit(CurToken.Text);
                NextToken();
            }
            else if (CheckToken(TokenType.IDENT))
            {
                if (!Symbols.Contains(CurToken.Text))
                {
                    Abort($"Referencing variable before assignment: {CurToken.Text}");
                }
                Emitter.Emit(CurToken.Text);
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
            Match(TokenType.NEWLINE);
            while (CheckToken(TokenType.NEWLINE))
            {
                NextToken();
            }
        }
    }
}
