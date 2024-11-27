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
    static double semiMajorAxis; // 7,871,000 meters
    static double semiMinorAxis;
    static double eccentricity;
    static double semiLatusRectum; // 6,996.444.4444 meters

    static double orbitalAltitude = 1000000; // 1,000,000 meters
    static double velocity; // 7583.562 meters per second
    static double orbitalPeriod = 6950; // 6,950 seconds
    static double timeToNextApsis; //3,475 seconds

    static double G = 6.6743 * Math.Pow(10, -11); // teehee newtons*meters^2 / kilograms^2
    static double bodyMass = 5972000000 * Math.Pow(10, 15); // 5.972 x 10^24 kilograms
    static double bodyRadius = 12742000; // 12,742,000 meters

    // rocket info
    static double dV = 2000; //2,000 m/s of delta-V

    // math exclusives
    static double stopwatchAngle = 0;
    static double realisticAngle;
    static int timeAcceleration = 1;

    // formatting info
    static string whichApsis = "Periapsis";
    static string? readResult;
    static double stopwatchValue;
    static string inputMessage = "\n>type 'u' to update the simultion\n>type 'warp' to change the speed of time\n>type 'auto' to auto-update\n>type 'burn' to make a burn at apoapsis or periapsis\n\n>type 'adv' to toggle advanced data display\n>type 'q' to quit\n\n More features to be added\n";
    
    static bool displayVariables = false;

    public static void Main(string[] args)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        OrbitalCalcs(stopwatch);
        timeToNextApsis = orbitalPeriod / 2;



        Console.WriteLine("Welcome to your very own orbit!\n");
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
                /*
                OrbitalBurn(stopwatch);
                
                OrbitalCalcs(stopwatch);
                */

                Console.Clear();
                Console.WriteLine("sorry there's something REAL messed up about the burn delta-v requirements. maybe install a later patch release when it comes out?");
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
                Console.WriteLine("\nhave a good one m8");
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
                                               $"\t\t        {(whichApsis == "Periapsis" ? "v" : " ")}" +
                                                                           $"\n\t\t     _.----._        Semi-Major Axis: {semiMajorAxis.ToString("N1")} km" +
                                                                           $"\n\t\t   /ˆ        ˆ\\      Orbital Eccentricity: {eccentricity.ToString("N4")}" +
                                                                            "\n\t\t  |    ,-,     |" +
$"\n Periapsis:\t{(orbitalAltitude <= (semiLatusRectum - (bodyRadius)) ? ">" : " ")} |   |   |    | {(orbitalAltitude >= (semiLatusRectum - (bodyRadius) ) ? "<" : " ")}   Apoapsis:" +
                                                 $"\n {periapsis.ToString("N1")} \t  |    '-'     |     {apoapsis.ToString("N1")}" +
                                                                       $"\n km\t\t   \\,        ,/      km" +
                                                                            "\n\t\t     ''----''" +
                                            $"\n\t\t        {(whichApsis == "Apoapsis" ? "^" : " ")}");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n Altitude: {orbitalAltitude.ToString("N3")} meters\n Orbital Velocity: {velocity.ToString("N3")} m/s\n Orbital Period: {orbitalPeriod.ToString("N0")} seconds\n Time to {whichApsis}: {timeToNextApsis.ToString("N0")} seconds\n Warp Speed: {timeAcceleration}\n dV: {dV.ToString("N1")}x");

        Console.WriteLine(displayVariables ? $"\nDiagnostics:\nG: {G}\nBody Mass: {bodyMass}\nBody Radius: {bodyRadius.ToString("N0")}\nSemi-Latus Rectum: {semiLatusRectum.ToString("N4")}\nS-L R with Body: {(semiLatusRectum - (bodyRadius)).ToString("N4")}\nStopwatch Value: {stopwatchValue}\nStopwatch Angle: {stopwatchAngle}\nRealistic Angle: {realisticAngle}\n" : "\n");

            Console.ForegroundColor = ConsoleColor.White;
    }


    public static void OrbitalCalcs(Stopwatch stopwatch)
    {
        // OrbitalCalcs start
        periapsis *= 1000;
        apoapsis *= 1000;
        semiMajorAxis *= 1000;
        semiMinorAxis = Math.Sqrt(apoapsis * periapsis);

        semiMajorAxis = (apoapsis + periapsis + 2*bodyRadius) / 2;
        eccentricity = 1 - (2 / (((apoapsis + bodyRadius) / (periapsis + bodyRadius)) + 1));
        semiLatusRectum = semiMajorAxis * (1 - Math.Pow(eccentricity, 2));
        /*
        orbitalPeriod = Double.Tau * (Math.Sqrt( (Math.Pow(semiMajorAxis, 3)) / (G * bodyMass ) ));

        theres a HUUUGE frickn problem here
        */
        stopwatch.Stop();

        stopwatchValue = (stopwatch.Elapsed.TotalMilliseconds / 1000);
        stopwatchAngle = (((2 * double.Pi) / orbitalPeriod) * stopwatchValue * timeAcceleration) + stopwatchAngle;

        while (stopwatchAngle > double.Tau)
        {
            stopwatchAngle = 0 + (stopwatchAngle - 2 * double.Pi);
        }
        stopwatch.Reset();
        realisticAngle = 2 * (Math.Atan(Math.Sqrt(Math.Abs((1 + eccentricity) / (1 - eccentricity))) * (Math.Tan(stopwatchAngle / 2))));

        if (realisticAngle > double.Pi)
        {
            realisticAngle = (-1 * double.Pi) + (realisticAngle - double.Pi);
        }

        orbitalAltitude = ((semiMajorAxis * (1 - (eccentricity * eccentricity))) / (1 + eccentricity * Math.Cos(realisticAngle))) - (bodyRadius);

        velocity = Math.Sqrt(G * bodyMass * ((2 / (orbitalAltitude + (bodyRadius))) - (1 / semiMajorAxis)));
        timeToNextApsis -= stopwatchValue * timeAcceleration;

        if (timeToNextApsis < 0)
        {
            timeToNextApsis = (orbitalPeriod / 2) + timeToNextApsis;
            
            whichApsis = (whichApsis == "Apoapsis" ? "Periapsis" : "Apoapsis");
        }

        apoapsis /= 1000;
        periapsis /= 1000;
        semiMajorAxis /= 1000;
        // OrbitalCalcs end
    }


    //       Other Functions       //



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
        testSMA = testApo + testPeri + bodyRadius;
        dVNeeded = (Math.Sqrt(G * bodyMass * ((2 / (orbitalAltitude + (bodyRadius))) - (1 / testSMA)))) - velocity;
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
        semiMajorAxis = testApo + testPeri + bodyRadius;
        dVNeeded = (Math.Sqrt(G * bodyMass * ((2 / (orbitalAltitude + (bodyRadius))) - (1 / semiMajorAxis)))) - velocity;
        dV = dV - dVNeeded;

        return;
    }
}