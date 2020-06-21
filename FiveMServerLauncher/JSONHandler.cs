using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace FiveMServerLauncher
{
	public class JSONHandler
	{
		private readonly string jsonPath;

		public JSONHandler(string launcherDir)
		{
			jsonPath = launcherDir + @"\config.json";

			if (!File.Exists(jsonPath))
			{
				WriteJSON();
			}
		}

		private void WriteJSON()
		{
			StringBuilder sb = new StringBuilder();
			StringWriter sw = new StringWriter(sb);

			using (JsonWriter writer = new JsonTextWriter(sw))
			{
				writer.Formatting = Formatting.Indented;
				writer.WriteStartObject();

				writer.WritePropertyName("Server Directory");
				writer.WriteValue(serverDirectory);

				writer.WritePropertyName("Server Config Directory");
				writer.WriteValue(serverConfigDirectory);

				writer.WritePropertyName("Restart Enabled");
				writer.WriteValue(RestartInformation.Enabled);

				writer.WritePropertyName("SQL Backup");
				writer.WriteStartArray();
				writer.WriteValue(SQLBackup.Enabled);
				writer.WriteValue(SQLBackup.DumpDirectory);
				writer.WriteValue(SQLBackup.DatabaseName);
				writer.WriteValue(SQLBackup.Host);
				writer.WriteValue(SQLBackup.User);
				writer.WriteValue(SQLBackup.Password);
				writer.WriteValue(SQLBackup.BackupDirectory);
				writer.WriteEnd();

				int count = 0;

				foreach (RestartData data in RestartInformation.Data)
				{
					writer.WritePropertyName("Schedule Data " + count);
					writer.WriteStartArray();
					writer.WriteValue(data.RestartHour);
					writer.WriteValue(data.RestartMinute);
					writer.WriteValue(data.RestartType);
					writer.WriteValue(data.MinuteWarning);
					writer.WriteEnd();

					count++;
				}

				count = 0;

				foreach (NodeCMD data in NodeCMDInformation.Data)
				{
					writer.WritePropertyName("NodeCMD " + count);
					writer.WriteStartArray();
					writer.WriteValue(data.Directory);
					writer.WriteValue(data.Arguments);
					writer.WriteValue(data.Enabled);
					writer.WriteEnd();

					count++;
				}

				writer.WriteEndObject();
			}

			File.WriteAllText(jsonPath, sb.ToString());
		}

		public void UpdateJSON()
		{
			WriteJSON();
		}

		public bool UpdateUI { get; set; }

		public SQLBackup SQLBackup { get; private set; } = new SQLBackup(false, "", "", "", "", "", "");

		public void SQLBackupUpdate()
		{
			GetSQLBackup();
		}

		#region SQL Backup

		private void GetSQLBackup()
		{
			JsonTextReader reader = new JsonTextReader(new StringReader(File.ReadAllText(jsonPath)));
			NodeCMDInformation.Data.Clear();

			while (reader.Read())
			{
				if (reader.Value != null)
				{
					if (reader.TokenType == JsonToken.PropertyName && reader.Value.ToString().StartsWith("SQL Backup"))
					{
						reader.Read(); // Start Data
						reader.Read(); // SQL Backup Enabled

						if (reader.Value != null)
						{
							bool.TryParse(reader.Value.ToString(), out SQLBackup.Enabled);
						}

						reader.Read(); // SQL Dump Directory

						if (reader.Value != null)
						{
							SQLBackup.DumpDirectory = reader.Value.ToString();
						}

						reader.Read(); // Database Name

						if (reader.Value != null)
						{
							SQLBackup.DatabaseName = reader.Value.ToString();
						}

						reader.Read(); // Host

						if (reader.Value != null)
						{
							SQLBackup.Host = reader.Value.ToString();
						}

						reader.Read(); // User

						if (reader.Value != null)
						{
							SQLBackup.User = reader.Value.ToString();
						}

						reader.Read(); // Password

						if (reader.Value != null)
						{
							SQLBackup.Password = reader.Value.ToString();
						}

						reader.Read(); // Backup Directory

						if (reader.Value != null)
						{
							SQLBackup.BackupDirectory = reader.Value.ToString();
						}
					}
				}
			}
		}

		#endregion SQL Backup

		public NodeCMDInformation NodeCMDInformation { get; private set; } = new NodeCMDInformation();

		public void NodeCMDUpdate()
		{
			GetNodeCMD();
		}

		#region Node CMD

		private void GetNodeCMD()
		{
			JsonTextReader reader = new JsonTextReader(new StringReader(File.ReadAllText(jsonPath)));
			NodeCMDInformation.Data.Clear();

			while (reader.Read())
			{
				if (reader.Value != null)
				{
					if (reader.TokenType == JsonToken.PropertyName && reader.Value.ToString().StartsWith("NodeCMD"))
					{
						string dir = null;
						string arg = null;
						bool enab = false;

						reader.Read(); // Start Data
						reader.Read(); // NodeCMD Directory

						if (reader.Value != null)
						{
							dir = reader.Value.ToString();
						}

						reader.Read(); // NodeCMD Arguments

						if (reader.Value != null)
						{
							arg = reader.Value.ToString();
						}

						reader.Read(); // NodeCMD Enabled

						if (reader.Value != null)
						{
							bool.TryParse(reader.Value.ToString(), out enab);
						}

						NodeCMDInformation.Data.Add(new NodeCMD(enab, dir, arg));
					}
				}
			}
		}

		#endregion Node CMD

		public RestartInformation RestartInformation { get; private set; } = new RestartInformation();

		public void RestartDataUpdate()
		{
			GetRestartEnabled();
			GetRestartData();
		}

		#region Restart Enabled

		public void GetRestartEnabled()
		{
			JsonTextReader reader = new JsonTextReader(new StringReader(File.ReadAllText(jsonPath)));

			while (reader.Read())
			{
				if (reader.Value != null)
				{
					if (reader.TokenType == JsonToken.PropertyName && reader.Value.ToString().StartsWith("Restart Enabled"))
					{
						reader.Read(); // Property Name
						bool.TryParse(reader.Value.ToString(), out RestartInformation.Enabled);
					}
				}
			};
		}

		public void SetRestartEnabled(bool state)
		{
			RestartInformation.Enabled = state;
			WriteJSON();
		}

		#endregion Restart Enabled

		#region Restart Data

		private void GetRestartData()
		{
			JsonTextReader reader = new JsonTextReader(new StringReader(File.ReadAllText(jsonPath)));
			RestartInformation.Data.Clear();

			while (reader.Read())
			{
				if (reader.Value != null)
				{
					if (reader.TokenType == JsonToken.PropertyName && reader.Value.ToString().StartsWith("Schedule Data"))
					{
						reader.Read(); // StartArray
						reader.Read(); // Restart Hour
						int.TryParse(reader.Value.ToString(), out int hour);
						reader.Read(); // Restart Minute
						int.TryParse(reader.Value.ToString(), out int minute);
						reader.Read(); // Type
						System.Enum.TryParse(reader.Value.ToString(), out RestartType restartType);
						reader.Read(); // Minute Warning
						int.TryParse(reader.Value.ToString(), out int minuteWarning);
						RestartInformation.Data.Add(new RestartData(hour, minute, restartType, minuteWarning));
					}
				}
			}
		}

		public void AddRestartData(RestartData data)
		{
			RestartInformation.Data.Add(data);
			WriteJSON();
		}

		public void RemoveRestartData(RestartData data)
		{
			RestartInformation.Data.Remove(data);
			WriteJSON();
		}

		#endregion Restart Data

		#region Server Directory

		private string serverDirectory;

		public string GetServerDirectory()
		{
			JsonTextReader reader = new JsonTextReader(new StringReader(File.ReadAllText(jsonPath)));

			while (reader.Read())
			{
				if (reader.Value != null)
				{
					if (reader.TokenType == JsonToken.PropertyName && reader.Value.ToString().StartsWith("Server Directory"))
					{
						reader.Read(); // Property Name

						if (reader.Value != null)
						{
							serverDirectory = reader.Value.ToString();
						}
					}
				}
			}

			LogDirectory = serverDirectory + @"\LogFiles\";

			return serverDirectory;
		}

		public void SetServerDirectory(string p)
		{
			if (Directory.Exists(p))
			{
				serverDirectory = p;
				LogDirectory = serverDirectory + @"\LogFiles\";
				Directory.CreateDirectory(LogDirectory);
				WriteJSON();
			}
		}

		#endregion Server Directory

		#region Server Config Directory

		private string serverConfigDirectory;

		public string GetServerConfigDirectory()
		{
			JsonTextReader reader = new JsonTextReader(new StringReader(File.ReadAllText(jsonPath)));

			while (reader.Read())
			{
				if (reader.Value != null)
				{
					if (reader.TokenType == JsonToken.PropertyName && reader.Value.ToString().StartsWith("Server Config Directory"))
					{
						reader.Read(); // Property Name

						if (reader.Value != null)
						{
							serverConfigDirectory = reader.Value.ToString();
						}
					}
				}
			}

			return serverConfigDirectory;
		}

		public void SetServerConfigDirectory(string p)
		{
			if (File.Exists(p))
			{
				serverConfigDirectory = p;
				WriteJSON();
			}
		}

		#endregion Server Config Directory

		#region Log Directory

		public string LogDirectory { get; private set; }

		#endregion Log Directory
	}
}