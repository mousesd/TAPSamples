using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            //# Note that console applications don’t cause this deadlock.
            //# They have a thread pool SynchronizationContext instead of a one-chunk-at-a-time SynchronizationContext,
            //# so when the await completes, it schedules the remainder of the async method on a thread pool thread.
            var deadlock = new DeadlockSample();
            deadlock.RunDeadlock();

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
