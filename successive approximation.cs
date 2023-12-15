using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Diagnostics;

namespace successive_approximation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            char exitKey = 'f';

            do
            {
                //vis "Skriv "Choose bit depth (8, 16, 24, or all): " i consol
                Console.WriteLine("Choose bit depth (8, 16, 24, or all): ");
                //læs input
                string input = Console.ReadLine().ToLower();

                int[] bitDepths;
                //hvis input er lig "all" set bitDepth til (8,16,24)
                if (input == "all")
                {
                    bitDepths = new int[] { 8, 16, 24 };
                }
                // ellers hvis input er lig en af de nævnte integers 8, 16 eller 24
                else if (int.TryParse(input, out int selectedDepth) && (selectedDepth == 8 || selectedDepth == 16 || selectedDepth == 24))
                {
                    //set bitDepth til {selectedDepth}
                    bitDepths = new int[] { selectedDepth };
                }
                //ellers skriv "Invalid input. Please enter 8, 16, 24, or all." i konsolen og start loop om igen
                else
                {
                    Console.WriteLine("Invalid input. Please enter 8, 16, 24, or all.");
                    continue;
                }
                //skriv i konsolen "Enter the analog input voltage (between 0 and 5): "
                Console.WriteLine("Enter the analog input voltage (between 0 and 5): ");
                //variablen er en double fordi inputet kan værre er decimal tal
                double analogInput;
                //mens analogInput ikke er en værdig mellem 0 og 5
                while (!double.TryParse(Console.ReadLine(), out analogInput) || analogInput < 0 || analogInput > 5)
                {
                    //skriv i konsolen "Invalid input. Please enter a valid numeric value between 0 and 5."
                    Console.WriteLine("Invalid input. Please enter a valid numeric value between 0 and 5.");
                    // skriv i konsolen "Enter the analog input voltage: "
                    Console.Write("Enter the analog input voltage: ");
                }
                //for hvert bitDepth i bitDepth
                foreach (int bitDepth in bitDepths)
                {
                    //skriv i konsolen "Running for Bit Depth: {bitDepth}"
                    Console.WriteLine($"Running for Bit Depth: {bitDepth}");
                    RunSuccessiveApproximation(analogInput, bitDepth);
                }

                Console.WriteLine("Press 'f' to exit or any other key to run again.");
            } while (Console.ReadKey().KeyChar != exitKey);
        }

        static void RunSuccessiveApproximation(double analogInput, int bitDepth)
        {
            double[] digitalOutput = new double[bitDepth];
            double referenceVoltage = 5; 
            double compareThreshold = 0;

            Console.WriteLine($"Running Successive Approximation for {bitDepth} bits:");

            Stopwatch stopwatch = new Stopwatch();

            // Successive approximation ADC konvertering
            for (int i = 0; i < bitDepth; i++)
            {
                stopwatch.Start();

                digitalOutput[i] = 1;
                compareThreshold = 0;

                // beregner compareThreshold
                for (int j = 0; j <= i; j++)
                {
                    compareThreshold += digitalOutput[j] * referenceVoltage / Math.Pow(2, j);
                }

                // samenlign analog input og comparThreshold og set bit til 1 eller 0
                if (analogInput - compareThreshold < 0)
                {
                    digitalOutput[i] = 0;
                }

                stopwatch.Stop();
                Console.WriteLine($"Operation {i + 1} took {stopwatch.ElapsedMilliseconds} ms");
                stopwatch.Reset();
            }

            // skriv endeligt digital output i konsolen
            Console.WriteLine($"Final Digital Output ({bitDepth} bits): " + string.Join(", ", digitalOutput));

           
            for (int i = 1; i < bitDepth; i++)
            {
                stopwatch.Start();
                double linearError = Math.Abs(analogInput - compareThreshold);
                stopwatch.Stop();
                Console.WriteLine($"  Linear Error for Bit {i + 1} took {stopwatch.ElapsedMilliseconds} ms: {linearError}");
                stopwatch.Reset();
            }

            
            stopwatch.Start();
            double closestValue = Math.Round(compareThreshold * (Math.Pow(2, bitDepth) - 1) / referenceVoltage) * referenceVoltage / (Math.Pow(2, bitDepth) - 1);
            stopwatch.Stop();
            Console.WriteLine($"Finding closest value took {stopwatch.ElapsedMilliseconds} ms");

           
            double distortion = Math.Abs(analogInput - closestValue);
            Console.WriteLine($"Distortion: {distortion}\n");
        }
    }
}

