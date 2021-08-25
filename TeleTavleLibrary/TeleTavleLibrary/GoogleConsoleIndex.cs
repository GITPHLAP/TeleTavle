using Google.Apis.Auth.OAuth2;
using Google.Apis.Indexing.v3;
using Google.Apis.Indexing.v3.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TeleTavleLibrary
{
    public class GoogleConsoleIndex
    {
        public event EventHandler<LogEventArgs> LogEvent;
        private GoogleCredential googleCredential;

        private GoogleCredential GetGoogleCredential()
        {
            //Path to the credentials file
            var path = Environment.CurrentDirectory + "/telefontavlen-c88ce2f3a6b5.json";

            GoogleCredential credential;
            //Get credentials
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(new[] { "https://www.googleapis.com/auth/indexing" });
            }

            return credential;
        }
        /// <summary>
        /// Sends a request to google about indexing. action could be URL_UPDATED OR URL_DELETED. Make sure to await, so the request is done.  
        /// Use .result after method
        /// </summary>
        /// <param name="URLToIndex"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public PublishUrlNotificationResponse IndexURL(string URLToIndex, string action)
        {
            var credential = googleCredential.UnderlyingCredential;
            //Adding credentials
            var googleIndexingApiClientService = new IndexingService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential
            });
            //The body of the post request
            var requestBody = new UrlNotification
            {
                Url = URLToIndex,
                Type = action
            };

            var publishRequest = new UrlNotificationsResource.PublishRequest(googleIndexingApiClientService, requestBody);

            //If something goes wrong, it will throw exception
            var executedRequest = publishRequest.ExecuteAsync().Result;

            LogEvent?.Invoke(this, new LogEventArgs($"{URLToIndex} , er indexeret", InformationType.Successful));
            return executedRequest;
        }

        public GoogleConsoleIndex()
        {
            googleCredential = GetGoogleCredential();
        }

    }
}
