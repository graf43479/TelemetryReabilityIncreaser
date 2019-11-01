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
            /*
             TelemetryEngine.TelemetryEngine.ShowData(TelemetryEngine.TelemetryEngine.m1);
             Console.WriteLine(new string('-',50));
             TelemetryEngine.TelemetryEngine.ShowData(TelemetryEngine.TelemetryEngine.mW);

             TelemetryEngine.TelemetryEngine.Start();
             TelemetryEngine.TelemetryEngine.ShowData(TelemetryEngine.TelemetryEngine.mRes);
             */

            //Matrix matrix = new Matrix(Directory.GetCurrentDirectory() + "/JsonData", "m1_4");

            string path = Directory.GetCurrentDirectory() + "/JsonData";

            List<RawDataMatrix> matrixes = new List<RawDataMatrix>();

            for (int i = 1; i <=5; i++)
            {
                for (int j = 1; j <=6; j++)
                {
                    matrixes.Add(new RawDataMatrix(path, i, j));
                }
            }
           
            foreach (var item in matrixes)
            {
                Console.WriteLine(item.Name);
            }

            Matrix mBase = new Matrix(path, "mBase");
            Matrix mW = new Matrix(path, "mW5");

            IEnumerable<string> allCases = GetFilteredCombinations(6, 5);
            //проверка на уникальность
            //Console.WriteLine($"{allCases.Count()}/{allCases.Distinct().Count()}");

            List<RawDataMatrix> resultMatrixes = new List<RawDataMatrix>();

            string testCase = allCases.Skip(190).First();
            Console.WriteLine("Тестовый вариант:" + testCase);
            Console.WriteLine("Имена матриц для обобщенного массива:");
            for (int i = 0; i < testCase.Length; i++)
            {
                string m_name = $"m{i + 1}_{testCase[i]}";
                Console.WriteLine(m_name);
                resultMatrixes.Add(matrixes.FirstOrDefault(x => x.Name == m_name));
            }
            //Console.WriteLine(resultMatrixes.Count);
            //Вывод несоответствий базовой матрице
            foreach (RawDataMatrix m in resultMatrixes)
            {
                Console.WriteLine($"Matrix {m.Name}. Mismathces: {m.GetMismatches(mBase)}");
            }

            //начало основного алгоритма
            //public TelemetryEngine(List<RawDataMatrix> matrixes, Matrix mWeights, Matrix mBase)
            MatrixProcessor processor = new MatrixProcessor(resultMatrixes, mW, mBase);
            processor.Start();

        }

        /// <summary>
        /// Возвращает набор уникальных комбинаций для заданного числа каналов и интенсивности помех
        /// </summary>
        /// <param name="intensityCount">количество уровней помех</param>
        /// <param name="channelCount">количество каналов</param>
        /// <returns></returns>
        private static IEnumerable<string> GetFilteredCombinations(int intensityCount, int channelCount)
        {
            List<string> list = new List<string>();
            int combinations = (int)Math.Pow(intensityCount, channelCount);
            for (int i = 0; i < combinations-1; i++)
            {
                Func<int, int, int, char[]> func = fromDeci;
                string res = GetCaseFiltered(func(intensityCount, channelCount, i));
                if (res != null)
                {
                    list.Add(res);
                    //Console.WriteLine("global#: " + globalCounter + ". #" + i + ": " + res);
                }
            }
            return list;
        }
              
        static int globalCounter = 0;
        
        static  char ReVal(int num)
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
            char[] ch = new char[size] ;

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
}
