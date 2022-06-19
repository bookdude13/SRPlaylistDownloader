using Newtonsoft.Json;
using SRModCore;
using SRPlaylistDownloader.Models;
using Synth.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SRPlaylistDownloader.Services
{
    public class PlaylistService
    {
        // From PlayListFile
        private const string PLAYLIST_FILE_PATTERN = "*.playlist";

        private SRLogger logger;
        private string playlistDirectoryPath;

        public PlaylistService(SRLogger logger) : this(logger, Application.dataPath + "/../Playlist/") {}

        public PlaylistService(SRLogger logger, string playlistDirectoryPath)
        {
            this.logger = logger;
            this.playlistDirectoryPath = playlistDirectoryPath;
        }

        public List<PlaylistFile> GetAllPlaylists()
        {
            var playlists = new List<PlaylistFile>();
            
            logger.Debug("Getting playlists");
            var playlistFileInfos = FileUtils.TryGetFilesInDirectory(logger, playlistDirectoryPath, PLAYLIST_FILE_PATTERN);
            logger.Debug($"{playlistFileInfos.Count} playlist files found");

            foreach (var playlistFileInfo in playlistFileInfos)
            {
                var playlistFileRaw = FileUtils.TryReadAllFromFileInfo(logger, playlistFileInfo);
                if (playlistFileRaw != null)
                {
                    try
                    {
                        var playlist = JsonConvert.DeserializeObject<PlaylistFile>(playlistFileRaw);
                        playlists.Add(playlist);
                    }
                    catch (Exception ex)
                    {
                        logger.Error($"Failed to parse playlist from file {playlistFileInfo.Name}", ex);
                    }
                }
            }

            return playlists;
        }
    }
}
