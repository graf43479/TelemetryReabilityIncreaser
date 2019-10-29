using System;

namespace GUI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            TelemetryEngine.TelemetryEngine.ShowData(TelemetryEngine.TelemetryEngine.m1);
            Console.WriteLine(new string('-',50));
            TelemetryEngine.TelemetryEngine.ShowData(TelemetryEngine.TelemetryEngine.mW);

            TelemetryEngine.TelemetryEngine.Start();
            TelemetryEngine.TelemetryEngine.ShowData(TelemetryEngine.TelemetryEngine.mRes);
        }
    }
}
