using System;

namespace ADOLoader.Core.TweakSettings {

    public interface ITweakMinMax<T> where T : IComparable<T> {
        T MinValue { get; }
        T MaxValue { get; }
        T ChangeBy { get; }
        T DefaultValue { get; }
    };

    public abstract class TweakSettingMinMax<T> : TweakSetting<T>, ITweakMinMax<T>
        where T : struct, IEquatable<T>, IComparable<T> {
        public TweakSettingMinMax(string name, T minValue, T maxValue, T changeBy) :
            this(name, default, minValue, maxValue, changeBy) { }

        public TweakSettingMinMax(string name, T defaultValue, T minValue, T maxValue, T changeBy) : base(name,
            defaultValue) {
            MinValue = minValue;
            MaxValue = maxValue;
            ChangeBy = changeBy;
            DefaultValue = defaultValue;
        }

        public T MinValue { get; }
        public T MaxValue { get; }
        public T ChangeBy { get; }
        public T DefaultValue { get; }

        public abstract override void OnRightArrow();
        public abstract override void OnLeftArrow();

        public override T Value {
            set {
                if (value.CompareTo(MaxValue) >= 0) _value = MaxValue;
                else if (value.CompareTo(MinValue) <= 0) _value = MinValue;
                else _value = value;
            }
        }
    }

    public class TweakSettingInt : TweakSettingMinMax<int> {
        public TweakSettingInt(string name, int defaultValue, int minValue = int.MinValue,
            int maxValue = int.MaxValue, int changeBy = 1) :
            base(name, defaultValue, minValue, maxValue, changeBy) { }

        public override void OnRightArrow() {
            Value += ChangeBy;
        }

        public override void OnLeftArrow() {
            Value -= ChangeBy;
        }
    }

    public class TweakSettingFloat : TweakSettingMinMax<float> {
        public TweakSettingFloat(string name, float defaultValue, float minValue = float.NegativeInfinity,
            float maxValue = float.PositiveInfinity, float changeBy = 1) : base(name, defaultValue, minValue,
            maxValue, changeBy) { }

        public override void OnRightArrow() {
            Value += ChangeBy;
        }

        public override void OnLeftArrow() {
            Value -= ChangeBy;
        }
    }
}