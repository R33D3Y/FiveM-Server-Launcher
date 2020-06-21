using MySql.Data.MySqlClient;
using System;

namespace FiveMServerLauncher
{
	public class MySQLHandler
	{
		private readonly string connectionString;

		public MySQLHandler(string host, string db, string usr, string pass)
		{
			connectionString = "server=" + host + ";user=" + usr + ";database=" + db + ";port=3306;";

			if (pass != null && pass != "")
			{
				connectionString += "password=" + pass;
			}

			Test();
		}

		private void Test()
		{
			MySqlConnection conn = new MySqlConnection(connectionString);
			try
			{
				Console.WriteLine("Connecting to MySQL...");
				conn.Open();

				string sql = "SELECT * FROM Jobs";
				MySqlCommand cmd = new MySqlCommand(sql, conn);
				MySqlDataReader rdr = cmd.ExecuteReader();

				while (rdr.Read())
				{
					Console.WriteLine(rdr[0] + " -- " + rdr[1] + " -- " + rdr[2]);
				}
				rdr.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}

			conn.Close();
			Console.WriteLine("Done.");
		}
	}
}
