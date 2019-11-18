using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TelemetryEngine;

namespace GUI
{
    class Program
    {
        static async Task Main(string[] args) 
        {
            //Порядок действия гуя
            //1. Инициализировать класс 
            //2. Получить набор вариантов
            //3. Выбрать вариант
            //4. Получить расчет по выбранному варианту
            //5. Отобразить результаты на форме

            string path = Directory.GetCurrentDirectory() + "/JsonData";

            await MatrixDataInitializer.GenerateAsync();

            Engine engine = new Engine(path);
            IEnumerable<string> combinations = engine.GetFilteredCombinations();
            //выбор необходимой            
            RawDataMatrix result = engine.PerformCombination(combinations.Skip(50).First());
            Console.WriteLine(new string('-', 50));
            Console.WriteLine(result);
        }
    }
}
