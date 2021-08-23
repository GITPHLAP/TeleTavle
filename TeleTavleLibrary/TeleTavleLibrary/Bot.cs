using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Text;

namespace TeleTavleLibrary
{
    public abstract class Bot
    {
        public event EventHandler LogEvent;


        public IWebDriver GetChromeDriver()
        {
            try
            {
                IWebDriver chromeDriver = new ChromeDriver();
                return chromeDriver;
            }
            catch (Exception)
            {
                throw;
            }


        }
    }
}
