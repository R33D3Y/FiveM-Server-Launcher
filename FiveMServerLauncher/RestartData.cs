using System.Collections.Generic;

namespace FiveMServerLauncher
{
	public class RestartData
	{
		public bool Enabled = false;
		public readonly List<(int RestartHour, int RestartMinute, RestartType RestartType, int MinuteWarning)> Data = new List<(int, int, RestartType, int )>();
	}

	public enum RestartType
	{ 
		Restart,
		Start,
		Stop
	}
}