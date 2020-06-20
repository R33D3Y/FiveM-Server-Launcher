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

		public SQLBackup(bool enabled, string dumpDir, string dbName, string host, string user, string pass, string backupDir)
		{
			Enabled = enabled;
			DumpDirectory = dumpDir;
			DatabaseName = dbName;
			Host = host;
			User = user;
			Password = pass;
			BackupDirectory = backupDir;
		}
	}
}