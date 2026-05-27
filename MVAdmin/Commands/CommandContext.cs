using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using InfinityScript;

namespace MVAdmin
{
    internal class CommandContext
    {
        public BaseScript Base { get; private set; }
        public Command Sender { get; private set; }
        public Entity Player { get; private set; }
        public string Alias { get; private set; }
        public string[] Values { get; private set; }
        public Settings Settings { get; private set; }
        public Bot Bot { get; private set; }

        public CommandContext(BaseScript script, Command sender, Entity player, string alias, string[] values)
        {
            Base = script;
            Sender = sender;
            Player = player;
            Alias = alias;
            Values = values;
            Settings = (script as MVAdmin).Settings;
            Bot = (script as MVAdmin).Bot;
        }

        public bool TryParseBool(string value, out bool result)
        {
            value = value.ToLowerInvariant().Trim();
            if (value == "y" || value == "ye" || value == "yes" || value == "on" || value == "true" || value == "1")
            {
                result = true;
                return true;
            }
            else if (value == "n" || value == "no" || value == "off" || value == "false" || value == "0")
            {
                result = false;
                return true;
            }
            else
            {
                result = false;
                return false;
            }
        }

        public string JoinWithSkip(string separator, string[] values, int startIndex = 0, int? count = null)
        {
            if (values == null)
                return null;

            if (separator == null)
                separator = string.Empty;

            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Start index cannot be negative.");

            if (startIndex >= values.Length)
                return string.Empty;

            int actualCount;
            if (count.HasValue)
            {
                if (startIndex + count.Value > values.Length)
                    actualCount = values.Length - startIndex;
                else
                    actualCount = count.Value;
            }
            else
                actualCount = values.Length - startIndex;

            if (actualCount <= 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            sb.Append(values[startIndex]);
            for (int i = 1; i < actualCount; i++)
            {
                sb.Append(separator);
                sb.Append(values[startIndex + i]);
            }

            return sb.ToString();
        }

        public string GetDvar(string dvar)
        {
            return (Base as MVAdmin).GetDvar(dvar);
        }

        public void SetDvar(string dvar, string value)
        {
            (Base as MVAdmin).SetDvar(dvar, value);
        }

        public void IPrintLnBold(Entity player, string message)
        {
            player.Call("iprintlnbold", new Parameter[] { message });
        }

        public void IPrintLn(Entity player, string message)
        {
            player.Call("iprintln", new Parameter[] { message });
        }

        public void ChangeTeam(Entity player, string team)
        {
            player.SetField("sessionteam", team);
            player.Notify("menuresponse", new Parameter[] { "team_marinesopfor", team });
        }

        public void GiveMaxAmmo(Entity player)
        {
            player.Call("giveMaxAmmo", new Parameter[] { player.CurrentWeapon });
        }

        public bool IsUnlimitedAmmo(Entity player)
        {
            return player.HasField("is_unlimited_ammo") && player.GetField<int>("is_unlimited_ammo") == 1;
        }

        public void SetUnlimitedAmmo(Entity player, bool result)
        {
            player.SetField("is_unlimited_ammo", result ? 1 : 0);
        }

        public void Suicide(Entity player)
        {
            Base.AfterDelay(100, () =>
            {
                player.Call("suicide", new Parameter[0]);
            });
        }

        public void Execute(string command)
        {
            Utilities.ExecuteCommand(command);
        }

        public void Log(string message)
        {
            (Base as MVAdmin).Info(message);
        }

        public Entity FindPlayer(string info)
        {
            if (int.TryParse(info, out int num))
                return Base.Players.FirstOrDefault(p => Utils.GetEntityNumber(p) == num);

            return Base.Players.FirstOrDefault(p => p.Name.Trim().ToLower().Contains(info.Trim().ToLower()) || p.HWID == info.Trim().ToUpper());
        }

        public IEnumerable<Entity> FindPlayers(string info)
        {
            if (int.TryParse(info, out int num))
                return Base.Players.Where(p => Utils.GetEntityNumber(p) == num);

            return Base.Players.Where(p => p.Name.Trim().ToLower().Contains(info.Trim().ToLower()) || p.HWID == info.Trim().ToUpper());
        }

        public string FindMap(string map)
        {
            map = map.ToLower();

            //Standard Maps
            if (map == "seatown" || map == "sea")
                return "mp_seatown";
            else if (map == "dome" || map == "dom")
                return "mp_dome";
            else if (map == "arkaden" || map == "ark")
                return "mp_plaza2";
            else if (map == "bakaara" || map == "bak")
                return "mp_mogadishu";
            else if (map == "resistance" || map == "res")
                return "mp_paris";
            else if (map == "downturn" || map == "dow")
                return "mp_exchange";
            else if (map == "bootleg" || map == "boo")
                return "mp_bootleg";
            else if (map == "carbon" || map == "car")
                return "mp_carbon";
            else if (map == "hardhat" || map == "har")
                return "mp_hardhat";
            else if (map == "lockdown" || map == "loc")
                return "mp_alpha";
            else if (map == "village" || map == "vil")
                return "mp_vilage";
            else if (map == "fallen" || map == "fal")
                return "mp_lambeth";
            else if (map == "outpost" || map == "out")
                return "mp_radar";
            else if (map == "interchange" || map == "intc")
                return "mp_interchange";
            else if (map == "underground" || map == "und")
                return "mp_underground";
            else if (map == "mission" || map == "mis")
                return "mp_bravo";

            //[DLC 1] Collection 1
            else if (map == "piazza" || map == "pia")
                return "mp_italy";
            else if (map == "liberation" || map == "lib")
                return "mp_park";
            else if (map == "overwatch" || map == "ove")
                return "mp_overwatch";
            else if (map == "blackbox" || map == "bla")
                return "mp_morningwood";

            //[DLC 2] Collection 2
            else if (map == "sanctuary" || map == "san")
                return "mp_meteora";
            else if (map == "foundation" || map == "fou")
                return "mp_cement";
            else if (map == "oasis" || map == "oas")
                return "mp_qadeem";
            else if (map == "erosion" || map == "ero")
                return "mp_courtyard_ss";
            else if (map == "aground" || map == "agr")
                return "mp_aground_ss";

            //[DLC 3] Collection 3: Chaos Pack
            else if (map == "uturn" || map == "utr")
                return "mp_burn_ss";
            else if (map == "vortex" || map == "vor")
                return "mp_six_ss";
            else if (map == "intersection" || map == "ints")
                return "mp_crosswalk_ss";

            //[DLC 4] Collection 4: Final Assault
            else if (map == "boardwalk" || map == "boa")
                return "mp_boardwalk";
            else if (map == "decommission" || map == "dec")
                return "mp_shipbreaker";
            else if (map == "gulch" || map == "gul")
                return "mp_moab";
            else if (map == "offshore" || map == "off")
                return "mp_roughneck";
            else if (map == "parish" || map == "par")
                return "mp_nola";

            //Face Off
            else if (map == "lookout" || map == "loo")
                return "mp_restrepo_ss";
            else if (map == "getaway" || map == "get")
                return "mp_hillside_ss";

            //Free
            else if (map == "terminal" || map == "ter")
                return "mp_terminal_cls";

            else
                return string.Empty;
        }

        public string FindMod(string mod, bool hardCore)
        {
            var type = (hardCore ? "HC" : "SC");
            mod = mod.ToLower();

            //Standard
            if (mod == "dm" || mod == "ffa")
                return "FFA-" + type;
            else if (mod == "war" || mod == "tdm")
                return "TDM-" + type;
            else if (mod == "sd")
                return "SD-" + type;
            else if (mod == "sab")
                return "SAB-" + type;
            else if (mod == "dom")
                return "DOM-" + type;
            else if (mod == "koth" || mod == "hq")
                return "HQ-" + type;
            else if (mod == "ctf")
                return "CTF-" + type;
            else if (mod == "dd")
                return "DD-" + type;
            else if (mod == "conf" || mod == "kc")
                return "KC-" + type;
            else if (mod == "tdef")
                return "TDEF-" + type;

            //Alternative
            else if (mod == "grnd" || mod == "dz")
                return "DZ-" + type;
            else if (mod == "tjugg" || mod == "tj")
                return "TJ-" + type;
            else if (mod == "jugg" || mod == "jug")
                return "JUG-" + type;
            else if (mod == "gun" || mod == "gg")
                return "GG-" + type;
            else if (mod == "infect" || mod == "inf")
                return "INF-" + type;
            else if (mod == "oic")
                return "OIC-" + type;

            else
                return string.Empty;
        }
    }
}
