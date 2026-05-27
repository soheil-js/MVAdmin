using InfinityScript;

namespace MVAdmin
{
    internal class Bot
    {
        private static string _prefix = "^3[^4MV^3] ";

        public void SayToAll(string message, string colorCode = "^7")
        {
            Utilities.RawSayAll(_prefix + colorCode + message);
        }

        public void SayToPlayer(Entity player, string message, string colorCode = "^7")
        {
            Utilities.RawSayTo(player, _prefix + colorCode + message);
        }

        public void OkToAll(string message) => SayToAll(message, "^2");
        public void OkToPlayer(Entity player, string message) => SayToPlayer(player, message, "^2");

        public void InfoToAll(string message) => SayToAll(message, "^5");
        public void InfoToPlayer(Entity player, string message) => SayToPlayer(player, message, "^5");

        public void WarningToAll(string message) => SayToAll(message, "^:");
        public void WarningToPlayer(Entity player, string message) => SayToPlayer(player, message, "^:");

        public void ErrorToAll(string message) => SayToAll(message, "^1");
        public void ErrorToPlayer(Entity player, string message) => SayToPlayer(player, message, "^1");
    }
}
