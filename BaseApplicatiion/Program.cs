using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Text.Json;

namespace BaseApplicatiion
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var a = new int[4, 5] { { 1, 2, 3, 4, 5 }, { 6,7 , 8, 9, 10 }, { 11,12,13,14,15}, { 16,17,18,19,20} };
            //string result = JsonConvert.SerializeObject(a);

            using (FileStream fs = new FileStream("user.json", FileMode.OpenOrCreate))
            {
               byte[] array = System.Text.Encoding.Default.GetBytes(JsonConvert.SerializeObject(a));
               fs.Write(array, 0, array.Length);             
            }

            Console.WriteLine("Deserialization");

            using (StreamReader reader = new StreamReader("user.json"))
            {
                string json = reader.ReadToEnd();
                var b = new int[4, 5];
                // dynamic c = JsonConvert.DeserializeObject(json);

                JsonSerializerSettings serSettings = new JsonSerializerSettings();
                serSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                b = JsonConvert.DeserializeObject<int[,]>(json, serSettings);


                if (b != null)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            Console.Write(b[i,j] + " ");
                        }
                        Console.WriteLine();
                    }
                }
            }

            //using (FileStream fs = new FileStream("user.json", FileMode.OpenOrCreate))
            //{
            //    var b = new int[4, 5];
            //    b = JsonConvert.DeserializeObject<int[,]>(fs);  //await JsonSerializer.Deserialize<Person>(fs);
            //    Console.WriteLine($"Name: {restoredPerson.Name}  Age: {restoredPerson.Age}");
            //}
            //var serializer = new JsonSerializer();

            //using (var sr = new StreamReader(stream))
            //using (var jsonTextReader = new JsonTextReader(sr))
            //{
            //    return serializer.Deserialize(jsonTextReader);
            //}
        }
    }
}
