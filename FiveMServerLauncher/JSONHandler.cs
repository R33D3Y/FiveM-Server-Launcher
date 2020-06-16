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
				writer.WriteValue(restartEnabled);

				int count = 0;

				foreach ((int hour, int minute) restartData in restartDataList)
				{
					writer.WritePropertyName("Restart Data " + count);
					writer.WriteStartArray();
					writer.WriteValue(restartData.hour);
					writer.WriteValue(restartData.minute);
					writer.WriteEnd();

					count++;
				}

				writer.WriteEndObject();
			}

			File.WriteAllText(jsonPath, sb.ToString());
		}

		#region Restart Enabled

		private bool restartEnabled;

		public bool GetRestartEnabled()
		{
			JsonTextReader reader = new JsonTextReader(new StringReader(File.ReadAllText(jsonPath)));

			while (reader.Read())
			{
				if (reader.Value != null)
				{
					if (reader.TokenType == JsonToken.PropertyName && reader.Value.ToString().StartsWith("Restart Enabled"))
					{
						reader.Read(); // Property Name
						bool.TryParse(reader.Value.ToString(), out restartEnabled);
					}
				}
			}

			return restartEnabled;
		}

		public void SetRestartEnabled(bool state)
		{
			restartEnabled = state;
		}

		#endregion Restart Enabled

		#region Restart Data

		private List<(int hour, int minute)> restartDataList;

		public List<(int hour, int minute)> GetRestartData()
		{
			restartDataList = new List<(int hour, int minute)>();
			JsonTextReader reader = new JsonTextReader(new StringReader(File.ReadAllText(jsonPath)));

			while (reader.Read())
			{
				if (reader.Value != null)
				{
					if (reader.TokenType == JsonToken.PropertyName && reader.Value.ToString().StartsWith("Restart Data"))
					{
						reader.Read(); // StartArray
						reader.Read(); // Hour
						int.TryParse(reader.Value.ToString(), out int hour);
						reader.Read(); // Minute
						int.TryParse(reader.Value.ToString(), out int minute);
						restartDataList.Add((hour, minute));
					}
				}
			}

			return restartDataList;
		}

		public void SetRestartData(List<(int hour, int minute)> list)
		{
			restartDataList = list;
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