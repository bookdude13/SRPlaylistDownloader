using SRModCore;
using SRPlaylistDownloader.Models;
using SRPlaylistDownloader.Services;
using System;
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
        private HashSet<string> existingHashes = null;
        private bool initialized = false;

        public void Init(SRLogger logger, PlaylistService playlistService, SynthriderzService synthriderzService)
        {
            this.logger = logger;
            this.playlistService = playlistService;
            this.synthriderzService = synthriderzService;
            
            existingHashes = GetExistingHashes();
            initialized = true;
        }

        public void DownloadMissingPlaylistsItems()
        {
            if (!initialized)
            {
                logger.Error("Not initialized; not fetching missing playlists items");
                return;
            }

            logger.Msg("Checking playlists for missing items");
            var playlists = playlistService.GetAllPlaylists();
            logger.Msg($"{playlists.Count} playlists found.");
            foreach (var playlist in playlists)
            {
                logger.Msg($"\tPlaylist {playlist.PlaylistName}");
                DownloadMissingPlaylistItems(playlist.Items);
            }
            logger.Msg("Done queueing downloads of missing items");

            // After all songs are downloaded, refresh the song list
            //logger.Msg("Refreshing song list");
            //SongSelectionManager.GetInstance.RefreshSongList(false);
        }

        private void DownloadMissingPlaylistItems(List<PlaylistItem> playlistItems)
        {
            var itemsToDownload = playlistItems.Where(item => !existingHashes.Contains(item.Hash)).ToList();
            if (itemsToDownload.Count == 0)
            {
                logger.Msg("All playlist items already downloaded");
            }
            else
            {
                logger.Msg($"Downloading {itemsToDownload.Count} missing playlist items");
                List<string> hashesToDownload = itemsToDownload.Select(item => item.Hash).ToList();
                StartCoroutine(synthriderzService.DownloadSongsByHash(hashesToDownload));
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
