//Класс для работы с несоответсвиями между матрицами и эталонными значениями

// ---------------------------------------------------------------------------
// Авторское право © ООО "%CompanyName". Авторские права защищены.
// Copyright. JSC «%CompanyName» 2016. All rights reserved
// Компания: %CompanyName
// Подразделение: %Department
// Author: Oleg Vorontsov
// Description: Настоящий класс содержит набор координат, которые не отличаются от эталоннго значения матрицы mBase 
// Rational: Упрощение работы с набором координат и иных показателей, отличающихся от эталона 
// ---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace TelemetryEngine
{
    public class MismatchesCoordList
    {
        private List<Coord> coords;
        private readonly string name;

        /// <summary>
        /// Характеристика (возможно временная), содержащая рассчет эффективности работы алгоритма для ИТОГОВОЙ матрицы 
        /// </summary>
        public string Gamma { get; set; }

        /// <summary>
        /// Конструктор, примающий отличные от эталона координаты и имя блока данных (для идентификации)
        /// </summary>
        /// <param name="name">Имя блока данных (например m2_3)</param>
        /// <param name="coords">Набор координат</param>
        public MismatchesCoordList(string name, List<Coord> coords)
        {
            this.coords = coords;
            this.name = name;
        }

        /// <summary>
        /// Индексатор
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Coord this[int index]
        {
            get { return coords[index]; }
            set { coords[index] = value; }
        }

        /// <summary>
        /// Количество несоответствий
        /// </summary>
        public int N => coords.Count;

        public string Name => name;

        /// <summary>
        /// Вернуть все координаты
        /// </summary>
        public List<Coord> Coords => coords;

        /// <summary>
        /// Вывод информации о матрице
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (!String.IsNullOrEmpty(name))
            {
                char channel = name[1];
                char intensity = name[3];

                //если это итоговая матрица (блок данных)
                if (channel == '0' && intensity == '0')
                {
                    //return String.Format($"Обобщенные данные. N={k}. {Gamma}");
                    return String.Format($"{Gamma}");
                }
                else
                { 
                    return String.Format($"Канал: {channel}. Уровень помех: {intensity}. N={N}" );
                }
            }
            return "";
        }

    }
}
