using System;
using System.Collections.Generic;
using System.Text;

namespace TelemetryEngine
{
    public class MismatchesCoordList
    {
        private List<Coord> coords;
        private readonly string name;
        public string Gamma { get; set; }

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

        public override string ToString()
        {
            if (!String.IsNullOrEmpty(name))
            {
                char channel = name[1];
                char intensity = name[3];

                //если это итоговая матрица (блок данных)
                if (channel == '0' && intensity == '0')
                {
                    return String.Format($"Обобщенные данные. N={k}. {Gamma}");
                }
                else
                { 
                    return String.Format($"Канал: {channel}. Уровень помех: {intensity}. N={k}" );
                }


            }
            return "";
            //return 
        }

    }
}
