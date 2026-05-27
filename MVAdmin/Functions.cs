using MVAdmin.Utility;
using System.IO;
using System.Linq;
using System.Text;

namespace MVAdmin
{
    internal static class Functions
    {
        public static void Owner(CommandContext context)
        {
            var player = context.Player;
            if (!string.IsNullOrWhiteSpace(context.Settings.Owner))
            {
                context.Bot.ErrorToPlayer(player, Constants.MESSAGE_OWNER_HAS_BEEN_SELECTED);
                return;
            }

            context.Settings.SetOwner(player);
            context.Bot.OkToPlayer(player, Constants.MESSAGE_OWNER);
        }

        public static void Me(CommandContext context)
        {
            int id = Utils.GetEntityNumber(context.Player);
            string name = context.Player.Name;
            string hwid = context.Player.HWID;
            context.Bot.SayToPlayer(context.Player, $"{id} - {name} - {hwid}");
            context.Log($"{id} - {name} - {hwid}");
        }

        public static void Info(CommandContext context)
        {
            var player = context.Player;
            if (!context.Settings.IsOwner(player) && !context.Settings.IsAdmin(player))
            {
                context.Bot.ErrorToPlayer(player, Constants.MESSAGE_NO_PERMISSION);
                return;
            }

            if (context.Values.Length == 0)
            {
                context.Bot.WarningToPlayer(player, "Usage: !info <player>");
                return;
            }


            string info = context.JoinWithSkip(" ", context.Values);
            var users = context.FindPlayers(info);
            if (users.Count() == 0)
            {
                context.Bot.WarningToPlayer(player, "Player not found.");
                return;
            }

            foreach (var user in users)
            {
                string id = Utils.GetEntityNumber(user).ToString();
                string name = user.Name;
                string hwid = user.HWID;
                string kills = user.GetField<string>("kills");
                string deaths = user.GetField<string>("deaths");
                string score = user.GetField<string>("score");

                context.Bot.SayToPlayer(player, $"{id} - {name} - {hwid} (kills: {kills}, deaths: {deaths}, score: {score})");
                context.Log($"{id} - {name} - {hwid} (kills: {kills}, deaths: {deaths}, score: {score})");
            }
        }

        public static void FastRestart(CommandContext context)
        {
            var player = context.Player;
            if (!context.Settings.IsOwner(player) && !context.Settings.IsAdmin(player))
            {
                context.Bot.ErrorToPlayer(player, Constants.MESSAGE_NO_PERMISSION);
                return;
            }

            context.Execute("fast_restart");
            context.Bot.WarningToAll(Constants.MESSAGE_FAST_RESTARTED.Replace("<player>", player.Name));
            context.Log(Constants.MESSAGE_FAST_RESTARTED.Replace("<player>", player.Name));
        }

        public static void MapRestart(CommandContext context)
        {
            var player = context.Player;
            if (!context.Settings.IsOwner(player) && !context.Settings.IsAdmin(player))
            {
                context.Bot.ErrorToPlayer(player, Constants.MESSAGE_NO_PERMISSION);
                return;
            }

            context.Execute("map_restart");
            context.Bot.WarningToAll(Constants.MESSAGE_MAP_RESTARTED.Replace("<player>", player.Name));
            context.Log(Constants.MESSAGE_MAP_RESTARTED.Replace("<player>", player.Name));
        }

        public static void Rotate(CommandContext context)
        {
            var player = context.Player;
            if (!context.Settings.IsOwner(player) && !context.Settings.IsAdmin(player))
            {
                context.Bot.ErrorToPlayer(player, Constants.MESSAGE_NO_PERMISSION);
                return;
            }

            context.Execute("map_rotate");
            context.Bot.WarningToAll(Constants.MESSAGE_MAP_ROTATED.Replace("<player>", player.Name));
            context.Log(Constants.MESSAGE_MAP_ROTATED.Replace("<player>", player.Name));
        }

