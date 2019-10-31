using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            List<RawDataMatrix> matrixes = new List<RawDataMatrix>()
            {
                new RawDataMatrix(path, 1, 1),
                new RawDataMatrix(path, 1, 2),
                new RawDataMatrix(path, 1, 3),
                new RawDataMatrix(path, 1, 4),
                new RawDataMatrix(path, 1, 5),
                new RawDataMatrix(path, 1, 6),

                new RawDataMatrix(path, 2, 1),
                new RawDataMatrix(path, 2, 2),
                new RawDataMatrix(path, 2, 3),
                new RawDataMatrix(path, 2, 4),
                new RawDataMatrix(path, 2, 5),
                new RawDataMatrix(path, 2, 6),

                new RawDataMatrix(path, 3, 1),
                new RawDataMatrix(path, 3, 2),
                new RawDataMatrix(path, 3, 3),
                new RawDataMatrix(path, 3, 4),
                new RawDataMatrix(path, 3, 5),
                new RawDataMatrix(path, 3, 6),

                new RawDataMatrix(path, 4, 1),
                new RawDataMatrix(path, 4, 2),
                new RawDataMatrix(path, 4, 3),
                new RawDataMatrix(path, 4, 4),
                new RawDataMatrix(path, 4, 5),
                new RawDataMatrix(path, 4, 6),

                new RawDataMatrix(path, 5, 1),
                new RawDataMatrix(path, 5, 2),
                new RawDataMatrix(path, 5, 3),
                new RawDataMatrix(path, 5, 4),
                new RawDataMatrix(path, 5, 5),
                new RawDataMatrix(path, 5, 6)
            };

            foreach (var item in matrixes)
            {
                Console.WriteLine(item.Name);
            }

            //exceptions = new List<string>();
            //exceptions = new string[];
            //6^5
            for (int i = 0; i < 7776; i++)
            {
                //TODO: отсеять лишние, оставить 251 комбинацию
                string res = fromDeci(6, i);
                if (res!=null)
                {
                    Console.WriteLine("global#: "+ globalCounter+ ". #"+ i +": "+ res);
                }
            }        
    }

        static int globalCounter = 0;
        static List<string> exceptions = new List<string>(); 
       static  char reVal(int num)
        {
            if (num >= 0 && num <= 9)
                return (char)(num + '0');
            else
                return (char)(num - 10 + 'A');
        }

        

        public static string fromDeci(int baseNum, int inputNum)
        {
            string res;
            int index = 0; 

            char[] ch = new char[]{ '0', '0', '0', '0', '0'};
            while (inputNum > 0)
            {
                ch[index++] = reVal(inputNum % baseNum);
                inputNum /= baseNum;
            }
            res =  new string(ch);
            char[] charArray = res.ToCharArray();
            

            string result = new string(charArray);            

            foreach (var item in exceptions)
            {
                //TODO: Добавить случаи, когда 2 крайних, 3, крайних и т.д. совпадают
                if (item == new string(result.ToCharArray().Reverse().ToArray()))
                {                  
                    return null;
                }
                
                
            }
            
            exceptions.Add(result);            
            globalCounter++;

            //Console.WriteLine("=>" + res);
            Array.Reverse(charArray);            
            return new string(charArray);
        }

        
    }
}
