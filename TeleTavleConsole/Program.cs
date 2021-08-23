using System;
using TeleTavleLibrary;
using System.Diagnostics;

namespace TeleTavleConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = new SEFBot();
            SearchResultSEF sss = new SearchResultSEF();
            sss.searchResult = new SearchResult();
            sss.searchResult.Url = "https://teletavletest.elkok.dk/cykelhandlere/brobike-din-autoriserede-cykelforhandler";
            Console.WriteLine(sss.searchResult.UrlLocation);
            Stopwatch watch = new Stopwatch();
            watch.Start();
            test.CrawlInformation(sss);
            watch.Stop();
            Console.WriteLine(watch.Elapsed.Seconds);

            Console.WriteLine(sss.Header);
        }
    }
}
