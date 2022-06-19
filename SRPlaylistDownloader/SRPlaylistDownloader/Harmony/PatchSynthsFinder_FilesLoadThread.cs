using HarmonyLib;
using SRModCore;
using SRPlaylistDownloader.Models;
using Synth.SongSelection;
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
        private static bool isReloading = false;

        private static void Msg(string message)
        {
            MainMod.Instance.LoggerInstance.Msg(message);
        }
        
        public static bool Prefix(bool isRefresh)
        {
            Msg($"In FilesLoadThread. isRefresh? {isRefresh} isDownloading? {isDownloading} isReloading? {isReloading}");
            if (!isDownloading && !isReloading && isRefresh)
            {
                // Only get songs when the user prompts, to avoid unexpected delays or UI reloads.
                // TODO Maybe have this as a setting?
                isDownloading = true;
            }

            return true;
        }

        private static void OnDownload(string songHash)
        {
            SynthsFinder.CurrentLoadedSong += 1;
        }

        private static IEnumerator DownloadAndComplete(SynthsFinder instance, PlaylistDownloadManager manager, HashSet<PlaylistItem> missingItems)
        {
            yield return instance.StartCoroutine(manager.DownloadPlaylistsItems(missingItems, OnDownload));
            Msg($"Done downloading. {SynthsFinder.CurrentLoadedSong} out of {SynthsFinder.CurrentTotalSongs} songs");

            FinishDownload(instance);
        }

        public static void Postfix(SynthsFinder __instance, bool isRefresh)
        {
            if (isReloading)
            {
                Msg("Done reloading");
                isReloading = false;
            }
            else if (isDownloading && isRefresh)
            {
                SynthsFinder.IsLoadThreadFinished = false;
                var traverse = new Traverse(__instance);
                traverse.Property<bool>("CanStartCustomStagesLoad").Value = false;

                Msg("Starting download in postfix");
                var manager = MainMod.Instance.playlistDownloadManager;
                var missingItems = manager.GetMissingPlaylistItems();

                // Reset counts
                SynthsFinder.CurrentLoadedSong = 0;
                SynthsFinder.CurrentTotalSongs = missingItems.Count;

                __instance.StartCoroutine(DownloadAndComplete(__instance, manager, missingItems));
            }
        }

        public static void FinishDownload(SynthsFinder instance)
        {
            SynthsFinder.IsLoadThreadFinished = true;
            new Traverse(instance).Property<bool>("CanStartCustomStagesLoad").Value = true;

            // Call again now that song files are all in place
            isReloading = true;
            isDownloading = false;
            SongSelectionManager.GetInstance?.RefreshSongList(false);
        }
    }
}
