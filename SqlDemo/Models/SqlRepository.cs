using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace SqlDemo.Models
{
    public class SqlRepository
    {
        private string connectionString;

        public SqlRepository(string connectionString) => this.connectionString = connectionString;

        public SqlDataReader ExecuteUnsafeQuery(string query)
        {
            SqlConnection connection = new SqlConnection(this.connectionString);
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    return command.ExecuteReader(CommandBehavior.CloseConnection);
                }
            }
        }
        public Int32 ExecuteUnsafeNonQuery(string query)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }
    }
}