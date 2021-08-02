using System;

namespace ADOLoader.Core.TweakSettings {
    public interface ISizableSetting {
        int Height { get; set; }
    }

    public interface ITweakHideName : ITweakHideValue { }

    public class TweakSettingsGUILayout : TweakSetting<object>, ISizableSetting, ITweakHideName {
        public TweakSettingsGUILayout(string name, int height = 24) : base(name) {
            Height = height;
        }
        public Action OnGUI { get; set; }
        public int Height { get; set; }
    }
}