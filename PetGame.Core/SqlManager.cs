using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace PetGame.Core
{
    /// <summary>
    ///     Handles connection to the SQL server.
    /// </summary>
    public class SqlManager
    {
        private readonly string ConnectionString;

        /// <summary>
        ///     Constructor. Accepts the connection string to the SQL server.
        /// </summary>
        /// <param name="connectionString">
        ///     The connection string to use to connect to the SQL server.
        /// </param>
        public SqlManager(string connectionString)
        {
            ConnectionString = connectionString;
        }

        /// <summary>
        ///     Establishes a connection to the SQL server.
        /// </summary>
        public SqlConnection EstablishDataConnection
        {
            get
            {
                var x = new SqlConnection(ConnectionString);
                x.Open();
                return x;
            }
        }
    }
}
