using System;
using GOAT.Readers.BBReaders;

namespace GOAT
{
    class Program
    {
        static void Main(string[] args)
        {
            BBDownloader bbd = new BBDownloader();

            bbd.Download();
            

            bbd.SaveToJsonFile("C:/Users/old-gentleman/source/repos/ConsoleApp1/ConsoleApp1/Resources/BitBayJson.json");

            Console.ReadKey();

        }
    }
}
