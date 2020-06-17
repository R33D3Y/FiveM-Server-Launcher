using System.Windows;
using System.Windows.Controls;

namespace FiveMServerLauncher
{
	/// <summary>
	/// Interaction logic for RestartControl.xaml
	/// </summary>
	public partial class RestartControl : UserControl
	{
		private readonly RestartData restartData;
		private readonly JSONHandler jsonHandler;

		public RestartControl(int count, RestartData rData, JSONHandler jHandler)
		{
			InitializeComponent();
			restartData = rData;
			jsonHandler = jHandler;

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

			comboBoxHour.SelectedIndex = restartData.RestartHour;
			comboBoxMinute.SelectedIndex = restartData.RestartMinute;

			switch (restartData.RestartType)
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

			comboBoxWarningMinute.SelectedIndex = restartData.MinuteWarning;
		}

		private void ComboBoxHour_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			restartData.RestartHour = comboBoxHour.SelectedIndex;
			jsonHandler.UpdateJSON();
		}

		private void ComboBoxMinute_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			restartData.RestartMinute = comboBoxMinute.SelectedIndex;
			jsonHandler.UpdateJSON();
		}

		private void RadioButton_Checked(object sender, RoutedEventArgs e)
		{
			if ((bool)radioButtonStart.IsChecked)
			{
				restartData.RestartType = RestartType.Start;
			}
			else if ((bool)radioButtonRestart.IsChecked)
			{
				restartData.RestartType = RestartType.Restart;
			}
			else
			{
				restartData.RestartType = RestartType.Stop;
			}

			jsonHandler.UpdateJSON();
		}

		private void ComboBoxWarningMinute_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			restartData.MinuteWarning = comboBoxWarningMinute.SelectedIndex;
			jsonHandler.UpdateJSON();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			jsonHandler.RestartInformation.Data.Remove(restartData);
			jsonHandler.UpdateJSON();
			jsonHandler.UpdateUI = true;
		}
	}
}