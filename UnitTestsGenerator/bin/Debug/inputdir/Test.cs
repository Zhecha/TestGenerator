using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace UnitTestsGenerator.inputdir
{
    class Test1
    {
        public void Input(int arg)
        {
            int sum = 5 + arg;
            Console.WriteLine(sum.ToString());
            Console.ReadKey();
        }
    }
}

namespace Testclass1
{
    class Test2
    {
        public void Input1(string arg)
        {
            string sum = arg + "lala";
            Console.WriteLine(sum);
            Console.ReadKey();
        }
    }
}
