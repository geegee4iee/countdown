using Countdown.Utils;
using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.System;
using Windows.UI.Notifications;

namespace Countdown
{
    public class CountdownToastBuilder
    {
        public static async Task<ToastNotification> GetToastNotification()
        {
            var users = await User.FindAllAsync(UserType.LocalUser);
            var user = users.FirstOrDefault();
            var properties = await user.GetPropertiesAsync(new string[] { KnownUserProperties.FirstName, KnownUserProperties.LastName });
            string title = $"Hello {properties[KnownUserProperties.FirstName]}, {properties[KnownUserProperties.LastName]}!";
            string content = $"Remaining {TimeCalculatorUtils.GetRemainingTimeFromNowToTheEndOfDay().ToShortTime()}";
            string logo = "Assets/SmallTile.scale-100.png";
            ToastVisual visual = new ToastVisual()
            {
                BindingGeneric = new ToastBindingGeneric()
                {
                    Children =
                    {
                        new AdaptiveText()
                        {
                            Text = title
                        },

                        new AdaptiveText()
                        {
                            Text = content
                        }
                    },

                    AppLogoOverride = new ToastGenericAppLogo()
                    {
                        Source = logo,
                        HintCrop = ToastGenericAppLogoCrop.Circle
                    }
                },
            };

            int conversationId = 384928;

            ToastContent toastContent = new ToastContent()
            {
                Visual = visual,

                Launch = new QueryString()
                {
                    {"action", "viewConversation" },
                    {"conversationId", conversationId.ToString() }
                }.ToString()
            };

            var toast = new ToastNotification(toastContent.GetXml());
            toast.ExpirationTime = DateTime.Now.AddMinutes(25);
            return toast;
        }
    }
}
