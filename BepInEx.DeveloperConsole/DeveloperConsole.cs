using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using System.Linq;
using UnityEngine;

namespace BepInEx
{
    [BepInPlugin(GUID, PluginName, Version)]
    public partial class DeveloperConsole : BaseUnityPlugin
    {
        public const string GUID = "com.bepis.bepinex.developerconsole";
        public const string PluginName = "Developer Console";
        public const string Version = "1.0";
        internal static new ManualLogSource Logger;

        private bool showingUI = false;
        private static string TotalLog = "";
        private Rect UI = new Rect(20, 20, 400, 400);
        private static Vector2 scrollPosition = Vector2.zero;

        public static ConfigFile BepinexConfig { get; } = new ConfigFile(Utility.CombinePaths(Paths.ConfigPath, "BepInEx.cfg"), false);

        public static ConfigWrapper<int> LogDepth { get; private set; }
        public static ConfigWrapper<bool> LogUnity { get; private set; }

        public DeveloperConsole()
        {
            LogDepth = Config.Wrap("Config", "Log buffer size", "Size of the log buffer in characters", 16300);
            LogUnity = BepinexConfig.Wrap("Logging", "UnityLogListening", "Enables showing unity log messages in the BepInEx logging system.", true);
                       
            Logging.Logger.Listeners.Add(new LogListener());
            Logger = base.Logger;
        }

        private static void OnEntryLogged(LogEventArgs logEventArgs)
        {
            string current = $"{TotalLog}\r\n{logEventArgs.Data?.ToString()}";
            if (current.Length > LogDepth.Value)
            {
                var trimmed = current.Remove(0, 1000);

                // Trim until the first newline to avoid partial line
                var newlineHit = false;
                trimmed = new string(trimmed.SkipWhile(x => !newlineHit && !(newlineHit = (x == '\n'))).ToArray());

                current = "--LOG TRIMMED--\n" + trimmed;
            }
            TotalLog = current;

            scrollPosition = new Vector2(0, float.MaxValue);
        }

        protected void OnGUI()
        {
            if (showingUI)
                UI = GUILayout.Window(589, UI, WindowFunction, "Developer Console");
        }

        protected void Update()
        {
            if (Input.GetKeyDown(KeyCode.Pause))
                showingUI = !showingUI;
        }

        private void WindowFunction(int windowID)
        {
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Clear console"))
                    TotalLog = "Log cleared";
                if (GUILayout.Button("Dump scene"))
                    SceneDumper.DumpScene();

                LogUnity.Value = GUILayout.Toggle(LogUnity.Value, "Unity");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical(GUI.skin.box);
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
                {
                    GUILayout.BeginVertical();
                    {
                        GUILayout.FlexibleSpace();
                        GUILayout.TextArea(TotalLog, GUI.skin.label);
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();

            GUI.DragWindow();
        }
    }
}