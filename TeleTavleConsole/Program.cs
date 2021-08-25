using System;
using TeleTavleLibrary;
using System.Diagnostics;
using System.Collections.Generic;

namespace TeleTavleConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //List<string> searchwords = new List<string>();
            //searchwords.Add("rainbow simbascorner");

            //TTManager manager = new TTManager();

            //manager.StartProcess(searchwords);


            //var test = new SEFBot();
            //SearchResultSEF sss = new SearchResultSEF();
            //sss.SearchResult = new SearchResult();
            //sss.SearchResult.Url = "https://teletavletest.elkok.dk/cykelhandlere/brobike-din-autoriserede-cykelforhandler";
            //Console.WriteLine(sss.SearchResult.UrlLocation);
            //Stopwatch watch = new Stopwatch();
            //watch.Start();
            ////test.CrawlInformation(sss);
            var j = new GoogleConsoleIndex();
            var h = j.IndexURL("https://teletavletest.elkok.dk/testside123", "URL_UPDATED");
            //watch.Stop();
            //Console.WriteLine(watch.Elapsed.Milliseconds);

            //Console.WriteLine(sss.Header);
        }
    }
}
