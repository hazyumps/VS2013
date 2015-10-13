using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
//using System.Threading;
using System.Threading.Tasks;

namespace GmailQuickstart
{
    class Program
    {
        static string[] Scopes = { GmailService.Scope.GmailReadonly };
        static string ApplicationName = "Gmail API .NET Quickstart";

        static void Main(string[] args)
        {
            UserCredential credential;

            using (var stream =
                new FileStream("C:\\Files\\my.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/gmail-dotnet-quickstart");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    System.Threading.CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Gmail API service.
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            string userId = "me";
            string threadId = "14ea6f66f112fc79";
            //lsThr(service, userId);

            Console.WriteLine(GetThread(service,userId,threadId).Messages);
        }

        public static Thread GetThread(GmailService service, String userId, String threadId)
        {
            try
            {

                return service.Users.Threads.Get(userId, threadId).Execute();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }

            return null;
        }


        public static List<Thread> lsThr(GmailService service, String userId)
        {
            List<Thread> result = new List <Thread>();
            UsersResource.ThreadsResource.ListRequest request = service.Users.Threads.List(userId);

            do
            {
                try
                {
                    ListThreadsResponse response = request.Execute();
                    result.AddRange(response.Threads);
                    if (response.Threads != null)
                    {
                        foreach (Thread t in response.Threads)
                        {
                            //GetThread(service, userId, t.Id);
                            Console.WriteLine(t.Id);
                        }
                    }
                    request.PageToken = response.NextPageToken;


                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occured: " + e.Message);
                }

            } while (!String.IsNullOrEmpty(request.PageToken));
            
            return result;
        }

       
    }

}