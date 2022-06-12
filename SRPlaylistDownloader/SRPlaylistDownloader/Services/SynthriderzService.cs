using SRModCore;
using Synth.SongSelection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace SRPlaylistDownloader.Services
{
    public class SynthriderzService
    {
        private readonly string baseUrl = "https://synthriderz.com/api";
        private readonly string userAgent = $"SRPlaylistDownloader/{Assembly.GetExecutingAssembly().GetName().Version}";

        private SRLogger logger;

        public SynthriderzService(SRLogger logger)
        {
            this.logger = logger;
        }

        private string GetCustomSongsPath()
        {
            return Application.dataPath + "/../CustomSongs";
        }

        /// <summary>
        /// Downloads songs from Synthriderz.com to the CustomSongs directory.
        /// Overwrites existing files.
        /// </summary>
        /// <param name="songHashes">Song hashes to download</param>
        public IEnumerator DownloadSongsByHash(List<string> songHashes)
        {
            string tempPath = $"{GetCustomSongsPath()}/temp_downloading.synth";

            // If there was a leftover temp file from the last run, remove it
            FileUtils.TryDeleteFile(logger, tempPath);

            foreach (var songHash in songHashes)
            {
                // Don't download if it already exists

                string downloadUrl = $"{baseUrl}/beatmaps/hash/download/{songHash}";
                logger.Debug($"Downloading from url {downloadUrl}");
                using (UnityWebRequest www = UnityWebRequest.Get(downloadUrl))
                {
                    // Save to temporary location first
                    www.downloadHandler = new DownloadHandlerFile(tempPath);
                    www.timeout = 30;
                    www.SetRequestHeader("User-Agent", userAgent);
                    yield return www.SendWebRequest();

                    if (www.isNetworkError)
                    {
                        logger.Error($"Failed to download song {songHash}");

                        // Clean up temp file
                        FileUtils.TryDeleteFile(logger, tempPath);
                    }
                    else
                    {
                        logger.Msg($"Download successful for {songHash}");

                        // Move from temp to final location
                        string contentDisposition = www.GetResponseHeader("Content-Disposition");
                        logger.Debug("CD: " + contentDisposition);
                        string mapName = www.GetResponseHeader("Content-Disposition").Split('"')[1];
                        string finalPath = $"{GetCustomSongsPath()}/{mapName}";
                        
                        if (!FileUtils.ForceMoveFile(logger, tempPath, finalPath))
                        {
                            FileUtils.TryDeleteFile(logger, tempPath);
                        }
                    }
                }
            }

            // After all songs are downloaded, refresh the song list
            //logger.Msg("Refreshing song list");
            //SongSelectionManager.GetInstance.RefreshSongList(false);
        }
    }
}
