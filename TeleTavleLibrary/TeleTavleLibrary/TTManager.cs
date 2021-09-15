using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace TeleTavleLibrary
{
    public class TTManager
    {
        public event EventHandler<LogEventArgs> LogEvent;

        // global variables
        GoogleSearchBot gSearchBot;

        SEFBot sefBot;

        PingBot pingbot;

        GoogleConsoleIndex gConsole;

        /// <summary>
        /// This starts the whole proces
        /// </summary>
        /// <param name="searchWords"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public List<SearchResultSEF> StartProcessParallel(List<string> searchWords, CancellationToken token)
        {
            if (searchWords.Count > 0 || searchWords != null)
            {
                //List<Task<List<SearchResult>>> tasks = new List<Task<List<SearchResult>>>();

                List<SearchResult> searchResults = new List<SearchResult>();

                CheckToken(token);
                try
                {
                    //create parallel foreach with every searchword for every searchword add every searchresult from GetSearchInformation to searchResults
                    Parallel.ForEach(searchWords, searchWord => GetSearchInformation(searchWord).ForEach(searchresult => searchResults.Add(searchresult)));
                }
                catch (Exception)
                {
                    CheckToken(token);
                }

                CheckToken(token);

                //This is where all the found SEF's are saved
                List<SearchResultSEF> finalSEFList = new List<SearchResultSEF>();

                //Find all the sef information and ping and index the result
                Parallel.ForEach(searchResults, searchResult =>
                    {
                        if (token.IsCancellationRequested)
                        {
                            return;
                        }

                        SearchResultSEF foundSefs = FindSef(searchResult, token);

                        if (token.IsCancellationRequested)
                        {
                            return;
                        }

                        //add found sef to final sef list
                        finalSEFList.Add(foundSefs);

                        //Ping and index the results
                        PingSearchResult(foundSefs);

                    });
                CheckToken(token);

                try
                {
                    var IndexedList = gConsole.IndexBatchURL(finalSEFList.Select(x => x.SearchResult.Url).ToList(), "URL_UPDATED").Result;
                }
                catch (Exception e)
                {
                    NewLogEvent(this, new LogEventArgs($"Kunne ikke indexere siderne...  {e}", InformationType.Failed));
                }

                return finalSEFList;
            }
            else
            {
                NewLogEvent(this, new LogEventArgs("Der er ingen søgeord i listen, kan ikke fortsætte...", InformationType.Warning));
                return null;
            }
        }

        SearchResultSEF FindSef(SearchResult searchResult, CancellationToken token)
        {
            CheckToken(token);
            SearchResultSEF sefSearchResult = SEFInformation(searchResult);

            
            sefSearchResult.Header = "TEST123"; //TODO: This line is for test issue remove it

            return sefSearchResult;
        }

        //TODO: This part is for test issue remove it
        List<SearchResult> GetSearchInformationTest(string searchWords)
        {
            List<SearchResult> test = new List<SearchResult>();

            for (int i = 0; i < 2; i++)
            {
                test.Add(new SearchResult { Url = "https://rainbow.simbascorner.dk/", Rank = i, SearchWord = searchWords });
            }
            Task.Delay(800);


            return test;
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

        }

        public void SubscribeEvents()
        {
            gSearchBot.LogEvent += NewLogEvent;

            sefBot.LogEvent += NewLogEvent;

            pingbot.LogEvent += NewLogEvent;

            gConsole.LogEvent += NewLogEvent;

            //Must read the credentials from file after events
            gConsole.GetGoogleCredential();
        }

        /// <summary>
        /// Checks if the user has cancelled the token
        /// </summary>
        /// <param name="token"></param>
        void CheckToken(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
        }

        private void NewLogEvent(object sender, LogEventArgs e)
        {
            LogEvent?.Invoke(sender, e);

            Console.WriteLine($"{e.Time}: {e.Message}");

        }
    }
}
