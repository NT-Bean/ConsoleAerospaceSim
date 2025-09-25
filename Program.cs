using System;
using System.Threading;
using System.IO;
using System.Timers;
using System.Diagnostics;
using System.Numerics;

// ALTITUDE REVERTS TO APOAPSIS WHEN REACHES PERIAPSIS
public class OrbitalSim
{
    public static void Main(string[] args)
    {
        double periapsis = 1000.0; // 1,000,000 meters
        double apoapsis = 2000.0; // 2,000,000 meters
        double semiMajorAxis = 7871.0; // 7,871,000 meters
        double eccentricity = 0.3333;
        
        double orbitalAltitude = 1000000; // 1,000,000 meters
        double velocity = 7583.562; // 7583.562 meters per second
        double orbitalPeriod = 6950; // 6,950 seconds
        double timeToNextApsis = 3475; //3,475 seconds

        double G = 6.6743 * Math.Pow(10, -11);
        double bodyMass = 5972000000 * Math.Pow(10, 15); // 5.972 x 10^24 kilograms
        double bodyDiameter = 12742000; // 12,742,000 meters


        // math exclusives
        double eccAlt;
        double stopwatchAngle = 0;
        double realisticAngle;
        int timeAcceleration = 1;

        // other
        string whichApsis = "Apoapsis";
        string? readResult;
        double stopwatchValue;

        Console.Clear();
        Console.WriteLine("Welcome to your very own in-console Keplerian orbit simulator!\n");
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        Console.ForegroundColor = ConsoleColor.White;

        Console.WriteLine(" Orbital Information:");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(
                          $"\n\t\t   _.----._        Semi-Major Axis: {semiMajorAxis.ToString("N1")} meters" +
                          $"\n\t\t /ˆ        ˆ\\      Orbital Eccentricity: {eccentricity.ToString("N4")}" +
                           "\n\t\t|    ,-,     |" +
                 $"\n Periapsis:\t|   |   |    |     Apoapsis:" +
$"\n {periapsis.ToString("N1")} \t|    '-'     |     {apoapsis.ToString("N1")}" +
                      $"\n km\t\t \\,        ,/      km" +
                           "\n\t\t   ''----''");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n Altitude: {orbitalAltitude.ToString("N3")}\n Orbital Velocity: {velocity.ToString("N3")}\n Orbital Period: {orbitalPeriod.ToString("N0")}\n Time to {whichApsis}: {timeToNextApsis.ToString("N0")} seconds");
        Console.ForegroundColor = ConsoleColor.White;

        Console.WriteLine("\n>type 'u' to update the simultion\n>type 'w' to change the speed of time\n>type 'a' to auto-update (WARNING: CANNOT BE UNDONE (for now), MUST BE CLOSED WITH CTRL+C\n>type 'b' to make a burn at apoapsis or periapsis\n\n>type 's' to save orbital statistics\n>type 'l' to load orbital statictics\n>type 'q' to quit\n\n More features to be added\n");

    CharacterAccept:
        readResult = Console.ReadLine();
        if (readResult != null)
        {
            readResult = readResult.ToLower();

            if (readResult == "u")
            {
                // OrbitalCalcs start
                periapsis *= 1000;
                apoapsis *= 1000;
                semiMajorAxis *= 1000;

                semiMajorAxis = (apoapsis + periapsis + bodyDiameter) / 2;
                eccentricity = 1 - 2 / ((apoapsis / periapsis) + 1);

                stopwatch.Stop();

                stopwatchValue = (stopwatch.Elapsed.TotalMilliseconds / 1000);
                stopwatchAngle = (((2 * double.Pi) / orbitalPeriod) * stopwatchValue * timeAcceleration) + stopwatchAngle;

                while (stopwatchAngle > 2 * double.Pi)
                {
                    stopwatchAngle = 0 + (stopwatchAngle - 2 * double.Pi);
                }
                stopwatch.Reset();
                eccAlt = (1 - (2 / (((2 * semiMajorAxis - bodyDiameter / 2 - periapsis) / (bodyDiameter / 2 + periapsis)) + 1)));
                realisticAngle = 2 * (Math.Atan(Math.Sqrt(Math.Abs((1 + eccAlt) / (1 - eccAlt))) * (Math.Tan(stopwatchAngle / 2))));

                orbitalAltitude = ((semiMajorAxis * (1 - (eccAlt * eccAlt))) / (1 + eccAlt * Math.Cos(realisticAngle))) - (bodyDiameter / 2);

                velocity = Math.Sqrt(G * bodyMass * ((2 / (orbitalAltitude + (bodyDiameter / 2))) - (1 / semiMajorAxis)));
                timeToNextApsis -= stopwatchValue * timeAcceleration;

                if (timeToNextApsis < 0)
                {
                    timeToNextApsis = (orbitalPeriod / 2) + timeToNextApsis;
                    if (whichApsis == "Apoapsis")
                        whichApsis = "Periapsis";
                    else if (whichApsis == "Periapsis")
                        whichApsis = "Apoapsis";
                }

                apoapsis /= 1000;
                periapsis /= 1000;
                semiMajorAxis /= 1000;
                stopwatch.Start();
                // OrbitalCalcs end

                Console.Clear();

                Console.WriteLine(" Orbital Information:");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(
                                  $"\n\t\t   _.----._        Semi-Major Axis: {semiMajorAxis.ToString("N1")} meters" +
                                  $"\n\t\t /ˆ        ˆ\\      Orbital Eccentricity: {eccentricity.ToString("N4")}" +
                                   "\n\t\t|    ,-,     |" +
                         $"\n Periapsis:\t|   |   |    |     Apoapsis:" +
        $"\n {periapsis.ToString("N1")} \t|    '-'     |     {apoapsis.ToString("N1")}" +
                              $"\n km\t\t \\,        ,/      km" +
                                   "\n\t\t   ''----''");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n Altitude: {orbitalAltitude.ToString("N3")} meters\n Orbital Velocity: {velocity.ToString("N3")} meters per second\n Orbital Period: {orbitalPeriod.ToString("N0")} seconds\n Time to {whichApsis}: {timeToNextApsis.ToString("N0")} seconds\n\nDiagnostics:\nRealistic Angle: {realisticAngle}\nStopwatch Angle: {stopwatchAngle}\nStopwatch Value: {stopwatchAngle}");
                Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine("\n>type 'u' to update the simultion\n>type 'w' to change the speed of time\n>type 'a' to auto-update (WARNING: CANNOT BE UNDONE (for now), MUST BE CLOSED WITH CTRL+C\n>type 'b' to make a burn at apoapsis or periapsis\n\n>type 's' to save orbital statistics\n>type 'l' to load orbital statictics\n>type 'q' to quit\n\n More features to be added\n");
                goto CharacterAccept;
            }

            else if (readResult == "a")
            {
                do
                {
                    // OrbitalCalcs start
                    periapsis *= 1000;
                    apoapsis *= 1000;
                    semiMajorAxis *= 1000;

                    semiMajorAxis = (apoapsis + periapsis + bodyDiameter) / 2;
                    eccentricity = 1 - 2 / ((apoapsis / periapsis) + 1);

                    stopwatch.Stop();

                    stopwatchValue = (stopwatch.Elapsed.TotalMilliseconds / 1000);
                    stopwatchAngle = (((2 * double.Pi) / orbitalPeriod) * stopwatchValue * timeAcceleration) + stopwatchAngle;

                    while (stopwatchAngle > 2 * double.Pi)
                    {
                        stopwatchAngle = 0 + (stopwatchAngle - 2 * double.Pi);
                    }
                    stopwatch.Reset();
                    eccAlt = (1 - (2 / (((2 * semiMajorAxis - bodyDiameter / 2 - periapsis) / (bodyDiameter / 2 + periapsis)) + 1)));
                    realisticAngle = 2 * (Math.Atan(Math.Sqrt(Math.Abs((1 + eccAlt) / (1 - eccAlt))) * (Math.Tan(stopwatchAngle / 2))));

                    if (realisticAngle > double.Pi)
                    {
                        realisticAngle = (-1 * double.Pi) + (realisticAngle - double.Pi);
                    }

                    orbitalAltitude = ((semiMajorAxis * (1 - (eccAlt * eccAlt))) / (1 + eccAlt * Math.Cos(realisticAngle))) - (bodyDiameter / 2);

                    velocity = Math.Sqrt(G * bodyMass * ((2 / (orbitalAltitude + (bodyDiameter / 2))) - (1 / semiMajorAxis)));
                    timeToNextApsis -= stopwatchValue * timeAcceleration;

                    if (timeToNextApsis < 0)
                    {
                        timeToNextApsis = (orbitalPeriod / 2) + timeToNextApsis;
                        if (whichApsis == "Apoapsis")
                            whichApsis = "Periapsis";
                        else if (whichApsis == "Periapsis")
                            whichApsis = "Apoapsis";
                    }

                    apoapsis /= 1000;
                    periapsis /= 1000;
                    semiMajorAxis /= 1000;
                    stopwatch.Start();
                    // OrbitalCalcs end

                    Console.Clear();
                    Console.WriteLine(" Orbital Information:");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(
                                      $"\n\t\t   _.----._        Semi-Major Axis: {semiMajorAxis.ToString("N1")} km" +
                                      $"\n\t\t /ˆ        ˆ\\      Orbital Eccentricity: {eccentricity.ToString("N4")}" +
                                       "\n\t\t|    ,-,     |" +
                             $"\n Periapsis:\t|   |   |    |     Apoapsis:" +
            $"\n {periapsis.ToString("N1")} \t|    '-'     |     {apoapsis.ToString("N1")}" +
                                  $"\n km\t\t \\,        ,/      km" +
                                       "\n\t\t   ''----''");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\n Altitude: {orbitalAltitude.ToString("N3")}\n Orbital Velocity: {velocity.ToString("N3")}\n Orbital Period: {orbitalPeriod.ToString("N0")} seconds\n Time to {whichApsis}: {timeToNextApsis.ToString("N0")} seconds\n\nDiagnostics:\nRealistic Angle: {realisticAngle}\nStopwatch Angle: {stopwatchAngle}\nStopwatch Value: {stopwatchValue}");
                    Console.ForegroundColor = ConsoleColor.White;
                    Thread.Sleep(200);
                } while (whichApsis != "shblimblenut");

            }

            else if (readResult == "w")
            {
                bool warpValid = false;
                do
                {
                    Console.WriteLine("Input warp value:");
                    readResult = Console.ReadLine();
                    if (readResult != null)
                    {
                        warpValid = Int32.TryParse(readResult, out timeAcceleration);
                        if (warpValid)
                        {
                            timeAcceleration = Int32.Parse(readResult);
                            warpValid = true;
                        }
                        else
                        {
                            Console.WriteLine("Invalid warp value.");
                            continue;
                        }
                    }
                    else
                    {
                        Console.WriteLine("No input detected.");
                        continue;
                    }
                } while (warpValid == false);
                Console.WriteLine("\n>type 'u' to update the simultion\n>type 'w' to change the speed of time\n>type 'a' to auto-update (WARNING: CANNOT BE UNDONE (for now), MUST BE CLOSED WITH CTRL+C\n>type 'b' to make a burn at apoapsis or periapsis\n\n>type 's' to save orbital statistics\n>type 'l' to load orbital statictics\n>type 'q' to quit\n\n More features to be added\n");
                goto CharacterAccept;
            }

            else if (readResult == "s")
            {
                bool directoryValid = false;
                do
                {
                    Console.WriteLine("Please input file directory (ex. 'C:\\\\Users\\\\user\\\\Downloads {\\\\OrbitalSimStats.txt})\nNOTE: USE 2 BACKSLASHES IN FILE DIRECTORY:");
                    readResult = Console.ReadLine();
                    if (readResult != null)
                    {
                        if (Directory.Exists(readResult))
                        {
                            string path = readResult + "\\OrbitalSimStats.txt";

                            eccAlt = (1 - (2 / (((2 * semiMajorAxis - bodyDiameter / 2 - periapsis) / (bodyDiameter / 2 + periapsis)) + 1)));
                            realisticAngle = 2 * (Math.Atan(Math.Sqrt(Math.Abs((1 + eccAlt) / (1 - eccAlt))) * (Math.Tan(stopwatchAngle / 2))));

                            using (StreamWriter saver = new StreamWriter(path))
                            {
                                if (!File.Exists(path))
                                {
                                    File.Create(path);
                                }
                                saver.Write($"PE: {periapsis}\nAP: {apoapsis}\nSM: {semiMajorAxis}\nEC: {eccentricity}\nAL: {orbitalAltitude}\nVE: {velocity}\nOP: {orbitalPeriod}\nNA: {timeToNextApsis}\nRA: {realisticAngle}\nWA: {whichApsis}");
                                saver.Flush();
                                Console.WriteLine($"\nSaved successfully to {readResult}. You can go check if you want, I don't mind.");
                                directoryValid = true;
                            }
                        }
                        else if (readResult == "q")
                        {
                            Console.WriteLine("\n>type 'u' to update the simultion\n>type 'w' to change the speed of time\n>type 'a' to auto-update (WARNING: CANNOT BE UNDONE (for now), MUST BE CLOSED WITH CTRL+C\n>type 'b' to make a burn at apoapsis or periapsis\n\n>type 's' to save orbital statistics\n>type 'l' to load orbital statictics\n>type 'q' to quit\n\n More features to be added\n");
                            goto CharacterAccept;
                        }
                        else
                        {
                            Console.WriteLine("That directory does not exist or is invalid.");
                            continue;
                        }
                    }
                    else
                    {
                        Console.WriteLine("No input detected.");
                        continue;
                    }
                } while (directoryValid == false);

                Console.WriteLine("\n>type 'u' to update the simultion\n>type 'w' to change the speed of time\n>type 'a' to auto-update (WARNING: CANNOT BE UNDONE (for now), MUST BE CLOSED WITH CTRL+C\n>type 'b' to make a burn at apoapsis or periapsis\n\n>type 's' to save orbital statistics\n>type 'l' to load orbital statictics\n>type 'q' to quit\n\n More features to be added\n");
                goto CharacterAccept;
            }

            /*
            else if (readResult == "l")
            {
                bool directoryValid = false;
                do
                {
                    Console.WriteLine("Please input file directory (ex. 'C:\\\\Users\\\\user\\\\Downloads\\\\OrbitalSimStats.txt)\nNOTE: USE 2 BACKSLASHES IN FILE DIRECTORY:");
                    readResult = Console.ReadLine();

                    if (readResult != null)
                    {
                        Console.WriteLine(readResult);
                        if (Directory.Exists(readResult))
                        {
                            using (StreamReader sr = new StreamReader(readResult))
                            {
                                // Read and display lines from the file until the end of the file is reached.
#pragma warning disable CS8602
                                
                                periapsis = Convert.ToDouble(sr.ReadLine().Substring(4));
                                apoapsis = Convert.ToDouble(sr.ReadLine().Substring(4));
                                semiMajorAxis = Convert.ToDouble(sr.ReadLine().Substring(4));
                                eccentricity = Convert.ToDouble(sr.ReadLine().Substring(4));
                                orbitalAltitude = Convert.ToDouble(sr.ReadLine().Substring(4));
                                velocity = Convert.ToDouble(sr.ReadLine().Substring(4));
                                orbitalPeriod = Convert.ToDouble(sr.ReadLine().Substring(4));
                                timeToNextApsis = Convert.ToDouble(sr.ReadLine().Substring(4));
                                realisticAngle = Convert.ToDouble(sr.ReadLine().Substring(4));
                                whichApsis = sr.ReadLine().Substring(4);
                                
                                Console.WriteLine("beans");
                                directoryValid = true;
#pragma warning restore CS8602
                            }
                        }
                        else if (readResult == "q")
                        {
                            Console.WriteLine("\n>type 'u' to update the simultion\n>type 'w' to change the speed of time\n>type 'a' to auto-update (WARNING: CANNOT BE UNDONE (for now), MUST BE CLOSED WITH CTRL+C\n>type 'b' to make a burn at apoapsis or periapsis\n\n>type 's' to save orbital statistics\n>type 'l' to load orbital statictics\n>type 'q' to quit\n\n More features to be added\n");
                            goto CharacterAccept;
                        }
                        else
                        {
                            Console.WriteLine("That directory does not exist or is invalid.");
                            continue;
                        }
                    }
                    else
                    {
                        Console.WriteLine("No input detected.");
                        continue;
                    }
                } while (!directoryValid);
                Console.ReadLine();
            }
            */

            else if (readResult == "q")
            {
                Console.WriteLine("\nBye bye!");
                Thread.Sleep(500);
                Environment.Exit(1);
            }

            else
            {
                Console.WriteLine("Invalid input. Please select a valid input.");
                Console.WriteLine("\n>type 'u' to update the simultion\n>type 'w' to change the speed of time\n>type 'a' to auto-update (WARNING: CANNOT BE UNDONE (for now), MUST BE CLOSED WITH CTRL+C\n>type 'b' to make a burn at apoapsis or periapsis\n\n>type 's' to save orbital statistics\n>type 'l' to load orbital statictics\n>type 'q' to quit\n\n More features to be added\n");
                goto CharacterAccept;
            }
        }
        
        else
        {
            Console.WriteLine("No input detected.");
            Console.WriteLine("\n>type 'u' to update the simultion\n>type 'w' to change the speed of time\n>type 'a' to auto-update (WARNING: CANNOT BE UNDONE (for now), MUST BE CLOSED WITH CTRL+C\n>type 'b' to make a burn at apoapsis or periapsis\n\n>type 's' to save orbital statistics\n>type 'q' to quit\n\n More features to be added\n");
            goto CharacterAccept;
        }
    }
}