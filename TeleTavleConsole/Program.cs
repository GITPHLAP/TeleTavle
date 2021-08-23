using System;
using TeleTavleLibrary;

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
            test.CrawlInformation(sss);
        }
    }
}
