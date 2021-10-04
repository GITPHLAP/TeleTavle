using System;
using OpenQA.Selenium;

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

            //TODO: This part is for test issue changed it
            //SiteLogin(chromeDriver, "https://telefontavlen.dk/administrator/index.php?option=com_sh404sef&c=metas&layout=default&view=metas");
            SiteLogin(chromeDriver, "https://teletavletest.elkok.dk/administrator/index.php?option=com_sh404sef&c=metas&layout=default&view=metas");

            URLManagerSearch(chromeDriver, input.SearchResult);

            GetSEFInformation(chromeDriver, input);

            QuitChromeDriver(chromeDriver);

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
                //to select specific row 
                int rownum = 1;
                //Get table tbody with all rows 
                IWebElement table = chromeDriver.FindElement(By.CssSelector("#adminForm > div > div.shl-main-list-wrapper.shl-no-margin-left.span12.shl-main-list-wrapper-padding > div.span10.shl-no-margin-left > table > tbody"));
                var rows = table.FindElements(By.TagName("tr"));

                foreach (var row in rows)
                {
                    var urlTD = row.FindElement(By.XPath($"/html/body/div[2]/section/div/div/div[3]/form/div/div[3]/div[2]/table/tbody/tr[{rownum}]/td[3]"));

                    //need to be a list to avoid error if an a tag could not be found
                    var a = urlTD.FindElement(By.TagName("a"));

                    //get the first part of the a tag text, its should match with UrlLocation
                    if (a.Text.Split("(")[0] == searchResultSEF.SearchResult.UrlLocation + "\r\n")
                    {
                        //Find title and description from site
                        string header = chromeDriver.FindElement(By.XPath($"/html/body/div[2]/section/div/div/div[3]/form/div/div[3]/div[2]/table/tbody/tr[{rownum}]/td[4]/div/textarea")).Text;
                        string description = chromeDriver.FindElement(By.XPath($"/html/body/div[2]/section/div/div/div[3]/form/div/div[3]/div[2]/table/tbody/tr[{rownum}]/td[5]/div/textarea")).Text;

                        //Add information 
                        searchResultSEF.Header = string.IsNullOrEmpty(header) ? searchResultSEF.SearchResult.UrlLocation : header; // ?: operator do the same as if else
                        searchResultSEF.Description = description;
                    }

                    rownum++;
                }
            }
            catch (Exception)
            {

                NewLogEvent(new LogEventArgs($"SEFBot kan ikke finde overskrift og beskrivelse til facebook opslaget URL: {searchResultSEF.SearchResult.Url}", InformationType.Failed));

            }

        }

    }
}
