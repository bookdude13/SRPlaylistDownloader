using SRModCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPlaylistDownloader
{
    public static class FileUtils
    {
        public static void TryDeleteFile(SRLogger logger, string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Failed to delete file at {path}", ex);
            }
        }

        public static bool ForceMoveFile(SRLogger logger, string sourceFile, string destinationFile)
        {
            try
            {
                if (File.Exists(sourceFile))
                {
                    if (File.Exists(destinationFile))
                    {
                        logger.Msg($"Replacing file {sourceFile} with {destinationFile}");
                        File.Replace(sourceFile, destinationFile, null);
                        return true;
                    }
                    else
                    {
                        logger.Msg($"Moving file {sourceFile} to {destinationFile}");
                        File.Move(sourceFile, destinationFile);
                        return true;
                    }
                }
                else
                {
                    logger.Msg($"Source file {sourceFile} doesn't exist, so can't move!");
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Failed to move/replace {sourceFile} with {destinationFile}", ex);
            }

            return false;
        }
    }
}
