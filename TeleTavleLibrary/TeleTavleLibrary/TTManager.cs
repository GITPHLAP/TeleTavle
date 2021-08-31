using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        public void StartProcess(List<string> searchWords)
        {
            if (searchWords.Count > 0 || searchWords != null)
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
            else
            {
                NewLogEvent(this, new LogEventArgs("Der er ingen søgeord i listen, kan ikke fortsætte...", InformationType.Warning));
            }
        }

        public async Task<List<SearchResultSEF>> StartProcessParallelAsync(List<string> searchWords, CancellationToken token)
        {
            if (searchWords.Count > 0 || searchWords != null)
            {

                List<Task<List<SearchResult>>> tasks = new List<Task<List<SearchResult>>>();

                CheckToken(token);
                try
                {
                    Parallel.ForEach(searchWords, searchWord => { tasks.Add(Task.Run(() => GetSearchInformation(searchWord))); });

                }
                catch (Exception)
                {
                    CheckToken(token);
                }


                //Search results that need to find their SEF
                List<SearchResult>[] searchResults = await Task.WhenAll(tasks);

                CheckToken(token);

                //This is where all the found SEF's are saved
                List<SearchResultSEF> searchResultSEFList = new List<SearchResultSEF>();

                Parallel.ForEach(searchResults, searchResult =>
                    {
                        CheckToken(token);
                        List<SearchResultSEF> foundSEFs = IndexPingAndFindSEF(searchResult, token);

                        //add the found SEF to another list with all the SEF
                        for (int i = 0; i < foundSEFs.Count; i++)
                        {
                            CheckToken(token);
                            searchResultSEFList.Add(foundSEFs[i]);
                        }

                    });

                return searchResultSEFList;
            }
            else
            {
                NewLogEvent(this, new LogEventArgs("Der er ingen søgeord i listen, kan ikke fortsætte...", InformationType.Warning));
                return null;
            }
        }

        //TODO: Refactor this method name dont make sense 
        List<SearchResultSEF> IndexPingAndFindSEF(List<SearchResult> searchResult, CancellationToken token)
        {
            List<SearchResultSEF> sefResults = new List<SearchResultSEF>();

            foreach (SearchResult result in searchResult)
            {
                CheckToken(token);
                SearchResultSEF sefSearchResult = SEFInformation(result);

                //TODO: TEST skal fjernes
                sefSearchResult.Header = "TEST123";

                sefResults.Add(sefSearchResult);

                //Ping the result
                PingSearchResult(sefSearchResult);

                CheckToken(token);

                IndexURL(result.Url);

                CheckToken(token);
            }
            return sefResults;
        }

        //TODO:Test SKAL FJERNES
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

            //Must read the credentials from file after event
            gConsole.GetGoogleCredential();
        }

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
