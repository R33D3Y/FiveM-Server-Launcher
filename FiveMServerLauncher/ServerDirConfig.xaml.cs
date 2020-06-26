using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace FiveMServerLauncher
{
	/// <summary>
	/// Interaction logic for ServerDirConfig.xaml
	/// </summary>
	public partial class ServerDirConfig : Window
	{
		private readonly MainWindow MainWindow;
		private readonly JSONHandler jsonHandler;

		public ServerDirConfig(MainWindow mw, JSONHandler jh)
		{
			InitializeComponent();

			MainWindow = mw;
			jsonHandler = jh;

			if (jsonHandler.GetServerDirectory() != null || jsonHandler.GetServerDirectory() != "")
			{
				textBoxDir.Text = jsonHandler.GetServerDirectory();
			}

			if (jsonHandler.GetServerConfigDirectory() != null || jsonHandler.GetServerConfigDirectory() != "")
			{
				textBoxConfig.Text = jsonHandler.GetServerConfigDirectory();
			}
		}

		private void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				DragMove();
			}
		}

		private void ButtonOkay_Click(object sender, RoutedEventArgs e)
		{
			if ((jsonHandler.GetServerDirectory() != null || jsonHandler.GetServerDirectory() != "") || (jsonHandler.GetServerConfigDirectory() != null || jsonHandler.GetServerConfigDirectory() != ""))
			{
				Close();
			}
			else
			{
				System.Windows.MessageBox.Show("Opps! You forgot to select a Directory.", "FiveM Server Launcher", MessageBoxButton.OK, (MessageBoxImage)MessageBoxIcon.Error);
			}
		}

		private void ButtonCancel_Click(object sender, RoutedEventArgs e)
		{
			if ((jsonHandler.GetServerDirectory() != null || jsonHandler.GetServerDirectory() != "") || (jsonHandler.GetServerConfigDirectory() != null || jsonHandler.GetServerConfigDirectory() != ""))
			{
				Close();
			}
			else
			{
				MessageBoxResult msgResult = System.Windows.MessageBox.Show("Do you want to close the application?", "FiveM Server Launcher", MessageBoxButton.OKCancel, (MessageBoxImage)MessageBoxIcon.Error);

				if (msgResult == MessageBoxResult.OK)
				{
					Close();
					MainWindow.Close();
				}
			}
		}

		private void ButtonBrowseDir_Click(object sender, RoutedEventArgs e)
		{
			using (var dialog = new FolderBrowserDialog())
			{
				DialogResult result = dialog.ShowDialog();
				jsonHandler.SetServerDirectory(dialog.SelectedPath);
				textBoxDir.Text = dialog.SelectedPath;
			}
		}

		private void ButtonBrowseConfig_Click(object sender, RoutedEventArgs e)
		{
			using (var dialog = new OpenFileDialog())
			{
				DialogResult result = dialog.ShowDialog();
				jsonHandler.SetServerConfigDirectory(dialog.FileName);
				textBoxConfig.Text = dialog.FileName;
			}
		}
	}
}