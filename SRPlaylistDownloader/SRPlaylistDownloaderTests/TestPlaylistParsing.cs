using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRModCore;
using SRPlaylistDownloader.Services;
using System;
using System.IO;

namespace SRPlaylistDownloaderTests
{
    [TestClass]
    public class TestPlaylistParsing
    {
        [TestMethod]
        public void TestValidPlaylistParsed()
        {
            var playlistDir = $"{AppDomain.CurrentDomain.BaseDirectory}/TestPlaylists/Valid";
            Assert.IsTrue(Directory.Exists(playlistDir));

            var logger = new TestLogger();
            var playlistService = new PlaylistService(logger, playlistDir);
            var playlists = playlistService.GetAllPlaylists();
            Assert.IsNotNull(playlists);
            Assert.AreEqual(1, playlists.Count);
            Assert.AreEqual("Emi Force Training", playlists[0].PlaylistName);
            Assert.AreEqual(16, playlists[0].Items.Count);
        }

        [TestMethod]
        public void TestInvalidPlaylistIgnored()
        {
            var playlistDir = $"{AppDomain.CurrentDomain.BaseDirectory}/TestPlaylists/Invalid";
            Assert.IsTrue(Directory.Exists(playlistDir));

            var logger = new TestLogger();
            var playlistService = new PlaylistService(logger, playlistDir);
            var playlists = playlistService.GetAllPlaylists();
            Assert.IsNotNull(playlists);
            Assert.AreEqual(0, playlists.Count);
        }

        [TestMethod]
        public void TestNoPlaylistDirectoryIgnored()
        {
            var playlistDir = $"{AppDomain.CurrentDomain.BaseDirectory}/NonexistentDirectory";

            var logger = new TestLogger();
            var playlistService = new PlaylistService(logger, playlistDir);
            var playlists = playlistService.GetAllPlaylists();
            Assert.IsNotNull(playlists);
            Assert.AreEqual(0, playlists.Count);
        }
    }
}
