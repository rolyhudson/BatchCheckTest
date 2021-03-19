using BH.oM.Data.Library;
using BH.oM.Test.Results;
using System;
using System.IO;
using serialiser = BH.Engine.Serialiser;
using BH.Engine.UnitTest;
using System.Collections.Generic;

namespace BatchCheckTest
{
    class Program
    {
        static int Passes = 0;
        static int Tests = 0;
        static List<string> FailingDataset;
        static void Main(string[] args)
        {
            
            if(args.Length ==0 || !File.Exists(args[0]))
            {
                Console.WriteLine("Please provide the path to a text file with the folders to search for unit test datasets.");
                return;
            }

            FailingDataset = new List<string>();
            BH.Engine.Reflection.Compute.LoadAllAssemblies();

            using (StreamReader sr = new StreamReader(args[0]))
            {
                string path = sr.ReadLine();
                while (path != null)
                {
                    if (Directory.Exists(path))
                    {
                        Console.WriteLine($"Testing Datasets in {path}");
                        foreach (string f in Directory.GetFiles(path, "*.json*", SearchOption.AllDirectories))
                            RunTest(f);
                    }
                    path = sr.ReadLine();
                }
            }

            Console.WriteLine($"Passes: {Passes} of {Tests} Tests.");

            foreach(string file in FailingDataset)
                Console.WriteLine($"{file} did not pass.");

            Console.WriteLine("Press enter to end.");
            Console.ReadLine();
        }

        static void RunTest(string path)
        {
            using(StreamReader sr = new StreamReader(path))
            {
                string line = sr.ReadLine();
                while(line != null)
                {
                    var custom = serialiser.Convert.FromJson(line);
                    if(custom is Dataset)
                    {
                        Tests++;
                        Dataset dataset = custom as Dataset;
                        TestResult testResult = dataset.CheckTests();
                        if (testResult.Status == BH.oM.Test.TestStatus.Pass)
                            Passes++;
                        else
                            FailingDataset.Add(path);
                    }
                    line = sr.ReadLine();
                }
            }
        }
    }
}
