using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Sheets_Database;

namespace MegaMetaBot
{
    class Program
    {
        private readonly string CommandPrefix = "!";
        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();


        public async Task MainAsync()
        {
            Console.WriteLine("Logging in...\n");



            // Create a new Discord Socket Client to handle anything discord related.
            Guild.client = new DiscordSocketClient();



            // Read the credentials with login information.
            string tokenFromTestFile = System.IO.File.ReadAllText("./Credentials/Token.txt");



            // Login to discord.
            await Guild.client.LoginAsync(TokenType.Bot, tokenFromTestFile);
            await Guild.client.StartAsync();



            Console.WriteLine("Connected to Discord.");



            // If a message is posted on discord, analyze it.
            Guild.client.MessageReceived += ProcessMessage;



            // Stop the console from closing.
            await Task.Delay(-1);
        }





        private async Task ProcessMessage(SocketMessage Message)
        {
            // Ignore bots
            if (Message.Author.IsBot) return;



            // Only listen to the approved channel
            //if (Message.Channel.Id != Guild.CoG_MetaBotTestingChannel) return;



            //// Make a global version for ease of access later.
            //Guild.Message = Message;



            // If the user's message started with CommandPrefix, it may have been a command. Check for a matching command.
            if (Message.Content.StartsWith(CommandPrefix))
            {


                // Create a list called data, containing everything from the spreadsheet.
                List<Database.CommandData> data = Database.init();









                // If the user's message is !list, show them a list of commands.
                if (Message.Content.ToLower().Substring(CommandPrefix.Length) == "list" || Message.Content.ToLower().Substring(CommandPrefix.Length) == "help")
                {
                    //string commandList = "";
                    string informationCategory = "";
                    string itemsCategory = "";
                    string playerManagementCategory = "";
                    string serverManagementCategory = "";
                    string otherCategory = ""; // Anything not in the other categories goes in this one automatically.


                    foreach (Database.CommandData myData in data)
                    {
                        // Used to get only the first trigger name when a trigger has multiple names.
                        // Up to 1024 characters can be used per field. 2048 per description. 6000 total.
                        string[] Triggers = myData.Trigger.Split(',');

                        if (myData.Category == "Information") informationCategory += Triggers[0] + ", ";
                        else if (myData.Category == "Items") itemsCategory += Triggers[0] + ", ";
                        else if (myData.Category == "Player Management") playerManagementCategory += Triggers[0] + ", ";
                        else if (myData.Category == "Server Management") serverManagementCategory += Triggers[0] + ", ";
                        else otherCategory += Triggers[0] + ", ";
                    }

                    // Remove the final ", ".
                    informationCategory = informationCategory.Substring(0, informationCategory.Length - 2);
                    itemsCategory = itemsCategory.Substring(0, itemsCategory.Length - 2);
                    playerManagementCategory = playerManagementCategory.Substring(0, playerManagementCategory.Length - 2);
                    serverManagementCategory = serverManagementCategory.Substring(0, serverManagementCategory.Length - 2);
                    otherCategory = otherCategory.Substring(0, otherCategory.Length - 2);

                    // Build a discord embed to show the commands.
                    EmbedBuilder InfoEmbed = new EmbedBuilder();
                    InfoEmbed.WithTitle("Help");
                    InfoEmbed.WithColor(Color.Green);
                    if (informationCategory != "") InfoEmbed.AddField("Information:", informationCategory, false);
                    if (itemsCategory != "") InfoEmbed.AddField("Items:", itemsCategory, false);
                    if (playerManagementCategory != "") InfoEmbed.AddField("Player Management:", playerManagementCategory, false);
                    if (serverManagementCategory != "") InfoEmbed.AddField("Server Management:", serverManagementCategory, false);
                    if (otherCategory != "") InfoEmbed.AddField("Other:", otherCategory, false);


                    // Send the result to discord.
                    await Say.Embed(Message, InfoEmbed);
                }








                // Loop through each list item.

                //clean up the command message before looping
                //cut off the command prefx by its length and make the message lowercase for comparison to command in loop
                string message = Message.Content.ToLower().Substring(CommandPrefix.Length);

                foreach (Database.CommandData myData in data)
                {


                    // Skip blank rows.
                    if (myData.Trigger != null)
                    {

                        // Split all possible triggers in the database into an array so we can use aliases.
                        string[] allPossibleTriggersAsArray = myData.Trigger.Split(',');

                        // Check each of the possible triggers individually.
                        foreach (string possibleTrigger in allPossibleTriggersAsArray)
                        {
                            // Check if the current list item matches what the user wrote.
                            if (message == possibleTrigger.ToLower())
                            {


                                // Prepare a new discord embed to send to discord later.
                                EmbedBuilder FeedbackEmbed = new EmbedBuilder();



                                // Sets the title for the embed.
                                if (myData.Title != null)
                                {
                                    FeedbackEmbed.WithTitle(myData.Title);
                                }
                                // Stop checking this row, as there is no title, suggesting it's incomplete.
                                else break;



                                // Adds an image at the bottom of the embed if one is found.
                                if (myData.Image != null)
                                {
                                    FeedbackEmbed.WithImageUrl(myData.Image);
                                }



                                // Adds a description field if appropriate data is found.

                                if (myData.Description != null)
                                {
                                    FeedbackEmbed.WithDescription(myData.Description);
                                }



                                // Set the side colour.
                                FeedbackEmbed.WithColor(Color.Green);



                                // These add new fields to the embed if values are found.
                                // Also checks that there are no empty fields, to protect from errors.
                                if (myData.Field_1_Data != null || myData.Field_1_Name != null)
                                {
                                    FeedbackEmbed.AddField(myData.Field_1_Name, myData.Field_1_Data, false);
                                }



                                // If a name and data is found, create the discord embed field.
                                if (myData.Field_2_Data != null || myData.Field_2_Name != null)
                                {
                                    FeedbackEmbed.AddField(myData.Field_2_Name, myData.Field_2_Data, false);
                                }



                                // If a name and data is found, create the discord embed field.
                                if (myData.Field_3_Data != null || myData.Field_3_Name != null)
                                {
                                    FeedbackEmbed.AddField(myData.Field_3_Name, myData.Field_3_Data, false);
                                }



                                // If a name and data is found, create the discord embed field.
                                if (myData.Field_4_Data != null || myData.Field_4_Name != null)
                                {
                                    FeedbackEmbed.AddField(myData.Field_4_Name, myData.Field_4_Data, false);
                                }



                                // If a name and data is found, create the discord embed field.
                                if (myData.Field_5_Data != null || myData.Field_5_Name != null)
                                {
                                    FeedbackEmbed.AddField(myData.Field_5_Name, myData.Field_5_Data, false);
                                }



                                // If a name and data is found, create the discord embed field.
                                if (myData.Field_6_Data != null || myData.Field_6_Name != null)
                                {
                                    FeedbackEmbed.AddField(myData.Field_6_Name, myData.Field_6_Data, false);
                                }



                                // If a footer is found, create the discord embed footer.
                                if (myData.Footer != null)
                                {
                                    FeedbackEmbed.WithFooter(myData.Footer);
                                }



                                // Send the discord embed to discord.
                                Say.Embed(Message, FeedbackEmbed);



                                // Stop looping through the list, because a match has been found.
                                break;
                            }
                        }



                    }
                }
            }
        }
    }
}
