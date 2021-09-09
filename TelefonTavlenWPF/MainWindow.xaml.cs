using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using TeleTavleLibrary;

namespace TelefonTavlenWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string everyinformationEvents = "";
        TTManager ttManager;
        CancellationTokenSource cancellationTokenSource;
        CancellationToken processToken;

        public MainWindow()
        {
            InitializeComponent();

            //Tokens for stopping program
            cancellationTokenSource = new CancellationTokenSource();
            processToken = cancellationTokenSource.Token;

            ttManager = new TTManager();
            ttManager.LogEvent += TTManager_LogEvent;
            ttManager.SubscribeEvents();

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
                    brush = Brushes.LimeGreen;
                    break;
                case InformationType.Failed:
                    brush = Brushes.Red;
                    cancellationTokenSource.Cancel();
                    consoleStatusBox.Document.Blocks.Add(new Paragraph(new Run("STOPPET") { Foreground = brush }));

                    ShowMsgPopUp(e);
                    break;
                case InformationType.Information:
                    brush = Brushes.Black;
                    //add the information event to a list, when the process is done then the whole list will be in details section
                    everyinformationEvents += e.Message + "\n";
                    break;
                case InformationType.Warning:
                    brush = Brushes.DarkOrange;
                    break;
                default:
                    break;
            }
            //Write text to the consolebox and add the color.
            //TODO: Use this: consoleStatusBox.Document.Blocks.Add(new Paragraph(new Run(e.Time + ": " + e.Message) { Foreground = brush }));
            consoleStatusBox.Document.Blocks.Add(new Paragraph(new Run(e.Time.Minute +" . " + e.Time.Second + " . " +e.Time.Millisecond + ": " + e.Message) { Foreground = brush })); //TODO:remove this
            consoleStatusBox.ScrollToEnd();
        }

        private void ShowMsgPopUp(LogEventArgs e)
        {
            MsgPopUpWindow popup = new MsgPopUpWindow(e.informationType, e.Message);
            //It will fail if the mainwindow is not shown when trying set owner on popup
            if (this.Visibility == Visibility.Visible)
            {
                popup.Owner = this;
            }
            popup.ShowDialog();
        }

        private void AddSearchWord_Click(object sender, RoutedEventArgs e)
        {
            //If the input is not empty add it to the list.
            if (!string.IsNullOrWhiteSpace(searchwordInput.Text))
            {
                //Make sure it doesent already exist 
                if (!SearchWordListbox.Items.Contains(searchwordInput.Text.Trim().ToLower()))
                {
                    //Add to list with search words
                    SearchWordListbox.Items.Add(searchwordInput.Text.Trim().ToLower());

                    //Delete the text because it is put into a list
                    searchwordInput.Clear();

                    //Enable start btn
                    Startbtn.IsEnabled = true;
                }
                else
                {
                    ShowMsgPopUp(new LogEventArgs("Søgeordet findes allerede", InformationType.Information));
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
                await Task.Run(() =>
                {
                    searchResultSEFs = ttManager.StartProcessParallel(searchwords, processToken);
                }, processToken);

                //add fb results to FB post list
                facebookpostList.ItemsSource = searchResultSEFs;

                //Create and show mail draft
                MailDraft mailDraft = new MailDraft();
                MailDraftTextBox.Document = mailDraft.CreateMailDraft(searchResultSEFs, searchwords);

                restartbtn.IsEnabled = true;

                //every information events will be add as message
                MsgPopUpWindow popup = new MsgPopUpWindow(InformationType.Successful, everyinformationEvents);
                popup.Owner = this;
                popup.ShowDialog();
            }
            catch (OperationCanceledException oe)
            {
                WriteToErrorLog(oe.ToString());
            }

        }

        private void StartProcessBtns()
        {
            Startbtn.IsEnabled = false;
            AddSearchWord.IsEnabled = false;
            SearchWordListbox.IsEnabled = false;
            searchwordInput.IsEnabled = false;
        }

        private void Restartbtn_Click(object sender, RoutedEventArgs e)
        {
            //Empty everything
            SearchWordListbox.Items.Clear();
            facebookpostList.ItemsSource = null;
            consoleStatusBox.Document.Blocks.Clear();
            MailDraftTextBox.Document.Blocks.Clear();
            everyinformationEvents = "";
            fbTextBox.Clear();
            searchwordInput.Clear();

            //Enable for input
            EnableButtonsForStart();
        }

        private void EnableButtonsForStart()
        {
            SearchWordListbox.IsEnabled = true;
            searchwordInput.IsEnabled = true;
            restartbtn.IsEnabled = false;
            AddSearchWord.IsEnabled = true;
            Startbtn.IsEnabled = false;
        }

        private void FacebookpostList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
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
            catch (Exception ee)
            {
                WriteToErrorLog(ee.ToString());
            }
        }

        private void CopyObjectText(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (sender is RichTextBox box)
                {
                    CreateCopyToolTip(box);

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
                    TextBox textBox = (TextBox)sender;
                    CreateCopyToolTip(textBox);

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
            catch (Exception ee)
            {
                WriteToErrorLog(ee.ToString());
            }
        }

        private void SearchwordInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddSearchWord_Click(this, new RoutedEventArgs());
            }
        }

        private async void CreateCopyToolTip(Control box)
        {
            try
            {
                // ToolTip to show that the text is copied
                ToolTip toolTip = new ToolTip
                {
                    Content = "Kopieret"
                };
                if (((ToolTip)box.ToolTip) == null)
                {
                    ToolTipService.SetToolTip(box, toolTip);

                    ((ToolTip)box.ToolTip).IsOpen = true;

                    //wait for the task so ToolTip only is visible 2 seconds
                    await Task.Delay(2000);

                    ((ToolTip)box.ToolTip).IsOpen = false;

                    //Set tooltip to null so its not show when hover over the box
                    ToolTipService.SetToolTip(box, null);
                }
            }
            catch (Exception e)
            {
                WriteToErrorLog(e.ToString());
            }
        }

        //TODO: DELETE THIS 
        private void WriteToErrorLog(string message)
        {
            string filename = "ErrorLog.txt";

            StreamWriter sw = new StreamWriter(filename, true);
            //DataTime.Now:G is the same as .ToString("G")
            sw.WriteLine($"[{DateTime.Now:G}]  {message}");

            sw.Flush();

            sw.Close();
        }

        private void SearchWordListbox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
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

                //Disable start button if searchword list is empty 
                if (searchWords.Items.IsEmpty)
                {
                    EnableButtonsForStart();
                }
            }
            catch (Exception ee)
            {
                WriteToErrorLog(ee.ToString());
            }
        }
    }
}
