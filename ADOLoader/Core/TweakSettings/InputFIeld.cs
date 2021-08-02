using ADOLoader.Component;

namespace ADOLoader.Core.TweakSettings {
    public interface ITweakInputField: ITweakNoArrow {
        public FormatInputField InputField { get; set; }
    }

    public class TweakSettingTweakInputField<T> : TweakSetting<T>, ITweakInputField {
        public TweakSettingTweakInputField(string name, T defaultValue = default) : base(name, defaultValue) { }
        public FormatInputField InputField { get; set; }

        public override T Value {
            set {
                InputField.text = value.ToString();
                base.Value = value;
            }
        }
    }
}