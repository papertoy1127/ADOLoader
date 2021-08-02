using System;
using System.Collections.Generic;
using HarmonyLib;
using MelonLoader;
using UnityEngine;

namespace ADOLoader.RDStringPatch {
	public class Patch {
		internal static Dictionary<string, Dictionary<SystemLanguage, string>> PatchedStrings =
			new();

		[HarmonyPatch(typeof(RDString), "GetWithCheck")]
		internal static class RDStringGetPatch {
			public static bool Prefix(string key, out bool exists, ref string __result) {
				if (key.Contains("Grayscale"))
					MelonLogger.Msg(key);
				if (PatchedStrings.ContainsKey(key)) {
					var lang = (SystemLanguage) Enum.Parse(typeof(SystemLanguage), Persistence.GetLanguage());
					if (PatchedStrings[key].ContainsKey(lang)) {
						__result = PatchedStrings[key][lang];
						exists = true;
						return false;
					}

					if (PatchedStrings[key].ContainsKey(SystemLanguage.Unknown)) {
						__result = PatchedStrings[key][SystemLanguage.Unknown];
						exists = true;
						return false;
					}
				}

				exists = false;
				return true;
			}
		}

		public static void PatchRDString(string key, params (SystemLanguage, string)[] LanguageStrings) {
			PatchRDString(key, null, false, LanguageStrings);
		}

		public static void PatchRDString(string key, string defaultValue,
			params (SystemLanguage, string)[] LanguageStrings) {
			PatchRDString(key, defaultValue, false, LanguageStrings);
		}

		public static void PatchRDString(string key, bool overrideExists,
			params (SystemLanguage, string)[] LanguageStrings) {
			PatchRDString(key, null, overrideExists, LanguageStrings);
		}

		public static void PatchRDString(string key, string defaultValue, bool overrideExists,
			params (SystemLanguage, string)[] LanguageStrings) {
			if (!PatchedStrings.ContainsKey(key)) {
				PatchedStrings[key] = new Dictionary<SystemLanguage, string>();
			}

			foreach (var (language, value) in LanguageStrings) {
				if (overrideExists || !PatchedStrings[key].ContainsKey(language))
					PatchedStrings[key][language] = value;
			}

			if (overrideExists || !PatchedStrings[key].ContainsKey(SystemLanguage.Unknown)) {
				PatchedStrings[key][SystemLanguage.Unknown] = defaultValue;
			}
		}
	}
}