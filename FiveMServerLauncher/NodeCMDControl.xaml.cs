using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace FiveMServerLauncher
{
	/// <summary>
	/// Interaction logic for RestartControl.xaml
	/// </summary>
	public partial class NodeCMDControl : System.Windows.Controls.UserControl
	{
		private readonly MainWindow MainWindow;
		private readonly NodeCMD nodeCMD;
		private readonly JSONHandler jsonHandler;

		public NodeCMDControl(MainWindow mw, int count, NodeCMD nCMD, JSONHandler jHandler)
		{
			InitializeComponent();

			MainWindow = mw;
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
			MainWindow.UpdateUI();
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

		private void ButtonBrowseDir_Click(object sender, RoutedEventArgs e)
		{
			using (var dialog = new FolderBrowserDialog())
			{
				DialogResult result = dialog.ShowDialog();
				nodeCMD.Directory = dialog.SelectedPath;
				textBoxDir.Text = dialog.SelectedPath;
			}
		}
	}
}