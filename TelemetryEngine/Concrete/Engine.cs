//Класс выполняющий оснонвые задачи по комбинациям ВХД

// ---------------------------------------------------------------------------
// Авторское право © ООО "%CompanyName". Авторские права защищены.
// Copyright. JSC «%CompanyName» 2016. All rights reserved
// Компания: %CompanyName
// Подразделение: %Department
// Author: Oleg Vorontsov
// Description: Класс предназначен для анализа исходных данных, отображения возможных комбинаций каналов и интенсивностей помех,
//              расчета выбранной пользователем комбинации  и предоставления результирующего блока данных с сопутствующей информацией (gamma)  
// Rational: Упрощение представления управлением комбинаций исходных данных   
// ---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TelemetryEngine.Interfaces;

namespace TelemetryEngine 
{
    /// <summary>
    /// Класс должен подгружать исходные 30 матриц и формировать таблицу возможных комбинаций 
    /// Далее пользователь должен выбирать комбинации и инициировать работу движка получения обобщенного массива данных    
    /// </summary>
    public class Engine : IEngine
    {
        private string path;
        private int intensityCount; //6
        private int channelCount;

        private List<RawDataMatrix> matrixes;
        private ICollection<RawDataMatrix> resultMatrixes;
        
        //Эталонная матрица
        private Matrix mBase;

        //матрица весов
        private Matrix mW;

        //Оценка эффективности алгоритма
        public string Gamma { get; set; }

        /// <summary>
        /// Конструктор. Инициализирует данные из json в каталоге path
        /// </summary>
        /// <param name="path">путь к json-файлам</param>
        public Engine(string path)
        {
            this.path = path;
            intensityCount = 6;
            IsValid = true;
            InitializeMatrixes();
        }

        /// <summary>
        /// Инициализирует 30/24/18 матриц (в зависимости от количества каналов)
        /// </summary>
        public void InitializeMatrixes()
        {
            matrixes = new List<RawDataMatrix>();

            for (int i = 1; i <= 5; i++)
            {
                for (int j = 1; j <= 6; j++)
                {
                    RawDataMatrix m = new RawDataMatrix(path, i, j);
                    if (m.IsInitialized)
                    {
                        matrixes.Add(m);
                    }
                }
            }

            channelCount = matrixes.Count / 6; //кол-во помеховых обстановок -6, а каналов 5,4 или 3

            switch (channelCount)
            {
                case 5:
                    mW = new Matrix(path, $"mW{channelCount}");
                    break;
                case 4:
                    mW = new Matrix(path, $"mW{channelCount}");
                    break;
                case 3:
                    mW = new Matrix(path, $"mW{channelCount}");
                    break;
                default:
                    throw new Exception($"Не определены исходные матрицы. Количество исходных матриц:{channelCount}");
            }

            if (!mW.IsInitialized)
            {
                throw new Exception($"Матрица весов mW{channelCount} не обнаружена. Завершение приложения");
            }

            //foreach (var item in matrixes)
            //{
            //    Console.WriteLine(item.Name);
            //}

            mBase = new Matrix(path, "mBase");
            if (!mBase.IsInitialized)
            {
                throw new Exception($"Эталонная матрица mBase не обнаружена. Завершение приложения");
            }
        }

        public Matrix Etalon => mBase;

        /// <summary>
        /// Расчет выбранной комбинации каналов/помех
        /// </summary>
        /// <param name="testCase"></param>
        /// <returns>Результирующий блок данных</returns>
        public RawDataMatrix PerformCombination(string testCase)
        {
            //Console.WriteLine("Тестовый вариант:" + testCase);

            resultMatrixes = new List<RawDataMatrix>();
            //Console.WriteLine("Имена матриц для обобщенного массива:");
            for (int i = 0; i < testCase.Length; i++)
            {
                string m_name = $"m{i + 1}_{testCase[i]}";
               // Console.WriteLine(m_name);
                resultMatrixes.Add(matrixes.FirstOrDefault(x => x.Name == m_name));
            }
            //Console.WriteLine(resultMatrixes.Count);
            //Вывод несоответствий базовой матрице
            //foreach (RawDataMatrix m in resultMatrixes)
            //{
            //    Console.WriteLine($"Matrix {m.Name}. Mismathces: {m.GetMismatches(mBase).Count()}");
            //}

            //начало основного алгоритма
            MatrixProcessor processor = new MatrixProcessor(resultMatrixes, mW, mBase);
            RawDataMatrix result = processor.GetResult();
            Gamma = processor.GetGamma();
            return result;
        }

