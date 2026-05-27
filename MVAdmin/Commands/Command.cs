using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using InfinityScript;

namespace MVAdmin
{
    internal class Command
    {
        private readonly Action<CommandContext> _action;
        public List<string> Aliases { get; private set; }

        public Command(Action<CommandContext> action, params string[] aliases)
        {
            _action = action;
            Aliases = new List<string>(aliases);
        }

        public bool IsCommand(string command)
        {
            var args = GetCommandArguments(command).ToArray();
            if (args.Length > 0)
            {
                string alias = GetAlias(args);
                return Aliases.Contains(alias);
            }
            return false;
        }

        public void Execute(BaseScript script, Entity player, string command)
        {
            var args = GetCommandArguments(command.Trim()).ToArray();
            CommandContext context = new CommandContext(script, this, player, GetAlias(args), GetValues(args));
            _action(context);
        }

        public string GetAlias(string[] args)
        {
            if (args.Length > 0)
                return args[0].Trim().ToLower().Remove(0, 1);

            return string.Empty;
        }

        public string[] GetValues(string[] args)
        {
            if (args.Length > 1)
                return SkipElements(args, 1);

            return new string[0];
        }

        public List<string> GetCommandArguments(string commandLine)
        {
            var arguments = new List<string>();
            MatchCollection matches = Regex.Matches(commandLine, "'((?:\\\\'|[^'])*)'|(\\S+)");

            foreach (Match match in matches)
            {
                if (match.Groups[1].Success)
                {
                    string singleQuotedArg = match.Groups[1].Value;
                    singleQuotedArg = singleQuotedArg.Replace("\\'", "'").Replace("\\\\", "\\");
                    arguments.Add(singleQuotedArg);
                }
                else if (match.Groups[2].Success)
                {
                    arguments.Add(match.Groups[2].Value);
                }
            }
            return arguments;
        }

        private string[] SkipElements(string[] input, int numToSkip)
        {
            if (input == null) return null;
            if (numToSkip < 0) throw new ArgumentOutOfRangeException(nameof(numToSkip));

            if (numToSkip >= input.Length)
            {
                return new string[0];
            }

            int newLength = input.Length - numToSkip;
            string[] result = new string[newLength];

            Array.Copy(input, numToSkip, result, 0, newLength);

            return result;
        }
    }
}
