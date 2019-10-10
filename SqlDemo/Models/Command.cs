using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace SqlDemo.Models
{
    public class CommandModel
    {
        public static string GetCommands()
        {
            string connectionString = "Server=DESKTOP-J8D4HHQ;Database=CmdApi;Trusted_Connection=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //string commandText = "SELECT * FROM dbo.CommandItems";
/*CREATE PROCEDURE [dbo].[GetCommands] @Platform nvarchar(MAX)
as
SELECT * from dbo.CommandItems WHERE dbo.CommandItems.Platform=@Platform
GO*/
                string commandText = "dbo.GetCommands"; // \".net core ef\"";
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;  // CommandType.Text;
                    command.Parameters.AddWithValue("@Platform", ".net core ef");
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        List<string> result = new List<string>();
                        while (reader.Read())
                        {
                            result.Add(String.Format("id:\'{0}\' howto:\'{1}\' platform:\'{2}\' command:\'{3}\'", reader["Id"], reader["HowTo"], reader["Platform"], reader["CommandLine"]));
                        }
                        return String.Join("\r\n", result.ToArray());
                    }
                }
            }
        }
    }
}