        public static void Map(CommandContext context)
        {
            var player = context.Player;
            if (!context.Settings.IsOwner(player) && !context.Settings.IsAdmin(player))
            {
                context.Bot.ErrorToPlayer(player, Constants.MESSAGE_NO_PERMISSION);
                return;
            }

            string mapName = context.JoinWithSkip(" ", context.Values);
            string map = context.FindMap(mapName);
            if (string.IsNullOrWhiteSpace(map))
            {
                context.Bot.WarningToPlayer(player, "Map not found.");
                return;
            }

            string gameType = context.GetDvar("g_gametype");
            bool isHardCore = context.GetDvar("g_hardcore") == "1";
            string mod = context.FindMod(gameType, isHardCore);
            if (string.IsNullOrWhiteSpace(mod))
            {
                context.Bot.WarningToPlayer(player, "Mod not found.");
                return;
            }

            File.WriteAllText(PathProvider.Dspl, $"{map}, {mod}, 1");
            context.Execute("map_rotate");
        }

        public static void Mod(CommandContext context)
        {
            var player = context.Player;
            if (!context.Settings.IsOwner(player) && !context.Settings.IsAdmin(player))
            {
                context.Bot.ErrorToPlayer(player, Constants.MESSAGE_NO_PERMISSION);
                return;
            }

            string modName = context.JoinWithSkip(" ", context.Values).Trim().ToLower();
            bool isHardCore = context.GetDvar("g_hardcore") == "1";
            if (context.Values.Length > 1)
            {
                modName = context.Values[0].Trim().ToLower();
                if (!context.TryParseBool(context.Values[1].Trim().ToLower(), out bool result))
                {
                    context.Bot.InfoToPlayer(player, "Usage: !mod <name> <isHardCore on|off>");
                    return;
                }
                isHardCore = result;
            }

            string mod = context.FindMod(modName, isHardCore);
            if (string.IsNullOrWhiteSpace(mod))
            {
                context.Bot.WarningToPlayer(player, "Mod not found.");
                return;
            }

            if (!PathProvider.DsrExists(mod))
            {
                context.Bot.WarningToPlayer(player, $"{mod} not found.");
                return;
            }

            string map = context.GetDvar("mapname");
            File.WriteAllText(PathProvider.Dspl, $"{map}, {mod}, 1");
            context.Execute("map_rotate");
        }

        public static void Version(CommandContext context)
        {
            context.Bot.InfoToPlayer(context.Player, $"MVAdmin v{AppVersion.Get()}");
            context.Bot.InfoToPlayer(context.Player, $"Developer: ^7Soheil Jashnsaz");
            context.Bot.InfoToPlayer(context.Player, $"Github: ^7https://github.com/soheil-js");
            context.Bot.InfoToPlayer(context.Player, $"Repository: ^7https://github.com/soheil-js/MVAdmin");
            
        }

        public static void Server(CommandContext context)
        {
            var player = context.Player;
            if (!context.Settings.IsOwner(player))
            {
                context.Bot.ErrorToPlayer(player, Constants.MESSAGE_NO_PERMISSION);
                return;
            }

            string command = context.JoinWithSkip(" ", context.Values);
            context.Execute(command);
            context.Log(Constants.MESSAGE_COMMAND_LOG.Replace("<command>", command).Replace("<player>", player.Name));
        }

        public static void ChangePlayersAliveState(CommandContext context)
        {
            var player = context.Player;
            if (!context.Settings.IsOwner(player) && !context.Settings.IsAdmin(player))
            {
                context.Bot.ErrorToPlayer(player, Constants.MESSAGE_NO_PERMISSION);
                return;
            }

            var isShow = !context.Settings.ShowPlayersAlive;
            if (context.Values.Length > 0)
            {
                if (!context.TryParseBool(context.JoinWithSkip(" ", context.Values).Trim().ToLower(), out bool result))
                {
                    context.Bot.InfoToPlayer(player, "Usage: !showPlayersAlive <on|off>");
                    return;
                }

                isShow = result;
            }
            context.Settings.ChangePlayersInfoState(isShow);
            if (context.Settings.ShowPlayersAlive)
                context.Bot.WarningToPlayer(player, "Players information enabled.");
            else
                context.Bot.WarningToPlayer(player, "Players information disabled.");
        }

