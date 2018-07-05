using Microsoft.QueryStringDotNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Countdown
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            _tileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
            _tileUpdater.EnableNotificationQueue(true);

            HashSet<string> taskNames = new HashSet<string>();
            taskNames.Add("My Background Trigger");
            taskNames.Add("CountdownBackgroundTasks.ExampleBackgroundTask");
            taskNames.Add("ToastBackgroundTask");
            taskNames.Add("CountdownBackgroundTasks.MyExampleBackgroundTask");

            var tasks = BackgroundTaskRegistration.AllTasks.ToList();
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (taskNames.Contains(task.Value.Name))
                {
                    task.Value.Unregister(true);
                }
            }

            RegisterBackgroundTask("CountdownBackgroundTasks.ExampleBackgroundTask", new SystemTrigger(SystemTriggerType.TimeZoneChange, false), "CountdownBackgroundTasks.ExampleBackgroundTask");
            RegisterBackgroundTask("My Background Trigger", new TimeTrigger(15, false));
            RegisterBackgroundTask("ToastBackgroundTask", new ToastNotificationActionTrigger());
        }

        private async Task<BackgroundTaskRegistration> RegisterBackgroundTask(string taskName, IBackgroundTrigger trigger, string taskEntry = null)
        {
            if (BackgroundTaskRegistration.AllTasks.Any(i => i.Value.Name == taskName)) return null;

            BackgroundAccessStatus status = await BackgroundExecutionManager.RequestAccessAsync();

            BackgroundTaskBuilder builder = new BackgroundTaskBuilder()
            {
                Name = taskName,

            };

            if (taskEntry != null)
            {
                builder.TaskEntryPoint = taskEntry;
            }

            builder.SetTrigger(trigger);

            BackgroundTaskRegistration registration = builder.Register();

            return registration;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e?.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e?.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e?.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        protected override void OnActivated(IActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            

            if (e is IToastNotificationActivatedEventArgs)
            {
                var toastActivationArgs = e as ToastNotificationActivatedEventArgs;

                QueryString args = QueryString.Parse(toastActivationArgs.Argument);

                if (rootFrame.BackStack.Count == 0)
                {
                    rootFrame.BackStack.Add(new PageStackEntry(typeof(MainPage), null, null));
                }
            }

            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            base.OnBackgroundActivated(args);

            var deferral = args.TaskInstance.GetDeferral();

            switch(args.TaskInstance.Task.Name)
            {
                case "ToastBackgroundTask":
                    var details = args.TaskInstance.TriggerDetails as ToastNotificationActionTriggerDetail;
                    if (details != null)
                    {
                        string arguments = details.Argument;
                        var userInput = details.UserInput;
                    }
                    break;
                case "My Background Trigger":
                    NotifyTiles();
                    break;
            }

            deferral.Complete();
        }

        private TileUpdater _tileUpdater;
        private void NotifyTiles()
        {
            var notification = new TileNotification(new CountdownTile().CreateTileContent().GetXml());
            notification.ExpirationTime = DateTimeOffset.UtcNow.AddMinutes(10);

            _tileUpdater.Update(notification);

            if (SecondaryTile.Exists("Countdown"))
            {
                var updater = TileUpdateManager.CreateTileUpdaterForSecondaryTile("Countdown");

                updater.Update(notification);
            }
        }

        public void RunUpdateCountdownTileBackgroundTask()
        {
            BackgroundTaskRegistration taskRegistration = null;
            try
            {
                var timeTrigger = new TimeTrigger(15, false);
                var builder = new BackgroundTaskBuilder();
                builder.Name = "My Background Trigger";
                builder.SetTrigger(timeTrigger);
                builder.AddCondition(new SystemCondition(SystemConditionType.UserPresent));
                taskRegistration = builder.Register();
            }
            catch (Exception e)
            {
                if (taskRegistration != null)
                {
                    taskRegistration.Unregister(true);
                }

                throw e;
            }
            finally
            {
                
            }

        }
    }
}
