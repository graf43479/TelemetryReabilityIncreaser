//Класс выполняющий основные вычисления для получения итогового блока данных

// ---------------------------------------------------------------------------
// Авторское право © ООО "%CompanyName". Авторские права защищены.
// Copyright. JSC «%CompanyName» 2016. All rights reserved
// Компания: %CompanyName
// Подразделение: %Department
// Author: Oleg Vorontsov
// Description: Класс получает данные для выбранной комбинации ВХД, а далее осуществляет перебор на основе весовых коэффициентов.
//              В итоге получается блок данных с минимальным количеством поврежденных данных. В самом худшем случае количество ошибок 
//              равно минимальному одного из исходных блоков данных (автовыбор)
//              Также осуществляется показатель эффективности алгоритма GetGamma и E
// Rational: Фрагментация основных расчетов для повышения наглядности   
// ---------------------------------------------------------------------------

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using TelemetryEngine.Interfaces;

namespace TelemetryEngine
{
    public class MatrixProcessor : IMatrixProcessor
    {           
        private int k = -1;              //Итерация достоерности (1-166)
        private int N = -1;                //Текущая достоверность

        private IEnumerable<RawDataMatrix> matrixes;
        private Matrix mW;
        private Matrix mBase;
        private int nm=1000; //для 166-ой комбинации
        
        private RawDataMatrix mResult;

        public MatrixProcessor(IEnumerable<RawDataMatrix> matrixes, Matrix mWeights, Matrix mBase)
        {          
            this.mBase = mBase;
            mW = mWeights;
            //матрицы ранжируются по убыванию достоверности 
            this.matrixes = matrixes.OrderByDescending(x=>x.GetMismatches(mBase).N).ToList();         
        }

        public int Nm => nm;
        public int Na => matrixes.Min(x => x.GetMismatches(mBase).N);
        public int Ns => mResult.GetMismatches(mBase).N;

        /// <summary>
        /// Оценка эффективности алгоритма
        /// </summary>
        /// <returns></returns>
        public string GetGamma()
        {
            //nm
            int gs = ((new int[] { Na, Nm }).Min() - Ns)*100/((new int[] { Na, Nm, Ns }).Min()+24);
            const int gamma = 15;

            int E;

            if (gs >= gamma)
            {
                E=1;
            }
            else if (gs <= -gamma)
            {
                E=-1;
            }
            else
            {
                E = 0;
            }

            return $"Na={Na}. Nm = {Nm}. Ns = {Ns}. k={k}. gs={gs}. E={E}";
        }

        /// <summary>
        /// функция возвращает результирующее значение 5 элементов на основе весов
        /// </summary>
        /// <param name="arr">Массив значений [i,j] элемента кажого блока данных. Возможные значения: 0,1,2,3</param>
        /// <param name="w">порядковый номер кортежа весовых коэффициентов</param>
        /// <returns></returns>
        private int GetResultCell(int[] arr, int w)
        {
            int[] vals = new int[4]; //массив возможных значений            
            for (int i = 0; i < arr.Length; i++)
            {
                //if(w=)
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
                        Console.WriteLine("Нештатная ситуация");
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

        /// <summary>
        /// Основная функция рассчета достоверности. w - текущий весовой коэффициент
        /// </summary>
        /// <param name="w"></param>
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

            int N_tmp = matrix.GetMismatches(mBase).N; //CheckReliability(matrix);
            if (N_tmp < 52)
            {
                int p = N_tmp; 
            }
            
            if (w == mW.GetXSize()-1)
            {
                nm = N_tmp;
                Console.WriteLine("NM:===>" + nm);
            }
           // Console.WriteLine($">>>Текущая достоверность: {N_tmp }<<<");
            if (N == -1 || N_tmp < N)
            {
                N = N_tmp;
                k = w;
                Console.WriteLine($"K:====>{k}. N={N}");
                mResult =matrix;
            }        
    }
        
        /// <summary>
        /// Функция просчитывает для всех кортежей матрицы весов наиболее достоверный вариант 
        /// </summary>
        /// <returns>Самый достоверный вариант блока данных</returns>
        public RawDataMatrix GetResult()
        {
            int counter = 0;
           // for (int i = 0; i < mW.GetXSize(); i++)
            for (int i = mW.GetXSize()-1; i >= 0; i--)
            {
                counter++;
                CalculateReliability(i);
               // Console.WriteLine($"Итерация: {counter}. Макс. достоверность: {N}");                
                if (N == 0)
                    break;
            }                      
            return mResult;
        }
    }    
}
