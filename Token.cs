namespace Teeny_Tiny
{
    public class Token
    {
        public string Text { get; set; }
        public TokenType? Kind { get; set; }
        public Token(string tokenText, TokenType? tokenType)
        {
            Text = tokenText;
            Kind = tokenType;
        }

        public static TokenType? CheckIfKeyword(string tokText)
        {
            foreach (var kind in Enum.GetValues(typeof(TokenType)).Cast<TokenType>())
            {
                if (kind.ToString().Equals(tokText) && (int)kind >= 100 && (int)kind < 200)
                {
                    return kind;
                }
            }
            return null;
        }
    }

    public enum TokenType
    {
        EOF = -1,
        NEWLINE = 0,
        NUMBER = 1,
        IDENT = 2,
        STRING = 3,
        // Keywords
        LABEL = 101,
        GOTO = 102,
        PRINT = 103,
        INPUT = 104,
        LET = 105,
        IF = 106,
        THEN = 107,
        ENDIF = 108,
        WHILE = 109,
        REPEAT = 110,
        ENDWHILE = 111,
        // Operators
        EQ = 201,
        PLUS = 202,
        MINUS = 203,
        ASTERISK = 204,
        SLASH = 205,
        EQEQ = 206,
        NOTEQ = 207,
        LT = 208,
        LTEQ = 209,
        GT = 210,
        GTEQ = 211
    }
}
