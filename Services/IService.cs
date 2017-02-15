using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace JustTheBasics
{
    public interface IService
    {
        bool IsEnabled { get; set; }
        Task PreEnable(DiscordSocketClient client);
        Task PreDisable(DiscordSocketClient client);
    }
}