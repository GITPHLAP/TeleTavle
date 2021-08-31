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
        CancellationTokenSource cancellationTokenSource;
        CancellationToken processToken;

        public MainWindow()
        {
            InitializeComponent();

            ttManager = new TTManager();
            ttManager.LogEvent += TTManager_LogEvent;
            cancellationTokenSource = new CancellationTokenSource();
            processToken = cancellationTokenSource.Token;
        }

        private void TTManager_LogEvent(object sender, LogEventArgs e)
        {
            cancellationTokenSource.Cancel();
            //The main thread is the only one to add text to control.
            Dispatcher.Invoke(new Action(() =>
            {
                consoleStatusBox.Document.Blocks.Add(new Paragraph(new Run(e.Message)));
                consoleStatusBox.ScrollToEnd();
            }));

        }

        private void AddSearchWord_Click(object sender, RoutedEventArgs e)
        {
            //If the input is not empty add it to the list.
            if (!string.IsNullOrWhiteSpace(searchwordInput.Text))
            {
                //Add to list with search words
                SearchWordListbox.Items.Add(searchwordInput.Text);

                //Delete the text because it is put into a list
                searchwordInput.Text = "";
            }
        }

        private async void Startbtn_Click(object sender, RoutedEventArgs e)
        {
            //Get the list from the control
            List<string> searchwords = SearchWordListbox.Items.Cast<string>().ToList();
            List<SearchResultSEF> searchResultSEFs = new List<SearchResultSEF>();
            //Start the process
            try
            {

                await Task.Run(async () =>
                {
                    searchResultSEFs = await ttManager.StartProcessParallelAsync(searchwords);


                }, processToken);
            }
            catch (Exception)
            {

                throw;
            }

            //add fb results to FB post list
            facebookpostList.DataContext = searchResultSEFs;

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
            //facebookpostList.DataContext = PhilipMethods.testSEF();

        }

        private void facebookpostList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //cast to listbox
            ListBox searchresults = (ListBox)sender;
            //Remove the item clicked on
            //If null dont do anything
            if (searchresults.SelectedItem == null)
            {
                return;
            }

            //create the text in right format
            string fbText = ((SearchResultSEF)searchresults.SelectedItem).Header + "\n" + ((SearchResultSEF)searchresults.SelectedItem).Description;

            //set text to clipboard
            Clipboard.SetText(fbText);

            //set fb text to textbox
            fbTextBox.Text = fbText;
        }

        private void CopyObjectText(object sender, MouseButtonEventArgs e)
        {
            var textBox = (TextBox)sender;

            Clipboard.SetText(textBox.Text);
        }
    }
}
