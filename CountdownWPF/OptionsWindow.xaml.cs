using CountdownWPF.Configurations;
using CountdownWPF.Services;
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
        }

        private void btnUpdateBlockList_Click(object sender, RoutedEventArgs e)
        {
            string[] content = txtBlockUrlList.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            Settings.RuntimeConfigs.UpdateBlockedUrls(content);
            ChromeTabBlockingService.Instance.RefreshBlockingUrls();
        }
    }
}
