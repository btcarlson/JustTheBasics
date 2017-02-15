using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace JustTheBasics
{
    public class PingTracker : IService
    {
        public bool IsEnabled { get; set; }

        private List<int> _pingHistory;

        public Task PreDisable(DiscordSocketClient client)
        {
            client.LatencyUpdated -= pingHandler;
            _pingHistory = null;

            return Task.CompletedTask;
        }

        public Task PreEnable(DiscordSocketClient client)
        {
            _pingHistory = new List<int>();
            client.LatencyUpdated += pingHandler;

            return Task.CompletedTask;
        }

        public int GetLastPing() => _pingHistory?.Last() ?? 0;

        public int GetAvgPing() => (int)Math.Round(_pingHistory.Average(), MidpointRounding.AwayFromZero);

        public int GetMaxPing() => _pingHistory.Max();

        public int GetMinPing() => _pingHistory.Min();

        public (int Last, int Avg, int Max, int Min) GetStats() => (GetLastPing(), GetAvgPing(), GetMaxPing(), GetMinPing());

        public void ResetTracker()
        {
            if (IsEnabled)
                _pingHistory = new List<int>();
        }

        private Task pingHandler(int old, int current)
        {
            _pingHistory.Add(current);

            return Task.CompletedTask;
        }
    }
}