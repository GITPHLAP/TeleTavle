﻿using OpenQA.Selenium;
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
                //Chrome service to supress diagnostic and hide console
                ChromeDriverService cDS = ChromeDriverService.CreateDefaultService();
                cDS.SuppressInitialDiagnosticInformation = true;
                cDS.HideCommandPromptWindow = true;

                //Chrome options to hide chrome application  
                ChromeOptions co = new ChromeOptions();
                co.AddArguments("headless");

                //This useragent prevent reCAPTCHAR
                co.AddArguments("--user-agent=Mozilla / 5.0(Windows NT 10.0; Win64; x64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 92.0.4515.131 Safari / 537.36");
                
                //does not log so much with this argument
                co.AddArgument("--log-level=3");

                IWebDriver chromeDriver = new ChromeDriver(cDS, co);

                webdrivers.Add(chromeDriver);

                return chromeDriver;
            }
            catch (Exception e)
            {
                NewLogEvent(new LogEventArgs($"Kunne ikke oprette chromedriver... {e}", InformationType.Failed));

                return null;
            }

        }

        protected virtual void NewLogEvent(LogEventArgs e)
        {
            LogEvent?.Invoke(this, e);
        }

    }
}
