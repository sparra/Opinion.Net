using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkerRoleCommandProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            // Cleanup default EF DB initializers.
            DatabaseSetup.Initialize();

            using (var processor = new OpinionProcessor(false))
            {
                processor.Start();

                Console.WriteLine("Host started");
                Console.WriteLine("Press enter to finish");
                Console.ReadLine();

                processor.Stop();
            }
        }
    }
}
