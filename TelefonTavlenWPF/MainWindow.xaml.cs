using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TeleTavleLibrary;

namespace TelefonTavlenWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TTManager ttManager;

        public MainWindow()
        {
            InitializeComponent();

            ttManager = new TTManager();
            ttManager.LogEvent += TTManager_LogEvent;
        }

        private void TTManager_LogEvent(object sender, LogEventArgs e)
        {
            Console.Out.WriteLine(e.Message);
        }

        private void AddSearchWord_Click(object sender, RoutedEventArgs e)
        {
            //If the input is not empty add it to the list.
            if (!string.IsNullOrWhiteSpace(searchwordInput.Text))
            {
                //Add to list with search words
                SearchWordListbox.Items.Add(searchwordInput.Text);
            }
        }

        private async void Startbtn_Click(object sender, RoutedEventArgs e)
        {

            List<string> searchwords = SearchWordListbox.Items.Cast<string>().ToList();

            List<SearchResultSEF> searchResultSEFs = await ttManager.StartProcessParallelAsync(searchwords);
        }

        private void SearchWordListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //cast to listbox
            ListBox searchWords = (ListBox)sender;
            //Remove the item clicked on
            //If null dont do anything
            if (searchWords.SelectedItem == null)
            {
                return;
            }
            //Remove selected item
            searchWords.Items.Remove(searchWords.SelectedItem);
            //Must wait because event gets fired like 3 times if not.
            Task.Delay(100).Wait();
        }

        private void SearchWordListbox_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void restartbtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void facebookpostList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
