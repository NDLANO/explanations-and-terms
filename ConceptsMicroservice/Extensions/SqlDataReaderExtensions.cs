using System.Data.SqlClient;
using Npgsql;

namespace ConceptsMicroservice.Extensions
{
    public static class SqlDataReaderExtensions
    {
        public static string SafeGetString(this NpgsqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
                return reader.GetString(colIndex);
            return null;
        }
    }
}
