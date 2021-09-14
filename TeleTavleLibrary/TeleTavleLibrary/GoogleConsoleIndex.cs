using Google.Apis.Auth.OAuth2;
using Google.Apis.Indexing.v3;
using Google.Apis.Indexing.v3.Data;
using Google.Apis.Requests;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static Google.Apis.Indexing.v3.UrlNotificationsResource;

namespace TeleTavleLibrary
{
    public class GoogleConsoleIndex
    {
        public event EventHandler<LogEventArgs> LogEvent;
        private GoogleCredential googleCredential;

        /// <summary>
        /// Used to read the credentials from the json file
        /// </summary>
        /// <returns></returns>
        public void GetGoogleCredential()
        {
            try
            {
                //Path to the credentials file
                string path = Environment.CurrentDirectory + "/telefontavlen-c88ce2f3a6b5.json";

                GoogleCredential credential;
                //Get credentials
                using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream).CreateScoped(new[] { "https://www.googleapis.com/auth/indexing" });
                }

                googleCredential = credential;
            }
            catch (Exception e)
            {
                LogEvent?.Invoke(this,new LogEventArgs($"GoogleConsoleIndex kunne ikke finde bruger oplysninger (json filen telefontavlen-c88ce2f3a6b5)...  {e}",
                    InformationType.Failed));
            }
            
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
            if (googleCredential == null)
            {
                return null;
            }
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

            try
            {
                //If something goes wrong, it will throw exception
                var executedRequest = publishRequest.ExecuteAsync().Result;
                LogEvent?.Invoke(this, new LogEventArgs($"{URLToIndex} , er indexeret", InformationType.Successful));
                return executedRequest;
            }
            catch (Exception e)
            {
                LogEvent?.Invoke(this, new LogEventArgs($"GoogleConsoleIndex kunne ikke sende anmodning om indeksering...  {e}",
                    InformationType.Failed));
            }
            return null;
        }


        public async Task<List<PublishUrlNotificationResponse>> IndexBatchURL(List<string> URLsToIndex, string action)
        {
            if (googleCredential == null)
            {
                return null;
            }
            var credential = googleCredential.UnderlyingCredential;
            //Adding credentials
            var googleIndexingApiClientService = new IndexingService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential
            });

            var request = new BatchRequest(googleIndexingApiClientService);

            List<PublishUrlNotificationResponse> notificationResponses = new List<PublishUrlNotificationResponse>();

            foreach (var URLToIndex in URLsToIndex)
            {


                //The body of the post request
                var requestBody = new UrlNotification
                {
                    Url = URLToIndex,
                    Type = action
                };

                request.Queue<PublishUrlNotificationResponse>(
               new UrlNotificationsResource.PublishRequest(googleIndexingApiClientService, requestBody), (response, error, i, message) =>
               {
                   notificationResponses.Add(response);
               });


            }
            await request.ExecuteAsync();
            return await Task.FromResult(notificationResponses);
        }

        public GoogleConsoleIndex()
        {

        }

    }
}
