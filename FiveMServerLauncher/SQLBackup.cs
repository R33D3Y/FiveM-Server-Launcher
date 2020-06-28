namespace FiveMServerLauncher
{
	public class SQLBackup
	{
		public bool Enabled;
		public string DumpDirectory;
		public string DatabaseName;
		public string Host;
		public string User;
		public string Password;
		public string BackupDirectory;
		public string BackupTimer;

		public SQLBackup(bool enabled, string dumpDir, string dbName, string host, string user, string pass, string backupDir, string timer)
		{
			Enabled = enabled;
			DumpDirectory = dumpDir;
			DatabaseName = dbName;
			Host = host;
			User = user;
			Password = pass;
			BackupDirectory = backupDir;
			BackupTimer = timer;
		}
	}
}