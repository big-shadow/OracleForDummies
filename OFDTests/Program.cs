using System;
using System.Collections.Generic;

namespace OFDTests
{
    partial class Program
    {
        static List<Test> tests = new List<Test>();

        static void Main(string[] args)
        {
            MakeTests();

            string result = string.Empty;

            foreach (Test test in tests)
            {
                Timer timer = new Timer();
                timer.Start();

                if (test.RunTest())
                {
                    result = "[Yes]   ";
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    result = "[No]    ";
                    Console.ForegroundColor = ConsoleColor.Red;
                }

                timer.Stop();

                Console.Write(Environment.NewLine + result);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(test.Name);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("        " + timer.Duration + " Second(s)");
            }

            Console.Read();
            Environment.Exit(0);
        }
    }

    public class Test
    {
        public delegate bool Run();
        public Run RunTest;
        public string Name;

        public Test(string name, Run func)
        {
            this.RunTest = func;
            this.Name = name;
        }
    }
}
