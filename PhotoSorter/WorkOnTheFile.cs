using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace PhotoSorter
{
    class WorkOnTheFile
    {
        private const string TempDirectory = "TEMP";

        private static readonly string[] ExifExtensions = { "JPG", "JPEG", "PNG" };
        private static readonly string[] NonExifExtensions = { "MOV", "3GP", "AVI", "MPG", "MP4", "RAW", "SRW" };

        private static FileInfo _fileInfo;
        private static string _to;

        public static bool AddFileToDatabase(FileInfo fileInfo, string @to)
        {
            _fileInfo = fileInfo;
            _to = @to;

            var extansionResult = CheckExtension(_fileInfo.Name);

            switch (extansionResult)
            {
                case true:
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"[READ] (EXIF) name: {_fileInfo.Name} " +
                                      $"fullName: {_fileInfo.FullName} date: {_fileInfo.CreationTime} {_fileInfo.LastWriteTime}");
                    return AddExifFile();
                case false:
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"[READ] (NOT EXIF) name: {_fileInfo.Name} " +
                                      $"fullName: {_fileInfo.FullName} date: {_fileInfo.CreationTime} {_fileInfo.LastWriteTime}");
                    return AddNotExifFile();
                case null:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[SKIP] Unknowen extension {_fileInfo.Name}");
                    return false;
            }
            return false;
        }

        private static bool AddExifFile()
        {
            try
            {
                FileStream fileStream = File.Open(_fileInfo.FullName, FileMode.Open, FileAccess.Read);
                BitmapDecoder bitmapDecoder = BitmapDecoder.Create(fileStream, BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.Default); //"распаковали" снимок и создали объект decoder
                BitmapMetadata bitmapMetadata = (BitmapMetadata)bitmapDecoder.Frames[0].Metadata?.Clone();

                DateTime dateOfShot = Convert.ToDateTime(bitmapMetadata?.DateTaken);
                fileStream.Close();

                return CommonForAddFunction(dateOfShot);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{e.Message} FILENAME: {_fileInfo.FullName}");
                return false;
            }
                
        }

        private static bool AddNotExifFile()
        {
            return CommonForAddFunction(_fileInfo.LastWriteTime);
        }

        private static bool CommonForAddFunction(DateTime dateTime)
        {
            string filePathDestination;

            if (DataBaseControl.IsExistsRow(_fileInfo.Name, dateTime.ToString(CultureInfo.InvariantCulture)))
                return false;
            if (dateTime.ToString(CultureInfo.InvariantCulture) != "01/01/0001 00:00:00")
            {
                if (!Directory.Exists(Path.Combine(_to, dateTime.Year.ToString())))
                    Directory.CreateDirectory(Path.Combine(_to, dateTime.Year.ToString()));
                if (!Directory.Exists(Path.Combine(_to, dateTime.Year.ToString(), dateTime.Month.ToString())))
                    Directory.CreateDirectory(Path.Combine(_to, dateTime.Year.ToString(),
                        dateTime.Month.ToString()));

                filePathDestination = Path.Combine(_to, dateTime.Year.ToString(), dateTime.Month.ToString(), _fileInfo.Name);
                if (File.Exists(filePathDestination))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"File: {filePathDestination} already exists");
                    return false;
                }
                File.Copy(Path.Combine(_fileInfo.FullName), filePathDestination);
            }
            else
            {
                filePathDestination = Path.Combine(_to, TempDirectory, _fileInfo.Name);
                if(File.Exists(filePathDestination))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"File: {filePathDestination} already exists");
                    return false;
                }
                File.Copy(Path.Combine(_fileInfo.FullName), filePathDestination);
            }

            DataBaseControl.AddRow(_fileInfo.Name, dateTime.ToString(CultureInfo.InvariantCulture),
                filePathDestination.Substring(_to.Length));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[ADDED]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($" Row [{_fileInfo.Name}] [{dateTime}] has been added");
            return true;
        }

        /// <summary>
        /// Return true if it is exif extension
        /// </summary>
        /// <param name="file">File name</param>
        /// <returns></returns>
        private static bool? CheckExtension(string file)
        {
            if (ExifExtensions.Any(x => file.ToUpperInvariant().EndsWith(x)))
                return true;
            if (NonExifExtensions.Any(x => file.ToUpperInvariant().EndsWith(x)))
                return false;
            return null;
        }
    }
}
