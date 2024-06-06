namespace Teeny_Tiny
{
    public class Parser
    {
        public Parser(Lexer lexer)
        {
        }

        // Return true if the current token matches.
        public bool CheckToken(TokenType kind){}

        // Return true if the next token matches.
        public bool CheckPeek(TokenType kind){}

        // Try to match current token. If not, error. Advances the current token.
        public void Match(TokenType kind){}

        // Advances the current token.
        public void NextToken(){}

        public void Abort(string message)
        {
            Console.WriteLine($"Error. {message}");
            Environment.Exit(1);
        }
    }
}
