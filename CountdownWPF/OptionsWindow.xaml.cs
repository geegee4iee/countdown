using CountdownWPF.Configurations;
using CountdownWPF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CountdownWPF
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        public OptionsWindow()
        {
            InitializeComponent();

            txtBlockUrlList.Text = Settings.RuntimeConfigs.GetBlockedUrls().Aggregate((result, value) => result + "\r\n" + value);

            var registryKey = GetCurrentUserStartupRegistryKey();

            var startupSetting = registryKey.GetValue(Assembly.GetExecutingAssembly().GetName().Name);
            if (startupSetting != null)
            {
                runOnStartupCheckBox.IsChecked = true;
            }
        }

        private void btnUpdateBlockList_Click(object sender, RoutedEventArgs e)
        {
            string[] content = txtBlockUrlList.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            Settings.RuntimeConfigs.UpdateBlockedUrls(content);
            ChromeTabBlockingService.Instance.RefreshBlockingUrls();
        }

        private void runOnStartupCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.RegistryKey key = GetCurrentUserStartupRegistryKey();
            Assembly curAssembly = Assembly.GetExecutingAssembly();
            key.SetValue(curAssembly.GetName().Name, curAssembly.Location);
        }

        private void runOnStartupCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.RegistryKey key = GetCurrentUserStartupRegistryKey();
            Assembly curAssembly = Assembly.GetExecutingAssembly();
            key.DeleteValue(curAssembly.GetName().Name);
        }

        private Microsoft.Win32.RegistryKey GetCurrentUserStartupRegistryKey()
        {
            return Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        }
    }
}
