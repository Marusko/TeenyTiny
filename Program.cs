using System.Text;

namespace Teeny_Tiny
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Teeny Tiny Compiler");

            //if (args.Length != 2)
            //{
            //    Console.WriteLine("Error: Compiler needs source file as argument.");
            //    Environment.Exit(1);
            //}

            string source;
            using (StreamReader reader = new StreamReader("C:\\Users\\matus\\source\\repos\\Teeny Tiny progs\\average.teeny", Encoding.UTF8))
            {
                source = reader.ReadToEnd();
            }

            Lexer lexer = new Lexer(source);
            Emitter emitter = new Emitter("C:\\Users\\matus\\source\\repos\\Teeny Tiny progs\\average.c");
            Parser parser = new Parser(lexer, emitter);

            parser.Program();
            emitter.WriteFile();
            Console.WriteLine("Compiling completed.");
        }
    }
}
