using Discord.Commands;
using Discord.WebSocket;
using NLog;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace JustTheBasics
{
    public class PingTracker : ServiceBase
    {
        private List<int> _pingHistory;
        private Logger _logger = LogManager.GetLogger("PingTracker");

        protected override Task PreDisable()
        {
            Client.LatencyUpdated -= pingHandler;
            _pingHistory = null;

            return Task.CompletedTask;
        }

        protected override Task PreEnable()
        {
            _pingHistory = new List<int>();
            Client.LatencyUpdated += pingHandler;

            return Task.CompletedTask;
        }

        public int GetLastPing() => _pingHistory?.Last() ?? 0;

        public int GetAvgPing() => (int)Math.Round(_pingHistory.Average(), MidpointRounding.AwayFromZero);

        public int GetMaxPing() => _pingHistory.Max();

        public int GetMinPing() => _pingHistory.Min();

        public (int Last, int Avg, int Max, int Min) GetStats() => (GetLastPing(), GetAvgPing(), GetMaxPing(), GetMinPing());

        public void ResetTracker()
        {
            if (!IsEnabled)
                return;

            _pingHistory = new List<int>();
            _logger.Info("Ping Tracker Reset");
        }

        private Task pingHandler(int old, int current)
        {
            _pingHistory.Add(current);

            return Task.CompletedTask;
        }
    }
}