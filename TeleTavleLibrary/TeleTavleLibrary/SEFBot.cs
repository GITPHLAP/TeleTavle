using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace TeleTavleLibrary
{
    public class SEFBot :Bot, IInformationCrawler<SearchResultSEF, SearchResult>
    {
        public SearchResultSEF CrawlInformation(SearchResult input)
        {
            IWebDriver chromeDriver = GetChromeDriver();

            chromeDriver.Navigate().GoToUrl("https://teletavletest.elkok.dk/administrator/index.php?option=com_sh404sef&c=urls&layout=default&view=urls");



            return null;
        }
    }
}
