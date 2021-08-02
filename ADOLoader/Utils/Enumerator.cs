using System;
using System.Collections;
using System.Collections.Generic;

namespace ADOLoader.Utils {
    public static class Enumerator {
        public static bool EqualsEnumerator(this IEnumerable orig, IEnumerable toCompare) {
            var enumOrig = orig.GetEnumerator();
            var enumToCompare = toCompare.GetEnumerator();
            var result = true;
            
            while (enumOrig.MoveNext()) {
                if (!enumToCompare.MoveNext()) return false;
                result = result && enumOrig.Current == enumToCompare.Current;
            }

            if (enumOrig.MoveNext()) result = false;
            return result;

        }
        
        internal static bool CheckType(this IEnumerable<Type> target, IEnumerable<object> toCompare) {
            using var enumOrig = target.GetEnumerator();
            using var enumToCompare = toCompare.GetEnumerator();
            var result = true;
            
            while (enumOrig.MoveNext()) {
                if (!enumToCompare.MoveNext()) return false;
                var curr = enumToCompare.Current?.GetType() ?? typeof(object);
                var comp = enumOrig.Current;
                result = result && (curr == comp || curr.IsSubclassOf(comp));
            }

            if (enumOrig.MoveNext()) result = false;
            return result;

        }
    }
}