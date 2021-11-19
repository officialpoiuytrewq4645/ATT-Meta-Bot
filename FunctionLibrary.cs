using Discord;
using System;
using System.Threading.Tasks;

namespace MegaMetaBot
{
    public class Say
    {
        // This is called to send a message on discord.
        // Not needed for this bot, but people who use this bot as a basis for their own may appreciate it.
        public async static Task It(string whatToSay) 
        {
            await Guild.Message.Channel.SendMessageAsync(whatToSay);
        }





        // This is called to send an embed to discord.
        public async static void Embed(EmbedBuilder importedEmbedBuilder)
        {
            var responseChannel = Guild.client.GetChannel(Guild.Message.Channel.Id) as IMessageChannel;
            await responseChannel.SendMessageAsync("", false, importedEmbedBuilder.Build());
        }
    }
}
