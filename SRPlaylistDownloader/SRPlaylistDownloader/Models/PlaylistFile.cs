using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPlaylistDownloader.Models
{
    public struct PlaylistFile
    {
		[JsonProperty("namePlaylist")]
		public string PlaylistName { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("gradientTop")]
		public string GradientTop { get; set; }

		[JsonProperty("gradientDown")]
		public string GradientDown { get; set; }

		[JsonProperty("colorTitle")]
		public string ColorTitle { get; set; }

		[JsonProperty("colorTexture")]
		public string ColorTexture { get; set; }

		public int SelectedIconIndex { get; set; }

		public int SelectedTexture { get; set; }

		[JsonProperty("creationDate")]
		public string CreationDateStr { get; set; }

		public long CreationDate
        {
            get
            {
				return long.Parse(CreationDateStr);
            }
        }

		[JsonProperty("dataString")]
		public List<PlaylistItem> Items { get; set; }
	}
}
