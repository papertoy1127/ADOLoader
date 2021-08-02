using System.Collections.Generic;
using ADOLoader.Core.TweakSettings;

namespace ADOLoader.Core {
    public class Debug {
        public static Dictionary<string, TweakSetting> TweakSettings = new();

        public static void RegisterTweakSettings(IEnumerable<TweakSetting> settings) {
            foreach (var setting in settings) TweakSettings.Add(setting.Name, setting);
        }
    }
}