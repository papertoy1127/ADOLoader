using MelonLoader;
using Main = ADOLoader.LoaderMain;

#region Assembly attributes

[assembly: MelonInfo(typeof(Main), ModInfo.Name, ModInfo.Version, ModInfo.Author, ModInfo.DownloadLink)]
[assembly: MelonColor()]
[assembly: MelonGame(null, null)]
[assembly: HarmonyDontPatchAll]

#endregion

public static class ModInfo
{
    public const string Name = "ADOLoader";
    public const string Version = "1.0.0";
    public const string Author = "PERIOT";
    public const string DownloadLink = null;
}