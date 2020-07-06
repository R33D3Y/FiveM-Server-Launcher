using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FiveMServerLauncher
{
	/// <summary>
	/// Interaction logic for RestartControl.xaml
	/// </summary>
	public partial class UsersControl : UserControl
	{
		private readonly MainWindow MainWindow;

		public string ID { private set; get; }

		public string UserName { private set; get; }

		public UsersControl(MainWindow mw, string name, string id)
		{
			InitializeComponent();

			MainWindow = mw;
			ID = id;
			UserName = name;

			label.Content = id + " " + name;
		}

		private bool running;

		public bool GetRunning()
		{
			return running;
		}

		public void SetRunning(bool r)
		{
			//running = r;

			//if (running)
			//{
			//	label.Foreground = new SolidColorBrush(Colors.Green);
			//	buttonStart.Visibility = Visibility.Collapsed;
			//	buttonRestart.Visibility = Visibility.Visible;
			//	buttonStop.Visibility = Visibility.Visible;
			//}
			//else
			//{
			//	label.Foreground = new SolidColorBrush(Colors.Red);
			//	buttonStart.Visibility = Visibility.Visible;
			//	buttonRestart.Visibility = Visibility.Collapsed;
			//	buttonStop.Visibility = Visibility.Collapsed;
			//}
		}

		private void StartButton_Click(object sender, RoutedEventArgs e)
		{
			//label.Foreground = new SolidColorBrush(Colors.Green);
			//MainWindow.SendToConsole("start " + ID);
		}

		private void RestartButton_Click(object sender, RoutedEventArgs e)
		{
			//label.Foreground = new SolidColorBrush(Colors.Blue);
			//MainWindow.SendToConsole("restart " + ID);
		}

		private void StopButton_Click(object sender, RoutedEventArgs e)
		{
			//label.Foreground = new SolidColorBrush(Colors.Red);
			//MainWindow.SendToConsole("stop " + ID);
		}
	}
}