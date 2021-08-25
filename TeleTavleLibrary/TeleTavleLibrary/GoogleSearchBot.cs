using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;
using System.Linq;
using OpenQA.Selenium;

namespace TeleTavleLibrary
{
    public class GoogleSearchBot : Bot
    {

        public List<SearchResult> CrawlInformation(string searchWord)
        {
            List<SearchResult> searchResults = new List<SearchResult>();
            // Get search page 
            HtmlDocument document = new HtmlDocument();
            //load HTML from method returning a string 
            document.LoadHtml(GetSearchPage(searchWord));

            // Get all search results
            HtmlNode[] nodes = null;
            //Send warning if the bot cant find any results
            try
            {
                nodes = document.DocumentNode.SelectNodes("//div[@class='g']").ToArray();
            }
            catch (Exception)
            {
                NewLogEvent(new LogEventArgs($"Der er ikke fundet nogle søge resultater", InformationType.Warning));
            }

            if (nodes is null)
            {
                NewLogEvent(new LogEventArgs($"søge resultater er null", InformationType.Failed));
            }
            //start from number one
            int rankCounter = 1;
            foreach (HtmlNode searchresult in nodes)
            {
                //the link for searchresult
                Uri searchURL = new Uri(searchresult.SelectSingleNode(".//a").Attributes["href"].Value);
                //if the result contains teletavle.dk 
                //TODO: Skift til telefontavlen
                if (searchURL.Host == "rainbow.simbascorner.dk")
                {
                    //Instancer and set some properties
                    SearchResult result = new SearchResult
                    {
                        Url = searchURL.ToString(),
                        Rank = rankCounter,
                        SearchWord = searchWord,
                    };

                    //If the rank is 1 then check if the result is are featured snippet
                    if (result.Rank == 1)
                    {
                        //Get result for featured snippets - xpdopen is a class name for featured snippet
                        if (document.DocumentNode.SelectSingleNode("//div[@class='xpdopen']") != null)
                        {
                            //set the Featuredsnippet text on object
                            result.FeaturedSnippet = document.DocumentNode.SelectSingleNode("//div[@class='wDYxhc']").InnerText;
                        }
                    }

                    //add the object to the final list
                    searchResults.Add(result);
                }

                rankCounter++;
            }
            if (searchResults.Count == 0)
            {
                //No result for search word
                NewLogEvent(new LogEventArgs($"Der blev ikke fundet nogle resulter fra telefontavlen med søgeordet ({searchWord})", InformationType.Information));
            }
            else
            {
                NewLogEvent(new LogEventArgs($"Har fundet søgeresultater fra telefontavlen", InformationType.Successful));
            }
            return searchResults;
        }

        string GetSearchPage(string searchword)
        {
            //google url + search word 
            string url = "https://www.google.com/search?q=" + searchword;

            IWebDriver webDriver = GetChromeDriver();

            //navigate to url
            webDriver.Navigate().GoToUrl(url);

            string page = webDriver.PageSource;

            webDriver.Quit();
            //return page html
            return page;
        }

    }
}
