using InfinityScript;

namespace MVAdmin
{
    internal class Utils
    {
        public static string GetTeam(Entity player)
        {
            return player.GetField<string>("sessionteam");
        }

        public static bool IsSpectating(Entity player)
        {
            return GetTeam(player) == "spectator";
        }

        public static bool TeamIsAxis(Entity player)
        {
            return GetTeam(player) == "axis";
        }

        public static bool TeamIsAllies(Entity player)
        {
            return GetTeam(player) == "allies";
        }

        public static bool TeamIsNone(Entity player)
        {
            return GetTeam(player) == "none";
        }

        public static int GetEntityNumber(Entity player)
        {
            return player.Call<int>("getentitynumber", new Parameter[0]);
        }

        public static bool EqualsHwid(Entity player, string hwid)
        {
            if (player == null || string.IsNullOrWhiteSpace(hwid))
                return false;

            if (player.HWID == hwid)
                return true;

            return false;
        }
    }
}
