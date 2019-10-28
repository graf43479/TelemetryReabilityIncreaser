using System;
using System.Linq;
using System.Text.Json;

namespace TelemetryEngine
{
    public static class TelemetryEngine
    {
        static TelemetryEngine()
        {
            m1 = new int[m, n];
            m2 = new int[m, n];
            m3 = new int[m, n];
            m4 = new int[m, n];
            m5 = new int[m, n];
            mRes = new int[m, n];
            mBase = new int[m, n];
            mW = new int[w, 5];

            //TODO: добавить заполнение матриц m1-5, mBase, mW
        }

        const int m = 32; //рамерность матрицы
        const int n = 20; //рамерность матрицы
        const int w = 165; //количество кортежей весовых коэффициентов

        public static int[,] m1;    //массив для 1го канала связи 
        public static int[,] m2;    //массив для 2го канала связи
        public static int[,] m3;    //массив для 3го канала связи
        public static int[,] m4;    //массив для 4го канала связи
        public static int[,] m5;    //массив для 5го канала связи

        public static int[,] mRes;    //результирующий массив
        public static int[,] mBase;   //эталонный массив массив
        public static int[,] mW;      //массив неизменных весовых коэффициентов

        private static int k = -1;             //текущая достоверность
                

        //функция возвращает количество ошибок итоговой матрицы по сравнению с эталонной 
        public static int CheckReliability(int[,] arr)
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
        public static int GetResultCell(int[] arr, int w)
        {
            int[] vals = new int[3]; //массив возможных значений
            for (int i = 0; i < 5; i++)
            {
                switch (arr[i])
                {
                    case 0:
                        vals[0] += mW[w, i];
                        break;
                    case 1:
                        vals[1] += mW[w, i];
                        break;
                    case 2:
                        vals[2] += mW[w, i];
                        break;
                    case 3:
                        vals[3] += mW[w, i];
                        break;
                    default:
                        break;
                }

                int max = vals.Max();
                for (int t = 0; t < vals.Length; t++)
                {
                    if (vals[t] == max)
                    { return t;  }
                }
                
            }
            return -1;
        }

        //Основная функция рассчета достоерности. W - текущий весовой коэффициент
        public static void Engine(int w)
        {
            //TODO:Пройти по каждому первому(вторму, nxm-му) элементу каждой из 5 матриц и выписать значение на основе 
            //матрицы весов в отдельную матрицу mRes
            int[,] m_tmp = new int[m,n];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    int[] cur_cell = new int[5];
                    cur_cell[0] = m1[i, j];
                    cur_cell[1] = m2[i, j];
                    cur_cell[2] = m3[i, j];
                    cur_cell[3] = m4[i, j];
                    cur_cell[4] = m5[i, j];


                    m_tmp[i, j] = GetResultCell(cur_cell, w);
                    if (m_tmp[i, j] == -1)
                    {
                        //TODO: написать в лог
                    }
                }
            }

            int k_tmp = CheckReliability(m_tmp);
            if (k == -1 || k_tmp < k)
            {
                k = k_tmp;
                mRes = m_tmp;
            }        
    }

        //начало расчета
        static void Start()
        {
            for (int i = w; i >= 0; i--)
            {
                Engine(i);
                if (k == 0)
                    break;
            }

            //в итоге mRes должен быть аполнен масимально достоерными данными

        }

    }

    
}
