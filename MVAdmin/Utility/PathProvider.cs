using System;
using System.IO;

namespace MVAdmin
{
    internal static class PathProvider
    {
        //Folders
        public static string Root { get; private set; }
        public static string Scripts { get; private set; }
        public static string Players2 { get; private set; }
        public static string MVAdmin { get; private set; }

        //Files
        public static string Dspl { get; private set; }

        static PathProvider()
        {
            Root = Environment.CurrentDirectory;
            Scripts = Path.Combine(Root, "scripts");
            Players2 = Path.Combine(Root, "players2");
            MVAdmin = Path.Combine(Scripts, "MVAdmin");

            Dspl = Path.Combine(Players2, "Default.dspl");

            if (!Directory.Exists(MVAdmin))
                Directory.CreateDirectory(MVAdmin);
        }

        public static bool DsrExists(string name)
        {
            string dsr = Path.Combine(Players2, $"{name}.dsr");
            return File.Exists(dsr);
        }
    }
}
