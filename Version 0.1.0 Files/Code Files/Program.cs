using System;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Numerics;

public class OrbitalSim
{

    // body and orbital info
    static double periapsis = 1000.0; // 1,000,000 meters
    static double apoapsis = 2000.0; // 2,000,000 meters
    static double semiMajorAxis = 7871.0; // 7,871,000 meters
    static double eccentricity = 0.3333;

    static double orbitalAltitude = 1000000; // 1,000,000 meters
    static double velocity = 7583.562; // 7583.562 meters per second
    static double orbitalPeriod = 6950; // 6,950 seconds
    static double timeToNextApsis = 3475; //3,475 seconds

    static double G = 6.6743 * Math.Pow(10, -11);
    static double bodyMass = 5972000000 * Math.Pow(10, 15); // 5.972 x 10^24 kilograms
    static double bodyDiameter = 12742000; // 12,742,000 meters

    // rocket info
    static double dV = 2000; //2,000 m/s of delta-V

    // math exclusives
    static double eccAlt;
    static double stopwatchAngle = 0;
    static double realisticAngle;
    static int timeAcceleration = 1;

    // formatting info
    static string whichApsis = "Apoapsis";
    static string? readResult;
    static double stopwatchValue;
    static string inputMessage = "\n>type 'u' to update the simultion\n>type 'warp' to change the speed of time\n>type 'auto' to auto-update\n>type 'burn' to make a burn at apoapsis or periapsis\n\n>type 'save' to save orbital statistics\n>type 'load' to load orbital statictics\n>type 'adv' to toggle advanced data display\n>type 'q' to quit\n\n More features to be added\n";
    
    static bool displayVariables = false;

    public static void Main(string[] args)
    {
        Console.WriteLine("Welcome to your very own orbit!\n");
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        Formatting();
        Console.WriteLine(inputMessage);

    CharacterAccept:

        readResult = Console.ReadLine();
        if (readResult != null)
        {
            readResult = readResult.ToLower();

            if (readResult == "u")
            {
                OrbitalCalcs(stopwatch);
                stopwatch.Start();

                Console.Clear();

                Formatting();
                Console.WriteLine(inputMessage);
                goto CharacterAccept;
            }

            else if (readResult == "auto" || readResult == "a")
            {
                while (!Console.KeyAvailable) // honorary shblimblenut     o7 my shitty code 
                {
                    OrbitalCalcs(stopwatch);
                    stopwatch.Start();

                    Console.Clear();
                    Formatting();
                    Console.WriteLine("Press any key to stop");
                    Thread.Sleep(200);
                }
                Console.WriteLine(inputMessage);
                goto CharacterAccept;
            }

            else if (readResult == "warp" || readResult == "w")
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
                Console.WriteLine(inputMessage);
                goto CharacterAccept;
            }

            else if (readResult == "burn" || readResult == "b")
            {
                OrbitalBurn(stopwatch);

                OrbitalCalcs(stopwatch);
                Console.Clear();
                Formatting();
                Console.WriteLine(inputMessage);
                stopwatch.Start();
                goto CharacterAccept;
            }

            else if (readResult == "adv")
            {
                displayVariables = !displayVariables; 

                OrbitalCalcs(stopwatch);
                stopwatch.Start();

                Console.Clear();

                Formatting();
                Console.WriteLine(inputMessage);
                goto CharacterAccept;
            }

            else if (readResult == "quit" || readResult == "q")
            {
                Console.WriteLine("\nBye bye!");
                Thread.Sleep(500);
                Environment.Exit(1);
            }

            else
            {
                Console.WriteLine("Invalid input. Please select a valid input.");
                Console.WriteLine(inputMessage);
                goto CharacterAccept;
            }
        }

