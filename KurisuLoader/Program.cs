using System;
using System.IO;
using System.Linq;
using System.Reflection;
using LeagueSharp;

namespace KurisuLoader
{
    class Root
    {

        public static string Player = ObjectManager.Player.SkinName;
        static void Main(string[] args)
        {
            new Root();
        }

        #region Properties

        /// <summary>
        /// Assembly Directory
        /// </summary>
        private static readonly string ADirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);


        /// <summary>
        /// Dynamic-link library location
        /// </summary>
        private readonly string _dllPath = ADirectory + @"\LeagueSharp\Kurisu" + Player + ".dll";

        /// <summary>
        /// Allowed files inside the directory
        /// </summary>
        private readonly String[] _validFiles =
        {
            "Assemblies", "Repositories", "KurisuMorgana.dll", "KurisuFiora.dll", "KurisuNidalee.dll", "KurisuBlitzcrank.dll"
        };

        /// <summary>
        /// Represents if assembly is loaded or not
        /// </summary>
        private readonly bool _loaded;

        #endregion

        #region Helpers

        public Root()
        {
            // Check Installation
            if (!ValidateInstallation())
            {
                _loaded = false;
                Game.PrintChat("Kurisu" + Player + ".ValidateInstallation Failed");
            }
            else
            {
                Cleanup();
                if (LoadAssembly())
                    _loaded = true;
                else
                {
                    Game.PrintChat("Kurisu" + Player + ".LoadAssembly Failed");
                }
            }
        }

        /// <summary>
        /// Checks if the assembly is installed right
        /// </summary>
        /// <returns></returns>
        private bool ValidateInstallation()
        {
            try
            {
                if (File.Exists(_dllPath))
                {
                    return true;
                }

                Game.PrintChat("Please contact Kurisu, for installation instructions, " + Player + " was not installed correctly.");

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Game.PrintChat("Exception thrown at Kurisu" + Player + ".ValidateInstallation");
                return false;
            }
        }

        /// <summary>
        /// Checks wheter the files in the directory are right
        /// </summary>
        /// <returns></returns>
        private void Cleanup()
        {
            try
            {
                var dInfo = new DirectoryInfo(ADirectory + @"\LeagueSharp\");
                for (int index = 0; index < dInfo.GetFiles().Length; index++)
                {
                    FileInfo file = dInfo.GetFiles()[index];
                    if (_validFiles.Contains(file.Name)) continue;
                    Game.PrintChat("<font color=\"#F2F2F2\">[Loader] </font> <font color=\"#D9D9D9\">Removing " + file.Name + " from updater...</font>");
                    file.Delete();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Game.PrintChat("Exception thrown at Kurisu" + Player + ".Cleanup");
            }
        }

        /// <summary>
        ///  Loads the LoL assembly
        /// </summary>
        /// <returns></returns>
        private bool LoadAssembly()
        {
            try
            {
                Assembly assembly = Assembly.LoadFile(_dllPath);
                Type _type = assembly.GetType("Kurisu" + Player + ".Root");
                var obj = Activator.CreateInstance(_type);
                Console.WriteLine("Launched");
                return obj != null;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Game.PrintChat("Exception thrown at Kurisu" + Player + ".LoadAssembly");
                return false;
            }
        }

        #endregion

    }
}
