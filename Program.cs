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
    static double semiMajorAxis;
    static double semiMinorAxis;
    static double eccentricity;
    static double semiLatusRectum; // 6,996.444.4444 meters
    static double velocity;

    static double orbitalAltitude = 1000000; // 1,000,000 meters
    static double orbitalPeriod = 6959; // 6,950 seconds
    static double timeToNextApsis; //3,475 seconds

    static double G = 6.6743 * Math.Pow(10, -11); // teehee newtons*meters^2 / kilograms^2
    static double bodyMass = 5972000000 * Math.Pow(10, 15); // 5.972 x 10^24 kilograms
    static double bodyRadius = 6371000; // 6,371,000 meters (average radius, eq. radius = 6,378)

    // rocket info
    // ...someday. someday soon.
    static double dV = 2000; //2,000 m/s of delta-V

    // math exclusives
    static double stopwatchAngle = 0;
    static double realisticAngle;
    static int timeAcceleration = 1;

    // formatting info
    static string whichApsis = "Periapsis";
    static string? readResult;
    static double stopwatchValue;
    static string inputMessage = "\n>type 'u' to update the simultion\n>type 'warp' to change the speed of time\n>type 'auto' to auto-update\n>type 'burn' to make a burn at apoapsis or periapsis\n\n\n>type 'q' to quit\n\n More features to be added\n";
    
    static bool displayVariables = false;

    public static void Main(string[] args)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        OrbitalCalcs(stopwatch);
        timeToNextApsis = orbitalPeriod / 2;



        Console.WriteLine("Welcome to your very own orbital sim!\n");
        Formatting();
        Console.WriteLine(inputMessage);

        while (true) {
            Input(stopwatch);
        }
    }

    public static void Input(Stopwatch stopwatch)
    {
        string? readResult = Console.ReadLine();
        if (readResult == null) {
            Console.WriteLine("Invalid input.");
            return;
        }

            switch (readResult)
            {
                case "u":
                    OrbitalCalcs(stopwatch);
                    stopwatch.Start();

                    ClearConsole();
                    Console.WriteLine("\x1b[3J");

                    Formatting();
                    Console.WriteLine(inputMessage);
                    break;
                case "auto":
                case "a":
                /* memorial to my god awful code
                 * 
                 * this line is dedicated to the "shblimblenut", a variable I initially used in a `do... while` to repeat the auto function
                 * this line is dedicated to my ridiculous amount of nested loops present in the input accept code
                 * this line is dedicated to my ENTIRE PROGRAM being run from the `Main()` function
                 * this line is dedicated to the og `readResult` variable I had, which was a field rather than a local variable
                 * 
                 * this line is dedicated to the god awful `readResult` valid input assurance system im STILL USING
                 * this line is dedicated to the fact that this is a console app that im trying to render graphics in
                 * 
                 * o7 my terrible code
                 */
                    while (!Console.KeyAvailable)
                    {
                        OrbitalCalcs(stopwatch);
                        stopwatch.Start();

                        ClearConsole();
                        Console.WriteLine("\x1b[3J");
                        Formatting();
                        Console.WriteLine("Press any key to stop");
                        Thread.Sleep(200);
                    }
                    Console.WriteLine(inputMessage);
                    break;
                case "warp":
                case "w":
                    bool warpValid = false;
                    while (warpValid == false)
                    {
                        Console.WriteLine("Input warp value:");
                        readResult = Console.ReadLine();
                        if (readResult != null && Int32.TryParse(readResult, out timeAcceleration) == true)
                        {
                            timeAcceleration = Int32.Parse(readResult);
                            Formatting();
                            Console.WriteLine(inputMessage);
                            break;
                    }
                        else if (readResult == "q")
                        {
                            break;
                        }
                        else
                    {
                        Console.WriteLine("Invalid input detected.");
                        continue;
                    }
                    }
                break;
                case "quit":
                case "q":
                    Console.WriteLine("\ncya nerds");
                    Thread.Sleep(500);
                    Environment.Exit(1);
                    break;
                default:
                    Console.WriteLine("Invalid input.");
                    break;
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
        
        orbitalPeriod = double.Tau*Math.Sqrt( (Math.Pow(semiMajorAxis, 3)) / (G*bodyMass) );

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

    // code stolen DIRECTLY from Stack Overflow (user "jdt")
    public static void ClearConsole()
    {
        Console.SetCursorPosition(0, 0);
        Console.CursorVisible = false;
        for (int y = 0; y < Console.WindowHeight; y++)
            Console.Write(new String(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, 0);
        Console.CursorVisible = true;
    }
}