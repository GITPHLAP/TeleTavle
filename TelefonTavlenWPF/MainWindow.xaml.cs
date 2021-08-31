using System;
using System.Collections.Generic;
using System.IO;
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
            ttManager.SubscribeEvents();

            cancellationTokenSource = new CancellationTokenSource();
            processToken = cancellationTokenSource.Token;

            EnableButtonsForStart();
        }

        private void TTManager_LogEvent(object sender, LogEventArgs e)
        {

            //The main thread is the only one to add text to control.
            Dispatcher.Invoke(new Action(() =>
            {
                WriteToConsole(e);
            }));

        }

        private void WriteToConsole(LogEventArgs e)
        {
            var brush = Brushes.Black;
            switch (e.informationType)
            {
                case InformationType.Successful:
                    brush = Brushes.Green;
                    break;
                case InformationType.Failed:
                    brush = Brushes.Red;
                    cancellationTokenSource.Cancel();
                    consoleStatusBox.Document.Blocks.Add(new Paragraph(new Run("STOPPET") { Foreground = brush }));
                    var popup = new MsgPopUpWindow(e.informationType, e.Message);
                    popup.ShowDialog();
                    break;
                case InformationType.Information:
                    brush = Brushes.Black;
                    break;
                case InformationType.Warning:
                    brush = Brushes.Orange;
                    break;
                default:
                    break;
            }
            //Write text to the consolebox and add the color.
            consoleStatusBox.Document.Blocks.Add(new Paragraph(new Run(e.Message) { Foreground = brush }));
            consoleStatusBox.ScrollToEnd();
        }

        private void AddSearchWord_Click(object sender, RoutedEventArgs e)
        {
            //If the input is not empty add it to the list.
            if (!string.IsNullOrWhiteSpace(searchwordInput.Text))
            {
                //Make sure it doesent already exist
                if (!SearchWordListbox.Items.Contains(searchwordInput.Text))
                {
                    //Add to list with search words
                    SearchWordListbox.Items.Add(searchwordInput.Text);

                    //Delete the text because it is put into a list
                    searchwordInput.Clear();

                    //Enable start btn
                    Startbtn.IsEnabled = true;
                }
            }
        }

        private async void Startbtn_Click(object sender, RoutedEventArgs e)
        {
            //Make sure the source is new, so that its not already canceled.
            cancellationTokenSource = new CancellationTokenSource();
            StartProcessBtns();

            //Get the list from the control
            List<string> searchwords = SearchWordListbox.Items.Cast<string>().ToList();
            List<SearchResultSEF> searchResultSEFs = new List<SearchResultSEF>();
            //Start the process
            try
            {
                processToken = cancellationTokenSource.Token;
                await Task.Run(async () =>
                {
                    searchResultSEFs = await ttManager.StartProcessParallelAsync(searchwords, processToken);
                }, processToken);
            }
            catch (OperationCanceledException)
            {
            }

            //add fb results to FB post list
            facebookpostList.DataContext = searchResultSEFs;

            //Create and show mail draft
            MailDraft mailDraft = new MailDraft();
            MailDraftTextBox.Document = mailDraft.CreateMailDraft(searchResultSEFs);

            restartbtn.IsEnabled = true;
        }

        private void StartProcessBtns()
        {
            Startbtn.IsEnabled = false;
            AddSearchWord.IsEnabled = false;
            SearchWordListbox.IsEnabled = false;
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


        private void restartbtn_Click(object sender, RoutedEventArgs e)
        {
            //Empty everything
            SearchWordListbox.ItemsSource = null;
            facebookpostList.ItemsSource = null;
            consoleStatusBox.Document.Blocks.Clear();
            MailDraftTextBox.Document.Blocks.Clear();
            fbTextBox.Clear();
            searchwordInput.Clear();

            //Enable for input
            EnableButtonsForStart();
        }

        private void EnableButtonsForStart()
        {
            SearchWordListbox.IsEnabled = true;
            restartbtn.IsEnabled = false;
            AddSearchWord.IsEnabled = true;
            Startbtn.IsEnabled = false;
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

            if (sender is RichTextBox box)
            {
                string rtfText;
                RichTextBox rtb = box;

                TextRange range = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);

                //create a empty memory stream where the text can be saved in 
                MemoryStream stream = new MemoryStream();
                range.Save(stream, DataFormats.Rtf);

                //Set beginning position for the stream
                stream.Seek(0, SeekOrigin.Begin);

                //read the memory stream
                StreamReader reader = new StreamReader(stream);
                rtfText = reader.ReadToEnd();

                Clipboard.SetText(rtfText, TextDataFormat.Rtf);

            }
            else
            {
                //Get control
                var textBox = (TextBox)sender;

                try
                {
                    Clipboard.SetText(textBox.Text);
                }
                catch (Exception ex)
                {
                    WriteToConsole(new LogEventArgs($"problemer med at kopiere {ex.Message}", InformationType.Warning));
                }
            }
        }
    }
}
