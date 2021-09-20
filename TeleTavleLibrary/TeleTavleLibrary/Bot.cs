using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;

namespace TeleTavleLibrary
{
    public abstract class Bot
    {
        public event EventHandler<LogEventArgs> LogEvent;

        //List of all drivers is used if the application fail and exit
        public static List<IWebDriver> webdrivers = new List<IWebDriver>();

        public IWebDriver GetChromeDriver()
        {
            try
            {
                return CreateChromeDriver();
            }
            catch (Exception e)
            {
                NewLogEvent(new LogEventArgs($" Noget gik galt, prøver igen {e}", InformationType.Warning));
            }
            //Try again
            try
            {
                return CreateChromeDriver();
            }
            catch (Exception e)
            {
                NewLogEvent(new LogEventArgs($"Kunne ikke oprette chromedriver. Læs i guiden om (hvordan man bruger programmet) og gå under sektionen 'Håndtering af chromedriver fejl'...  {e}", InformationType.Failed));
                return null;
            }

        }

        private static IWebDriver CreateChromeDriver()
        {
            //Chrome service to supress diagnostic and hide console
            ChromeDriverService cDS = ChromeDriverService.CreateDefaultService();
            cDS.SuppressInitialDiagnosticInformation = true;
            cDS.HideCommandPromptWindow = true;

            //Chrome options to hide chrome application  
            ChromeOptions co = new ChromeOptions();
            co.AddArguments("headless");

            //This useragent prevent reCAPTCHAR
            co.AddArguments("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.82 Safari/537.36");


            //does not log so much with this argument
            co.AddArgument("--log-level=3");

            co.AddArgument("no-sandbox");

            IWebDriver chromeDriver = new ChromeDriver(cDS, co, TimeSpan.FromMinutes(3));

            chromeDriver.Manage().Timeouts().PageLoad.Add(TimeSpan.FromSeconds(90));

            webdrivers.Add(chromeDriver);

            return chromeDriver;
        }

        protected virtual void NewLogEvent(LogEventArgs e)
        {
            LogEvent?.Invoke(this, e);
        }

    }
}
