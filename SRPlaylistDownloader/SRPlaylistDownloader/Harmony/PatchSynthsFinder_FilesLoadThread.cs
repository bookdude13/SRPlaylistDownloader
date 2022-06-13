using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPlaylistDownloader.Harmony
{
    [HarmonyPatch(typeof(SynthsFinder), "FilesLoadThread")]
    public class PatchSynthsFinder_FilesLoadThread
    {
        public static bool Prefix(bool isRefresh)
        {
            MainMod.Instance.LoggerInstance.Msg($"In FilesLoadThread. isRefresh? {isRefresh}");

            if (isRefresh)
            {
                // Only get songs when the user prompts, to avoid unexpected delays or UI reloads.
                // TODO Maybe have this as a setting?
                MainMod.Instance.OnSongsReload();
            }

            return true;
        }
    }
}
