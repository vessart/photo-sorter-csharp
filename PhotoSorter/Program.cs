using System;
using System.Globalization;
using System.IO;
using System.Security.Authentication.ExtendedProtection;
using System.Windows.Media.Imaging;

namespace PhotoSorter
{
    internal class Program
    {
        private static int _totalCount = 0;
        private static int _addedCount = 0;
        private const string TempDirectory = "TEMP";

        private static void Main(string[] args)
        {
            string @from = args[0];
            string @to = args[1];

            if (args.Length == 3 && args[2] == "--check")
            {
                DirectoryInfo @path = new DirectoryInfo(from);
                CheckDirectoryAndFiles(@path);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("[OK]");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" Check completed...");
            }

            DataBaseControl.CreateDataBase(@to);

            if (!Directory.Exists(Path.Combine(@to, TempDirectory)))
                Directory.CreateDirectory(Path.Combine(@to, TempDirectory));


            ;

            long rootFolderSize = DiskFolder.FolderSize(@to);

            foreach (string file in Directory.GetFiles(@from, "*", SearchOption.AllDirectories))
            {
                FileInfo fileInfo = new FileInfo(file);
                if (WorkOnTheFile.AddFileToDatabase(fileInfo, @to))
                {
                    rootFolderSize = rootFolderSize + fileInfo.Length;
                    _addedCount++;
                }
                _totalCount++;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[SUMMARY]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" Files added: " + _addedCount);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[SUMMARY]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" Files read: " + _totalCount);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[SUMMARY]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" Total size root directory megabytes: " 
                + rootFolderSize / 1024 / 1024 + ", bytes: " + rootFolderSize);
            Console.ReadLine();
        }

        private static void CheckDirectoryAndFiles(DirectoryInfo source)
        {
            try
            {
                foreach (DirectoryInfo dir in source.GetDirectories())
                {
                    CheckDirectoryAndFiles(dir);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine(dir.FullName);
                }
                foreach (FileInfo file in source.GetFiles())
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine(file.FullName);
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"File or directory was brocken: {e}");
            }
        }
    }
}
