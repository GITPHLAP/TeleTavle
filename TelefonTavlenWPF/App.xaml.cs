using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TeleTavleLibrary;

namespace TelefonTavlenWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            //quit every webdrivers in the list 
            int countofwebdrivers = Bot.webdrivers.Count;
            for (int i = 0; i < countofwebdrivers; i++)
            {
                if (Bot.webdrivers[i] != null)
                {
                    Bot.webdrivers[i].Quit();
                }
            }

            Environment.Exit(1);
        }
    }
}
