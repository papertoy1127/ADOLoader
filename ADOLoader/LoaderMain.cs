#define DEBUG

using System;
using System.IO;
using System.Reflection;
using ADOLoader.Core;
using ADOLoader.Core.TweakSettings;
using MelonLoader;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ADOLoader
{

    public enum Sounds {
        wa = 0,
        san,
        s,
        a,
        si,
        neun,
        gu,
        na
    }
    
    public class LoaderMain : MelonMod {
        public static bool load;
        
        public override void OnSceneWasLoaded(int buildIndex, string sceneName) {
            if (load) return;
            load = true;
            
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Mods", "ADOLoader", "adoloader");
            Assets.Setup(path);
            
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                var info = assembly.GetCustomAttribute<MelonInfoAttribute>();
                if (info == null) continue;
                MelonLogger.Msg(assembly.GetName().Name + ": " + info.SystemType.Name);
            }
            MelonLogger.Msg(AppDomain.CurrentDomain.GetAssemblies().Length);

            MelonLogger.Msg(path);
            HarmonyInstance.PatchAll();
            
            RDStringPatch.Patch.PatchRDString("pauseMenu.settings.category.Mods", "Mods", 
                (SystemLanguage.Korean, "모드")
            );
            RDStringPatch.Patch.PatchRDString("pauseMenu.settings.Mod_TestMod_imasans", "Sans Mode", 
                (SystemLanguage.Korean, "샌즈 모드")
            );
            RDStringPatch.Patch.PatchRDString("pauseMenu.settings.Mod_TestMod_TestInt", "Int Test", 
                (SystemLanguage.Korean, "정수 테스트")
            );
            RDStringPatch.Patch.PatchRDString("pauseMenu.settings.Mod_TestMod_TestFloat", "Float Test", 
                (SystemLanguage.Korean, "실수 테스트")
            );
            RDStringPatch.Patch.PatchRDString("pauseMenu.settings.Mod_TestMod_Input_Test", "String Test", 
                (SystemLanguage.Korean, "문자열 입력 테스트")
            );
            RDStringPatch.Patch.PatchRDString("pauseMenu.settings.Mod_TestMod_ColorSlider", "Color Slider", 
                (SystemLanguage.Korean, "RGB 슬라이더")
            );
            RDStringPatch.Patch.PatchRDString("pauseMenu.settings.Mod_TestMod_ColorAlphaSlider", "Color&Alpha Slider", 
                (SystemLanguage.Korean, "RGBA 슬라이더")
            );
            RDStringPatch.Patch.PatchRDString("pauseMenu.settings.Mod_TestMod_TestAction", "Action Test", 
                (SystemLanguage.Korean, "Action 테스트")
            );
            RDStringPatch.Patch.PatchRDString("pauseMenu.settings.Mod_TestMod_TestAction2", "Label");
            
            Core.Debug.RegisterTweakSettings(new TweakSetting[] {
                new TweakSettingBoolean("TestMod_imasans") {
                    OnValueChange = setting => { 
                        Debug.Log("asdf");
                        Patch.AmISans = setting.Value;
                    }
                },
                new TweakSettingColorSlider("TestMod_ColorSlider", false),
                new TweakSettingColorSlider("TestMod_ColorAlphaSlider"),
                new TweakSettingFloatSlider("TestMod_ColorAlphaS123213lider", 0, float.NegativeInfinity, float.PositiveInfinity, 1E+10f),
                new TweakSettingDropdown<string>("asdf", "언더테일 아시는구나! 혹시 모르시는분들에 대해 설명해드립니다 샌즈랑 언더테일의 세가지 엔딩루트중 몰살엔딩의 최종보스로 진.짜.겁.나.어.렵.습.니.다 공격은 전부다 회피하고 만피가 92인데 샌즈의 공격은 1초당 60이 다는데다가 독뎀까지 추가로 붙어있습니다.. 하지만 이러면 절대로 게임을 깰 수 가없으니 제작진이 치명적인 약점을 만들었죠. 샌즈의 치명적인 약점이 바로 지친다는것입니다. 패턴들을 다 견디고나면 지쳐서 자신의 턴을 유지한채로 잠에듭니다. 하지만 잠이들었을때 창을옮겨서 공격을 시도하고 샌즈는 1차공격은 피하지만 그 후에 바로날아오는 2차 공격을 맞고 죽습니다.".Split(' ')),
                new TweakSettingAction("TestMod_TestAction", () => MelonLogger.Msg("Test Action"), "RDString.TestMod_TestAction2"),
                new TweakSettingsGUILayout("TestMod_TestGUILayout", 48),
                new TweakSettingsGUILayout("TestMod_TestGUILayout2", 48),

            });
        }
    }
}