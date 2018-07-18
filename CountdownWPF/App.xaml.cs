﻿using Countdown.Core.Infrastructure;
using Countdown.Core.Models;
using CountdownWPF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
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
        }
    }
}
