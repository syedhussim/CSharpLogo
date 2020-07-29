using System;
using System.IO;
using Logo;

namespace CSharpLogo
{
    class Program
    {
        static void Main(string[] args)
        {

            using(StreamReader reader = new StreamReader("/home/syed/Desktop/samples/sample1")){

                string source = reader.ReadToEnd();

                Tokens Tokens = Lexer.Parse(source);

                Compiler compiler = new Compiler(Tokens);

                try{

                    string result = compiler.Execute();

                    using(StreamWriter writer = new StreamWriter("/home/syed/Desktop/logo.html")){
                        writer.Write(result);
                        writer.Flush();
                        Console.WriteLine("Compiled.");
                    }

                }catch(SyntaxError se){
                    ConsoleColor consoleColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(string.Format("{0}: {1}", se.GetType().ToString(),  se.Message));
                    Console.WriteLine("");
                    Console.WriteLine("Build failed. Please fix the errors and run again.");
                    Console.ForegroundColor = consoleColor;
                }
            };
        }
    }
}
