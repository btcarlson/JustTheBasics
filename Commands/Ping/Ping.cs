using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace JustTheBasics
{
    public class PingCommand : ModuleBase<SocketCommandContext>
    {
        private PingTracker _pingTracker;
        private ServiceManager _serviceManager;

        [Name("Ping"), Command("ping")]
        public async Task Ping()
        {
            if (!_pingTracker.IsEnabled)
            {
                await Context.ReplyAsync(":warning: Ping Tracker is disabled. Please enable and wait for a few heartbeats to pass");
                return;
            }

            var stats = _pingTracker.GetStats();

            var builder = new EmbedBuilder()
                .WithColor(new Color(74, 202, 224));

            builder.WithTitle("Pong!")
                .WithDescription("Have some stats! These are calculated by the Heartbeat Latency over the Discord Gateway");

            builder.AddField(x => {
                x.IsInline = true;
                x.Name = "Last";
                x.Value = $"{stats.Last} ms";
            });

            builder.AddField(x => {
                x.IsInline = true;
                x.Name = "Average";
                x.Value = $"{stats.Avg} ms";
            });

            builder.AddField(x => {
                x.IsInline = true;
                x.Name = "Min";
                x.Value = $"{stats.Min} ms";
            });

            builder.AddField(x => {
                x.IsInline = true;
                x.Name = "Max";
                x.Value = $"{stats.Max} ms";
            });

            await Context.ReplyAsync(builder.Build());
        }

        [Name("PingTracker Toggle"), Command("pingtracker")]
        public async Task PingTrackerToggle(OnOff onOff)
        {
            bool success = false;
            switch (onOff)
            {
                case OnOff.On:
                    success = await _serviceManager.TryEnable<PingTracker>();
                    break;
                case OnOff.Off:
                    success = await _serviceManager.TryDisable<PingTracker>();
                    break;
            }

            var action = onOff.ToString().ToLower();

            if (!success)
                await Context.ReplyAsync($":warning: The Ping Tracker is already {action}");
            else
                await Context.ReplyAsync($"The Ping Tracker has been turned {action}");
        }

        public PingCommand(IDependencyMap depMap)
        {
            _serviceManager = depMap.Get<ServiceManager>();
            _pingTracker = _serviceManager.GetService<PingTracker>();
        }

        public enum OnOff
        {
            On,
            Off,
        }
    }
}