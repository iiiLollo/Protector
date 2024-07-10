using ProjectM;
using Stunlock.Core;
using System.Text;
using Unity.Entities;
using VampireCommandFramework;

namespace Protector.Commands;
public static class ProtectorCommands
{

    [CommandGroup("Protector")]
    internal class Protector
    {
        [Command("list", "l", description: "Returns the list of current whitelisted clients.", adminOnly: true)] // Returns the list of currently whitelisted steamIDs
        public static void List(ChatCommandContext ctx)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Whitelisted SteamIDs:");
            var noWhitelisted = true;
            foreach (var steamID in Core.gKs.getWhitelisted())
            {
                noWhitelisted = false;
                sb.AppendLine($"" + steamID);
            }

            if (noWhitelisted)
                sb.AppendLine("No ids whitelisted");

            ctx.Reply(sb.ToString());
        }

        [Command("reload", "r", description: "Forces the reload of the whitelist file.", adminOnly: true)] // Forces the reload of the whitelist file
        public static void Reload(ChatCommandContext ctx)
        {
            Core.gKs.MarkForReload();
            ctx.Reply("List marked for reload");
        }

    }    
}