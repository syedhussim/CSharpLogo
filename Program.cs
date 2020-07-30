using System;
using System.IO;
using Logo;

namespace CSharpLogo
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length < 2){
                ConsoleColor consoleColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Missing 2 arguments.");
                Console.ForegroundColor = consoleColor;
                Console.WriteLine("Arg1 - Source file path");
                Console.WriteLine("Arg2 - Output file path");
            }else{
                try{
                    using(StreamReader reader = new StreamReader(args[0])){

                        string source = reader.ReadToEnd();

                        Tokens Tokens = Lexer.Parse(source);

                        Compiler compiler = new Compiler(Tokens);

                        try{

                            string result = compiler.Execute();

                            using(StreamWriter writer = new StreamWriter(args[1])){
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
                }catch(Exception e){
                    ConsoleColor consoleColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.ForegroundColor = consoleColor;
                }
            }
        }
    }
}
