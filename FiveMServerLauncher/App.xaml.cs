using System.Windows;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace FiveMServerLauncher
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			AppCenter.Start(AppToken.ID, typeof(Analytics), typeof(Crashes));
			AppCenter.SetEnabledAsync(true);
		}
	}
}