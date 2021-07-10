using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InspirobotDotNet.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("q")]
        public async Task PostQuote()
        {
            HttpClient httpClient = new HttpClient();
            string response = await httpClient.GetStringAsync("https://inspirobot.me/api?generate=true");
            await ReplyAsync(response);
        }
    }
}
