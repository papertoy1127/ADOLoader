using System;
using System.Collections.Generic;
using System.Linq;
using ADOLoader.Component;
using MelonLoader;
using UnityEngine;

namespace ADOLoader.Core.TweakSettings {
    public interface ITweakNoArrow {
        [Obsolete("On Right/Left Arrow in this setting is not used", true)]
        public abstract void OnLeftArrow();

        [Obsolete("On Right/Left Arrow in this setting is not used", true)]
        public abstract void OnRightArrow();
    }

    public interface ITweakHideValue : ITweakNoArrow { }

    public interface ITweakColorSlider: ITweakHideValue {
        public bool AlphaEnabled { get; }
        public ColorAlphaSlider Slider { get; set; }
    }

    public abstract class TweakSetting {
        internal static readonly Dictionary<PauseSettingButton, TweakSetting> Settings = new();
        
        public readonly string Name;
        public readonly Type ValueType;
        public PauseSettingButton PauseSettingButton { get; internal set; }
        
        private object m_value;

        internal object _value {
            get => m_value;
            set {
                m_value = value;
                _OnValueChange();
            }
        }

        internal abstract object _GetValue();

        protected TweakSetting(string name, Type type) {
            Name = name;
            ValueType = type;
        }

        internal abstract void _OnValueChange(); 
        
        internal abstract void _OnLeftArrow();
        internal abstract void _OnRightArrow();
        internal abstract void _OnEnter();

        public abstract void OnLeftArrow();
        public abstract void OnRightArrow();
        protected abstract void OnEnter();
        
        public override string ToString() {
            return _GetValue().ToString();
        }

        public static string GetTypeName(TweakSetting t) {
            if (t is TweakSettingAction) return "Action";
            if (t.ValueType == typeof(int)) return "Int";
            if (t.ValueType == typeof(bool)) return "Bool";
            if (t.ValueType.IsEnum) return $"Enum:{t.Name}";
            return t.ValueType.Name;
        }
    }
    public abstract class TweakSetting<T> : TweakSetting {
        public TweakSetting(string name, T defaultValue = default) : base(name, typeof(T)) {
            _value = defaultValue;
        }
        
        public virtual T Value {
            get => (T) _GetValue();
            set => _value = value;
        }
        
        internal override object _GetValue() => GetValue((T) _value);

        public virtual Func<T, T> GetValue { get; set; } = value => value;
        public virtual Action<TweakSetting<T>> OnValueChange { get; set; } = setting => { };
        public virtual Action<TweakSetting<T>> OnLateEnter { get; set; } = setting => { };

        internal override void _OnValueChange() => OnValueChange(this);
        internal sealed override void _OnLeftArrow() => OnLeftArrow();
        internal sealed override void _OnRightArrow() => OnRightArrow();
        internal sealed override void _OnEnter() {
            OnEnter();
            OnLateEnter(this);
        }

        public override void OnLeftArrow() { }
        public override void OnRightArrow() { }
        protected override void OnEnter() { }
    }

    public class TweakSettingBoolean : TweakSetting<bool> {
        public TweakSettingBoolean(string name, bool defaultValue = default) : base(name, defaultValue) { }

        public override void OnLeftArrow() {
            Value = !Value;
        }

        public override void OnRightArrow() {
            Value = !Value;
        }
    }
    
    public class TweakSettingEnum<T> : TweakSetting<T> where T: struct, Enum {
        public TweakSettingEnum(string name, T defaultValue = default) : base(name, defaultValue) {
            _enumMembers = Enum.GetNames(typeof(T)).ToList();
        }

        private readonly List<string> _enumMembers;

        public override void OnLeftArrow() {
            var index = _enumMembers.IndexOf(name);
            if (index <= 0) return;
            Enum.TryParse<T>(_enumMembers[index - 1], out var value);
            Value = value;
        }

        public override void OnRightArrow() {
            var index = _enumMembers.IndexOf(name);
            if (index >= _enumMembers.Count - 1) return;
            Enum.TryParse<T>(_enumMembers[index + 1], out var value);
            Value = value;
        }

        internal string name => Enum.GetName(typeof(T), Value);

        public override string ToString() {
            return Enum.GetName(typeof(T), Value) ?? "null";
        }
    }

    public class TweakSettingAction : TweakSetting<Action>, ITweakNoArrow {
        public TweakSettingAction(string name, Action action, string label = null) : base(name) {
            Value = action;
            Label = label;
        }

        internal override object _GetValue() {
            return GetValue((Action) _value);
        }

        public sealed override Action Value {
            set => _value = value;
        }

        public string Label { get; set; }
        
        #pragma warning disable
        [Obsolete("OnLateEnter in Action setting is not used. Use TweakSettingAction.Action instead.", true)]
        public override Action<TweakSetting<Action>> OnLateEnter { get; set; } = obj => { };
        #pragma warning restore
        protected override void OnEnter() {
            Value();
        }
    }

    public class TweakSettingColorSlider : TweakSetting<Color32>, ITweakColorSlider {
        public bool AlphaEnabled { get; }

        private ColorAlphaSlider _slider;

        public ColorAlphaSlider Slider {
            get => _slider;
            set {
                MelonLogger.Msg(_value);
                _slider = value;
                _slider.Value = (Color32) _value;
                _slider.onValueChanged.AddListener(val => Value = val);
            }
        }

        public TweakSettingColorSlider(string name, Color32 defaultValue = default, 
            bool alphaEnabled = true) : base(name, defaultValue) {
            AlphaEnabled = alphaEnabled;
            MelonLogger.Msg(Value);
        }
        
        public TweakSettingColorSlider(string name, bool alphaEnabled) : this(name) { }
    }
}