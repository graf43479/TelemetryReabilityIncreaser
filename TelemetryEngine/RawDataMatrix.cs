//Класс, определяющий работу блоком данных

// ---------------------------------------------------------------------------
// Авторское право © ООО "%CompanyName". Авторские права защищены.
// Copyright. JSC «%CompanyName» 2016. All rights reserved
// Компания: %CompanyName
// Подразделение: %Department
// Author: Oleg Vorontsov
// Description: Данный класс является наследником класса Matrix и реализовывает основные виды работ с блоком данных.  
//              Имеется возможность инициализации через JSON. Для этого в соответствующем конструкторе должен передаваться 
//              путь к папке с JSON-файлами. При этом конструктор предполагает, что имя JSON файла состоит из составных элементов:
//              например m2_6.json: m-константа, 2 - номер канала (1-3(4-5), 6 - интенсивность помех (1-6). Таким образом, чтобы инициализировать 
//              матрицу из JSON необходимо предоставить конструктору путь как папке с json-файлами, номер канала и номер интенсивности помех 
//              Вышеописанное не отменяет возможности инициализации матриц обычным присваиванием (через другой конструктор)
//              Также класс с помощью метода GetMismatches позволяет осуществлять сранение одного блока данных с другим и хранить информацию 
//              по несоответствиям (например с эталонным блоком данных) во вспомогательном классе MismatchesCoordList. 
// Rational: Упрощение работы с блоком данных
// ---------------------------------------------------------------------------

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
        private List<Coord> mismatches;

        /// <summary>
        /// Конструктор, заполняющий матрицу нулями
        /// </summary>
        /// <param name="m">Столбцы</param>
        /// <param name="n">Строки</param>
        public RawDataMatrix(int m, int n) : base(m,n)       { }

        /// <summary>
        /// Конструктор присваивающий блоку данных значения
        /// </summary>
        /// <param name="data">значения матрицы</param>
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
        /// <param name="otherMtrx">другая матрица, подразумевается эталонная</param>
        /// <returns>Количество несовпадений между матрицами</returns>
        public MismatchesCoordList GetMismatches(Matrix otherMtrx)
        {
            mismatches = new List<Coord>();
            this.ProccessFunctionWithData((i, j) =>
                {
                    if (data[i, j] != otherMtrx[i, j])
                    {
                        mismatches.Add(new Coord(i, j));
                    }
                });
            return new MismatchesCoordList(this.Name, mismatches);
        }
    }

    /// <summary>
    /// Класс координаты
    /// </summary>
    public class Coord
    {
        public int X { get; }
        public int Y { get; }

        public Coord(int x, int y)
        {            
            X = x;
            Y = y;
        }
    }
}