        public static void GetAdmins(CommandContext context)
        {
            var player = context.Player;
            var owner = context.Base.Players.FirstOrDefault(p => context.Settings.IsOwner(p));
            var admins = context.Base.Players.Where(p => context.Settings.IsAdmin(p));

            if (owner != null)
                context.Bot.SayToPlayer(player, $"^2Owner: ^7{owner.Name}");

            int adminsCount = admins.Count();
            if (adminsCount > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("^3Admins: ^7");
                for (int i = 0; i < adminsCount; i++)
                {
                    if (i == adminsCount - 1)
                        sb.Append(admins.ElementAt(i).Name);
                    else
                        sb.Append($"{admins.ElementAt(i).Name}, ");
                }

                context.Bot.SayToPlayer(player, sb.ToString());
            }

            if (owner == null && adminsCount == 0)
                context.Bot.WarningToPlayer(player, "Admins are offline.");
        }

        public static void PushAdmin(CommandContext context)
        {
            var player = context.Player;
            if (!context.Settings.IsOwner(player))
            {
                context.Bot.ErrorToPlayer(player, Constants.MESSAGE_NO_PERMISSION);
                return;
            }

            string info = context.JoinWithSkip(" ", context.Values);
            var user = context.FindPlayer(info);
            if (user == null)
            {
                context.Bot.WarningToPlayer(player, "Player not found.");
                return;
            }

            context.Settings.AddToAdmin(user);
            context.Bot.OkToPlayer(user, $"Congratulations, you became an admin!");
            context.Bot.WarningToPlayer(player, $"{user.Name} became an admin.");
        }

        public static void PopAdmin(CommandContext context)
        {
            var player = context.Player;
            if (!context.Settings.IsOwner(player))
            {
                context.Bot.ErrorToPlayer(player, Constants.MESSAGE_NO_PERMISSION);
                return;
            }

            string info = context.JoinWithSkip(" ", context.Values);
            var user = context.FindPlayer(info);
            if (user == null)
            {
                context.Bot.WarningToPlayer(player, "Player not found.");
                return;
            }

            context.Settings.RemoveFromAdmin(user);
            context.Bot.WarningToPlayer(user, "You have been removed from admin.");
            context.Bot.WarningToPlayer(player, $"{user.Name} has been removed from the admin role.");
        }

        public static void SayToAllFromBot(CommandContext context)
        {
            var player = context.Player;
            if (!context.Settings.IsOwner(player) && !context.Settings.IsAdmin(player))
            {
                context.Bot.ErrorToPlayer(player, Constants.MESSAGE_NO_PERMISSION);
                return;
            }

            string text = context.JoinWithSkip(" ", context.Values);
            context.Bot.SayToAll(text);
        }

        public static void SayToPlayerFromBot(CommandContext context)
        {
            var player = context.Player;
            if (!context.Settings.IsOwner(player) && !context.Settings.IsAdmin(player))
            {
                context.Bot.ErrorToPlayer(player, Constants.MESSAGE_NO_PERMISSION);
                return;
            }

            if (context.Values.Length < 2)
            {
                context.Bot.InfoToPlayer(player, "USAGE: !sayTo <player> <text>");
                return;
            }

            string info = context.Values[0];
            var user = context.FindPlayer(info);
            if (user == null)
            {
                context.Bot.WarningToPlayer(player, "Player not found.");
                return;
            }

            string text = context.JoinWithSkip(" ", context.Values, 1);

            context.Bot.WarningToPlayer(player, $"Your message was sent to {user.Name}.");
            context.Bot.SayToPlayer(user, text.Trim());
        }

