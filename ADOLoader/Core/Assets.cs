using System;
using System.Collections.Generic;
using MelonLoader;
using UnityEngine;

namespace ADOLoader.Core {
    public static class Assets {
        public static AssetBundle AssetBundle;
        
        public static Dictionary<Sounds, AudioClip> sounds = new();
        public static GameObject ColorSlider;
        public static GameObject ColorAlphaSlider;
        public static GameObject DropDown;
        public static GameObject Slider;

        public static void Setup(string path) {
            AssetBundle = AssetBundle.LoadFromFile(path);
            foreach (var name in AssetBundle.GetAllAssetNames()) {
                MelonLogger.Msg(name);
            }

            foreach (var clip in AssetBundle.LoadAllAssets<AudioClip>()) {
                if (Enum.TryParse<Sounds>(clip.name, out var sound)) {
                    sounds[sound] = clip;
                    MelonLogger.Msg(clip.name);
                }
            }
            
            ColorSlider = AssetBundle.LoadAsset<GameObject>("ColorSlider");
            ColorAlphaSlider = AssetBundle.LoadAsset<GameObject>("ColorAlphaSlider");
            DropDown = AssetBundle.LoadAsset<GameObject>("DropDown");
            Slider = AssetBundle.LoadAsset<GameObject>("Slider");

        }
    }
}