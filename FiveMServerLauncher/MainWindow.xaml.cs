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
		private RestartInformation restartInformation;

		public MainWindow()
		{
			InitializeComponent();

			jsonHandler = new JSONHandler(Directory.GetCurrentDirectory());

			jsonHandler.SetRestartEnabled(true);

			jsonHandler.AddRestartData(new RestartData(6, 00, RestartType.Restart, 5));
			jsonHandler.AddRestartData(new RestartData(12, 00, RestartType.Stop, 10));
			jsonHandler.AddRestartData(new RestartData(18, 00, RestartType.Start, 10));
			jsonHandler.AddRestartData(new RestartData(0, 00, RestartType.Restart, 5));

			jsonHandler.SetServerDirectory(@"C:\Users\jacks\Desktop\Additional Folders\TestFile");

			jsonHandler.RestartDataUpdate();
			restartInformation = jsonHandler.RestartInformation;

			//foreach (RestartData data_ in restartInformation.Data)
			//{
			//	Console.WriteLine("Restart - " + data_.RestartHour + ":" + data_.RestartMinute + " " + data_.RestartType + " " + data_.MinuteWarning);
			//}

			//Console.WriteLine(jsonHandler.GetServerDirectory());
			//Console.WriteLine("Restart Enabled - " + restartInformation.Enabled);

			RestartControl restartControl = new RestartControl(0, restartInformation.Data[0], jsonHandler);
			RestartControlPanel.Children.Add(restartControl);

			RestartControl restartControl1 = new RestartControl(1, restartInformation.Data[1], jsonHandler);
			RestartControlPanel.Children.Add(restartControl1);

			RestartControl restartControl2 = new RestartControl(2, restartInformation.Data[2], jsonHandler);
			RestartControlPanel.Children.Add(restartControl2);

			RestartControl restartControl3 = new RestartControl(3, restartInformation.Data[3], jsonHandler);
			RestartControlPanel.Children.Add(restartControl3);
		}
	}
}