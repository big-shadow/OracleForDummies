using System;
using System.Collections.Generic;
using OFD;

namespace OFDTests
{
    partial class Program
    {
        static List<Test> tests = new List<Test>();

        static void Main(string[] args)
        {
            MakeTests();

            foreach (Test test in tests)
            {
                if (test.RunTest())
                {
                    Console.WriteLine("[Yes]   " + test.Name);
                }
                else
                {
                    Console.WriteLine("[No]    " + test.Name);
                }
            }

            Console.Read();
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

    public class Thing : Model
    {
        public string Name { get; set; }

        public Thing(int id = 0) : base(id)
        {

        }
    }
}
