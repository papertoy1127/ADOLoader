using System;

namespace ADOLoader.Core.Attributes {
    public class TweakAttribute : Attribute {
        // 트윅을 묶어서 정리할 수 있는 기능
        public Type BaseModType;
        public string TweakName;

        public TweakAttribute(Type baseModType, string tweakName) {
            BaseModType = baseModType;
            TweakName = tweakName;
        }
    }
}