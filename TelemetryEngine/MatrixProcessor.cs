using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TelemetryEngine
{
    public class MatrixProcessor
    {
        
        //const int m = 20; //рамерность матрицы
        //const int n = 32; //рамерность матрицы
        //const int w = 165; //количество кортежей весовых коэффициентов
                
        private int k = -1;              //текущая достоверность

        private ICollection<RawDataMatrix> matrixes;
        private Matrix mW;
        private Matrix mBase;

        private RawDataMatrix mResult;

        public MatrixProcessor(ICollection<RawDataMatrix> matrixes, Matrix mWeights, Matrix mBase)
        {
            this.matrixes = matrixes;
            mW = mWeights;
            this.mBase = mBase;
        }
        
        
        public void ShowData(Matrix arr)
        {

            //int sizeX = arr.GetYSize(); //число строк
            //int sizeY = arr.GetXSize(); //число столбцов
            arr.ProccessFunctionWithData((i, j) =>
            {
                Console.Write(arr[i, j] + " ");
                if (j == arr.GetYSize()-1)
                {
                    Console.WriteLine("\n");
                }
            });

            //for (int i = 0; i < sizeX; i++)
            //{
            //    for (int j = 0; j < sizeY; j++)
            //    {
            //        Console.Write(arr[i,j] + " ");
            //    }
            //    Console.WriteLine("\n");
            //}
        }

        //функция возвращает количество ошибок итоговой матрицы по сравнению с эталонной 
        //private int CheckReliability(RawDataMatrix arr)
        //{
        //    int counter = 0;
        //    arr.ProccessFunctionWithData((i, j) =>
        //    {
        //        if (arr[i, j] != mBase[i, j])
        //        {
        //            counter++;
        //        }
        //    });            
        //    return counter;
        //}

        //функция возвращает результирующее значение 5 элементов на основе весов
        private int GetResultCell(int[] arr, int w)
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
        private void CalculateReliability(int w)
        {
            RawDataMatrix matrix = new RawDataMatrix(mBase.GetXSize(), mBase.GetYSize());
            for (int i = 0; i < mBase.GetXSize(); i++)
            {
                for (int j = 0; j < mBase.GetYSize(); j++)
                {
                    int mCount = matrixes.Count();
                    int[] cur_cell = new int[mCount];
                    try
                    {
                        int counter = 0;
                        foreach (var m in matrixes)
                        {
                            cur_cell[counter] = m[i, j];
                            counter++;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }                   
                    matrix[i, j] = GetResultCell(cur_cell, w);                    
                }
            }

            int k_tmp = matrix.GetMismatches(mBase).Count(); //CheckReliability(matrix);
            Console.WriteLine($">>>Текущая достоверность: {k_tmp }<<<");
            if (k == -1 || k_tmp < k)
            {
                k = k_tmp;
                mResult =matrix;
            }        
    }

        //начало расчета
        public RawDataMatrix GetResult()
        {
            int counter = 0;
            //for (int i = 0; i < w; i++)
            for (int i = mW.GetXSize()-1; i >= 0; i--)
            {
                counter++;
                CalculateReliability(i);
                Console.WriteLine($"Итерация: {counter}. Макс. достоверность: {k}");
                Thread.Sleep(150);
                if (k == 0)
                    break;
            }
            return mResult;
        }
    }    
}
