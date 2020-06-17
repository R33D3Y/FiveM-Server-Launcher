using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FiveMServerLauncher
{
	class JSONHandler
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
				writer.WriteValue(path);

				writer.WritePropertyName("Restart Enabled");
				writer.WriteValue(RestartData.Enabled);

				int count = 0;

				foreach ((int RestartHour, int RestartMinute, RestartType RestartType, int MinuteWarning) data in RestartData.Data)
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

		public RestartData RestartData { get; private set; } = new RestartData();

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
						bool.TryParse(reader.Value.ToString(), out RestartData.Enabled);
					}
				}
			};
		}

		public void SetRestartEnabled(bool state)
		{
			RestartData.Enabled = state;
			WriteJSON();
		}

		#endregion Restart Enabled

		#region Restart Data

		private void GetRestartData()
		{
			JsonTextReader reader = new JsonTextReader(new StringReader(File.ReadAllText(jsonPath)));
			RestartData.Data.Clear();

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
						RestartData.Data.Add((hour, minute, restartType, minuteWarning));
					}
				}
			}
		}

		public void SetRestartData(List<(int RestartHour, int RestartMinute, RestartType RestartType, int MinuteWarning)> data)
		{
			RestartData.Data.Clear();
			RestartData.Data.AddRange(data);
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

			return serverDirectory;
		}

		public void SetServerDirectory(string path)
		{
			serverDirectory = path;
			WriteJSON();
		}

		#endregion Server Directory
	}
}