        /// <summary>
        /// Возвращает информацию по всем несоответствиям эталонной матрице
        /// </summary>
        /// <returns></returns>
        public List<MismatchesCoordList> GetMatrixDifference()
        {
            List<MismatchesCoordList> allMatrixesMismatches = new List<MismatchesCoordList>();
            foreach (RawDataMatrix matrix in resultMatrixes)
            {
                MismatchesCoordList coords = matrix.GetMismatches(mBase);
                allMatrixesMismatches.Add(coords);
            }
            return allMatrixesMismatches;
        }

        public bool IsValid { get;}

        /// <summary>
        /// Возвращает набор уникальных комбинаций для заданного числа каналов и интенсивности помех
        /// </summary>       
        /// <returns></returns>
        public IEnumerable<Items> GetFilteredCombinations()
        {
            List<Items> list = new List<Items>();
            int combinations = (int)Math.Pow(intensityCount, channelCount);
            
            for (int i = 0; i < combinations - 1; i++)
            {
                Func<int, int, int, char[]> func = fromDeci;
                string res = GetCaseFiltered(func(intensityCount, channelCount, i));
                if (res != null)
                {
                    list.Add(new Items{Id=0, Name = res } );
                    
                    //Console.WriteLine("global#: " + globalCounter + ". #" + i + ": " + res);
                }
            }

            int counter = 1;
            list = list.OrderBy(x => x.Name).ToList();
            foreach (Items item in list)
            {
                item.Id = counter;
                counter++;
            }
            return list;
        }

        //TODO: после тестов убрать
        static int globalCounter = 0;

        static char ReVal(int num)
        {
            if (num >= 0 && num <= 9)
                return (char)(num + '0');
            else
                return (char)(num - 10 + 'A');
        }

        private static string ReverseString(string srtVarable)
        {
            return new string(srtVarable.Reverse().ToArray());
        }

        /// <summary>
        /// первод из десятичной системы в произвольную
        /// </summary>
        /// <param name="baseNum">основание системы счисления</param>
        /// <param name="size">битовая маска</param>
        /// <param name="inputNum">входное значение</param>
        /// <returns>Возвращает массив с инвертированным значением </returns>
        private static char[] fromDeci(int baseNum, int size, int inputNum)
        {
            string res;
            int index = 0;
            char[] ch = new char[size];

            for (int i = 0; i < ch.Length; i++)
            {
                ch[i] = '0';
            }

            while (inputNum > 0)
            {
                ch[index++] = ReVal(inputNum % baseNum);
                inputNum /= baseNum;
            }
            res = new string(ch);
            char[] charArray = res.ToCharArray();
            Array.Reverse(charArray);
            return charArray;
        }

        /// <summary>
        /// Функция преобразования и фильтрации комбинаций
        /// </summary>
        /// <param name="raw">массив данных преобразованных из десятичной системы в 6-ричную</param>
        /// <returns></returns>
        private static string GetCaseFiltered(char[] raw)
        {
            for (int i = 0; i < raw.Length - 1; i++)
            {
                if (raw[i] < raw[i + 1])
                {
                    return null;
                }
            }
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i]++;
            }
            globalCounter++;
            return ReverseString(new string(raw));
        }        
    }

    /// <summary>
    /// Класс для отображения комбинации и её порядкового номера
    /// </summary>
    public class Items
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
   
}
