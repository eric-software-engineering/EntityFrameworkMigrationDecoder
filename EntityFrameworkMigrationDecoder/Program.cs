using System;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Xml.Linq;

// Source: https://tech.trailmax.info/2014/03/inside_of_ef_migrations/
// Returns the decoded migration XML of a migration from the __MigrationHistory table
namespace EntityFrameworkMigrationDecoder
{
    class Program
    {
        static void Main(string[] args)
        {
            DecompressDatabaseMigration("MigrationsName");
            Console.ReadKey();
        }

        public static void DecompressDatabaseMigration(String migrationName)
        {
            // TODO: Replace the connection string
            const string ConnectionString = "ConnectionString";
            var sqlToExecute = String.Format("select model from __MigrationHistory where migrationId like '%{0}'", migrationName);

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand(sqlToExecute, connection);
                var reader = command.ExecuteReader();

                if (!reader.HasRows)
                    throw new Exception("Now Rows to display. Probably migration name is incorrect");

                while (reader.Read())
                {
                    var model = (byte[])reader["model"];
                    var decompressed = Decompress(model);
                    Console.WriteLine(decompressed);
                }
            }
        }

        public static XDocument Decompress(byte[] bytes)
        {
            using (var memoryStream = new MemoryStream(bytes))
            using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                return XDocument.Load(gzipStream);
        }
    }
}
