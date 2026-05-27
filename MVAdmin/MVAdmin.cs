using System;
using System.Linq;
using System.Collections.Generic;
using InfinityScript;

namespace MVAdmin
{
    public class MVAdmin : BaseScript
    {
        private readonly List<Command> _commands = new List<Command>()
        {
            new Command(Functions.Owner, "owner"),
            new Command(Functions.Me, "me"),
            new Command(Functions.Info, "info"),
            new Command(Functions.PushAdmin, "push_admin", "add_admin", "pushadmin", "addadmin"),
            new Command(Functions.PopAdmin, "pop_admin", "remove_admin", "popadmin", "removeadmin"),
            new Command(Functions.GetAdmins, "admins"),
            new Command(Functions.FastRestart, "fast_restart", "fast_reset", "fast_res", "fastrestart", "fastreset", "fastres", "restart", "reset", "res"),
            new Command(Functions.MapRestart, "map_restart", "map_reset", "map_res", "maprestart", "mapreset", "mapres"),
            new Command(Functions.Rotate, "map_rotate", "maprotate", "rotate"),
            new Command(Functions.Map, "map"),
            new Command(Functions.Mod, "mod"),
            new Command(Functions.ChangePlayersAliveState, "show_players_alive", "showplayersalive"),
            new Command(Functions.SayToAllFromBot, "say"),
            new Command(Functions.SayToPlayerFromBot, "say_to", "sayto"),
            new Command(Functions.Afk, "afk"),
            new Command(Functions.ChangeTeam, "change_team", "changeteam"),
            new Command(Functions.ChangeHostName, "host_name", "hostname"),
            new Command(Functions.AxisName, "axis_name", "axisname"),
            new Command(Functions.AxisIcon, "axis_icon", "axisicon"),
            new Command(Functions.AlliesName, "allies_name", "alliesname"),
            new Command(Functions.AlliesIcon, "allies_icon", "alliesicon"),
            new Command(Functions.Suicide, "suicide", "kill"),
            new Command(Functions.Kick, "kick"),
            new Command(Functions.Ban, "ban"),
            new Command(Functions.TempBan, "temp-ban", "tempban", "tban"),
            new Command(Functions.Unban, "unban"),
            new Command(Functions.Lock, "lock"),
            new Command(Functions.Unlock, "unlock"),
            new Command(Functions.Version, "version", "ver"),
            new Command(Functions.Server, "server", "console"),

            new Command(Functions.Cheat, "cheat"),
            new Command(Functions.Semtex, "semtex"),
            new Command(Functions.Flash, "flash"),
            new Command(Functions.MaxAmmo, "max_ammo", "maxammo", "mammo"),
            new Command(Functions.UnlimitedAmmo, "unlimited_ammo", "unlimitedammo", "uammo"),
            new Command(Functions.GetWeapon, "get_weapon", "getweapon"),
            new Command(Functions.GiveWeapon, "give_weapon", "giveweapon", "gweapon"),
            new Command(Functions.ChangeWeapon, "change_weapon", "changeweapon", "cweapon"),
        };

        internal Settings Settings { get; private set; }
        internal Bot Bot { get; private set; }

        public MVAdmin()
        {
            Settings = new Settings();
            Bot = new Bot();

            Tick += OnTick;
            PlayerConnected += OnPlayerConnected;
            HudLogo();
        }

        private void OnTick()
        {
            if (!string.IsNullOrWhiteSpace(Settings.HostName))
                Utilities.ExecuteCommand($"sv_hostname \"{Settings.HostName}\"");

            Utilities.ExecuteCommand($"g_TeamName_Allies {Settings.AlliesName}");
            Utilities.ExecuteCommand($"g_TeamName_Axis {Settings.AxisName}");
            Utilities.ExecuteCommand($"g_TeamIcon_Allies {Settings.AlliesIcon}");
            Utilities.ExecuteCommand($"g_TeamIcon_Axis {Settings.AxisIcon}");
        }

        private void OnPlayerConnected(Entity player)
        {
            HudPlayersAlive(player);
            LoadUser(player);
        }