        public static void Afk(CommandContext context)
        {
            var player = context.Player;
            if (context.Values.Length > 0)
            {
                if (!context.Settings.IsOwner(player) && !context.Settings.IsAdmin(player))
                {
                    context.Bot.ErrorToPlayer(player, Constants.MESSAGE_NO_PERMISSION);
                    return;
                }

                string info = context.JoinWithSkip(" ", context.Values);
                player = context.FindPlayer(info);
                if (player == null)
                {
                    context.Bot.WarningToPlayer(context.Player, "Player not found.");
                    return;
                }
            }

            context.ChangeTeam(player, "spectator");

            if (player.Name != context.Player.Name)
                context.Bot.WarningToPlayer(context.Player, $"{player.Name} became a spectator.");
        }

        public static void ChangeTeam(CommandContext context)
        {
            var player = context.Player;
            if (context.Values.Length > 0)
            {
                if (!context.Settings.IsOwner(player) && !context.Settings.IsAdmin(player))
                {
                    context.Bot.ErrorToPlayer(player, Constants.MESSAGE_NO_PERMISSION);
                    return;
                }

                string info = context.JoinWithSkip(" ", context.Values);
                player = context.FindPlayer(info);
                if (player == null)
                {
                    context.Bot.WarningToPlayer(context.Player, "Player not found.");
                    return;
                }
            }
            if (Utils.TeamIsAxis(player))
            {
                context.ChangeTeam(player, "allies");
                context.Bot.WarningToPlayer(context.Player, $"{player.Name} was transferred to the allies team.");
            }
            else if (Utils.TeamIsAllies(player))
            {
                context.ChangeTeam(player, "axis");
                context.Bot.WarningToPlayer(context.Player, $"{player.Name} was transferred to the axis team.");
            }
            else if (Utils.IsSpectating(player))
            {
                context.Bot.WarningToPlayer(context.Player, $"{player.Name} is a spectator.");
            }
            else
            {
                context.Bot.WarningToPlayer(context.Player, "The team cannot be changed.");
            }
        }

        public static void ChangeHostName(CommandContext context)
        {
            var player = context.Player;
            if (!context.Settings.IsOwner(player))
            {
                context.Bot.ErrorToPlayer(player, Constants.MESSAGE_NO_PERMISSION);
                return;
            }

            string name = context.JoinWithSkip(" ", context.Values).Trim();
            context.Settings.ChangeHostName(name);
        }

        public static void AlliesName(CommandContext context)
        {
            var player = context.Player;
            if (!context.Settings.IsOwner(player))
            {
                context.Bot.ErrorToPlayer(player, Constants.MESSAGE_NO_PERMISSION);
                return;
            }

            string name = context.JoinWithSkip(" ", context.Values);
            context.Settings.ChangeAlliesName(name);
        }

        public static void AlliesIcon(CommandContext context)
        {
            var player = context.Player;
            if (!context.Settings.IsOwner(player))
            {
                context.Bot.ErrorToPlayer(player, Constants.MESSAGE_NO_PERMISSION);
                return;
            }

            string icon = context.JoinWithSkip(" ", context.Values);
            context.Settings.ChangeAlliesIcon(icon);
        }

        public static void AxisName(CommandContext context)
        {
            var player = context.Player;
            if (!context.Settings.IsOwner(player))
            {
                context.Bot.ErrorToPlayer(player, Constants.MESSAGE_NO_PERMISSION);
                return;
            }

            string name = context.JoinWithSkip(" ", context.Values);
            context.Settings.ChangeAxisName(name);
        }

        public static void AxisIcon(CommandContext context)
        {
            var player = context.Player;
            if (!context.Settings.IsOwner(player))
            {
                context.Bot.ErrorToPlayer(player, Constants.MESSAGE_NO_PERMISSION);
                return;
            }

            string icon = context.JoinWithSkip(" ", context.Values);
            context.Settings.ChangeAxisIcon(icon);
        }

