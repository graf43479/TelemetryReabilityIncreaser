using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace TelemetryEngine
{
    public static class TelemetryEngine
    {
        static TelemetryEngine()
        {
            GetDataFromJson(ref m1, "m1.json");
            GetDataFromJson(ref m2, "m2.json");
            GetDataFromJson(ref m3, "m3.json");
            GetDataFromJson(ref m4, "m4.json");
            GetDataFromJson(ref m5, "m5.json");
            GetDataFromJson(ref mBase, "mBase.json");
            GetDataFromJson(ref mW, "mW.json");         
        }

        const int m = 20; //рамерность матрицы
        const int n = 32; //рамерность матрицы
        const int w = 165; //количество кортежей весовых коэффициентов

        public static readonly int[,] m1;    //массив для 1го канала связи 
        public static readonly int[,] m2;    //массив для 2го канала связи
        public static readonly int[,] m3;    //массив для 3го канала связи
        public static readonly int[,] m4;    //массив для 4го канала связи
        public static readonly int[,] m5;    //массив для 5го канала связи

        public static int[,] mRes;              //результирующий массив
        public static readonly int[,] mBase;    //эталонный массив массив
        public static readonly int[,] mW;       //массив неизменных весовых коэффициентов

        private static int k = -1;              //текущая достоверность


        private static void GetDataFromJson(ref int[,] a, string jsName)
        {
            try
            {
                Console.WriteLine($"Попытка инициализации файла {jsName}") ;
                using (StreamReader reader = new StreamReader(jsName))
                {
                    string json = reader.ReadToEnd();
                    JsonSerializerSettings serSettings = new JsonSerializerSettings();
                    serSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    a = JsonConvert.DeserializeObject<int[,]>(json, serSettings);
                }
                Console.WriteLine($"Файл {jsName} считан");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка чтения файла {jsName}. Текст ошибки: {ex.Message}");
            }
        }

        public static void ShowData(int[,] arr)
        {
            int sizeX = arr.GetLength(0); //число строк
            int sizeY = arr.GetLength(1); //число столбцов
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    Console.Write(arr[i,j] + " ");
                }
                Console.WriteLine("\n");
            }
        }

        //функция возвращает количество ошибок итоговой матрицы по сравнению с эталонной 
        private static int CheckReliability(int[,] arr)
        {
            int counter = 0;
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (arr[i, j] != mBase[i, j])
                    {
                        counter++;
                    }
                }
            }
            return counter;
        }

        //функция возвращает результирующее значение 5 элементов на основе весов
        private static int GetResultCell(int[] arr, int w)
        {
            int[] vals = new int[4]; //массив возможных значений            
            for (int i = 0; i < arr.Length; i++)
            {
                int wVal = mW[w, i];
                switch (arr[i])
                {
                    case 0:
                        vals[0] += wVal; 
                        break;
                    case 1:
                        vals[1] += wVal; 
                        break;
                    case 2:
                        vals[2] += wVal; 
                        break;
                    case 3:
                        vals[3] += wVal; 
                        break;
                    default:
                        break;
                }
            }           

            int max = vals.Max();
            for (int t = 0; t < vals.Length; t++)
            {
                if (vals[t] == max)
                { return t; }
            }
            return -1;
        }

        //Основная функция рассчета достоерности. W - текущий весовой коэффициент
        private static void Engine(int w)
        {
            //матрицы весов в отдельную матрицу mRes
            int[,] m_tmp = new int[m,n];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    int[] cur_cell = new int[5];
                    try
                    {                        
                        cur_cell[0] = m1[i, j];
                        cur_cell[1] = m2[i, j];
                        cur_cell[2] = m3[i, j];
                        cur_cell[3] = m4[i, j];
                        cur_cell[4] = m5[i, j];
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine($"Ошибка при присваивании элемента  m1[{i}][{j}]");
                        //ShowData(m1);
                        //Console.WriteLine(new string('=', 50));
                        throw;
                    }                   
                    m_tmp[i, j] = GetResultCell(cur_cell, w);
                    if (m_tmp[i, j] == -1)
                    {
                        Console.WriteLine("Недопустимое значение");                        
                    }
                }
            }

            int k_tmp = CheckReliability(m_tmp);
            Console.WriteLine($">>>Текущая достоверность: {k_tmp }<<<");
            if (k == -1 || k_tmp < k)
            {
                k = k_tmp;
                mRes = m_tmp;
            }        
    }

        //начало расчета
        public static void Start()
        {
            int counter = 0;
            //for (int i = 0; i < w; i++)
             for (int i = w; i >= 0; i--)
            {
                counter++;
                Engine(i);
                Console.WriteLine($"Итерация: {counter}. Текущий вес.коэф. №: {i}. Макс. достоверность: {k}");
                //Thread.Sleep(150);
                if (k == 0)
                    break;
            }
        }
    }    
}
