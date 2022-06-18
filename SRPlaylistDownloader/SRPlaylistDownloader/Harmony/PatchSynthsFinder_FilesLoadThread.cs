using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SRPlaylistDownloader.Harmony
{
    [HarmonyPatch(typeof(SynthsFinder), "FilesLoadThread")]
    public class PatchSynthsFinder_FilesLoadThread
    {
        private static bool isDownloading = false;
        
        public static bool Prefix(SynthsFinder __instance, bool isRefresh)
        {
            MainMod.Instance.LoggerInstance.Msg($"In FilesLoadThread. isRefresh? {isRefresh} isDownloading? {isDownloading}");
            if (!isDownloading && isRefresh)
            {
                // Only get songs when the user prompts, to avoid unexpected delays or UI reloads.
                // TODO Maybe have this as a setting?

                isDownloading = true;
                __instance.StartCoroutine(DownloadAndComplete(__instance));
            }

            return true;
        }

        private static IEnumerator DownloadAndComplete(SynthsFinder instance)
        {
            var manager = MainMod.Instance.playlistDownloadManager;
            var missingItems = manager.GetMissingPlaylistItems();

            yield return instance.StartCoroutine(manager.DownloadPlaylistsItems(missingItems));
            FinishDownload(instance);
        }

        public static void Postfix(SynthsFinder __instance, bool isRefresh)
        {
            if (isRefresh)
            {
                if (isDownloading)
                {
                    MainMod.Instance.LoggerInstance.Msg("Still downloading in postfix");
                    SynthsFinder.IsLoadThreadFinished = false;
                    var traverse = new Traverse(__instance);

                    traverse.Property<bool>("CanStartCustomStagesLoad").Value = false;
                }
            }
        }

        public static void FinishDownload(SynthsFinder instance)
        {
            SynthsFinder.IsLoadThreadFinished = true;
            new Traverse(instance).Property<bool>("CanStartCustomStagesLoad").Value = true;

            // Call again now that song files are all in place
            //SynthsFinder.UpdateSynthsLists(true);

            isDownloading = false;
        }
    }
}
