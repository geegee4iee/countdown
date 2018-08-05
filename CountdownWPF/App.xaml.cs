using Countdown.Core.Infrastructure;
using Countdown.Core.Models;
using CountdownWPF.Infrastructure;
using CountdownWPF.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CountdownWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var currentProcess = Process.GetCurrentProcess();
            if (Process.GetProcessesByName(currentProcess.ProcessName).Length > 1)
            {
                Application.Current.Shutdown();
                return;
            }

            ServiceLocator.Setup.RegisterAssemblyForType<IAppUsageRecordRepository>(ConfigurationManager.AppSettings["ImplementedRepositoryAssembly"]).AsSingleTon();
            ServiceLocator.Setup.RegisterAssemblyForType<IAppUsageRecordRepository, LocalAppUsageRecordRepository>().AsSingleTon("LocalRepository");

            base.OnStartup(e);

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(HandleUncaughtedException);
        }

        private void HandleUncaughtedException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = (Exception) e.ExceptionObject;

            StringBuilder msgs = new StringBuilder();

            do
            {
                msgs.Append(exception.Message + "\r\n");
                LoggingService.Log(exception.Message + "\r\n" + exception.StackTrace);
            } while ((exception = exception.InnerException) != null);

            MessageBox.Show(msgs.ToString());
        }
    }
}
