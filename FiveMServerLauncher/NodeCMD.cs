namespace FiveMServerLauncher
{
	public class NodeCMD
	{
		public bool Enabled;
		public string Directory;
		public string Arguments;

		public NodeCMD(bool enabled, string dir, string arg)
		{
			Enabled = enabled;
			Directory = dir;
			Arguments = arg;
		}
	}
}