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

		public MainWindow()
		{
			InitializeComponent();

			jsonHandler = new JSONHandler(Directory.GetCurrentDirectory());

			List<(int hour, int minute)> restartData = new List<(int hour, int minute)>();
			restartData.Add((6, 00));
			restartData.Add((12, 00));
			restartData.Add((18, 00));
			restartData.Add((00, 00));

			jsonHandler.SetRestartData(restartData);

			restartData = jsonHandler.GetRestartData();

			foreach ((int hour, int minute) data in restartData)
			{
				Console.WriteLine("Time - " + data.hour + ":" + data.minute);
			}
		}
	}
}