        else
        {
            Console.WriteLine("No input detected.");
            Console.WriteLine(inputMessage);
            goto CharacterAccept;
        }
    }




    public static void Formatting()
    {
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
        Console.WriteLine($"\n Altitude: {orbitalAltitude.ToString("N3")}\n Orbital Velocity: {velocity.ToString("N3")}\n Orbital Period: {orbitalPeriod.ToString("N0")}\n Time to {whichApsis}: {timeToNextApsis.ToString("N0")} seconds\n Warp Speed: {timeAcceleration}\n dV: {dV.ToString("N1")}");

        if (!displayVariables) {
            Console.WriteLine("\n");
        }

        else {
            Console.WriteLine($"\nDiagnostics:\nG: {G}\nBody Mass: {bodyMass}\nBody Diameter: {bodyDiameter}\nAlt. Eccentricity: {eccAlt}\nStopwatch Value: {stopwatchValue}\nStopwatch Angle: {stopwatchAngle}\nRealistic Angle: {realisticAngle}\n");
        }
        Console.ForegroundColor = ConsoleColor.White;
    }


    public static void OrbitalCalcs(Stopwatch stopwatch)
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

        while (stopwatchAngle > double.Tau)
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
        // OrbitalCalcs end
    }


    //       Other Functions       \\



    // holy shit why is this so long jesus christ
    // is it because of all the `Console.ReadLine();`s? no yeah its definitely that
    public static void OrbitalBurn(Stopwatch stopwatch)
    {
        double dVNeeded;
        string? readResultTheOtherOne;
        bool inputValid = false;
        do
        {
            Console.WriteLine("Would you like to adjust your apoapsis or periapsis? ['apoapsis'/'periapsis']");
            readResultTheOtherOne = Console.ReadLine();
            if (readResultTheOtherOne != null)
            {
                readResultTheOtherOne = readResultTheOtherOne.ToLower().Trim();
                if (readResultTheOtherOne == "apoapsis")
                {
                    whichApsis = "Apoapsis";
                    stopwatch.Stop();
                    stopwatch.Reset();
                    stopwatchAngle = 0;
                    inputValid = true;
                }
                else if (readResultTheOtherOne == "periapsis")
                {
                    whichApsis = "Periapsis";
                    stopwatch.Stop();
                    stopwatch.Reset();
                    stopwatchAngle = -1 * Double.Pi;
                    inputValid = true;
                }
                else if (readResultTheOtherOne == "q")
                {
                    Console.WriteLine(inputMessage);
                    stopwatch.Start();
                    return;
                }
                else
                {
                    Console.WriteLine("Invalid input.");
                    continue;
                }
            }
            else
            {
                Console.WriteLine("No input detected.");
                continue;
            }
        } while (inputValid == false);

        inputValid = false;
        double goalApsis = 0;
        do
        {
            Console.WriteLine("Input target apsis height in meters:");
            readResult = Console.ReadLine();
            if (readResult != null)
            {
                if (readResult != "q")
                {
                    inputValid = Double.TryParse(readResult, out goalApsis);
                    if (inputValid)
                    {
                        goalApsis = Double.Parse(readResult);
                        inputValid = true;
                    }
                    else
                    {
                        Console.WriteLine("Invalid target apsis.");
                        continue;
                    }
                }
                else
                {
                    Console.WriteLine(inputMessage);
                    stopwatch.Start();
                    return;
                }

            }
            else
            {
                Console.WriteLine("No input detected.");
                continue;
            }
        } while (inputValid == false);

        double testApo = apoapsis;
        double testPeri = periapsis;
        double testSMA;
        if (readResultTheOtherOne == "apoapsis")
            testApo = goalApsis;
        else
            testPeri = goalApsis;
        if (testPeri > testApo)
            (testPeri, testApo) = (testApo, testPeri);
        testSMA = testApo + testPeri + bodyDiameter / 2;
        dVNeeded = (Math.Sqrt(G * bodyMass * ((2 / (orbitalAltitude + (bodyDiameter / 2))) - (1 / testSMA)))) - velocity;
        Console.WriteLine($"To confirm, you would like to set your {readResultTheOtherOne} to {goalApsis.ToString("N1")} meters. This manuever will take {dVNeeded.ToString("N1")} m/s of delta-V.");

        inputValid = false;
        do
        {
            Console.WriteLine($"You currently have {dV.ToString("N1")} m/s of delta-V. Would you like to continue? [y/n]");
            readResult = Console.ReadLine();
            if (readResult != null)
            {
                readResult = readResult.Trim();
                if (readResult == "y")
                {
                    if (dVNeeded < dV)
                        inputValid = true;
                    else
                    {
                        Console.WriteLine("Oh no! You don't have enough delta-V.");
                        continue;
                    }
                }
                else if (readResult == "n")
                {
                    Console.WriteLine(inputMessage);
                    stopwatch.Start();
                    return;
                }
                else if (readResult == "q")
                {
                    Console.WriteLine(inputMessage);
                    stopwatch.Start();
                    return;
                }
                else
                {
                    Console.WriteLine("Invalid input.");
                    continue;
                }
            }
            else
            {
                Console.WriteLine("No input detected.");
                continue;
            }
        } while (inputValid == false);


        if (readResultTheOtherOne == "apoapsis")
            apoapsis = goalApsis;
        else
            periapsis = goalApsis;
        if (periapsis > apoapsis)
            (periapsis, apoapsis) = (apoapsis, periapsis);
        semiMajorAxis = testApo + testPeri + bodyDiameter / 2;
        dVNeeded = (Math.Sqrt(G * bodyMass * ((2 / (orbitalAltitude + (bodyDiameter / 2))) - (1 / semiMajorAxis)))) - velocity;
        dV = dV - dVNeeded;

        return;
    }
}