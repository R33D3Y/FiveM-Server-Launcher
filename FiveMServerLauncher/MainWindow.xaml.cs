using Microsoft.AppCenter.Analytics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace FiveMServerLauncher
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly JSONHandler jsonHandler;
		private readonly MySQLHandler mySQLHandler;

		private readonly RestartInformation restartInformation;
		private readonly NodeCMDInformation nodeCMDInformation;
		private readonly SQLBackup sqlBackup;

		private readonly DispatcherTimer UIUpdater = new DispatcherTimer();
		private readonly DispatcherTimer RestartScheduler = new DispatcherTimer();

		private Process serverProcess;
		private readonly List<Process> nodeProcesses = new List<Process>();

		private readonly List<ResourceManagerControl> resourceManagerControls = new List<ResourceManagerControl>();

		private string logFileLocation = "";
		private readonly bool closing = false;

		public MainWindow()
		{
			InitializeComponent();

			jsonHandler = new JSONHandler(Directory.GetCurrentDirectory());

			jsonHandler.SQLBackupUpdate();
			jsonHandler.RestartDataUpdate();
			jsonHandler.NodeCMDUpdate();

			sqlBackup = jsonHandler.SQLBackup;
			restartInformation = jsonHandler.RestartInformation;
			nodeCMDInformation = jsonHandler.NodeCMDInformation;

			mySQLHandler = new MySQLHandler(sqlBackup.Host,sqlBackup.DatabaseName, sqlBackup.User, sqlBackup.Password);

			while (jsonHandler.GetServerDirectory() == null || jsonHandler.GetServerDirectory() == "")
			{
				MessageBoxResult msgResult = System.Windows.MessageBox.Show("Please Select The Server Directory", "FiveM Server Launcher", MessageBoxButton.OKCancel, (MessageBoxImage)MessageBoxIcon.Error);

				if (msgResult == MessageBoxResult.Cancel)
				{
					closing = true;
					Close();
					break;
				}

				using (var dialog = new FolderBrowserDialog())
				{
					DialogResult result = dialog.ShowDialog();
					jsonHandler.SetServerDirectory(dialog.SelectedPath);
				}
			}

			if (!closing)
			{
				while (jsonHandler.GetServerConfigDirectory() == null || jsonHandler.GetServerConfigDirectory() == "")
				{
					MessageBoxResult msgResult = System.Windows.MessageBox.Show("Please Select The Server Config Directory", "FiveM Server Launcher", MessageBoxButton.OKCancel, (MessageBoxImage)MessageBoxIcon.Error);

					if (msgResult == MessageBoxResult.Cancel)
					{
						closing = true;
						Close();
						break;
					}

					using (var dialog = new OpenFileDialog())
					{
						DialogResult result = dialog.ShowDialog();
						jsonHandler.SetServerConfigDirectory(dialog.FileName);
					}
				}

				CreateSQLBackupControl();
				CreateRestartControls();
				CreateNodeCMDControls();

				UIUpdater.Interval = TimeSpan.FromSeconds(1);
				UIUpdater.Tick += UIUpdater_Tick;
				UIUpdater.Start();

				RestartScheduler.Interval = TimeSpan.FromSeconds(60);
				RestartScheduler.Tick += RestartScheduler_Tick;
				RestartScheduler.Start();

				Stop();

				versionLabel.Content = "Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
			}
		}

		private void CreateSQLBackupControl()
		{
			checkBoxSQLBackUp.IsChecked = sqlBackup.Enabled;
			textBoxSQLDumpDir.Text = sqlBackup.DumpDirectory;
			textBoxDatabase.Text = sqlBackup.DatabaseName;
			textBoxHost.Text = sqlBackup.Host;
			textBoxUser.Text = sqlBackup.User;
			textBoxPassword.Password = sqlBackup.Password;
			textBoxBackUpDir.Text = sqlBackup.BackupDirectory;
		}

		private void CreateRestartControls()
		{
			int count = 0;
			checkBoxRestartSchedule.IsChecked = jsonHandler.RestartInformation.Enabled;
			RestartControlPanel.Children.Clear();

			foreach (RestartData restartData in restartInformation.Data)
			{
				RestartControlPanel.Children.Add(new RestartControl(count, restartInformation.Data[count], jsonHandler));
				count++;
			}
		}

		private void CreateNodeCMDControls()
		{
			int count = 0;
			CMDNodeControlPanel.Children.Clear();

			foreach (NodeCMD nodeCMD in nodeCMDInformation.Data)
			{
				CMDNodeControlPanel.Children.Add(new NodeCMDControl(count, nodeCMDInformation.Data[count], jsonHandler));
				count++;
			}
		}

		#region Server

		private void Start()
		{
			DecideLogLocation();
			ResetConsole();
			nodeProcesses.Clear();
			resourceManagerControls.Clear();
			stackResourceManagement.Children.Clear();

			serverProcess = new Process();

			string file = jsonHandler.GetServerDirectory() + @"\run.cmd";
			ProcessStartInfo startFiveMInfo = new ProcessStartInfo(file)
			{
				WorkingDirectory = jsonHandler.GetServerDirectory(),
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardInput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				Arguments = "+exec " + jsonHandler.GetServerConfigDirectory()
			};

			serverProcess.StartInfo = startFiveMInfo;
			serverProcess.OutputDataReceived += (s, a) => Output_Data(s, a);
			serverProcess.Start();
			serverProcess.BeginOutputReadLine();

			foreach (NodeCMD nodeCMD in nodeCMDInformation.Data)
			{
				Process temp = new Process();
				ProcessStartInfo tempInfo = new ProcessStartInfo("cmd.exe")
				{
					CreateNoWindow = true,
					UseShellExecute = false,
					Arguments = @"/C cd " + nodeCMD.Directory + @" && " + nodeCMD.Arguments
				};

				temp.StartInfo = tempInfo;
				temp.Start();

				nodeProcesses.Add(temp);
			}
		}

		private void Restart()
		{
			Stop();
			Start();
		}

		private void Stop()
		{
			if (serverProcess != null && !serverProcess.HasExited)
			{
				serverProcess.Kill();
				serverProcess = null;
			}

			foreach (Process process in nodeProcesses)
			{
				try
				{
					if (!process.HasExited)
					{
						process.Kill();
					}
				}
				catch (Exception) { }
			}

			foreach (var process in Process.GetProcessesByName("FXServer"))
			{
				process.Kill();
			}

			foreach (var process in Process.GetProcessesByName("node"))
			{
				process.Kill();
			}

			try
			{
				DirectoryInfo di = new DirectoryInfo(jsonHandler.GetServerDirectory() + @"\cache");

				foreach (FileInfo file in di.GetFiles())
				{
					file.Delete();
				}

				foreach (DirectoryInfo dir in di.GetDirectories())
				{
					dir.Delete(true);
				}
			}
			catch (Exception) { }

			RunSQLBackup();
		}

		private void Output_Data(object sender, DataReceivedEventArgs a)
		{
			UpdateConsole(a.Data);
		}

		private void DecideLogLocation()
		{
			FileInfo toReplace = null;
			FileInfo[] infos = new DirectoryInfo(jsonHandler.LogDirectory).GetFiles();
			int increment = 0;
			int fileCount = infos.Select(x => x.Name.Contains("logFile_")).Count();

			if (fileCount < 10)
			{
				increment = fileCount;
				logFileLocation = jsonHandler.LogDirectory + @"logFile_" + increment + ".txt";
			}
			else
			{
				foreach (FileInfo fileInfo in infos)
				{
					if (fileInfo.Name.Contains("logFile_") && fileInfo.Extension == ".txt")
					{
						if (toReplace == null)
						{
							toReplace = fileInfo;
						}
						else if (fileInfo.LastWriteTime < toReplace.LastWriteTime)
						{
							toReplace = fileInfo;
						}
					}
				}

				logFileLocation = jsonHandler.LogDirectory + toReplace.Name;
			}

			File.WriteAllText(logFileLocation, "");
		}

		private void Log(string output)
		{
			try
			{
				File.AppendAllText(logFileLocation, output + "\n");
			}
			catch { }
		}

		#region Console Methods

		private void ResetConsole()
		{
			consoleTextbox.Document.Blocks.Clear();
		}

		private void Append_Text(string str)
		{
			Dispatcher.Invoke(() =>
			{
				if (consoleTextbox.Document.Blocks.Count >= 200)
				{
					consoleTextbox.Document.Blocks.Remove(consoleTextbox.Document.Blocks.FirstBlock);
				}
			});

			Dispatcher.Invoke(() =>
			{
				TextRange tr = new TextRange(consoleTextbox.Document.ContentEnd, consoleTextbox.Document.ContentEnd)
				{
					Text = RemoveColourNotifier(str) + "\n"
				};

				tr.ApplyPropertyValue(TextElement.ForegroundProperty, DecideColour(str.ToLower()));
			});

			string[] split = str.Split(' ');

			if (split.Length > 3)
			{
				if (str.Contains("Found new resource"))
				{
					ControlResourceManager(split[4], false);
				}
				else if (str.Contains("Started"))
				{
					ControlResourceManager(split[3], true);
				}
				else if (str.Contains("Stopping"))
				{
					ControlResourceManager(split[3], false);
				}
			}
		}

		private void ControlResourceManager(string str, bool run)
		{
			bool found = false;

			foreach (ResourceManagerControl rmc in resourceManagerControls)
			{
				if (rmc.ResourceName == str)
				{
					found = true;

					System.Windows.Application.Current.Dispatcher.Invoke(delegate
					{
						rmc.SetRunning(run);
					});
					
					break;
				}
			}

			if (!found)
			{
				System.Windows.Application.Current.Dispatcher.Invoke(delegate
				{
					ResourceManagerControl rmc = new ResourceManagerControl(this, str);
					stackResourceManagement.Children.Add(rmc);
					resourceManagerControls.Add(rmc);
				});
			}
		}

		private void UpdateConsole(string output)
		{
			if (output != null)
			{
				output = "[" + DateTime.Now.ToShortTimeString() + "] " + output;

				Append_Text(output);
				Log(RemoveColourNotifier(output));
			}
		}

		private SolidColorBrush DecideColour(string str)
		{
			if (str.Contains("found new resource") || str.Contains("creating script") || str.Contains("started resource") || str.Contains("instantiated instance") || str.Contains("successfully") || str.Contains("succeed") || str.Contains("save") || str.Contains("[92m") || str.Contains("[32m"))
			{
				return Brushes.Green;
			}
			else if (str.Contains("stopping resource") || str.Contains("warning"))
			{
				return Brushes.Orange;
			}
			else if (str.Contains("couldn't") || str.Contains("error") || str.Contains("fail") || str.Contains("[91m") || str.Contains("[31m"))
			{
				return Brushes.Red;
			}
			else if (str.Contains("[93m") || str.Contains("[33m"))
			{
				return Brushes.Yellow;
			}
			else if (str.Contains("[94m") || str.Contains("[34m"))
			{
				return Brushes.Blue;
			}
			else if (str.Contains("[95m") || str.Contains("[35m"))
			{
				return Brushes.Magenta;
			}
			else if (str.Contains("[96m") || str.Contains("[36m"))
			{
				return Brushes.Cyan;
			}

			return Brushes.White;
		}

		private string RemoveColourNotifier(string str)
		{
			string s = str;
			return s.Replace("[0m", "").Replace("[91m", "").Replace("[92m", "").Replace("[93m", "").Replace("[94m", "").Replace("[95m", "").Replace("[96m", "").Replace("[97m", "").Replace("[31m", "").Replace("[32m", "").Replace("[33m", "").Replace("[34m", "").Replace("[35m", "").Replace("[36m", "").Replace("[30m", "").Replace("[90m", "").Replace("", "");
		}

		#endregion Console Methods

		private void RunSQLBackup()
		{
			string folder = jsonHandler.SQLBackup.BackupDirectory + @"\" + DateTime.Now.ToString("MM-dd-yyyy");

			if (!Directory.Exists(folder))
			{
				Directory.CreateDirectory(folder);
			}

			string sqlBackupArgument = @"/C " + jsonHandler.SQLBackup.DumpDirectory + " " + jsonHandler.SQLBackup.DatabaseName + " -h " + jsonHandler.SQLBackup.Host + " -u " + jsonHandler.SQLBackup.User;

			if (sqlBackup.Password != null && sqlBackup.Password != "")
			{
				sqlBackupArgument += " -p " + jsonHandler.SQLBackup.Password;
			}

			sqlBackupArgument += " > " + folder + @"\" + DateTime.Now.ToString("HH-mm-ss") + ".sql";

			Process sqlBackupProcess = new Process();
			ProcessStartInfo sqlBackupInfo = new ProcessStartInfo("cmd.exe")
			{
				CreateNoWindow = true,
				UseShellExecute = false,
				Arguments = sqlBackupArgument
			};

			sqlBackupProcess.StartInfo = sqlBackupInfo;
			sqlBackupProcess.Start();
		}

		internal void SendToConsole(string v)
		{
			serverProcess.StandardInput.WriteLine(v);
		}

		#endregion Server

		#region Event Handlers

		private void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				DragMove();
			}
		}

		#region Dispatch Ticks

		private void UIUpdater_Tick(object sender, EventArgs e)
		{
			if (jsonHandler.UpdateUI)
			{
				CreateRestartControls();
				CreateNodeCMDControls();
				jsonHandler.UpdateUI = false;
			}
		}

		private void RestartScheduler_Tick(object sender, EventArgs e)
		{
			if (jsonHandler.RestartInformation.Enabled)
			{
				foreach (RestartData restartData in restartInformation.Data)
				{
					if (DateTime.Now.Hour == restartData.RestartHour && DateTime.Now.Minute == restartData.RestartMinute)
					{
						RestartStopStart(restartData);
					}
					else
					{
						if ((restartData.RestartMinute - restartData.MinuteWarning) < 0)
						{
							if (DateTime.Now.Hour == (restartData.RestartHour - 1) && DateTime.Now.Minute == (60 - restartData.MinuteWarning))
							{
								RestartStopStartWarning(restartData);
							}
						}
						else if (DateTime.Now.Hour == restartData.RestartHour && DateTime.Now.Minute == (restartData.RestartMinute - restartData.MinuteWarning))
						{
							RestartStopStartWarning(restartData);
						}
					}
				}
			}
		}

		private void RestartStopStart(RestartData restartData)
		{
			switch (restartData.RestartType)
			{
				case RestartType.Restart:
					Restart();
					break;
				case RestartType.Start:
					Start();
					break;
				case RestartType.Stop:
					Stop();
					break;
			}
		}

		private void RestartStopStartWarning(RestartData restartData)
		{
			for (int i = 0; i < 5; i++)
			{
				serverProcess.StandardInput.WriteLine(@"say ====| " + restartData.MinuteWarning + @" Minutes Till An Automated Server " + restartData.RestartType + "@! |====");
			}
		}

		#endregion Dispatch Ticks

		private void ButtonAddRestartSchedule_Click(object sender, RoutedEventArgs e)
		{
			jsonHandler.AddRestartData(new RestartData(0, 0, RestartType.Restart, 0));
			CreateRestartControls();
		}

		private void CheckBoxRestartSchedule_Checked(object sender, RoutedEventArgs e)
		{
			jsonHandler.RestartInformation.Enabled = (bool)checkBoxRestartSchedule.IsChecked;
			jsonHandler.UpdateJSON();
		}

		private void ConsoleTextbox_TextChanged(object sender, TextChangedEventArgs e)
		{
			consoleTextbox.ScrollToEnd();
		}

		private void InputText_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				if (serverProcess != null)
				{
					serverProcess.StandardInput.WriteLine(inputText.Text);
					Analytics.TrackEvent("Input Commands: " + inputText.Text);
					inputText.Text = "";
				}
			}
		}

		private void SubmitInput_Click(object sender, RoutedEventArgs e)
		{
			if (serverProcess != null)
			{
				serverProcess.StandardInput.WriteLine(inputText.Text);
				Analytics.TrackEvent("Input Commands: " + inputText.Text);
				inputText.Text = "";
			}
		}

		private void StartServer_Click(object sender, RoutedEventArgs e)
		{
			Start();
		}

		private void RestartServer_Click(object sender, RoutedEventArgs e)
		{
			Restart();
		}

		private void StopServer_Click(object sender, RoutedEventArgs e)
		{
			Stop();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (serverProcess != null)
			{
				MessageBoxResult msgResult = System.Windows.MessageBox.Show("The Server Is Still Running, Are You Sure?", "FiveM Server Launcher", MessageBoxButton.YesNo, (MessageBoxImage)MessageBoxIcon.Error);

				if (msgResult == MessageBoxResult.Yes)
				{
					Stop();
				}
				else
				{
					e.Cancel = true;
				}
			}
		}

		private void ButtonMinimise_Click(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}

		private void ButtonClose_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void ButtonAddCMDNode_Click(object sender, RoutedEventArgs e)
		{
			jsonHandler.NodeCMDInformation.Data.Add(new NodeCMD(true, "", ""));
			CreateRestartControls();
			CreateNodeCMDControls();
		}

		#region Sliding Panel

		private void BtnLeftMenuHide_Click(object sender, RoutedEventArgs e)
		{
			ShowHideMenu("sbHideLeftMenu", btnLeftMenuHide, btnLeftMenuShow, pnlLeftMenu);
		}

		private void BtnLeftMenuShow_Click(object sender, RoutedEventArgs e)
		{
			ShowHideMenu("sbShowLeftMenu", btnLeftMenuHide, btnLeftMenuShow, pnlLeftMenu);
			Analytics.TrackEvent("Restart Schedule & CMD/Node");
		}

		private void BtnRightMenuHide_Click(object sender, RoutedEventArgs e)
		{
			ShowHideMenu("sbHideRightMenu", btnRightMenuHide, btnRightMenuShow, pnlRightMenu);
		}

		private void BtnRightMenuShow_Click(object sender, RoutedEventArgs e)
		{
			ShowHideMenu("sbShowRightMenu", btnRightMenuHide, btnRightMenuShow, pnlRightMenu);
			Analytics.TrackEvent("SQL Data");
		}

		private void ShowHideMenu(string board, System.Windows.Controls.Button btnHide, System.Windows.Controls.Button btnShow, StackPanel pnl)
		{
			var sb = Resources[board] as Storyboard;
			sb.Begin(pnl);

			if (board.Contains("Show"))
			{
				btnHide.Visibility = Visibility.Visible;
				btnShow.Visibility = Visibility.Hidden;
			}
			else if (board.Contains("Hide"))
			{
				btnHide.Visibility = Visibility.Hidden;
				btnShow.Visibility = Visibility.Visible;
			}
		}

		#endregion Sliding Panel

		#region SQL Backup

		private void TextBoxSQLDumpDir_LostFocus(object sender, RoutedEventArgs e)
		{
			sqlBackup.DumpDirectory = textBoxSQLDumpDir.Text;
			jsonHandler.UpdateJSON();
		}

		private void TextBoxDatabase_LostFocus(object sender, RoutedEventArgs e)
		{
			sqlBackup.DatabaseName = textBoxDatabase.Text;
			jsonHandler.UpdateJSON();
		}

		private void TextBoxHost_LostFocus(object sender, RoutedEventArgs e)
		{
			sqlBackup.Host = textBoxHost.Text;
			jsonHandler.UpdateJSON();
		}

		private void TextBoxUser_LostFocus(object sender, RoutedEventArgs e)
		{
			sqlBackup.User = textBoxUser.Text;
			jsonHandler.UpdateJSON();
		}

		private void TextBoxPassword_LostFocus(object sender, RoutedEventArgs e)
		{
			sqlBackup.Password = textBoxPassword.Password;
			jsonHandler.UpdateJSON();
		}

		private void TextBoxBackUpDir_LostFocus(object sender, RoutedEventArgs e)
		{
			sqlBackup.BackupDirectory = textBoxBackUpDir.Text;
			jsonHandler.UpdateJSON();
		}

		private void CheckBoxSQLBackUp_Checked(object sender, RoutedEventArgs e)
		{
			sqlBackup.Enabled = (bool)checkBoxSQLBackUp.IsChecked;
			jsonHandler.UpdateJSON();
		}

		private void ButtonManualSQLBackup_Click(object sender, RoutedEventArgs e)
		{
			RunSQLBackup();
		}

		#endregion SQL Backup

		#endregion Event Handlers
	}
}