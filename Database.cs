using System;
using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;
using System.Reflection;

namespace Sheets_Database
{
    public static class Database
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
        static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static string ApplicationName = "ATT Meta Bot";

        //this is literally google's example code
        public static List<CommandData> init()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("./Credentials/credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define request parameters.
            String spreadsheetId = "1mhEJL32ovYawQOHNo3XnmaBlmbt0HtF-Z2BVpGimPEU";
            //start at second row to ignore header text
            String range = "Data!A2:V";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);

            List<CommandData> ResultData = new List<CommandData>();

            // https://docs.google.com/spreadsheets/d/1mhEJL32ovYawQOHNo3XnmaBlmbt0HtF-Z2BVpGimPEU/edit
            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    CommandData data = new CommandData();
                    //easy way to populate each field of a class by crawling along it
                    int i = 0;
                    foreach (FieldInfo val in typeof(CommandData).GetFields(BindingFlags.Public | BindingFlags.Instance))
                    {
                        //damn you inconsistent zero based thingies
                        if(row.Count - 1  < i)
                        {
                            break;
                        }
                        val.SetValue(data, row[i]);
                        // I know I know (get it lol) i could use a for loop but i like doing this
                        i++;
                    }

                    ResultData.Add(data);
                }

            }
            else
            {
                //argh errories
            }
            return ResultData;
        }
        public class CommandData
        {
            public string Trigger;
            public string Title;
            public string Image;
            public string Description;
            public string Footer;
            public string Field_1_Name;
            public string Field_1_Data;
            public string Field_2_Name;
            public string Field_2_Data;
            public string Field_3_Name;
            public string Field_3_Data;
            public string Field_4_Name;
            public string Field_4_Data;
            public string Field_5_Name;
            public string Field_5_Data;
            public string Field_6_Name;
            public string Field_6_Data;
            public override string ToString()
            {
                return Trigger;
            }

        }
    }
}




