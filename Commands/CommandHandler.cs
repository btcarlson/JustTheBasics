using Discord.Commands;
using Discord.WebSocket;
using NLog;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace JustTheBasics
{
    public class CommandHandler
    {
        private DiscordSocketClient _client;
        private CommandService _commandService;
        private ServiceDependencyMap _dependencyMap;
        private Logger _logger = LogManager.GetLogger("Commands");
        private string _prefix;
        private ulong[] _ownerIds;

        private bool _isReady = false;

        public CommandService CommandService => _commandService;

        public CommandHandler(DiscordSocketClient client, string prefix, ulong[] ownerIds, ServiceDependencyMap dependencyMap)
        {
            _client = client;
            _prefix = prefix;
            _ownerIds = ownerIds;
            _dependencyMap = dependencyMap;

            _commandService = new CommandService(new CommandServiceConfig()
            {
                // Private bots wouldn't really benefit from running Mixed or Async. Sync is recommended.
                DefaultRunMode = RunMode.Sync,
                CaseSensitiveCommands = false
            });

            _client.MessageReceived += handler;
        }

        private async Task handler(SocketMessage arg)
        {
            if (!_isReady)
                return;

            var msg = arg as SocketUserMessage;

            if (msg == null)
                return;

            #if BETA
            if (!_ownerIds.Contains(msg.Author.Id))
                return;
            #endif

            if (msg.Author.IsBot)
                return;

            int argPos = 0;

            if (!msg.HasStringPrefix(_prefix, ref argPos))
                return;

            var context = new SocketCommandContext(_client, msg);

            var result = await _commandService.ExecuteAsync(context, argPos, _dependencyMap, MultiMatchHandling.Best);

            _logger.Info($"Command ran by {context.User} in {context.Channel.Name} - {context.Message.Content}");

            if (result.IsSuccess)
                return;


            string response = null;

            switch (result)
            {
                case SearchResult searchResult:
                    // "Commnd not found" messages are frowned upon, but if you know your usage environment, this is where you would impliment the logic
                    break;
                case ParseResult parseResult:
                    response = $":warning: There was an error parsing your command: `{parseResult.ErrorReason}`";
                    break;
                case PreconditionResult preconditionResult:
                    response = $":warning: A precondition of your command failed: `{preconditionResult.ErrorReason}`";
                    break;
                case ExecuteResult executeResult:
                    response = $":warning: Your command failed to execute. If this persists, contact the Bot Developer.\n`{executeResult.Exception.Message}`";
                    _logger.Error(executeResult.Exception);
                    break;
            }

            if (response != null)
                await context.ReplyAsync(response);
        }

        public void StartListening()
        {
            _logger.Info("Now listening for commands");
            _isReady = true;
        }

        public async Task AddAllCommands()
            => await _commandService.AddModulesAsync(Assembly.GetEntryAssembly());
    }
}