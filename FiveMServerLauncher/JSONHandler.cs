using Newtonsoft.Json;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace FiveMServerLauncher
{
	public class JSONHandler
	{
		private string path;
		private string jsonPath;

		public JSONHandler(string launcherDir)
		{
			path = launcherDir;
			jsonPath = launcherDir + @"\config.json";
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

				writer.WritePropertyName("Restart Enabled");
				writer.WriteValue(RestartInformation.Enabled);

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

				writer.WriteEndObject();
			}

			File.WriteAllText(jsonPath, sb.ToString());
		}

		public void UpdateJSON()
		{
			WriteJSON();
		}

		public bool UpdateUI { get; set; }

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
						serverDirectory = reader.Value.ToString();
					}
				}
			}

			LogDirectory = serverDirectory + @"\LogFiles\";

			return serverDirectory;
		}

		public void SetServerDirectory(string p)
		{
			serverDirectory = p;
			LogDirectory = serverDirectory + @"\LogFiles\";
			WriteJSON();
		}

		#endregion Server Directory

		#region Log Directory

		public string LogDirectory { get; private set; }

		#endregion Log Directory
	}
}