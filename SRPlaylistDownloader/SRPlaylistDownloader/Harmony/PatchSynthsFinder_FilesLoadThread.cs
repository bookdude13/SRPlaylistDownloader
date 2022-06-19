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
            //Msg($"In FilesLoadThread. isRefresh? {isRefresh} isDownloading? {isDownloading} isReloading? {isReloading}");
            
            // Only get songs when the user prompts (refresh).
            // This lets the playlistDownloadManager be created in the main menu and have all the main menu load before messing with the UI
            if (!isDownloading && !isReloading && isRefresh)
            {
                isDownloading = true;
            }

            return true;
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
                var missingHashes = missingItems.Select(item => item.Hash).ToHashSet();
                Msg($"{missingHashes.Count} unique items found for download");

                // Reset counts
                SynthsFinder.CurrentLoadedSong = 0;
                SynthsFinder.CurrentTotalSongs = missingHashes.Count;

                __instance.StartCoroutine(DownloadAndComplete(__instance, manager, missingHashes));
            }
        }

        private static void OnDownload(string songHash)
        {
            SynthsFinder.CurrentLoadedSong += 1;
        }

        private static IEnumerator DownloadAndComplete(SynthsFinder instance, PlaylistDownloadManager manager, HashSet<string> missingHashes)
        {
            yield return instance.StartCoroutine(manager.DownloadPlaylistsItems(missingHashes, OnDownload));
            FinishDownload(instance);
        }

        public static void FinishDownload(SynthsFinder instance)
        {
            // Mark all necessary flags to indicate "unfinished"
            SynthsFinder.IsLoadThreadFinished = true;
            new Traverse(instance).Property<bool>("CanStartCustomStagesLoad").Value = true;

            // Call again now that song files are all in place
            isReloading = true;
            isDownloading = false;
            SongSelectionManager.GetInstance?.RefreshSongList(false);
        }
    }
}