        public static void Suicide(CommandContext context)
        {
            var player = context.Player;
            if (context.Values.Length > 0)
            {
                if (!context.Settings.IsOwner(player) && !context.Settings.IsAdmin(player))
                {
                    context.Bot.ErrorToPlayer(player, Constants.MESSAGE_NO_PERMISSION);
                    return;
                }

                string info = context.JoinWithSkip(" ", context.Values);
                player = context.FindPlayer(info);
                if (player == null)
                {
                    context.Bot.WarningToPlayer(context.Player, "Player not found.");
                    return;
                }
            }

            context.Suicide(player);

            if (player.Name != context.Player.Name)
                context.Bot.WarningToPlayer(context.Player, $"{player.Name} committed suicide.");
        }

        public static void Kick(CommandContext context)
        {
            var player = context.Player;
            if (!context.Settings.IsOwner(player) && !context.Settings.IsAdmin(player))
            {
                context.Bot.ErrorToPlayer(player, Constants.MESSAGE_NO_PERMISSION);
                return;
            }

            if (context.Values.Length < 1)
            {
                context.Bot.InfoToPlayer(player, "USAGE: !kick <player> <reason>");
                return;
            }

            string info = context.Values[0];
            var user = context.FindPlayer(info);
            if (user == null)
            {
                context.Bot.WarningToPlayer(context.Player, "Player not found.");
                return;
            }

            if (context.Settings.IsOwner(user) || (!context.Settings.IsOwner(player) && context.Settings.IsAdmin(user)))
            {
                context.Bot.WarningToPlayer(context.Player, "shoma ejaze ekhraj admin ra nadarid.");
                return;
            }

            string reason = "You have been kicked.";
            if (context.Values.Length > 1)
                reason = context.JoinWithSkip(" ", context.Values, 1);

            context.Execute($"dropclient {Utils.GetEntityNumber(user)} \"{reason}\"");
            context.Bot.WarningToPlayer(context.Player, $"{user.Name} kick shod!");
        }

        public static void Ban(CommandContext context)
        {
            var player = context.Player;
            if (!context.Settings.IsOwner(player) && !context.Settings.IsAdmin(player))
            {
                context.Bot.ErrorToPlayer(player, Constants.MESSAGE_NO_PERMISSION);
                return;
            }

            if (context.Values.Length < 1)
            {
                context.Bot.InfoToPlayer(player, "USAGE: !ban <player> <reason>");
                return;
            }

            string info = context.Values[0];
            var user = context.FindPlayer(info);
            if (user == null)
            {
                context.Bot.WarningToPlayer(context.Player, "Player not found.");
                return;
            }

            if (context.Settings.IsOwner(user) || (!context.Settings.IsOwner(player) && context.Settings.IsAdmin(user)))
            {
                context.Bot.WarningToPlayer(context.Player, "shoma ejaze ban kardan admin ra nadarid.");
                return;
            }

            string reason = "You have been permanently banned.";
            if (context.Values.Length > 1)
                reason = context.JoinWithSkip(" ", context.Values, 1);

            context.Execute($"banclient {Utils.GetEntityNumber(user)} \"{reason}\"");
            context.Bot.WarningToPlayer(context.Player, $"{user.Name} ban shod!");
        }

        public static void TempBan(CommandContext context)
        {
            var player = context.Player;
            if (!context.Settings.IsOwner(player) && !context.Settings.IsAdmin(player))
            {
                context.Bot.ErrorToPlayer(player, Constants.MESSAGE_NO_PERMISSION);
                return;
            }

            if (context.Values.Length < 1)
            {
                context.Bot.InfoToPlayer(player, "USAGE: !tempBan <player> <reason>");
                return;
            }

            string info = context.Values[0];
            var user = context.FindPlayer(info);
            if (user == null)
            {
                context.Bot.WarningToPlayer(context.Player, "Player not found.");
                return;
            }

            if (context.Settings.IsOwner(user) || (!context.Settings.IsOwner(player) && context.Settings.IsAdmin(user)))
            {
                context.Bot.WarningToPlayer(context.Player, "shoma ejaze ban kardan admin ra nadarid.");
                return;
            }

            string reason = "You have been temporarily banned.";
            if (context.Values.Length > 1)
                reason = context.JoinWithSkip(" ", context.Values, 1);

            context.Execute($"tempBanClient {Utils.GetEntityNumber(user)} \"{reason}\"");
            context.Bot.WarningToPlayer(context.Player, $"{user.Name} ban shod!");
        }

