﻿using System;
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
        /// <summary>
        /// This starts the whole proces
        /// </summary>
        /// <param name="searchWords"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public List<SearchResultSEF> StartProcessParallelAsync(List<string> searchWords, CancellationToken token)
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
                        CheckToken(token);

                        SearchResultSEF foundSefs = FindSef(searchResult, token);

                        //add found sef to final sef list
                        finalSEFList.Add(foundSefs);

                        //Ping and index the results
                        PingSearchResult(foundSefs);

                        CheckToken(token);

                        IndexURL(foundSefs.SearchResult.Url);

                        CheckToken(token);

                    });

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
            //SearchResultSEF sefResults = new SearchResultSEF();

            CheckToken(token);
            SearchResultSEF sefSearchResult = SEFInformation(searchResult);

            //TODO: TEST skal fjernes
            sefSearchResult.Header = "TEST123";

            return sefSearchResult;
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
