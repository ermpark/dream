using System;
using System.IO;
using System.Net;
using System.Text;
using AngleSharp.Parser.Html;
namespace Dream
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        /// <summary>
        /// 获取页面html内容
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        static public string GetHtml(string url)
        {
            HttpWebRequest myReq =
            (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)myReq.GetResponse();
            // Get the stream associated with the response.
            Stream receiveStream = response.GetResponseStream();
            // Pipes the stream to a higher level stream reader with the required encoding format. 
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            return readStream.ReadToEnd();
        }
    }
}
