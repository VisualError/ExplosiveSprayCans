using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using LethalSprayCans.Patches;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

namespace LethalSprayCans
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class LethalSprayCanBase : BaseUnityPlugin
    {
        private const string modGUID = "Ryokune.ExplosiveSprayCans";
        private const string modName = "Lethal Spray Cans";
        private const string modVersion = "1.0.0";
        private readonly Harmony harmony = new Harmony(modGUID);

        internal static ConfigEntry<int> ProbabilityMin;
        internal static ConfigEntry<int> ProbabilityMax;
        internal static ConfigEntry<int> InfiniteSpray;
        internal static ConfigEntry<int> RefreshToggle;
        internal static ConfigEntry<KeyControl> KeyToggle;
        public static ManualLogSource mls;
        private static LethalSprayCanBase instance;
        void Awake()
        {
            //AssetBundle.LoadFromMemory();
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }

            if (instance == null)
            {
                instance = this;
            }

            ProbabilityMin = Config.Bind("Probability", "Min", 0, "Min Multiplier");
            ProbabilityMax = Config.Bind("Probability", "Max", 20, "Max Multiplier");

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo($"Plugin {modName} is loaded!");

            harmony.PatchAll(typeof(LethalSprayCanBase));
            harmony.PatchAll(typeof(SprayCanPatch));
        }
    }
}
