using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using OpenQA.Selenium;

namespace TeleTavleLibrary
{
    public class JoomlaBot : Bot
    {

        public void SiteLogin(IWebDriver chromeDriver, string startUrl)
        {
            //Goto website
            chromeDriver.Navigate().GoToUrl(startUrl);

            //Find username 
            IWebElement usernameBox = chromeDriver.FindElement(By.Id("mod-login-username"));
            IWebElement passwordBox = chromeDriver.FindElement(By.Id("mod-login-password"));

            //Get username and password, and send to the input fields
            usernameBox.SendKeys(ConfigurationManager.AppSettings.Get("JUser"));
            passwordBox.SendKeys(ConfigurationManager.AppSettings.Get("JPass"));
            //Find the login button.
            chromeDriver.FindElement(By.XPath(@"/html/body/div[1]/div/div/form/fieldset/div[4]/div/div/button")).Click();
        }

    }
}
