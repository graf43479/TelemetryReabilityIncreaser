//Класс с основными функциями по работе с матрицами

// ---------------------------------------------------------------------------
// Авторское право © ООО "%CompanyName". Авторские права защищены.
// Copyright. JSC «%CompanyName» 2016. All rights reserved
// Компания: %CompanyName
// Подразделение: %Department
// Author: Oleg Vorontsov
// Description: Настоящий класс упрощает работу с матрицами: позволяет инициировать их из json-файла или передачей двумерного массива в конструктор.
//          Предоставляет доступ к элементам матрицы, позволяет узнать размерность и допускает произвольные действия над членами матрицы.
// Rational: Данный класс является базовым и упрощает читаемости и понимание в целевых классах.  
// ---------------------------------------------------------------------------
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TelemetryEngine
{
    public class Matrix
    {
        protected int[,] data;

        /// <summary>
        /// Конструктор, создающий матрицу mxn, заполненную нулями 
        /// </summary>
        /// <param name="m">Количество столбцов</param>
        /// <param name="n">Количество строк</param>
        public Matrix(int m, int n)
        {            
            data = new int[m,n];
            
            ProccessFunctionWithData((i, j) => data[i, j] = 0);
        }

        /// <summary>
        /// Конструктор для случая, когда данные берутся не из JSON файла.
        /// </summary>
        /// <param name="data">Двумерный массив 32х20</param>
        public Matrix(int[,] data)
        {
            this.data = data;
        }

        /// <summary>
        /// Конструктор матрицы с инициализацией из json-файла
        /// </summary>
        /// <param name="path">путь к директории с json-объектами</param>
        /// <param name="name">имя json-объекта для инициализации</param>
        public Matrix(string path, string name)
        {            
            string fullName = path + "/" + name + ".json";
            try
            {
                //Console.WriteLine($"Попытка инициализации файла {jsName}");
                using (StreamReader reader = new StreamReader(fullName))
                {                    
                    string json = reader.ReadToEnd();
                    JsonSerializerSettings serSettings = new JsonSerializerSettings();
                    serSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    data = JsonConvert.DeserializeObject<int[,]>(json, serSettings);
                }
                //Console.WriteLine($"Файл {jsName} считан");
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Ошибка чтения файла {fullName}. Текст ошибки: {ex.Message}");
                data = null;
            }
        }

        /// <summary>
        /// Инициализирована ли матрица
        /// </summary>
        public bool IsInitialized => data != null;
                        
        /// <summary>
        /// индексатор
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int this[int x, int y]
        {
            get { return data[x, y]; }
            set { data[x, y] = value; }
        }

        /// <summary>
        /// количество столбцов (N)
        /// </summary>        
        public int GetXSize()
        {
            return data.GetLength(0);
        }


        /// <summary>
        /// количество строк (M)
        /// </summary> 
        public int GetYSize() 
        {
            return data.GetLength(1);
        }

        /// <summary>
        /// Обертка для манипуляций с данными
        /// </summary>
        /// <param name="func">лямбда для работы с i,j -ым элементом массива</param>
        public void ProccessFunctionWithData(Action<int, int> func)
        {
            for (int i = 0; i < this.GetXSize(); i++)
            {
                for (int j = 0; j < this.GetYSize(); j++)
                {
                    func(i, j);
                }
            }
        }

        /// <summary>
        /// Текстовое представление матрицы
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = "";
            this.ProccessFunctionWithData((i, j) =>
            {
                if (j == 0)
                {
                    result+="\n";
                }
                result+=data[i, j] + " ";
            });
            return result;
        }      
    }
}
