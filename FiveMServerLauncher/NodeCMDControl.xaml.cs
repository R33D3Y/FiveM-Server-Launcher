using System;
using System.Windows;
using System.Windows.Controls;

namespace FiveMServerLauncher
{
	/// <summary>
	/// Interaction logic for RestartControl.xaml
	/// </summary>
	public partial class NodeCMDControl : UserControl
	{
		private readonly NodeCMD nodeCMD;
		private readonly JSONHandler jsonHandler;

		public NodeCMDControl(int count, NodeCMD nCMD, JSONHandler jHandler)
		{
			InitializeComponent();

			nodeCMD = nCMD;
			jsonHandler = jHandler;

			label.Content = "CMD Command " + count;
			textBoxDir.Text = nCMD.Directory;
			textBoxArg.Text = nCMD.Arguments;
			checkBox.IsChecked = nCMD.Enabled;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			jsonHandler.NodeCMDInformation.Data.Remove(nodeCMD);
			jsonHandler.UpdateJSON();
			jsonHandler.UpdateUI = true;
		}

		private void CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			nodeCMD.Enabled = (bool)checkBox.IsChecked;
			jsonHandler.UpdateJSON();
		}

		private void TextBoxDir_LostFocus(object sender, RoutedEventArgs e)
		{
			nodeCMD.Directory = textBoxDir.Text;
			jsonHandler.UpdateJSON();
		}

		private void TextBoxArg_LostFocus(object sender, RoutedEventArgs e)
		{
			nodeCMD.Arguments = textBoxArg.Text;
			jsonHandler.UpdateJSON();
		}
	}
}