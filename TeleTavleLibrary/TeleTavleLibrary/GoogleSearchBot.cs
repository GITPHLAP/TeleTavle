using System;
using System.Collections.Generic;
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
            string sp = GetSearchPage(searchWord);
            if (sp == null)
            {
                return null;
            }

            //load HTML from method returning a string 
            document.LoadHtml(sp);

            // Get all search results
            HtmlNode[] nodes;

            //Send warning if the bot cant find any results
            try
            {
                nodes = document.DocumentNode.SelectNodes("//div[@class='g']").ToArray();

            }
            catch (Exception ex)
            {
                NewLogEvent(new LogEventArgs($"Der er ikke fundet nogle søge resultater for {searchWord}... {ex.Message}", InformationType.Information));
                return searchResults;
            }

            //start from number one
            int rankCounter = 1;
            int searchwordnum = 1;
            foreach (HtmlNode searchresult in nodes)
            {
                //try to find subnodes 
                HtmlNodeCollection subnodes = searchresult.SelectNodes(".//div[@class='g jNVrwc Y4pkMc']");

                if (subnodes != null)
                {
                    foreach (HtmlNode subnode in subnodes)
                    {
                        //the link for searchresult
                        Uri searchURI = new Uri(subnode.SelectSingleNode(".//a").Attributes["href"].Value);

                        //if the result contains telefontavlen.dk 
                        if (searchURI.Host == "rainbow.simbascorner.dk")
                        {
                            //add the object to the final list
                            searchResults.Add(CreateResultFromNode(document, searchURI, rankCounter, searchWord, searchwordnum));

                            searchwordnum++;

                        }
                        rankCounter++;
                    }

                }
                else
                {
                    //the link for searchresult
                    Uri searchURI = new Uri(searchresult.SelectSingleNode(".//a").Attributes["href"].Value);

                    //if the result contains telefontavlen.dk 
                    if (searchURI.Host == "rainbow.simbascorner.dk")
                    {
                        //add the object to the final list
                        searchResults.Add(CreateResultFromNode(document, searchURI, rankCounter, searchWord, searchwordnum));

                        searchwordnum++;

                    }
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

        SearchResult CreateResultFromNode(HtmlDocument document, Uri searchURI, int rankCounter, string searchWord, int searchwordnum)
        {
            //Instancer and set some properties
            SearchResult result = new SearchResult
            {
                Url = searchURI.ToString(),
                Rank = rankCounter,
                SearchWord = searchWord,
                SearchWordWithNum = $"{searchWord}{searchwordnum}"
            };

            //If the url contains "osdownloads" then tell it to user when its done 
            if (searchURI.AbsolutePath.Contains("osdownloads"))
            {
                //if it doesn't match then there is a alias for the url and the URL prop should be set to the response 
                if (ResponseUri(searchURI) is Uri responseUri
                    && responseUri != searchURI)
                {
                    result.Url = responseUri.ToString();
                }
                else
                {
                    NewLogEvent(new LogEventArgs($"Url'en indeholder osdownloads... URL: {searchURI}", InformationType.Information));
                }
            }

            //If the rank is 1 then check if the result is are featured snippet
            if (result.Rank == 1)
            {
                var featuredText = document.DocumentNode.SelectSingleNode("//*[@id='rso']/div[1]/div/div[1]/div/div[1]/div/div[1]/div/span/span");
                //Get result for featured snippets - xpdopen is a class name for featured snippet
                if (featuredText != null)
                {
                    //set the Featuredsnippet text on object
                    result.FeaturedSnippet = featuredText.InnerText;
                }
            }

            return result;
        }

        private Uri ResponseUri(Uri uri)
        {
            HtmlWeb htmlWeb = new HtmlWeb();

            htmlWeb.Load(uri);

            Uri responseUri = htmlWeb.ResponseUri;

            return responseUri;

        }

        string GetSearchPage(string searchword)
        {
            //google url + search word 
            string url = "https://www.google.com/search?q=" + searchword;

            IWebDriver webDriver = GetChromeDriver();

            if (webDriver == null)
            {
                return null;
            }
            //navigate to url
            webDriver.Navigate().GoToUrl(url);

            string page = webDriver.PageSource;

            HtmlDocument document = new HtmlDocument();
            //load HTML from method returning a string 
            document.LoadHtml(page);

            if (CheckForReCaptcha(document))
            {
                return null;
            }

            page = webDriver.PageSource;

            QuitChromeDriver(webDriver);
            //return page html
            return page;
        }

        bool CheckForReCaptcha(HtmlDocument document)
        {

            if (document.DocumentNode.SelectNodes("//iframe[@title='reCAPTCHA']") is HtmlNodeCollection)
            {

                NewLogEvent(new LogEventArgs($"Processen kan ikke søge på søgeord da der er en reCAPTCHA ", InformationType.Failed));
                return true;
            }

            return false;

        }

    }
}
