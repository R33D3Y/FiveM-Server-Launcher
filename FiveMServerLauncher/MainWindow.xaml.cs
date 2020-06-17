using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FiveMServerLauncher
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private JSONHandler jsonHandler;
		private RestartData restartData;

		public MainWindow()
		{
			InitializeComponent();

			jsonHandler = new JSONHandler(Directory.GetCurrentDirectory());

			jsonHandler.SetRestartEnabled(true);

			List<(int RestartHour, int RestartMinute, RestartType RestartType, int MinuteWarning)> data = new List<(int RestartHour, int RestartMinute, RestartType RestartType, int MinuteWarning)>
			{
				(6, 00, RestartType.Restart, 5),
				(12, 00, RestartType.Stop, 10),
				(18, 00, RestartType.Start, 10),
				(0, 00, RestartType.Restart, 5),
			};

			jsonHandler.SetRestartData(data);
			jsonHandler.SetServerDirectory(@"C:\Users\jacks\Desktop\Additional Folders\TestFile");

			jsonHandler.RestartDataUpdate();
			restartData = jsonHandler.RestartData;

			foreach ((int RestartHour, int RestartMinute, RestartType RestartType, int MinuteWarning) data_ in restartData.Data)
			{
				Console.WriteLine("Restart - " + data_.RestartHour + ":" + data_.RestartMinute + " " + data_.RestartType + " " + data_.MinuteWarning);
			}

			Console.WriteLine(jsonHandler.GetServerDirectory());
			Console.WriteLine("Restart Enabled - " + restartData.Enabled);

			RestartControl restartControl = new RestartControl(0, restartData);
			RestartControlPanel.Children.Add(restartControl);

			RestartControl restartControl1 = new RestartControl(1, restartData);
			RestartControlPanel.Children.Add(restartControl1);

			RestartControl restartControl2 = new RestartControl(2, restartData);
			RestartControlPanel.Children.Add(restartControl2);

			RestartControl restartControl3 = new RestartControl(3, restartData);
			RestartControlPanel.Children.Add(restartControl3);
		}
	}
}