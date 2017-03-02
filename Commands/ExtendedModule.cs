using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace JustTheBasics
{
    public abstract class ExtendedModuleBase : ModuleBase<SocketCommandContext>
    {
        public static ServiceDependencyMap DepMap { get; private set; } = null;

        public static void SetDependencyMap(ServiceDependencyMap dependencyMap)
        {
            if (DepMap == null)
                DepMap = dependencyMap;
        }
    }
}