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
            //Get driver
            IWebDriver chromeDriver = GetChromeDriver();

            SiteLogin(chromeDriver, "https://teletavletest.elkok.dk/administrator/index.php?option=com_sh404sef&c=metas&layout=default&view=metas");

            URLManagerSearch(chromeDriver, input.searchResult);

            GetSEFInformation(chromeDriver, input);

            chromeDriver.Quit();

            NewLogEvent(new LogEventArgs($"Har fundet SEF information for {input.searchResult.Url}", InformationType.Successful));
        }

        void URLManagerSearch(IWebDriver chromeDriver, SearchResult input)
        {
            //Input text to search
            chromeDriver.FindElement(By.Id("search_all")).SendKeys(input.UrlLocation);
            //Click on the search button
            //chromeDriver.FindElement(By.XPath("/html/body/div[2]/section/div/div/div[3]/form/div/div[1]/div[2]/div[2]/button[1]")).Click();
            chromeDriver.FindElement(By.CssSelector("#shl-main-searchbar-right-block > div.btn-group.pull-left.hidden-phone > button:nth-child(1)")).Click();

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
                //TODO: implement log to tell user something is wrong.

            }
            //Put title and description into object

        }

    }
}
