using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using TelemetryEngine;

namespace GUI
{
    class Program
    {
        static void Main(string[] args)
        {
            //Порядок действия гуя
            //1. Инициализировать класс 
            //2. Получить набор вариантов
            //3. Выбрать вариант
            //4. Получить расчет по выбранному варианту
            //5. Отобразить результаты на форме

            string path = Directory.GetCurrentDirectory() + "/JsonData";
            Engine engine = new Engine(path);
            IEnumerable<string> combinations = engine.GetFilteredCombinations();
            //выбор необходимой            
            RawDataMatrix result = engine.PerformCombination(combinations.Skip(50).First());
            Console.WriteLine(new string('-', 50));
            Console.WriteLine(result);
        }
    }
}
