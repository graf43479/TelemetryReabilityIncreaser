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

        public RawDataMatrix(int m, int n) : base(m,n)       { }
        public RawDataMatrix(int[,] data) : base (data)        { }

        /// <summary>
        /// Конструктор с инициализатором
        /// </summary>
        /// <param name="path">путь к json объектам</param>
        /// <param name="channel">номер канала (1-5)</param>
        /// <param name="intensity">интенсивность помех (1-6)</param>
        public RawDataMatrix(string path, int channel, int intensity) : base(path, "m" + channel + "_" + intensity)
        {

            this.channel = channel;
            this.intensity = intensity;
        }


        /// <summary>
        /// Имя матрицы, основанное на сочетании номера канала и интенсивности помех
        /// </summary>
        public string Name => "m" + channel + "_" + intensity;




        /// <summary>
        /// Сопостовление матриц
        /// </summary>
        /// <param name="otherMtrx">другая матрица</param>
        /// <returns>Количество несовпадений между матрицами</returns>
        public int GetMismatches(Matrix otherMtrx)
        {
            int mismatch=0;
            this.ProccessFunctionWithData((i, j) =>
                {
                    if (data[i, j] != otherMtrx[i, j])
                    {
                        mismatch++;
                    }
                });
            return mismatch;
        }

    }
}
