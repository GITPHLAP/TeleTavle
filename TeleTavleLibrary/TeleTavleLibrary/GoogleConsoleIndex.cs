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
        private GoogleCredential _googleCredential;

        public GoogleConsoleIndex()
        {

            _googleCredential = GetGoogleCredential();
        }

        public GoogleCredential GetGoogleCredential()
        {
            var path = Environment.CurrentDirectory + "/telefontavlen-c88ce2f3a6b5.json";

            GoogleCredential credential;

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(new[] { "https://www.googleapis.com/auth/indexing" });
            }

            return credential;
        }
        /// <summary>
        /// Sends a request to google about indexing.URL_UPDATED OR URL_DELETED. Make sure to await, so the request is done
        /// </summary>
        /// <param name="jobUrl"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public Task<PublishUrlNotificationResponse> AddUpdateIndex(string jobUrl, string action)
        {
            var credential = _googleCredential.UnderlyingCredential;

            var googleIndexingApiClientService = new IndexingService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential
            });

            var requestBody = new UrlNotification
            {
                Url = jobUrl,
                Type = action
            };

            var publishRequest = new UrlNotificationsResource.PublishRequest(googleIndexingApiClientService, requestBody);
            //If something goes wrong, it will throw exception
            return publishRequest.ExecuteAsync();
        }

    }
}