        public override EventEat OnSay3(Entity player, ChatType type, string name, ref string message)
        {
            if (!string.IsNullOrWhiteSpace(message) && message.StartsWith("!"))
            {
                string commandMessage = message;
                var command = _commands.FirstOrDefault(c => c.IsCommand(commandMessage));
                if (command != null)
                {
                    try
                    {
                        command.Execute(this, player, commandMessage);
                    }
                    catch (Exception ex)
                    {
                        Info($"Exception: {ex.Message}");
                    }
                }
                else
                {
                    Bot.ErrorToPlayer(player, Constants.MESSAGE_UNKNOWN_COMMAND);
                    Info(Constants.MESSAGE_UNKNOWN_COMMAND_FROM.Replace("<player>", player.Name).Replace("<command>", commandMessage));
                }
                return EventEat.EatGame;
            }
            return base.OnSay3(player, type, name, ref message);
        }

        public override void OnPlayerKilled(Entity victim, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        {
            if (!attacker.HasField("killstreak"))
                attacker.SetField("killstreak", 0);

            if (!victim.HasField("killstreak"))
                victim.SetField("killstreak", 0);


            int attackerKills = attacker.GetField<int>("killstreak") + 1;
            int victimKills = victim.GetField<int>("killstreak");
            attacker.SetField("killstreak", attackerKills);
            victim.SetField("killstreak", 0);

            if (victimKills >= 5)
            {
                Bot.SayToAll($"^2{victim.Name}'s ^7killing spree ended (^3{victimKills} ^7kills). He was killed by ^3{attacker.Name}!");
            }

            if (attackerKills == 5)
            {
                Bot.SayToAll($"Nice spree, ^2{attacker.Name}! ^7got ^35 ^7kills in a row.");
            }
            else if (attackerKills == 10)
            {
                Bot.SayToAll($"Nice spree, ^2{attacker.Name}! ^7got ^310 ^7kills in a row!");
            }

            if (mod == "MOD_HEAD_SHOT")
            {
                Bot.SayToAll($"^2{attacker.Name} ^7killed ^1{victim.Name} ^7by ^3 Headshot");
            }
            else if (mod == "MOD_EXPLOSIVE")
            {
                Bot.SayToAll($"^1{victim.Name} ^7has exploded!");
            }

            base.OnPlayerKilled(victim, inflictor, attacker, damage, mod, weapon, dir, hitLoc);
        }

        private void HudLogo()
        {
            HudElem logoHud = HudElem.CreateServerFontString("bigfixed", 0.5f);
            logoHud.Color = new Vector3(1f, 0.75f, 0f);
            logoHud.GlowAlpha = 1f;
            logoHud.GlowColor = new Vector3(0.349f, 0f, 0f);
            logoHud.SetText(Constants.HUD_PLUGIN_NAME);
            logoHud.SetPoint("TOPRIGHT", "TOPRIGHT", -7, 7);
        }

        private void HudPlayersAlive(Entity player)
        {
            HudElem playersHud = HudElem.CreateFontString(player, "hudbig", 0.6f);
            playersHud.SetPoint("BOTTOMCENTER", "BOTTOMCENTER", 0, -5);
            playersHud.HideWhenInMenu = true;
            OnInterval(50, () =>
            {
                string gameType = GetDvar("g_gametype");
                if (Settings.ShowPlayersAlive && gameType != "dm" && gameType != "gun" && gameType != "oic")
                {
                    string alliesCount = Call<int>("getteamplayersalive", new Parameter[] { "allies" }).ToString();
                    string axisCount = Call<int>("getteamplayersalive", new Parameter[] { "axis" }).ToString();
                    string friendCount = Utils.TeamIsAllies(player) ? alliesCount : axisCount;
                    string enemyCount = Utils.TeamIsAllies(player) ? axisCount : alliesCount;
                    playersHud.SetText(Constants.HUD_PLAYERS.Replace("<friendCount>", friendCount).Replace("<enemyCount>", enemyCount));
                }
                else
                    playersHud.SetText(string.Empty);

                return true;
            });
        }

        private void LoadUser(Entity player)
        {
            if (Settings.IsLocked(player, out string name))
            {
                if (player.Name != name)
                {
                    Bot.SayToAll($"'{player.Name}' is '{name}' old");
                }
            }
        }

        public void Info(string message)
        {
            Log.Info(message);
        }

        public string GetDvar(string dvar)
        {
            return Call<string>("getdvar", new Parameter[] { dvar });
        }

        public void SetDvar(string dvar, string value)
        {
            Call("setdvar", new Parameter[] { dvar, value });
        }
    }
}
