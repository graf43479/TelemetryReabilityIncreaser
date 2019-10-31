using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TelemetryEngine
{
    public class RawDataMatrix : Matrix
    {
        private int channel = 0;
        private int intensity = 0;
        
        public RawDataMatrix(int[,] data) : base (data)        { }

        public RawDataMatrix(string path, int channel, int intensity) : base(path, "m" + channel + "_" + intensity)
        {
            
            this.channel = channel;
            this.intensity = intensity;                       
        }


        public string Name => "m" + channel + "_" + intensity;
    }
}
