using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World");
            Console.ReadKey();

            Console.WriteLine("What is your name?");
            var input = Console.ReadLine();
            int numberOfTimes = 1;
            Console.WriteLine("Number of times please");
            numberOfTimes = int.Parse(Console.ReadLine());
            for (int i = 0; i < numberOfTimes; i++)
            {
                Console.WriteLine("Hello {0}, Welcome!", input);
            }
            Console.WriteLine("Please press <ENTER> to exit.");
            Console.ReadKey();
        }
    }
}
