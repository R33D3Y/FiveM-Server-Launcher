namespace FiveMServerLauncher
{
	public class RestartData
	{
		public int RestartHour;
		public int RestartMinute;
		public RestartType RestartType;
		public int MinuteWarning;

		public RestartData(int restartHour, int restartMinute, RestartType restartType, int minuteWarning)
		{
			RestartHour = restartHour;
			RestartMinute = restartMinute;
			RestartType = restartType;
			MinuteWarning = minuteWarning;
		}
	}
}