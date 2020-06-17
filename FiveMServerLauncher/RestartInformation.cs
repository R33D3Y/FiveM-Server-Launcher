using System.Collections.Generic;

namespace FiveMServerLauncher
{
	public class RestartInformation
	{
		public bool Enabled = false;
		public readonly List<RestartData> Data = new List<RestartData>();
	}

	public enum RestartType
	{ 
		Restart,
		Start,
		Stop
	}
}