using UnityEngine;
using UnityEngine.UI;

namespace ADOLoader.Core.TweakSettings {
    public interface ITweakSlider : ITweakHideValue {
        public Slider Slider { get; set; }
        public Text SliderValue { get; set; }
    }

    public class TweakSettingIntSlider : TweakSettingInt, ITweakSlider {
        public TweakSettingIntSlider(string name, int defaultValue = default, int minValue = 0, int maxValue = 10,
            int changeBy = 1) : base(name, defaultValue, minValue, maxValue, changeBy) { }

        private Slider _slider;

        public Slider Slider {
            get => _slider;
            set {
                _slider = value;
                _slider.maxValue = MaxValue;
                _slider.minValue = MinValue;
                _slider.value = Value;
                _slider.wholeNumbers = true;
                SliderValue = _slider.transform.Find("Text").GetComponent<Text>();
                SliderValue.font = RDC.data.koreanFont;
                _slider.onValueChanged.AddListener(val => {
                    Value = (int) Mathf.RoundToInt((val - DefaultValue) / ChangeBy) * ChangeBy + DefaultValue;
                    SliderValue.text = Value.ToString();
                });
                _slider.onValueChanged.Invoke(Value);
            }
        }

        public Text SliderValue { get; set; }

        public override int Value {
            set {
                if (value.CompareTo(MaxValue) <= 0 && value.CompareTo(MinValue) >= 0) {
                    base.Value = value;
                }
                _slider.value = Value;
            }
        }
    }
    
    
    public class TweakSettingFloatSlider : TweakSettingFloat, ITweakSlider {
        public TweakSettingFloatSlider(string name, float defaultValue = default, float minValue = 0, float maxValue = 10,
            float changeBy = 1) : base(name, defaultValue, minValue, maxValue, changeBy) { }

        private Slider _slider;

        public Slider Slider {
            get => _slider;
            set {
                _slider = value;
                _slider.maxValue = MaxValue;
                _slider.minValue = MinValue;
                _slider.value = Value;
                _slider.wholeNumbers = false;
                SliderValue = _slider.transform.Find("Text").GetComponent<Text>();
                SliderValue.font = RDC.data.koreanFont;
                _slider.onValueChanged.AddListener(val => {
                    Value = Mathf.Round((val - DefaultValue) / ChangeBy) * ChangeBy + DefaultValue;
                    SliderValue.text = Value.ToString();
                });
                _slider.onValueChanged.Invoke(Value);
            }
        }

        public Text SliderValue { get; set; }

        public override float Value {
            set {
                if (value.CompareTo(MaxValue) <= 0 && value.CompareTo(MinValue) >= 0) {
                    base.Value = value;
                }
                _slider.value = Value;
            }
        }
    }
}