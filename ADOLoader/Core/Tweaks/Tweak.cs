using System;
using System.Collections.Generic;

namespace ADOLoader.Core.Tweaks {
    public abstract class Tweak {
        internal static Dictionary<Type, Tweak> _tweakInstances = new();
        
    }
}