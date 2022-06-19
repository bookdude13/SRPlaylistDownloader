using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPlaylistDownloader.Models
{
    public struct PlaylistItem
    {
        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("name")]
		public string Name { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("beatmapper")]
        public string Mapper { get; set; }

        [JsonProperty("difficult")]
        public int DifficultyIndex { get; set; }

        [JsonProperty("trackDuration")]
        public double TrackDuration { get; set; }

        [JsonProperty("addedTime")]
        public long AddedTimeEpochSec { get; set; }
    }
}
