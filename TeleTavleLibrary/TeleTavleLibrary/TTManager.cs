using System;
using System.Collections.Generic;
using System.Text;

namespace TeleTavleLibrary
{
    public class TTManager
    {
        public event EventHandler<LogEventArgs> LogEvent;


        
        // globla variables
        GoogleSearchBot gSearchBot;

        SEFBot sefBot;

        PingBot pingbot;

        GoogleConsoleIndex gConsole;

        public void StartProcess(List<string> searchWords)
        {

            foreach (var searchWord in searchWords)
            {
                //Get search information and add it to a list
                List<SearchResult> searchResults = GetSearchInformation(searchWord);

                foreach (var result in searchResults)
                {
                    SearchResultSEF sefSearchResult = SEFInformation(result);

                    //TODO: TEST skal fjernes
                    sefSearchResult.Header = "TEST123";

                    //Ping the result
                    PingSearchResult(sefSearchResult);

                    IndexURL(result.Url);
                }
            }
        }

        List<SearchResult> GetSearchInformation(string searchWords)
        {
            return gSearchBot.CrawlInformation(searchWords);
        }

        SearchResultSEF SEFInformation(SearchResult searchResult)
        {
            SearchResultSEF sEFResult = new SearchResultSEF(searchResult);

            sefBot.CrawlInformation(sEFResult);

            return sEFResult;
        }

        void IndexURL(string urlstring)
        {
            //Send Update for url 
            gConsole.IndexURL(urlstring, "URL_UPDATED");
        }

        void PingSearchResult(SearchResultSEF sefSearchresult)
        {
            pingbot.DoOnWebsite(sefSearchresult);
        }

        public TTManager()
        {
            gSearchBot = new GoogleSearchBot();

            sefBot = new SEFBot();

            pingbot = new PingBot();

            gConsole = new GoogleConsoleIndex();

            gSearchBot.LogEvent += NewLogEvent;

            sefBot.LogEvent += NewLogEvent;

            pingbot.LogEvent += NewLogEvent;

            gConsole.LogEvent += NewLogEvent;

        }

        private void NewLogEvent(object sender, LogEventArgs e)
        {
            LogEvent?.Invoke(sender, e);

            //Console.WriteLine($"{e.Time}: {e.Message}");

        }
    }
}
