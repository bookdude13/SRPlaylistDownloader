using SRModCore;
using SRPlaylistDownloader.Models;
using SRPlaylistDownloader.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SRPlaylistDownloader
{
    public class PlaylistDownloadManager : MonoBehaviour
    {
        private SRLogger logger;
        private PlaylistService playlistService;
        private SynthriderzService synthriderzService;
        private bool initialized = false;

        public void Init(SRLogger logger, PlaylistService playlistService, SynthriderzService synthriderzService)
        {
            this.logger = logger;
            this.playlistService = playlistService;
            this.synthriderzService = synthriderzService;
            
            initialized = true;
        }

        public List<PlaylistItem> GetMissingPlaylistItems()
        {
            if (!initialized)
            {
                logger.Error("Not initialized; not fetching missing playlist items");
                return new List<PlaylistItem>();
            }

            var existingHashes = GetExistingHashes();

            logger.Msg("Checking playlists for missing items");
            var playlists = playlistService.GetAllPlaylists();

            logger.Msg($"{playlists.Count} playlists found.");
            var itemsToDownload = new List<PlaylistItem>();
            foreach (var playlist in playlists)
            {
                var newItems = playlist.Items.Where(item => !existingHashes.Contains(item.Hash)).ToList();
                logger.Msg($"\tPlaylist {playlist.PlaylistName} has {playlist.Items.Count} items, {newItems.Count} to download");
                itemsToDownload.AddRange(newItems);
            }

            return itemsToDownload;
        }

        public IEnumerator DownloadPlaylistsItems(HashSet<string> hashesToDownload, Action<string> OnDownload)
        {
            if (!initialized)
            {
                logger.Error("Not initialized; not downloading playlists items");
            }
            else
            {
                yield return StartCoroutine(synthriderzService.DownloadSongsByHash(hashesToDownload, OnDownload));
            }
        }

        private HashSet<string> GetExistingHashes()
        {
            var hashes = GetExistingOstDlcMapHashes();
            hashes.UnionWith(GetExistingCustomMapHashes());
            return hashes;
        }

        private HashSet<string> GetExistingOstDlcMapHashes()
        {
            var hashes = new HashSet<string>();

            var mapsFiles = SynthsFinder.SynthsList;
            foreach (var file in mapsFiles)
            {
                var chart = SynthsFinder.GetFromCache(file.Name);
                if (chart != null && chart.LeaderboardHash != null)
                {
                    hashes.Add(chart.LeaderboardHash);
                }
            }

            return hashes;
        }

        private HashSet<string> GetExistingCustomMapHashes()
        {
            var hashes = new HashSet<string>();

            var customMapsFiles = SynthsFinder.CustomSynthsList;
            foreach (var file in customMapsFiles)
            {
                var chart = SynthsFinder.GetFromCache(file.Name);
                if (chart != null && chart.LeaderboardHash != null)
                {
                    hashes.Add(chart.LeaderboardHash);
                }
            }

            return hashes;
        }
    }
}
