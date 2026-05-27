using System;
using System.IO;
using System.Collections.Generic;
using InfinityScript;

namespace MVAdmin
{
    internal class Settings
    {
        private readonly IniReader _config;
        private readonly IniReader _users;

        public string Owner { get; private set; }
        public List<string> Admins { get; private set; }
        public bool ShowPlayersAlive { get; private set; }
        public string HostName { get; private set; }
        public string AlliesName { get; private set; }
        public string AlliesIcon { get; private set; }
        public string AxisName { get; private set; }
        public string AxisIcon { get; private set; }

        public bool CheatIsActivate { get; set; }

        public Settings()
        {
            string configPath = Path.Combine(PathProvider.MVAdmin, "Config.ini");
            string usersPath = Path.Combine(PathProvider.MVAdmin, $"Users.ini");

            _config = new IniReader(configPath);
            _users = new IniReader(usersPath);

            Owner = _config.ReadString("Permission", "Owner", string.Empty);

            try
            {
                var admins = _config.ReadString("Permission", "Admins", string.Empty);
                string[] parts = admins.Trim().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                Admins = new List<string>(parts);
            }
            catch
            {
                Admins = new List<string>();
            }

            ShowPlayersAlive = _config.ReadBoolean("Hud", "ShowPlayersAlive", true);
            HostName = _config.ReadString("Setting", "HostName", "^1Red^7&^4Blue");
            AlliesName = _config.ReadString("Setting", "AlliesName", "^1Red");
            AlliesIcon = _config.ReadString("Setting", "AlliesIcon", "cardicon_pacifier_pink");
            AxisName = _config.ReadString("Setting", "AxisName", "^4Blue");
            AxisIcon = _config.ReadString("Setting", "AxisIcon", "cardicon_pacifier_blue");

            if (!File.Exists(configPath))
            {
                _config.Write("Permission", "Owner", Owner);
                _config.Write("Permission", "Admins", string.Empty);
                _config.Write("Hud", "ShowPlayersAlive", ShowPlayersAlive);
                _config.Write("Setting", "HostName", HostName);
                _config.Write("Setting", "AlliesName", AlliesName);
                _config.Write("Setting", "AlliesIcon", AlliesIcon);
                _config.Write("Setting", "AxisName", AxisName);
                _config.Write("Setting", "AxisIcon", AxisIcon);
            }
        }

        public void SetOwner(Entity player)
        {
            Owner = player.HWID;
            _config.Write("Permission", "Owner", Owner);
        }

        public bool IsOwner(Entity player)
        {
            if (Utils.EqualsHwid(player, Owner))
                return true;
            return false;
        }

        public void AddToAdmin(Entity player)
        {
            var hwid = player.HWID;
            if (!Admins.Contains(hwid))
            {
                Admins.Add(hwid);
                _config.Write("Permission", "Admins", string.Join(",", Admins));
            }
        }

        public void RemoveFromAdmin(Entity player)
        {
            if (Admins.Remove(player.HWID))
                _config.Write("Permission", "Admins", string.Join(",", Admins));
        }
        
        public bool IsAdmin(Entity player)
        {
            if (Admins.Contains(player.HWID))
                return true;
            return false;
        }

        public void ChangePlayersInfoState(bool isShow)
        {
            ShowPlayersAlive = isShow;
            _config.Write("Hud", "ShowPlayersAlive", ShowPlayersAlive);
        }

        public void ChangeHostName(string name)
        {
            HostName = name;
            _config.Write("Setting", "HostName", HostName);
        }

        public void ChangeAlliesName(string name)
        {
            AlliesName = name;
            _config.Write("Setting", "AlliesName", AlliesName);
        }

        public void ChangeAlliesIcon(string icon)
        {
            AlliesIcon = icon;
            _config.Write("Setting", "AlliesIcon", AlliesIcon);
        }

        public void ChangeAxisName(string name)
        {
            AxisName = name;
            _config.Write("Setting", "AxisName", AxisName);
        }

        public void ChangeAxisIcon(string icon)
        {
            AxisIcon = icon;
            _config.Write("Setting", "AxisIcon", AxisIcon);
        }

        public bool IsLocked(Entity player, out string name)
        {
            string playerName = _users.ReadString("GUIDNames", player.HWID, string.Empty);
            if (!string.IsNullOrWhiteSpace(playerName))
            {
                name = playerName;
                return true;
            }

            name = string.Empty;
            return false;
        }

        public void LockedPlayer(Entity player)
        {
            string name = _users.ReadString("GUIDNames", player.HWID, string.Empty);
            if (string.IsNullOrWhiteSpace(name))
                _users.Write("GUIDNames", player.HWID, player.Name);
        }

        public void UnlockedPlayer(Entity player)
        {
            string name = _users.ReadString("GUIDNames", player.HWID, string.Empty);
            if (!string.IsNullOrWhiteSpace(name))
                _users.DeleteKey("GUIDNames", player.HWID);
        }
    }
}
