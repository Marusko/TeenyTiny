namespace Teeny_Tiny
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var source = "IF+-123 foo*THEN/";
            Lexer lexer = new Lexer(source);

            var token = lexer.GetToken();

            while (token.Kind != TokenType.EOF)
            {
                Console.WriteLine($"{token.Kind} :: {token.Text}");
                token = lexer.GetToken();
            }
        }
    }
}
