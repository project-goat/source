using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography.X509Certificates;

namespace BitBayLoader
{
    class Program
    {
        private const string PublicUri="https://bitbay.net/API/Public";
        private const long START_TID = 10000;
        private const long CHUNKS_TO_LOAD = 10;
        private const string CURRENCY_PAIR = "LTCUSD";
        static void Main(string[] args)
        {
            Console.WriteLine("Loads trades data from BitBay");

            // if(false)
            // {
            //     Console.WriteLine("Right expression: <trade_nr_since> <number_of_chunks_to_load> <currency_pair> <out_file>");
            //     Console.WriteLine("<trade_nr_since> - ID of the trade to start, 0 (zero) is first trade");
            //     Console.WriteLine("<number_of_chunks_to_load> - loads by 50-trades chunk so how many chunks needs to load, e.g. 5 = 250 trades");
            //     Console.WriteLine("<currency_pair> - BTCPLN, ETHPLN, LSKPLN, ?LSKUSD");
            //     Console.WriteLine("<out_file> - name of .json file to save trades data");
            //     return;
            // }

            long startTID = START_TID;
            long chunksToLoad = CHUNKS_TO_LOAD;
            string currencyPair = CURRENCY_PAIR;
            //string fileToSave = args[3];

            Console.WriteLine($"Reading data from {PublicUri}");

            InitAPI();
            var allTrades = new List<BBTrade>(); //All trades
            var tid = startTID;
            for(int idxChunk = 1; idxChunk <= chunksToLoad; idxChunk++)
            {
                Console.WriteLine($"Chunk {idxChunk}/{chunksToLoad} since {tid}");

                var tradesChunk = GetTrades(ccy : currencyPair, tidSince: tid);

                var first = tradesChunk.FirstOrDefault();

                if(first == null || tradesChunk.Count() < 50)
                {
                    break;
                }

                var last = tradesChunk.Last();
                Console.WriteLine($"Range {first.TID}/{first.DateLocal} to {last.TID}/{last.DateLocal}");
                //Console.WriteLine($"Range {first.TID}/{first.Date} to {last.TID}/{last.Date}");
                allTrades.AddRange(tradesChunk);
                tid=last.TID;
            }

            Console.WriteLine($"End of reading data");

            foreach(var Trade in allTrades)
            {
                Console.WriteLine($"date: {Trade.DateLocal}, price: {Trade.Price}, amount: {Trade.Amount}, tid: {Trade.Amount}");
                
            }
            Console.WriteLine(allTrades.Count());

        }

        private static List<BBTrade> GetTrades(string ccy, long tidSince = 0)
        {
            string param = tidSince > 0 ? $"?since={tidSince}" : string.Empty;
            Uri address = new Uri($"{PublicUri}/{ccy}/trades.json{param}");

            using(WebClient webClient = new WebClient())
            {
                var stream = webClient.OpenRead(address);
                var jsonSer = new DataContractJsonSerializer(typeof(List<BBTrade>));
                var lst = (List<BBTrade>) jsonSer.ReadObject(stream);
                return lst;
            }
        }

        private static void SaveToFile(string path, List<BBTrade> allTrades)
        {
            using(var stream = File.CreateText(path).BaseStream)
            {
                DataContractJsonSerializer jsonSer = new DataContractJsonSerializer(typeof(List<BBTrade>));
                jsonSer.WriteObject(stream, allTrades);
            }
            Console.WriteLine($"Data saved successfully");
        }

        private static void InitAPI()
        {
            ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; //Ssl3
        }

        private static bool ValidateRemoteCertificate(object sender,X509Certificate certificate,X509Chain chain,SslPolicyErrors ssl)
        {
            if(ssl == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }

            Console.WriteLine($"X509Certificate [{certificate.Subject}] PolicyError: '{ssl.ToString()}'");
            return false;
        }
    }
}