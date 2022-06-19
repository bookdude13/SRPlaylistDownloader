# Synth Riderz Playlist Downloader
Automatically download all custom songs from Synth Riderz playlists when starting the game.


## Installing
1. Prepare Synth Riders for modding by following the [MelonLoader](https://melonwiki.xyz/#/README) wiki guide
2. Download the newest version of SRPlaylistDownloader from the Releases section. Your web browser may warn you about the .zip file because it contains .dll files - this is normal, and is how mods are built.
3. Extract the contents of the .zip file to  `<path-to-synth-riders>\Mods` (create a new directory if it doesn't exist). On Windows, this will be at `C:\Program Files (x86)\Steam\steamapps\common\SynthRiders\Mods`. You should end up with something like `SynthRiders\Mods\SRPlaylistDownloader.dll`.
4. Run Synth Riders and enjoy!

## Using
This mod hooks into the "Refresh Song List" button at the top of the main menu. TO use it, simply hit that button and select the "Find New" or "Refresh All" options ("Find New" is generally faster).

This can take a while if a lot of songs need to be downloaded. 

## Implementation Details
- The main functionality is triggered from a manual song list refresh
- All playlist files under `SynthRiders\Playlist` are parsed, and all song hashes are put into a HashSet
- All OST/DLC tracks also have their hashes added to this "existing songs" HashSet
- All song hashes found in playlists that aren't in the existing set will be downloaded from synthriderz.com
  - OST/DLC will be "downloaded" from synthriderz.com, but without a Content-Disposition header in the response, they will be ignored
  - Files are downloaded first to `SynthRiders\CustomSongs\temp\temp_<hash>.synth`, then moved to the final location in `CustomSongs` with the proper name once the download finishes
- Downloads are synchronous, using Coroutines. In the future they could be parallelized, but for now this is simple and robust

## Known Issues
- If there are unrecognized entries in a playlist file (e.g. a hash that doesn't exist on synthriderz.com, like an unpublished draft), these will "download" every time and fail quickly every time. If you see a few really quick "downloads" when refreshing the song list, this is why. This should not have any negative effects except for increased confusion :)

---

## Disclaimer
This mod is not affiliated with Synth Riders or Kluge Interactive. Use at your own risk.

