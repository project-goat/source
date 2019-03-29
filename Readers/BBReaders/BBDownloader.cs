using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography.X509Certificates;

namespace GOAT.Readers.BBReaders
{
    class BBDownloader 
    {
        private const string _publicUri = "https://bitbay.net/API/Public";
        public long StartTid { get; set; }
        private long _chunksToLoad;
        private string _currencyPair;
        public List<BBTrade> AllTrades { get; } = new List<BBTrade>();

        public BBDownloader(long startTid = 0, long chunksToLoad = 10, string currencyPair="LTCUSD")
        {
            StartTid = startTid;
            _chunksToLoad = chunksToLoad;
            _currencyPair = currencyPair;
        }

        public void Download()
        {
            Console.WriteLine("Loads trades data from BitBay");
            
            Console.WriteLine($"Reading data from {_publicUri}");

            InitAPI();
            var tid = StartTid;

            for (int idxChunk = 1; idxChunk <= _chunksToLoad; idxChunk++)
            {
                Console.WriteLine($"Chunk {idxChunk}/{_chunksToLoad} since {tid}");

                var tradesChunk = GetTrades(ccy: _currencyPair, tidSince: tid);

                var first = tradesChunk.FirstOrDefault();

                if (first == null || tradesChunk.Count() < 50)
                {
                    break;
                }

                var last = tradesChunk.Last();
                Console.WriteLine($"Range {first.TID}/{first.DateLocal} to {last.TID}/{last.DateLocal}");
                AllTrades.AddRange(tradesChunk);
                tid = last.TID;
            }

            Console.WriteLine($"End of reading data");

            Console.WriteLine($"Downloaded: {AllTrades.Count()} trades");
        }

        //Download trades
        private static List<BBTrade> GetTrades(string ccy, long tidSince = 0)
        {
            string param = tidSince > 0 ? $"?since={tidSince}" : string.Empty;
            Uri address = new Uri($"{_publicUri}/{ccy}/trades.json{param}");

            using (WebClient webClient = new WebClient())
            {
                var stream = webClient.OpenRead(address);
                var jsonSer = new DataContractJsonSerializer(typeof(List<BBTrade>));
                var lst = (List<BBTrade>)jsonSer.ReadObject(stream);
                return lst;
            }
        }


        //Save to file
        public void SaveToJsonFile(string path)
        {
            if (File.Exists(path))
            {
                using (var stream = File.CreateText(path).BaseStream)
                {
                    DataContractJsonSerializer jsonSer = new DataContractJsonSerializer(typeof(List<BBTrade>));
                    jsonSer.WriteObject(stream, AllTrades);
                }
                Console.WriteLine($"Data saved successfully");
            }
            else
            {
                Console.WriteLine($"{path}: File doesn't exist!");
            }
        }
        private void SaveToCsvFile(string path)
        {

        }

        private static void InitAPI()
        {
            ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; //or Ssl3
        }

        private static bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors ssl)
        {
            if (ssl == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }

            Console.WriteLine($"X509Certificate [{certificate.Subject}] PolicyError: '{ssl.ToString()}'");
            return false;
        }
    }
}
