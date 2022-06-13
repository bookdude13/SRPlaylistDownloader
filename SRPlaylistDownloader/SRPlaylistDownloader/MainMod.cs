using MelonLoader;
using SRModCore;
using SRPlaylistDownloader.Models;
using SRPlaylistDownloader.Services;
using Synth.SongSelection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SRPlaylistDownloader
{
    public class MainMod : MelonMod
    {
        public static MainMod Instance { get; private set; } = null;

        private SRLogger logger;
        private PlaylistService playlistService;
        private SynthriderzService synthriderzService;
        private PlaylistDownloadManager playlistDownloadManager;

        public override void OnApplicationStart()
        {
            base.OnApplicationStart();

            logger = new MelonLoggerWrapper(LoggerInstance);
            Instance = this;
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            base.OnSceneWasInitialized(buildIndex, sceneName);

            SRScene scene = new SRScene(sceneName);
            if (scene.SceneType == SRScene.SRSceneType.WARNING)
            {
                // Setup services only once
                synthriderzService = new SynthriderzService(logger);
                playlistService = new PlaylistService(logger);
            }

            if (scene.SceneType == SRScene.SRSceneType.MAIN_MENU)
            {
                var downloadManagerGO = new GameObject("srplaylistmanager_main");
                playlistDownloadManager = downloadManagerGO.AddComponent<PlaylistDownloadManager>();
                playlistDownloadManager.Init(logger, playlistService, synthriderzService);

                // Start downloading right away
                playlistDownloadManager.DownloadMissingPlaylistsItems();
            }
        }

        public void OnSongsReload()
        {

        }
    }
}