        public static void Unban(CommandContext context)
        {
            var player = context.Player;
            if (!context.Settings.IsOwner(player) && !context.Settings.IsAdmin(player))
            {
                context.Bot.ErrorToPlayer(player, Constants.MESSAGE_NO_PERMISSION);
                return;
            }

            if (context.Values.Length < 1)
            {
                context.Bot.InfoToPlayer(player, "USAGE: !unban <hwid>");
                return;
            }

            string hwid = context.JoinWithSkip(" ", context.Values).Trim().ToUpper();
            context.Execute($"unban {hwid}");
            context.Bot.WarningToPlayer(context.Player, $"{hwid} unban shod!");
        }

        public static void Lock(CommandContext context)
        {
            var player = context.Player;
            if (!context.Settings.IsOwner(player) && !context.Settings.IsAdmin(player))
            {
                context.Bot.ErrorToPlayer(player, Constants.MESSAGE_NO_PERMISSION);
                return;
            }

            string info = context.JoinWithSkip(" ", context.Values);
            var user = context.FindPlayer(info);
            if (user == null)
            {
                context.Bot.WarningToPlayer(context.Player, "Player not found.");
                return;
            }

            context.Settings.LockedPlayer(user);
            context.Bot.WarningToPlayer(player, $"{user.Name} is locked!");
        }

        public static void Unlock(CommandContext context)
        {
            var player = context.Player;
            if (!context.Settings.IsOwner(player) && !context.Settings.IsAdmin(player))
            {
                context.Bot.ErrorToPlayer(player, Constants.MESSAGE_NO_PERMISSION);
                return;
            }

            string info = context.JoinWithSkip(" ", context.Values);
            var user = context.FindPlayer(info);
            if (user == null)
            {
                context.Bot.WarningToPlayer(context.Player, "Player not found.");
                return;
            }

            context.Settings.UnlockedPlayer(user);
            context.Bot.WarningToPlayer(player, $"{user.Name} is unlocked!");
        }

        public static void Cheat(CommandContext context)
        {
            var player = context.Player;
            if (!context.Settings.IsOwner(player) && !context.Settings.IsAdmin(player))
            {
                context.Bot.ErrorToPlayer(player, Constants.MESSAGE_NO_PERMISSION);
                return;
            }

            var isActive = !context.Settings.CheatIsActivate;
            if (context.Values.Length > 0)
            {
                if (!context.TryParseBool(context.JoinWithSkip(" ", context.Values).Trim().ToLower(), out bool result))
                {
                    context.Bot.InfoToPlayer(player, "Usage: !cheat <on|off>");
                    return;
                }

                isActive = result;
            }
            context.Settings.CheatIsActivate = isActive;
            if (context.Settings.CheatIsActivate)
            {
                context.Bot.WarningToAll("Cheating activated.");
            }
            else
            {
                foreach (var user in context.Base.Players)
                    if (context.IsUnlimitedAmmo(user))
                        context.SetUnlimitedAmmo(user, false);

                context.Bot.WarningToAll("Cheating deactivated.");
            }
                
        }

        public static void Semtex(CommandContext context)
        {
            var player = context.Player;
            if (!context.Settings.CheatIsActivate)
            {
                context.Bot.WarningToPlayer(player, "The cheat mode is deactivated, please enable it.");
                return;
            }

            player.GiveWeapon("semtex_mp");
        }

        public static void Flash(CommandContext context)
        {
            var player = context.Player;
            if (!context.Settings.CheatIsActivate)
            {
                context.Bot.WarningToPlayer(player, "The cheat mode is deactivated, please enable it.");
                return;
            }

            player.GiveWeapon("flash_grenade_mp");
        }

