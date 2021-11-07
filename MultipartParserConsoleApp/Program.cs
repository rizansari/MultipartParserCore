using System;
using System.Net.Http;

namespace MultipartParserConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromMilliseconds(5000);
                var requestUri = "http://10.1.1.182:8080/";
                var stream = httpClient.GetStreamAsync(requestUri).Result;

                MultipartParserCore.MultipartParser parser = new MultipartParserCore.MultipartParser(stream, "DigifortBoundary");
                parser.OnMessage += Parser_OnMessage;
                parser.StartParsing();
            }
        }

        private static void Parser_OnMessage(object sender, MultipartParserCore.ParsedMessage e)
        {
            Console.WriteLine("----start----");
            Console.WriteLine(e.Body);
            Console.WriteLine("-----end-----");
        }
    }
}
