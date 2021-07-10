using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace InspirobotDotNet
{
    class Program
    {
        private DiscordSocketClient _discordClient;
        private CommandService _discordCommands;
        private IServiceProvider _services;

        static void Main(string[] args)
        {
            Program program = new Program();
            program.RunBotAsync().GetAwaiter().GetResult();
        }

        private async Task RunBotAsync()
        {
            //Bot token must be added here
            string token = null;
            try
            {
                if(token == null)
                {
                    throw new ArgumentNullException();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Token must have value");
            }


            _discordClient = new DiscordSocketClient();
            _discordCommands = new CommandService();

            //.AddSingleton ensures only one instance is running
            _services = new ServiceCollection()
                .AddSingleton(_discordClient)
                .AddSingleton(_discordCommands)
                .BuildServiceProvider();

            _discordClient.Log += _discordClient_Log;

            await RegisterCommandsAsync();
            await _discordClient.LoginAsync(TokenType.Bot, token);
            await _discordClient.StartAsync();

            //Ensures the program won't close 
            await Task.Delay(-1);
        }

        private Task _discordClient_Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        private async Task RegisterCommandsAsync()
        {
            _discordClient.MessageReceived += HandleCommandAsync;
            await _discordCommands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_discordClient, message);

            //If another bot is trying to interact
            if (message.Author.IsBot)
            {
                return;
            }
            int argPos = 0;
            if (message.HasStringPrefix("!", ref argPos))
            {
                var result = await _discordCommands.ExecuteAsync(context, argPos, _services);
                if (!result.IsSuccess)
                {
                    Console.WriteLine(result.ErrorReason);
                }
            }
        }
    }
}
