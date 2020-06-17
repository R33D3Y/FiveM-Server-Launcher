using System;
using System.Collections.Generic;
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
	/// Interaction logic for RestartControl.xaml
	/// </summary>
	public partial class RestartControl : UserControl
	{
		private int scheduleCount;
		private RestartData restartData;

		public RestartControl(int count, RestartData rData)
		{
			InitializeComponent();
			scheduleCount = count;
			restartData = rData;

			label.Content = "Schedule " + count;

			for (int i = 0; i < 24; i++)
			{
				comboBoxHour.Items.Add(i);
			}

			for (int i = 0; i < 59; i++)
			{
				comboBoxMinute.Items.Add(i);
				comboBoxWarningMinute.Items.Add(i);
			}

			comboBoxHour.SelectedIndex = restartData.Data[scheduleCount].RestartHour;
			comboBoxMinute.SelectedIndex = restartData.Data[scheduleCount].RestartMinute;

			switch (restartData.Data[scheduleCount].RestartType)
			{
				case RestartType.Restart:
					radioButtonRestart.IsChecked = true;
					break;
				case RestartType.Stop:
					radioButtonStop.IsChecked = true;
					break;
				case RestartType.Start:
					radioButtonStart.IsChecked = true;
					break;
			}

			comboBoxWarningMinute.SelectedIndex = restartData.Data[scheduleCount].MinuteWarning;
		}
	}
}