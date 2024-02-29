using System;
using System.IO;
using System.Linq;

namespace PhotoSorter
{
    class DiskFolder
    {
        private static string _rootFolder;
        private static string _name;

        public DiskFolder(string rootFolder, long size)
        {
            _rootFolder = rootFolder;
            _name = CreateDiskFolder();
        }

        public static string Name() => _name;

        private static string CreateDiskFolder()
        {
            var folder = Directory.GetDirectories(_rootFolder).LastOrDefault(x => x.Contains("Disk "));
            if (folder == null)
            {
                Directory.CreateDirectory(Path.Combine(_rootFolder, "Disk 0"));
                return "Disk 0";
            }
            else
            {
                int number = Convert.ToInt32(folder.Substring(Path.Combine(_rootFolder, "Disk ").Length));
                var name = "Disk " + ++number;
                Directory.CreateDirectory(Path.Combine(_rootFolder, name));
                return name;
            }
        }
        public static long FolderSize(string folderName)
        {
            long size = 0;
            foreach (string file in Directory.GetFiles(folderName, "*", SearchOption.AllDirectories))
            {
                size = size + new FileInfo(file).Length;
            }
            return size;
        }
    }
}

