using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace PhotoSorter
{
    internal class DataBaseControl
    {
        private const string DataBaseFileName = "database.db";
        private static string _databaseFileName;
        private static SQLiteConnection _sqLiteConnection;

        public static void CreateDataBase(string path)
        {
            _databaseFileName = Path.Combine(path, DataBaseFileName);

            if (File.Exists(_databaseFileName))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("[OK]");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" Database file exists, create step has been skiped...");
                return;
            }

            OpenSQLiteConnection();

            SQLiteCommand command = new SQLiteCommand(_sqLiteConnection);
            command.CommandText = @"CREATE TABLE [INFO] (                    
                    [FILENAME] char(100) NOT NULL,
                    [DATE_OF_SHOT] char(100) NOT NULL,
                    [PATH] char(255) NOT NULL
                    );";
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[OK]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" Database file has been created...");
        }

        // ReSharper disable once InconsistentNaming
        private static void OpenSQLiteConnection()
        {
            _sqLiteConnection = new SQLiteConnection("Data Source=" + _databaseFileName + "; Version=3;");
            _sqLiteConnection.Open();
        }

        public static bool IsExistsRow(string filename, string dateOfShot)
        {
            OpenSQLiteConnection();
            SQLiteCommand command = new SQLiteCommand(_sqLiteConnection);
            command.CommandText = "SELECT FILENAME, DATE_OF_SHOT FROM INFO "
                                  + "WHERE FILENAME= '" + filename + "' AND DATE_OF_SHOT= '" + dateOfShot + "'";
            command.CommandType = CommandType.Text;
            return command.ExecuteScalar() != null;
        }

        public static void AddRow(string filename, string dateOfShot, string filePathDestination)
        {
            SQLiteCommand command = new SQLiteCommand(_sqLiteConnection);
            command.CommandText = "INSERT INTO info(FILENAME, DATE_OF_SHOT, PATH) "
                                  + "VALUES ('" + filename + "', '" + dateOfShot + "', '" + filePathDestination + "')";
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
        }
    }
}