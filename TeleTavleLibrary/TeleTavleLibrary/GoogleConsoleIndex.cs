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
                LogEvent?.Invoke(this, new LogEventArgs($"GoogleConsoleIndex kunne ikke finde bruger oplysninger (json filen telefontavlen-c88ce2f3a6b5)...  {e}",
                    InformationType.Failed));
            }

        }

        /// <summary>
        /// Sends a batch request to google about indexing. action could be URL_UPDATED OR URL_DELETED. Make sure to await, so the request is done.  
        /// Use .result after method
        /// </summary>
        /// <param name="URLsToIndex"></param>
        /// <param name="action"> URL_UPDATED or URL_DELETED</param>
        /// <returns></returns>
        public async Task<List<PublishUrlNotificationResponse>> IndexBatchURL(List<string> URLsToIndex, string action)
        {
            if (googleCredential == null)
            {
                return null;
            }
            var credential = googleCredential.UnderlyingCredential;

            //Adding credentials
            IndexingService googleIndexingApiClientService = new IndexingService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential
            });

            BatchRequest request = new BatchRequest(googleIndexingApiClientService);

            List<PublishUrlNotificationResponse> notificationResponses = new List<PublishUrlNotificationResponse>();

            foreach (string URLToIndex in URLsToIndex)
            {
                //The body of the post request
                UrlNotification requestBody = new UrlNotification
                {
                    Url = URLToIndex,
                    Type = action
                };

                request.Queue<PublishUrlNotificationResponse>(
               new PublishRequest(googleIndexingApiClientService, requestBody), (response, error, i, message) =>
               {
                   notificationResponses.Add(response);
               });
            }
            try
            {
                //Send request
                await request.ExecuteAsync();

                //Show all the indexed urls
                string logEventString = "";
                for (int i = 0; i < URLsToIndex.Count; i++)
                {
                    logEventString += URLsToIndex[i] + " \n";
                }

                LogEvent?.Invoke(this, new LogEventArgs($"{logEventString} , er indexeret", InformationType.Successful));
                return await Task.FromResult(notificationResponses);
            }
            catch (Exception e)
            {
                LogEvent?.Invoke(this, new LogEventArgs($"GoogleConsoleIndex kunne ikke sende anmodning om indeksering...URL:  {e}",
                    InformationType.Failed));
            }
            return null;
        }

        public GoogleConsoleIndex()
        {

        }

    }
}
