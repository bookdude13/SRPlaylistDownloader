using MelonLoader;
using SRModCore;
using SRPlaylistDownloader.Services;
using Synth.SongSelection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPlaylistDownloader
{
    public class MainMod : MelonMod
    {
        private static SRLogger logger;
        private SynthriderzService synthriderzService;


        public override void OnApplicationStart()
        {
            base.OnApplicationStart();

            logger = new MelonLoggerWrapper(LoggerInstance);
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            base.OnSceneWasInitialized(buildIndex, sceneName);

            SRScene scene = new SRScene(sceneName);
            if (scene.SceneType == SRScene.SRSceneType.WARNING)
            {
                // Setup services only once, and check playlist items right away
                synthriderzService = new SynthriderzService(logger);
            }

            if (scene.SceneType == SRScene.SRSceneType.MAIN_MENU)
            {
                logger.Msg("Downloading missing maps");
                var mapHashes = new List<string>(new string[] {
                    "7fd0cb0ac24b20312321ec1c57743fdaf133c03e4ea0621278cc5ad7a6edf648",
                    "0f8c8a5a2f6325ec1cf01e2a54b193d8fb8e3a2931fe21e56518330cf79b400f",
                    "fb630fc4d7e71b0c36929cf98171ab9076277cedeb8b9e2f4cbfc27efd12e713"
                });
                DownloadMissingCustomMaps(mapHashes);
            }
        }

        private void DownloadMissingCustomMaps(List<string> mapHashes)
        {
            if (SynthsFinder.FilesLoaded)
            {
                logger.Debug("Loading existing list of custom hashes");
                var existingHashes = GetExistingCustomMapHashes();
                logger.Msg($"{existingHashes.Count} customs found");

                var hashesToDownload = mapHashes.Where(hash => !existingHashes.Contains(hash)).ToList();
                if (hashesToDownload.Count == 0)
                {
                    logger.Msg("All playlist songs already downloaded");
                }
                else
                {
                    logger.Msg($"Downloading {hashesToDownload.Count} missing songs");
                    MelonCoroutines.Start(synthriderzService.DownloadSongsByHash(hashesToDownload));
                }
            }
            else
            {
                logger.Msg("Files not loaded yet; TODO try again soon");
            }
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
