using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GDMiniJSON;
using HarmonyLib;
using MelonLoader;
using ADOLoader.Component;
using ADOLoader.Core.TweakSettings;
using ADOLoader.Utils;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace ADOLoader {
	public static class Patch {
		#if DEBUG
		[HarmonyPatch(typeof(scrShowIfDebug), "Update")]
		public static class ShowSansMode {
			public static bool Prefix(scrShowIfDebug __instance, Text ___txt) {
				___txt.text = AmISans ? "샌즈 모드" : "";
				return false;
			}
		}
		public static bool AmISans = false;
		#endif

		public static void UpdateSetting(this SettingsMenu instance, PauseSettingButton setting,
			SettingsMenu.Interaction action) {
			if (!setting.name.StartsWith("Mod_")) {
				instance.invoke<object>("UpdateSetting")(setting, action);
				return;
			}

			bool isAction = setting.type == "Action";
			bool shiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
			object value = null;
			string name = setting.name;
			bool changeValue = true;

			TweakSetting tweakSetting = null;
			
			foreach (var (n, v) in Core.Debug.TweakSettings) {
				if ("Mod_" + n == name) {
					tweakSetting = v;
					break;
				}
			}

			if (tweakSetting == null) {
				MelonLogger.Error($"TweakSetting for setting {name} not found!");
				return;
			}

			tweakSetting.PauseSettingButton = setting;
			TweakSetting.Settings[setting] = tweakSetting;

			switch (action) {
				case SettingsMenu.Interaction.Activate:
					instance.set<int>("confirmationProgress")(instance.get<int>("confirmationProgress") + 1);
					tweakSetting._OnEnter();
					value = tweakSetting._GetValue();
					break;
				case SettingsMenu.Interaction.Increment:
					if (isAction) break;
					setting.PlayArrowAnimation(true);
					scrController.instance.pauseMenu.PlaySfx(2);
					tweakSetting._OnRightArrow();
					value = tweakSetting._GetValue();
					break;
				case SettingsMenu.Interaction.Decrement:
					if (isAction) break;
					setting.PlayArrowAnimation(false);
					scrController.instance.pauseMenu.PlaySfx(2);
					tweakSetting._OnLeftArrow();
					value = tweakSetting._GetValue();
					break;
				case SettingsMenu.Interaction.Refresh:
					if (tweakSetting is ITweakNoArrow) {
						setting.rightArrow.transform.ScaleXY(0);
						setting.leftArrow.transform.ScaleXY(0);
					}

					var buttoncont = setting.transform.Find("buttonContainer");
					var val = buttoncont.Find("Value").gameObject;

					if (tweakSetting is ITweakHideValue) {
						val.GetComponent<Text>().enabled = false;
					}
					
					if (tweakSetting is ITweakHideName) {
						buttoncont.Find("SettingName").gameObject.SetActive(false);
					}
					
					if (tweakSetting is ITweakColorSlider slider) {
						var colorSlider = Object.Instantiate(slider.AlphaEnabled ? Core.Assets.ColorAlphaSlider : Core.Assets.ColorSlider, buttoncont.transform);
						var position = buttoncont.position;
						var sliderComp = colorSlider.AddComponent<ColorAlphaSlider>();
						slider.Slider = sliderComp;
						//colorSlider.transform.position = position;
						var transform = colorSlider.GetComponent<RectTransform>();
						transform.AnchorPosX(-50);
						transform.ScaleXY(0.117f);
						var text = val.GetOrAddComponent<Text>();
						text.enabled = false;
					}
					
					if (tweakSetting is ITweakInputField field) {
						var input = val.GetOrAddComponent<FormatInputField>();
						input.Type = tweakSetting.ValueType;
						field.InputField = input;
						var transform = input.GetOrAddComponent<RectTransform>();
						transform.sizeDelta = new Vector2(100, 100);
						var text = val.GetOrAddComponent<Text>();
						input.textComponent = text;
						input.caretPosition = 0;
						text.alignment = TextAnchor.LowerRight;
						text.resizeTextMaxSize = 0;
						text.resizeTextMinSize = 0;
						transform.SizeDeltaX(transform.sizeDelta.x * 2f);
						transform.SizeDeltaY(15);
						var pos = transform.position;
						pos.x -= 50;
						pos.y += 18;
						transform.position = pos;
					}

					if (tweakSetting is ITweakDropdown drop) {
						var dropdown = Object.Instantiate(Core.Assets.DropDown, buttoncont.transform);
						var dropComp = dropdown.GetComponent<Dropdown>();
						drop.Dropdown = dropComp;
						dropComp.captionText.font = RDC.data.koreanFont;
						dropComp.captionText.fontSize = 24;
						dropComp.itemText.font = RDC.data.koreanFont;
						dropComp.itemText.fontSize = 20;
						dropComp.itemText.alignment = TextAnchor.UpperLeft;
						dropComp.itemText.transform.LocalMoveY(-10);
						var position = buttoncont.position;
						var transform = dropdown.GetComponent<RectTransform>();
						transform.AnchorPosX(-35);
						transform.ScaleXY(0.35f);
					}
					
					if (tweakSetting is ITweakSlider sslider) {
						var sliderObj = Object.Instantiate(Core.Assets.Slider, buttoncont.transform);
						var sliderComp = sliderObj.GetComponent<Slider>();
						sslider.Slider = sliderComp;
						var position = buttoncont.position;
						var transform = sliderObj.GetComponent<RectTransform>();
						transform.AnchorPosX(-45);
						transform.ScaleXY(0.12f);
					}

					setting.initialValue = tweakSetting._GetValue();
					value = setting.initialValue;
					break;
				case SettingsMenu.Interaction.ActivateInfo:
					break;
			}

			if (tweakSetting is TweakSettingAction tweakSettingAction) value = tweakSettingAction.Label;
			if (!changeValue) return;
			if (value is bool b) setting.valueLabel.text = RDString.Get((b) ? "pauseMenu.settings.on" : "pauseMenu.settings.off");
			else if (value is Enum) setting.valueLabel.text = Enum.GetName(value.GetType(), value);
			else if (value is string str) {
				setting.valueLabel.text = str.StartsWith("RDString.") ? RDString.Get($"pauseMenu.settings.Mod_{str}") : str;
			}
			else setting.valueLabel.text = value == null ? "" : string.Concat(value);
		}

		[HarmonyPatch(typeof(SettingsMenu), "UpdateSelectedSetting")]
		public static class UpdateSettingPatch {
			public static bool Prefix(SettingsMenu __instance, SettingsMenu.Interaction action) {
				__instance.UpdateSetting(__instance.get<PauseSettingButton>("selectedSetting"), action);
				return false;
			}
		}

		public static Sounds waType;
		
		[HarmonyPatch(typeof(scrController), "Start_Rewind")]
		public static class SansRePatch {
			public static void Postfix() {
				waType = Sounds.na;
			}
		}
		
		[HarmonyPatch(typeof(Input), "GetKeyDown", typeof(KeyCode))]
		public static class InputPatch1 {
			public static bool Prefix(KeyCode key, ref bool __result) {
				if (FormatInputField.AnyFocus) {
					__result = false;
					return false;
				}

				return true;
			}
		}

		private static bool selectSetting;
		private static PauseSettingButton toSelect;

		internal static void SelectPauseSetting(PauseSettingButton settingButton) {
			selectSetting = true;
			toSelect = settingButton;
		}

		[HarmonyPatch(typeof(SettingsMenu), "Update")]
		public static class SettingUpdatePatch {
			public static bool Prefix(SettingsMenu __instance) {
				if (RDInput.cancelPressed) {
					if (__instance.isEditingLevel) {
						scrController.instance.pauseMenu.Hide();
					}
					else {
						scrController.instance.pauseMenu.PlaySfx(2);
					}
				}

				if (__instance.isSelectingTab) {
					if (selectSetting) {
						selectSetting = false;
						__instance.Select(toSelect);
					}
					else if (RDInput.leftPress) {
						__instance.invoke<object>("SelectPreviousTab")();
					}
					else if (RDInput.rightPress) {
						__instance.invoke<object>("SelectNextTab")();
					}
					else if (RDInput.anyPlayerPress || RDInput.downPress) {
						__instance.isSelectingTab = false;
						__instance.invoke<object>("Select")(0, true);
					}
				}
				else {
					if (RDInput.downPress) {
						__instance.invoke<object>("SelectNextOption")();
					}
					else if (RDInput.upPress) {
						__instance.invoke<object>("SelectPreviousOption")();
					}

					if (RDInput.leftPress) {
						__instance.UpdateSelectedSetting(SettingsMenu.Interaction.Decrement);
					}
					else if (RDInput.rightPress) {
						__instance.UpdateSelectedSetting(SettingsMenu.Interaction.Increment);
					}
					else if (RDInput.anyPlayerPress) {
						__instance.UpdateSelectedSetting(SettingsMenu.Interaction.Activate);
					}
				}

				__instance.invoke<object>("ArrangeTabButtons")();
				return false;
			}
		}

#if DEBUG
		[HarmonyPatch(typeof(AudioManager), "_Play")]
		public static class SansSoundPatch {
			public static bool Prefix(AudioManager __instance, string snd, double time, float volume = 1f, int priority = 128) {
				AudioSource audioSource;
				if (AmISans == false)
					audioSource = __instance.MakeSource(snd, null);
				else {
					audioSource = __instance.MakeSource(null, Core.Assets.sounds[waType]);
					if (waType == Sounds.neun)
						audioSource.volume *= 2.5f;
					else {
						audioSource.volume *= 1.75f;
					}
					waType = (Sounds) (((int) waType + 1) % 8);
				}
				audioSource.pitch = 1f;
				audioSource.volume = volume;
				audioSource.priority = priority;
				audioSource.PlayScheduled(time);
				return false;
			}
		}
		#endif

		[HarmonyPatch(typeof(SettingsMenu), "GenerateSettings")]
		public static class GenSettingPatch {
			public static bool Prefix(SettingsMenu __instance, ref List<List<PauseSettingButton>> ___settingsTabs,
				ref List<SettingsTabButton> ___tabButtons) {
				string text = Resources.Load<TextAsset>("PauseMenuSettings").text;
				___settingsTabs = new List<List<PauseSettingButton>>();
				___tabButtons = new List<SettingsTabButton>();
				var settings = (Json.Deserialize(text) as Dictionary<string, object>);

				settings["Mods"] = new List<object>();
				foreach (var (n, v) in Core.Debug.TweakSettings) {
					var value = new Dictionary<string, object>();
					var name = "Mod_" + n;
					value["name"] = name;
					value["type"] = TweakSetting.GetTypeName(v);
					
					(settings["Mods"] as List<object>).Add(value);
				}
				#if DEBUG
				foreach (var kvp in settings) {
					MelonLogger.Msg("┣ " + kvp.Key);
					foreach (var dict in kvp.Value as List<object>) {
						var ddict = dict as Dictionary<string, object>;
						MelonLogger.Msg("┃ ┣ " + ddict["name"]);
						foreach (var kvp2 in ddict) {
							if (kvp2.Key == "name") continue;
							if (!(kvp2.Value is string) && kvp2.Value is IEnumerable value) {
								var size = 0;
								foreach (var inum in value) {
									size += 1;
								}

								MelonLogger.Msg("┃ ┃ ┣ " + $"{kvp2.Key}({value.GetType()}) size: {size}");
								foreach (var inum in value) {
									MelonLogger.Msg("┃ ┃ ┃ ┣ " + $"{inum}");
								}
							}
							else
								MelonLogger.Msg("┃ ┃ ┣ " + $"{kvp2.Key}({kvp2.Value.GetType()}): {kvp2.Value}");
						}
						MelonLogger.Msg("┃ ┃ ");
					}
					MelonLogger.Msg("┃ ");
				}
				#endif


				foreach (KeyValuePair<string, object> keyValuePair in settings) {
					List<object> list = keyValuePair.Value as List<object>;
					string key = keyValuePair.Key;
					List<PauseSettingButton> list2 = new List<PauseSettingButton>();

					foreach (object obj in list) {
						Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
						string text2 = dictionary["name"] as string;
						if (!dictionary.ContainsKey("exclude")) {
							if (dictionary.ContainsKey("available")) {
								bool flag = false;
								foreach (object obj2 in (dictionary["available"] as List<object>)) {
									string a = (string) obj2;
									if ((a == "iOS" && ADOBase.platform == Platform.iOS) ||
									    (a == "android" && ADOBase.platform == Platform.Android) ||
									    (a == "mobile" && ADOBase.isMobile) || (a == "desktop" &&
										    (ADOBase.platform == Platform.Mac ||
										     ADOBase.platform == Platform.Windows))) {
										flag = true;
										break;
									}
								}

								if (!flag) {
									continue;
								}
							}

							PauseSettingButton component = UnityEngine.Object
								.Instantiate<GameObject>(__instance.buttonPrefab, __instance.settingsContainer)
								.GetComponent<PauseSettingButton>();
							component.name = text2;
							list2.Add(component);
							component.type = (string) dictionary["type"];
							if (component.type == "Action") {
								component.leftArrow.transform.ScaleXY(0f, 0f);
								component.rightArrow.transform.ScaleXY(0f, 0f);
							}
							if (component.type == "Label") {
								component.transform.Find("buttonContainer").Find("Border").GetComponent<RectTransform>()
									.ScaleX(0);
								component.transform.Find("buttonContainer").Find("SettingName").GetComponent<RectTransform>()
									.ScaleXY(1.5f);
								component.transform.Find("buttonContainer").Find("SettingName").GetComponent<RectTransform>()
									.LocalMoveY(-4f);
								component.leftArrow.transform.ScaleXY(0f, 0f);
								component.rightArrow.transform.ScaleXY(0f, 0f);
							}

							if (dictionary.ContainsKey("min")) {
								component.minInt = (int) dictionary["min"];
							}

							if (dictionary.ContainsKey("max")) {
								component.maxInt = (int) dictionary["max"];
							}

							if (dictionary.ContainsKey("unit")) {
								component.unit = (string) dictionary["unit"];
							}

							if (dictionary.ContainsKey("changeBy")) {
								component.changeBy = (int) dictionary["changeBy"];
							}

							dictionary.ContainsKey("flipDescription");
							if (dictionary.ContainsKey("changeBySmall")) {
								component.changeBySmall = (int) dictionary["changeBySmall"];
							}
							else {
								component.changeBySmall = component.changeBy;
							}

							if (dictionary.ContainsKey("restartOnChange")) {
								component.restartOnChange = (bool) dictionary["restartOnChange"];
							}

							bool flag2 = false;
							if (ADOBase.isMobile) {
								PauseSettingButton pauseSettingButton = component;
								pauseSettingButton.descriptionKey = pauseSettingButton.descriptionKey +
								                                    "pauseMenu.settings.info." + text2 + ".mobile";
								RDString.GetWithCheck(component.descriptionKey, out flag2);
							}

							if (!flag2) {
								component.descriptionKey = "pauseMenu.settings.info." + text2;
								RDString.GetWithCheck(component.descriptionKey, out flag2);
							}

							component.hasDescription = flag2;
							__instance.UpdateSetting(component, SettingsMenu.Interaction.Refresh);
							
							/*
							typeof(SettingsMenu).GetMethod("UpdateSetting", AccessTools.all).Invoke(__instance,
								new object[] {component, SettingsMenu.Interaction.Refresh});*/
							component.SetFocus(false);
							component.label.text = RDString.Get("pauseMenu.settings." + component.name);
						}
					}

					if (list2.Count > 0) {
						SettingsTabButton component2 = UnityEngine.Object
							.Instantiate<GameObject>(__instance.tabButtonPrefab, __instance.tabButtonsContainer)
							.GetComponent<SettingsTabButton>();
						___tabButtons.Add(component2);
						component2.index = ___tabButtons.Count - 1;
						component2.SetFocus(false);
						component2.name = key;

						component2.label.text = RDString.Get("pauseMenu.settings.category." + key);

						switch (key) {
							case "Mods":
								component2.icon.sprite =
									Resources.Load<Sprite>("LevelEditor/LevelEvents/LevelSettings");
								break;
							default:
								component2.icon.sprite = Resources.Load<Sprite>("SettingsTabIcon/" + key);
								break;
						}


						___settingsTabs.Add(list2);
					}
				}

				return false;
			}
		}

		
		[HarmonyPatch(typeof(SettingsMenu), "SelectTab", typeof(int), typeof(bool))]
		public static class SelectTabPatch {
			public static void Postfix(SettingsMenu __instance, int index, bool force = false) {
				var selectedIndex = __instance.get<int>("selectedIndex");
				var selectedTab = __instance.get<int>("selectedTab");
				var settingsTabs = __instance.get<List<List<PauseSettingButton>>>("settingsTabs");
				var currentSettings = settingsTabs[selectedTab];
				var tabButtons = __instance.get<List<SettingsTabButton>>("tabButtons");
				
				foreach (PauseSettingButton pauseSettingButton in currentSettings) {
					if (TweakSetting.Settings.TryGetValue(pauseSettingButton, out var setting)) {
						if (setting is ISizableSetting sizableSetting) {
							var rectTransform = pauseSettingButton.GetComponent<RectTransform>();
							rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, sizableSetting.Height);
							var buttonContainer = pauseSettingButton.transform.Find("buttonContainer").GetComponent<RectTransform>();
							buttonContainer.sizeDelta = new Vector2(buttonContainer.sizeDelta.x, sizableSetting.Height);
							buttonContainer.localPosition = new Vector3(0, -sizableSetting.Height / 2.0f, 0);
						}
					}
				}
			}
		}
		
		[HarmonyPatch(typeof(PauseSettingButton), "SetFocus", typeof(bool))]
		public static class SelectPatch {
			public static bool Prefix(PauseSettingButton __instance, bool focus) {
				if (!TweakSetting.Settings.TryGetValue(__instance, out var setting)) return true;
				if (!(setting is ISizableSetting sizableSetting)) return true;
				__instance.label.color = (focus ? scrController.instance.pauseMenu.selectedLabelColor : scrController.instance.pauseMenu.unselectedLabelColor);
				__instance.valueLabel.color = __instance.label.color;
				__instance.rectangle.color = (focus ? scrController.instance.pauseMenu.selectedLabelColor : scrController.instance.pauseMenu.unselectedBorderColor);
				__instance.fill.color = (focus ? scrController.instance.pauseMenu.selectedFillColor : scrController.instance.pauseMenu.unselectedFillColor);
				__instance.rightArrow.gameObject.SetActive(focus);
				__instance.leftArrow.gameObject.SetActive(focus);
				__instance.infoArrow.gameObject.SetActive(focus && __instance.hasDescription);
				if (__instance.hasDescription)
				{
					__instance.info.text = RDString.Get(__instance.descriptionKey);
				}
				ContentSizeFitter fitter = scrController.instance.pauseMenu.settingsMenu.settingsContainer.GetComponent<ContentSizeFitter>();
				if (!focus || __instance.hasDescription)
				{
					__instance.infoMask.DOKill(true);
					__instance.infoMask.DOSizeDelta(new Vector2(__instance.infoMask.sizeDelta.x, (float)(focus ? sizableSetting.Height + 24 : sizableSetting.Height)), scrController.instance.pauseMenu.animationTime, false).SetEase(scrController.instance.pauseMenu.animationEase).SetUpdate(true).OnUpdate(delegate
					{
						fitter.enabled = true;
						fitter.SetLayoutVertical();
						fitter.enabled = false;
					}).OnComplete(delegate
					{
						fitter.enabled = true;
					});
				}
				if (focus)
				{
					__instance.buttonContainer.DOKill(true);
					__instance.buttonContainer.DOScale(1.02f, scrController.instance.pauseMenu.animationTime).SetEase(scrController.instance.pauseMenu.animationEase).SetUpdate(true).OnComplete(delegate
					{
						__instance.buttonContainer.DOScale(1f, scrController.instance.pauseMenu.animationTime).SetEase(scrController.instance.pauseMenu.animationEase).SetUpdate(true);
					});
				}

				return false;
			}
		}
		
		[HarmonyPatch(typeof(PauseSettingButton), "Awake")]
		public static class SettingWidthPatch {
			public static void Postfix(PauseButton __instance) {
				var transform = __instance.transform.Find("buttonContainer").GetComponent<RectTransform>();
				transform.SizeDeltaX(205);
			}
		}
	}
}