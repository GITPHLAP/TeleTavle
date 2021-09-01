using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Configuration;
using System.Collections.Specialized;

namespace TeleTavleLibrary
{
    public class SEFBot : JoomlaBot
    {

        public void CrawlInformation(SearchResultSEF input)
        {
            if (input == null)
            {
                NewLogEvent(new LogEventArgs($"SearchResultSEF i SEFBot må ikke være null", InformationType.Failed));
            }
            //Get driver
            IWebDriver chromeDriver = GetChromeDriver();

            SiteLogin(chromeDriver, "https://teletavletest.elkok.dk/administrator/index.php?option=com_sh404sef&c=metas&layout=default&view=metas");
            //SiteLogin(chromeDriver, "https://teletavletest.elkok.dk/administrator/indx.php?");

            URLManagerSearch(chromeDriver, input.SearchResult);

            GetSEFInformation(chromeDriver, input);

            chromeDriver.Quit();

            NewLogEvent(new LogEventArgs($"Har fundet SEF information for {input.SearchResult.Url}", InformationType.Successful));
        }

        void URLManagerSearch(IWebDriver chromeDriver, SearchResult input)
        {
            try
            {
                //Input text to search
                chromeDriver.FindElement(By.Id("search_all")).SendKeys(input.UrlLocation);

                //Click on the search button
                chromeDriver.FindElement(By.CssSelector("#shl-main-searchbar-right-block > div.btn-group.pull-left.hidden-phone > button:nth-child(1)")).Click();
            }
            catch (Exception)
            {
                NewLogEvent(new LogEventArgs($"SEFBot kan ikke søge efter url", InformationType.Failed));

            }


        }

        void GetSEFInformation(IWebDriver chromeDriver, SearchResultSEF searchResultSEF)
        {
            try
            {
                //Find title and description from site
                string header = chromeDriver.FindElement(By.XPath("/html/body/div[2]/section/div/div/div[3]/form/div/div[3]/div[2]/table/tbody/tr[1]/td[4]/div/textarea")).Text;
                string description = chromeDriver.FindElement(By.XPath("/html/body/div[2]/section/div/div/div[3]/form/div/div[3]/div[2]/table/tbody/tr[1]/td[5]/div/textarea")).Text;
                //Add information 
                searchResultSEF.Header = header;
                searchResultSEF.Description = description;
            }
            catch (Exception)
            {
                NewLogEvent(new LogEventArgs($"SEFBot kan ikke finde overskrift og beskrivelse til facebook opslaget", InformationType.Failed));

            }

        }

    }
}
