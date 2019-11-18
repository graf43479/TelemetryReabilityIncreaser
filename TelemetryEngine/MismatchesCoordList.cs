using System;
using System.Collections.Generic;
using System.Text;

namespace TelemetryEngine
{
    public class MismatchesCoordList
    {
        private List<Coord> coords;
        private readonly string name;

        public MismatchesCoordList(string name, List<Coord> coords)
        {
            this.coords = coords;
            this.name = name;
        }

        public Coord this[int index]
        {
            get { return coords[index]; }
            set { coords[index] = value; }
        }

        public int k => coords.Count;

        public string Name => name;

        public List<Coord> Coords => coords;

    }
}
