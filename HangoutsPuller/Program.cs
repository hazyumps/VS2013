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
using System.Threading.Tasks;

namespace HangoutPuller
{
    class Program
    {
        static string[] Scopes = { GmailService.Scope.GmailReadonly };
        static string ApplicationName = "HangoutPuller";

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

            //Define inputs 
            //'me' is special for being current logged in user
            //
            //Using the labelId of CHAT we pull all hangouts history
            //query will need to be modified in the future to be dynamic based on the Sharepoint site
            string userId = "me";
            string query = "from:ben.s.lerch@gmail.com ";
            string labelId = "CHAT";

            //Call the ListThread method to pull all threadIds based on the criteria listed
            List<Thread> t = ListThread(service, userId, labelId, query);

            //Enumerate through the list
            foreach (var x in t)
            {
                //Call the GetThread method to pull actual messages based on the threadIds found in the previous list
                Thread th = GetThread(service, userId, x.Id);

                //Enumerate the message contents
                foreach (var y in th.Messages)
                {
                    //Search the headers for the name of the sender and highlight it in RED so I can see that shit
                    foreach (var z in y.Payload.Headers)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(z.Value);
                    }

                    //Write out the chat to the console window!
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(y.Snippet);
                }

            }


            Console.ReadLine();
        }

        public static List<Message> ListMessages(GmailService service, String userId, String query)
        {
            List<Message> result = new List<Message>();
            UsersResource.MessagesResource.ListRequest request = service.Users.Messages.List(userId);
            request.Q = query;

            do
            {
                try
                {
                    ListMessagesResponse response = request.Execute();
                    result.AddRange(response.Messages);
                    request.PageToken = response.NextPageToken;
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                }

            } while (!String.IsNullOrEmpty(request.PageToken));

            return result;
        }

        public static Message GetMessage(GmailService service, String userId, String messageId)
        {
            try
            {
                return service.Users.Messages.Get(userId, messageId).Execute();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }

            return null;
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


        public static List<Thread> ListThread(GmailService service, String userId, String labelIds, String query)
        {
            List<Thread> result = new List<Thread>();
            UsersResource.ThreadsResource.ListRequest request = service.Users.Threads.List(userId);
            request.LabelIds = labelIds;
            request.Q = query;

            do
            {
                try
                {

                    ListThreadsResponse response = request.Execute();
                    result.AddRange(response.Threads);
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