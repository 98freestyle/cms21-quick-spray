// CMS21 Quick Spray
// Speeds up the rust penetrant spray on stuck bolts in Car Mechanic Simulator 2021.
//
// The spray is the game coroutine ToolsManager.ShowMountObjectSpray. It has no
// duration value to edit, so while that coroutine is running (and only then)
// the game speed is raised, and it's put back the moment the spray finishes.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MelonLoader;

[assembly: MelonInfo(typeof(QuickSpray.QuickSprayMod), "CMS21 Quick Spray", "1.0.0", "98freestyle")]
[assembly: MelonGame("Red Dot Games", "Car Mechanic Simulator 2021")]

namespace QuickSpray
{
    public class QuickSprayMod : MelonMod
    {
        private const string SprayCoroutinePrefix = "_ShowMountObjectSpray_d__";
        private const double FailsafeSeconds = 3.0;
        private const string ConfigFileName = "QuickSpray.cfg";

        private static MelonPreferences_Entry<bool> _enabled;
        private static MelonPreferences_Entry<float> _multiplier;

        private static PropertyInfo _timeScale;
        private static PropertyInfo _pointer;

        private static readonly HashSet<IntPtr> _activeSprays = new HashSet<IntPtr>();
        private static float _previousScale = 1f;
        private static bool _boosting;
        private static DateTime _boostStarted;
        private static bool _setupAttempted;

        public override void OnInitializeMelon()
        {
            // Settings live in their own file: UserData\QuickSpray.cfg
            var category = MelonPreferences.CreateCategory("QuickSpray", "CMS21 Quick Spray");
            category.SetFilePath(Path.Combine(MelonUtils.UserDataDirectory, ConfigFileName), autoload: true, printmsg: false);
            _enabled = category.CreateEntry("Enabled", true, description: "Turn Quick Spray on or off");
            _multiplier = category.CreateEntry("Multiplier", 5f, description: "How many times faster the rusty bolt spray plays (1 = normal)");
            category.SaveToFile(false);
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (_setupAttempted) return;
            _setupAttempted = true;

            _timeScale = FindType(t => t.FullName == "UnityEngine.Time")
                ?.GetProperty("timeScale", BindingFlags.Public | BindingFlags.Static);

            Type sprayCoroutine = FindType(t => t.Name.StartsWith(SprayCoroutinePrefix), "Assembly-CSharp-firstpass");
            MethodInfo moveNext = sprayCoroutine?.GetMethod("MoveNext");
            _pointer = sprayCoroutine?.GetProperty("Pointer");

            if (_timeScale == null || moveNext == null || _pointer == null)
            {
                LoggerInstance.Error($"Setup failed, game code not found (timeScale={_timeScale != null}, spray={moveNext != null}, pointer={_pointer != null}). The game may have updated.");
                return;
            }

            HarmonyInstance.Patch(moveNext,
                postfix: new HarmonyMethod(typeof(QuickSprayMod).GetMethod(nameof(AfterSprayStep), BindingFlags.Static | BindingFlags.NonPublic)));

            LoggerInstance.Msg($"Ready. Rusty bolt spray runs at x{_multiplier.Value}.");
        }

        public override void OnUpdate()
        {
            // If a spray is interrupted and never finishes, don't leave the game sped up.
            if (_boosting && (DateTime.UtcNow - _boostStarted).TotalSeconds > FailsafeSeconds)
            {
                _activeSprays.Clear();
                RestoreSpeed();
            }
        }

        // Runs after every step of the spray coroutine. __result is false once it has finished.
        private static void AfterSprayStep(object __instance, bool __result)
        {
            try
            {
                IntPtr spray = (IntPtr)_pointer.GetValue(__instance);

                if (__result && _activeSprays.Add(spray))
                    BoostSpeed();
                else if (!__result && _activeSprays.Remove(spray) && _activeSprays.Count == 0)
                    RestoreSpeed();
            }
            catch (Exception e)
            {
                MelonLogger.Error("[Quick Spray] " + e.Message);
            }
        }

        private static void BoostSpeed()
        {
            if (_boosting || !_enabled.Value) return;

            float current = (float)_timeScale.GetValue(null);
            if (current <= 0f) return; // game is paused, leave it alone

            _previousScale = current;
            _timeScale.SetValue(null, current * Multiplier);
            _boosting = true;
            _boostStarted = DateTime.UtcNow;
        }

        private static void RestoreSpeed()
        {
            if (!_boosting) return;

            // Only restore if nothing else (pause menu, debug speed) changed the speed meanwhile.
            float current = (float)_timeScale.GetValue(null);
            if (Math.Abs(current - _previousScale * Multiplier) < 0.001f)
                _timeScale.SetValue(null, _previousScale);

            _boosting = false;
        }

        private static float Multiplier => Math.Max(1f, _multiplier.Value);

        private static Type FindType(Func<Type, bool> match, string assemblyName = null) =>
            AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => assemblyName == null || a.GetName().Name == assemblyName)
                .SelectMany(GetTypesSafe)
                .FirstOrDefault(match);

        private static IEnumerable<Type> GetTypesSafe(Assembly assembly)
        {
            try { return assembly.GetTypes(); }
            catch (ReflectionTypeLoadException e) { return e.Types.Where(t => t != null); }
        }
    }
}
