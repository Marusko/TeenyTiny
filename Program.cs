using System.Text;

namespace Teeny_Tiny
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Teeny Tiny Compiler");

            if (args.Length != 2)
            {
                Console.WriteLine("Error: Compiler needs source file as argument.");
                Environment.Exit(1);
            }

            string source;
            using (StreamReader reader = new StreamReader(args[1], Encoding.UTF8))
            {
                source = reader.ReadToEnd();
            }

            Lexer lexer = new Lexer(source);
            Parser parser = new Parser(lexer);

            parser.program();
            Console.WriteLine("Parsing completed.");
        }
    }
}
