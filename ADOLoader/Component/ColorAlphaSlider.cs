using UnityEngine;
using UnityEngine.UI;
using ADOLoader.Utils;
using UnityEngine.Events;
using Color = UnityEngine.Color;

namespace ADOLoader.Component {
    public class ColorAlphaSlider : MonoBehaviour {
        public class CASliderEvent : UnityEvent<Color32> { }

        public Slider ColorSliderRed;
        public Slider ColorSliderGreen;
        public Slider ColorSliderBlue;
        public Slider AlphaSlider;
        public Image AlphaSliderImage;
        public Image ColorValue;
        public FormatInputField InputField;
        private bool AlphaEnabled;

        public Color32 Value {
            get {
                var r = (byte) ColorSliderRed.value;
                var g = (byte) ColorSliderGreen.value;
                var b = (byte) ColorSliderBlue.value;
                byte a;
                if (AlphaEnabled)
                    a = (byte) AlphaSlider.value;
                else
                    a = 255;
                return new Color32(r, g, b, a);
            }
            set {
                ColorSliderRed.value = value.r;
                ColorSliderGreen.value = value.g;
                ColorSliderBlue.value = value.b;
                if (AlphaEnabled) {
                    AlphaSlider.value = value.a;
                }
            }
        }

        public CASliderEvent onValueChanged { get; set; } = new();

        private void Awake() {
            ColorSliderRed = transform.Find("ColorSliderRed").GetComponent<Slider>();
            ColorSliderGreen = transform.Find("ColorSliderGreen").GetComponent<Slider>();
            ColorSliderBlue = transform.Find("ColorSliderBlue").GetComponent<Slider>();
            ColorValue = transform.Find("ColorValue").GetComponent<Image>();
            InputField = transform.Find("InputField").gameObject.AddComponent<FormatInputField>();
            InputField.Type = typeof(Color32);
            InputField.textComponent = InputField.transform.Find("Text").GetComponent<Text>();
            InputField.textComponent.font = RDC.data.koreanFont;
            InputField.textComponent.fontSize = 40;
            InputField.GetComponent<Image>().color = new Color(0, 0, 0, 0.7f);
            InputField.textComponent.color = Color.white;

            var alpha = transform.Find("AlphaSlider");
            AlphaEnabled = alpha != null;
            if (AlphaEnabled) {
                AlphaSlider = alpha.GetComponent<Slider>();
                AlphaSliderImage = AlphaSlider.transform.Find("Background").GetComponent<Image>();
                AlphaSlider.minValue = 0;
                AlphaSlider.maxValue = 255;
            }

            ColorSliderRed.minValue = 0;
            ColorSliderRed.maxValue = 255;
            ColorSliderGreen.minValue = 0;
            ColorSliderGreen.maxValue = 255;
            ColorSliderBlue.minValue = 0;
            ColorSliderBlue.maxValue = 255;
            
            ColorSliderRed.onValueChanged.AddListener(value => onValueChanged.Invoke(Value));
            ColorSliderGreen.onValueChanged.AddListener(value => onValueChanged.Invoke(Value));
            ColorSliderBlue.onValueChanged.AddListener(value => onValueChanged.Invoke(Value));
            if (AlphaEnabled)
                AlphaSlider.onValueChanged.AddListener(value => onValueChanged.Invoke(Value));
        }

        private bool _prevActive = false;

        private void Update() {
            if (!InputField.isFocused) {
                if (_prevActive) {
                    Value = (Color32) InputField.Value;
                }
                else {
                    ColorSliderRed.value = Mathf.RoundToInt(ColorSliderRed.value);
                    ColorSliderGreen.value = Mathf.RoundToInt(ColorSliderGreen.value);
                    ColorSliderBlue.value = Mathf.RoundToInt(ColorSliderBlue.value);
                    if (AlphaEnabled)
                        AlphaSlider.value = Mathf.RoundToInt(AlphaSlider.value);
                    ColorValue.color = Value;
                    if (AlphaEnabled)
                        InputField.text = ColorUtility.ToHtmlStringRGBA(Value);
                    else
                        InputField.text = ColorUtility.ToHtmlStringRGB(Value);
                }
            }

            if (AlphaEnabled)
                AlphaSliderImage.color = Value.WithAlpha(255);
            _prevActive = InputField.isFocused;
        }
    }
}