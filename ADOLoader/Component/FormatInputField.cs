using System;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ADOLoader.Component {
    public class FormatInputField : InputField {
        public static bool AnyFocus;
        public string InUpdatedValue = "";
        public object Value;
        public Type Type;
        public static bool CheckValue(Type type, string input, out string output, out object value) {
            var iv = CultureInfo.InvariantCulture;
            if (type == typeof(Color)) {
                if (!input.StartsWith("#")) input = "#" + input;
                input = input.ToUpperInvariant();
                if (ColorUtility.TryParseHtmlString(input, out var result1)) {
                    output = input;
                    value = result1;
                    return true;
                }
            } else if (type == typeof(Color32)) {
                if (!input.StartsWith("#")) input = "#" + input;
                input = input.ToUpperInvariant();
                if (ColorUtility.TryParseHtmlString(input, out var result1)) {
                    output = input;
                    value = (Color32) result1;
                    return true;
                }
            }
            else {
                output = input;
                value = default;
                return true;
            }

            output = null;
            value = default;
            return false;
        }

        protected override void Awake() {
            base.Awake();
            onEndEdit.AddListener(txt => {
                if (CheckValue(Type, text, out var output, out var value)) {
                    text = output;
                    Value = value;
                }
                else text = InUpdatedValue;
                AnyFocus = false;
            });
            if (Type == typeof(int)) characterValidation = CharacterValidation.Integer;
            if (Type == typeof(float)) characterValidation = CharacterValidation.Decimal;
            onValueChanged.AddListener(txt => {
                if (Type == typeof(Color) || Type == typeof(Color32)) {
                    var textToBuild = new StringBuilder();
                    foreach (var color in text.ToUpper().Where(color => ColorAllowedChars.Contains(color))) {
                        textToBuild.Append(color);
                    }

                    text = "#" + textToBuild;
                    caretPosition = int.MaxValue;
                    characterLimit = 9;
                }
            });
        }

        private static char[] ColorAllowedChars = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};
        
        private void Update() {
            if (text.IsNullOrEmpty()) {
                text = "#";
            }
            if (isFocused) {
                AnyFocus = true;
            }
            else {
                InUpdatedValue = text;
            }
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            AnyFocus = false;
        }
    }
}