        public static void MaxAmmo(CommandContext context)
        {
            var player = context.Player;
            if (!context.Settings.CheatIsActivate)
            {
                context.Bot.WarningToPlayer(player, "The cheat mode is deactivated, please enable it.");
                return;
            }

            context.GiveMaxAmmo(player);
            context.IPrintLnBold(player, "^2Your ammo is max");
        }

        public static void GetWeapon(CommandContext context)
        {
            var player = context.Player;
            if (!context.Settings.CheatIsActivate)
            {
                context.Bot.WarningToPlayer(player, "The cheat mode is deactivated, please enable it.");
                return;
            }

            context.Bot.WarningToPlayer(context.Player, $"Your weapon: {context.Player.CurrentWeapon}");
            context.Log($"{player.Name} weapon: {context.Player.CurrentWeapon}");
        }

        public static void GiveWeapon(CommandContext context)
        {
            var player = context.Player;
            if (!context.Settings.CheatIsActivate)
            {
                context.Bot.WarningToPlayer(player, "The cheat mode is deactivated, please enable it.");
                return;
            }

            string weapon = context.JoinWithSkip(" ", context.Values).Trim();
            player.GiveWeapon(weapon);
            player.SwitchToWeapon(weapon);

            if (player.HasWeapon(weapon))
            {
                context.IPrintLnBold(player, "^2You have received a weapon");
                context.Log($"{player.Name} received a {weapon}");
            }
            else
                context.IPrintLnBold(player, "^3Weapon not found");
        }

        public static void ChangeWeapon(CommandContext context)
        {
            var player = context.Player;
            if (!context.Settings.CheatIsActivate)
            {
                context.Bot.WarningToPlayer(player, "The cheat mode is deactivated, please enable it.");
                return;
            }

            string weapon = context.JoinWithSkip(" ", context.Values).Trim();

            if (player.HasWeapon(weapon))
            {
                player.SwitchToWeapon(weapon);
                return;
            }

            player.GiveWeapon(weapon);
            if (player.HasWeapon(weapon))
            {
                player.TakeWeapon(context.Player.CurrentWeapon);
                player.SwitchToWeapon(weapon);

                context.IPrintLnBold(player, "^2You have received a weapon");
                context.Log($"{player.Name} received a {weapon}");
            }
            else
                context.IPrintLnBold(player, "^3Weapon not found");
        }

        public static void UnlimitedAmmo(CommandContext context)
        {
            var player = context.Player;

            if (!context.Settings.IsOwner(player) && !context.Settings.IsAdmin(player))
            {
                context.Bot.ErrorToPlayer(player, Constants.MESSAGE_NO_PERMISSION);
                return;
            }

            if (!context.Settings.CheatIsActivate)
            {
                context.Bot.WarningToPlayer(player, "The cheat mode is deactivated, please enable it.");
                return;
            }

            var isActive = !context.IsUnlimitedAmmo(player);
            if (context.Values.Length > 0)
            {
                if (!context.TryParseBool(context.JoinWithSkip(" ", context.Values).Trim().ToLower(), out bool result))
                {
                    context.Bot.InfoToPlayer(player, "Usage: !unlimitedAmmo <on|off>");
                    return;
                }

                isActive = result;
            }

            if (context.IsUnlimitedAmmo(player) == isActive)
            {
                if (isActive)
                    context.IPrintLnBold(player, "Unlimited ammo is already active");
                else
                    context.IPrintLnBold(player, "Unlimited ammo is already deactive");

                return;
            }

            context.SetUnlimitedAmmo(player, isActive);
            if (isActive)
            {
                context.Base.OnInterval(50, () =>
                {
                    if (player.CurrentWeapon != "airdrop_marker_mp")
                        context.GiveMaxAmmo(player);

                    return context.Settings.CheatIsActivate && context.IsUnlimitedAmmo(player);
                });
                context.IPrintLnBold(player, "^2Unlimited ammo activated");
            }
            else
                context.IPrintLnBold(player, "^3Unlimited ammo deactivated");
        }
    }
}
