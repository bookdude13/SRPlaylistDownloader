using MiKu.NET;
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

            // Make sure temp directory exists
            FileUtils.EnsureDirectory(logger, FileUtils.GetCustomSongsTempPath());
        }

        

        private string GetTempSongPath(string songHash)
        {
            return $"{FileUtils.GetCustomSongsTempPath()}/temp_{songHash}.synth";
        }

        private IEnumerator DownloadSongByHash(string songHash)
        {
            string tempPath = GetTempSongPath(songHash);

            // If download was interrupted earlier, remove old file
            FileUtils.TryDeleteFile(logger, tempPath);

            string downloadUrl = $"{baseUrl}/beatmaps/hash/download/{songHash}";
            logger.Debug($"Downloading from url {downloadUrl}");
            using (UnityWebRequest www = UnityWebRequest.Get(downloadUrl))
            {
                // Save to temporary location first
                var downloadHandler = new DownloadHandlerFile(tempPath);
                downloadHandler.removeFileOnAbort = true;
                www.downloadHandler = downloadHandler;
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
                    var mapName = GetMapNameFromResponse(www);
                    if (mapName == null)
                    {
                        FileUtils.TryDeleteFile(logger, tempPath);
                    }
                    else
                    {
                        string finalPath = $"{FileUtils.GetCustomSongsPath()}/{mapName}";
                        FileUtils.ForceMoveFile(logger, tempPath, finalPath);
                    }
                }
            }
        }

        private string GetMapNameFromResponse(UnityWebRequest www)
        {
            // If "content-disposition" header is missing, the hash is unknown by synthriderz
            string contentDisposition = www.GetResponseHeader("content-disposition");
            if (contentDisposition == null)
            {
                logger.Msg("Song doesn't have content-disposition, likely unknown by Synthriderz (invalid entry)");
                return null;
            }

            // Value is something like this: attachment;filename="1527-Idina-Menzel-FROZEN-Main-Theme-Let-It-Go-RAPTOR.synth"
            var pieces = contentDisposition.Split('"');
            if (pieces.Length < 2)
            {
                logger.Error($"Failed to parse file name from content-disposition header: {contentDisposition}");
                return null;
            }

            string mapName = pieces[1];
            return mapName;
        }

        /// <summary>
        /// Downloads songs from Synthriderz.com to the CustomSongs directory.
        /// Overwrites existing files.
        /// </summary>
        /// <param name="songHashes">Song hashes to download</param>
        /// <param name="OnDownload">Function called after each download completes</param>
        public IEnumerator DownloadSongsByHash(HashSet<string> songHashes, Action<string> OnDownload)
        {
            foreach (var songHash in songHashes)
            {
                yield return DownloadSongByHash(songHash);
                OnDownload(songHash);
            }
        }
    }
}
