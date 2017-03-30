using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA_ControlStructures
{
    class Program
    {
        static void Main(string[] args)
        {
            //Operators
            //Comparisons operators
            //<,>,==, >=, <=, !=
            //Assignment opperators
            //+, -, /, *, +=, ++, --, -=, /= ,*= %, ^,
            //Logical operators.
            // |, ||, & , &&, !
            var i = 10;
            if (i < 10)
            {
                Console.WriteLine("< " + i);
            } else if (i > 10)
            {
                Console.WriteLine("> " + i);
            } else
            {
                Console.WriteLine(i);
            }
            ConsoleColor cc = new ConsoleColor();
            switch (cc)
            {
                
            }
            Console.ReadLine();
        }
    }
}
