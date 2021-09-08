using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TeleTavleLibrary;

namespace TelefonTavlenWPF
{
    /// <summary>
    /// Interaction logic for MsgPopUpWindow.xaml
    /// </summary>
    public partial class MsgPopUpWindow : Window
    {
        public MsgPopUpWindow(InformationType type, string msgdetails)
        {
            InitializeComponent();

            //global variable
            BitmapImage bitImg;

            switch (type)
            {
                //when the program is done then hit this
                case InformationType.Successful:
                    bitImg = new BitmapImage(new Uri(Environment.CurrentDirectory + @"\Images\Succesful.png"));

                    //set window title and icon
                    Title = "Processen er fuldført";
                    Icon = bitImg;

                    //Set icon image 
                    msgTypeIcon.Source = bitImg;

                    //msg title
                    msgTitle.Content = "Processen er fuldført";

                    SetMsgdetails(msgdetails);

                    break;
                case InformationType.Failed:
                    bitImg = new BitmapImage(new Uri(Environment.CurrentDirectory + @"\Images\Error.png"));

                    //set window title and icon
                    Title = "Fejl i processen";
                    Icon = bitImg;

                    //Set icon image 
                    msgTypeIcon.Source = bitImg;

                    msgTitle.Content = "Processen fejlede... Se nedenstående fejl besked";

                    SetMsgdetails(msgdetails);

                    break;
                case InformationType.Warning:
                    bitImg = new BitmapImage(new Uri(Environment.CurrentDirectory + @"\Images\Warning.png"));

                    //set window title and icon
                    Title = "Advarsel i processen";
                    Icon = bitImg;

                    //Set icon image 
                    msgTypeIcon.Source = bitImg;

                    //msg title
                    msgTitle.Content = "Processen har en advarsel... Se nedenstående fejl besked";

                    SetMsgdetails(msgdetails);

                    break;
                case InformationType.Information:
                    bitImg = new BitmapImage(new Uri(Environment.CurrentDirectory + @"\Images\Information.png"));

                    //set window title and icon
                    Title = "Information";
                    Icon = bitImg;

                    //Set icon image 
                    msgTypeIcon.Source = bitImg;

                    //msg title
                    msgTitle.Content = "OBS: Se nedenstående besked";

                    SetMsgdetails(msgdetails);

                    break;
                default:
                    break;
            }
        }
        private void SetMsgdetails(string msgdetails)
        {
            msgdetailsTextBox.Text = msgdetails;
            //Show textbox
            msgdetailsTextBox.Visibility = Visibility.Visible;
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
