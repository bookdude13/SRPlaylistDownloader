using MiKu.NET;
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
        public static string GetCustomSongsPath()
        {
            return Serializer.CHART_SAVE_PATH;
        }

        public static string GetCustomSongsTempPath()
        {
            return $"{GetCustomSongsPath()}/temp";
        }

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

        public static List<FileInfo> TryGetFilesInDirectory(SRLogger logger, string directoryPath, string searchPattern)
        {
            try
            {
                var directoryInfo = new DirectoryInfo(directoryPath);
                if (directoryInfo.Exists)
                {
                    return directoryInfo.GetFiles(searchPattern).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to list files in directory {directoryPath} with search pattern {searchPattern}", ex);
            }

            return new List<FileInfo>();
        }

        /// <summary>
        /// Returns all text from the given file info, or null on failure
        /// </summary>
        public static string TryReadAllFromFileInfo(SRLogger logger, FileInfo fileInfo)
        {
            try
            {
                return File.ReadAllText(fileInfo.FullName);
            }
            catch (Exception ex)
            {
                logger.Error($"Failed to read file {fileInfo.FullName}", ex);
            }

            return null;
        }

        public static void EnsureDirectory(SRLogger logger, string directoryPath)
        {
            try
            {
                Directory.CreateDirectory(directoryPath);
            }
            catch (Exception ex)
            {
                logger.Error($"Failed to create directory {directoryPath}", ex);
            }
        }

        public static void TryDeleteDirectoryRecursive(SRLogger logger, string directoryPath)
        {
            try
            {
                Directory.Delete(directoryPath, true);
            }
            catch (Exception ex)
            {
                logger.Error($"Failed to delete directory at {directoryPath}", ex);
            }
        }
    }
}
