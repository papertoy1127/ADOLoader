using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace ADOLoader.Core.TweakSettings {
    public interface ITweakDropdown: ITweakHideValue {
        public Dropdown Dropdown { get; set; }
    }

    public class TweakSettingDropdown<T> : TweakSetting<T>, ITweakDropdown {
        public TweakSettingDropdown(string name, IEnumerable<T> values) : base(name) {
            Values = new List<T>(values.Distinct());
        }

        private Dropdown _dropdown;
        public Dropdown Dropdown {
            get => _dropdown;
            set {
                _dropdown = value;
                _dropdown.options = Values.Select(value => new Dropdown.OptionData(value.ToString())).ToList();
                _dropdown.value = CurrentIndex;
                _dropdown.onValueChanged.AddListener(val => {
                    CurrentIndex = val;
                    Value = (T) _GetValue();
                });
            }
        }

        public int CurrentIndex { get; private set; }
        internal List<T> Values;
    }
}