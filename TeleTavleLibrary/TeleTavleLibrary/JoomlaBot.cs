using System;
using System.Configuration;
using OpenQA.Selenium;

namespace TeleTavleLibrary
{
    public abstract class JoomlaBot : Bot
    {
        public void SiteLogin(IWebDriver chromeDriver, string startUrl)
        {
            //Goto website
            chromeDriver.Navigate().GoToUrl(startUrl);

            IWebElement usernameBox = null;
            IWebElement passwordBox = null;

            try
            {
                //Find username amd password element
                usernameBox = chromeDriver.FindElement(By.Id("mod-login-username"));
                passwordBox = chromeDriver.FindElement(By.Id("mod-login-password"));
            }
            catch (Exception)
            {
                NewLogEvent(new LogEventArgs($"JoomlaBot kan ikke finde login siden på kundens hjemmeside", InformationType.Failed));
            }

            try
            {
                //Get username and password, and send to the input fields
                usernameBox.SendKeys(ConfigurationManager.AppSettings.Get("JUser"));
                passwordBox.SendKeys(ConfigurationManager.AppSettings.Get("JPass"));

            }
            catch (Exception)
            {
                NewLogEvent(new LogEventArgs($"JoomlaBot kunne ikke finde App.Config eller så er der udfordringer vedr. App.Config", InformationType.Failed));
            }

            try
            {
                //Find the login button.
                chromeDriver.FindElement(By.CssSelector(@"#form-login > fieldset > div:nth-child(4) > div > div > button ")).Click();
            }
            catch (Exception)
            {
                NewLogEvent(new LogEventArgs($"JoomlaBot kunne ikke finde login knappen", InformationType.Failed));
            }
        }

    }
}
