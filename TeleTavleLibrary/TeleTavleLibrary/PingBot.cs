﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace TeleTavleLibrary
{
    public class PingBot : JoomlaBot, IDoCrawler
    {
        public bool DoOnWebsite(SearchResultSEF searchResultSEF)
        {
            //Get driver
            IWebDriver chromeDriver = GetChromeDriver();

            SiteLogin(chromeDriver, "https://teletavletest.elkok.dk/administrator/index.php?option=com_jmap&task=pingomatic.display");

            if (!string.IsNullOrEmpty(searchResultSEF.Header))
            {
                CreateAndSavePing(searchResultSEF, chromeDriver);
            }

            try
            {
                SendPing(chromeDriver);
            }
            catch (Exception e)
            {
                NewLogEvent(new LogEventArgs($"Fejl i at sende ping:  ''{searchResultSEF.SearchResult.Url}''  {e}",
                    InformationType.Failed));
            }

            chromeDriver.Quit();

            NewLogEvent(new LogEventArgs($"Har pinget:  {searchResultSEF.SearchResult.Url}", InformationType.Successful));

            return true;
        }

        private void CreateAndSavePing(SearchResultSEF searchResultSEF, IWebDriver chromeDriver)
        {
            try
            {
                //Click new ping
                chromeDriver.FindElement(By.CssSelector("#toolbar-new > button")).Click();
                //Find the inputs and send title and url
                chromeDriver.FindElement(By.Id("title")).SendKeys(searchResultSEF.Header);
                chromeDriver.FindElement(By.Id("linkurl")).SendKeys(searchResultSEF.SearchResult.Url);
                //Save
                chromeDriver.FindElement(By.CssSelector("#toolbar-apply > button")).Click();
            }
            catch (Exception e)
            {

                NewLogEvent(new LogEventArgs($"Fejl i at pinge:  ''{searchResultSEF.SearchResult.Url}'' Kunne ikke oprette eller gemme ping...  {e}",
                    InformationType.Failed));
            }

        }

        private void SendPing(IWebDriver chromeDriver)
        {
            //Click send pings
            chromeDriver.FindElement(By.CssSelector("#toolbar-broadcast > button")).Click();

            bool isPinging = true;
            while (isPinging)
            {
                //After click, check if the ping is still being sent.
                Task.Delay(300);
                //If the element is still there, it is still being pinged.
                try
                {
                    chromeDriver.FindElement(By.Id("progressModal1"));

                }
                catch (Exception)
                {
                    //If element is not there
                    isPinging = false;
                }
            }
        }
    }
}
