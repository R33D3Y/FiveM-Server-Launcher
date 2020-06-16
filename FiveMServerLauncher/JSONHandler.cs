using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		public bool RestartEnabled()
		{
			return true;
		}

		public List<(int hour, int minute)> GetRestartData()
		{
			List<(int hour, int minute)> restartDataList = new List<(int hour, int minute)>();
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

		public void SetRestartData(List<(int hour, int minute)> restartDataList)
		{
			StringBuilder sb = new StringBuilder();
			StringWriter sw = new StringWriter(sb);

			using (JsonWriter writer = new JsonTextWriter(sw))
			{
				writer.Formatting = Formatting.Indented;
				writer.WriteStartObject();

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
	}
}