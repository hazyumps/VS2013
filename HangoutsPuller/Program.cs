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
            string threadId;
            string messageId = "15066e0e7a581e13";
            string query = "in:chats";
            string labelId = "CHAT";
            string name = "Jason Douthitt";
            List<Thread> t = ListThread(service, userId, labelId);

            foreach (var x in t)
            {
                Thread th = GetThread(service, userId, x.Id);
                foreach (var i in th.Messages)
                {
                    foreach (var y in i.Payload.Headers)
                    {
                        if (y.Value.StartsWith(name))
                        {
                            threadId = i.ThreadId;

                            Console.WriteLine(threadId);

                            //Console.ForegroundColor = ConsoleColor.Red;
                            //Console.WriteLine(y.Value);
                            //Console.ForegroundColor = ConsoleColor.White;
                            //Console.WriteLine(i.Snippet);
                            
                        }
                        
                    }
                    //Console.ForegroundColor = ConsoleColor.White;
                    //Console.WriteLine(i.Snippet);
                }
            }

            //Console.WriteLine(threadId);
            //Thread th2 = GetThread(service,userI)

            //Practice Recursion
            //foreach (var x in t)
            //{
               
            //    foreach (var z in x.Messages)
            //    {
            //        if (labelId.Equals(z.LabelIds))
            //        {
            //            Thread th = GetThread(service, userId, z.ThreadId);
            //            foreach (var i  in th.Messages)
            //            {
            //                foreach (var y in i.Payload.Headers)
            //                {
            //                    Console.ForegroundColor = ConsoleColor.Red;
            //                    Console.WriteLine(y.Value);
            //                }
            //                Console.ForegroundColor = ConsoleColor.White;
            //                Console.WriteLine(i.Snippet);
            //            }

            //        }
            //    }
            //}


            //foreach (var v in t)
            //{
            //    Console.WriteLine(v.Snippet);
            //}

            //Thread th = GetThread(service,userId,threadId);

            //foreach (var i in th.Messages)
            //{
            //    foreach (var y in i.Payload.Headers)
            //    {
            //        Console.ForegroundColor = ConsoleColor.Red;
            //        Console.WriteLine(y.Value);
            //    }
            //    Console.ForegroundColor = ConsoleColor.White;
            //    Console.WriteLine( i.Snippet );
            //}

            //ListMessages(service, userId, query);

            //Message msg = GetMessage(service, userId, messageId);
            //Console.WriteLine(msg.Snippet);
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


        public static List<Thread> ListThread(GmailService service, String userId, String labelIds)
        {
            List<Thread> result = new List <Thread>();
            UsersResource.ThreadsResource.ListRequest request = service.Users.Threads.List(userId);
            request.LabelIds = labelIds;

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