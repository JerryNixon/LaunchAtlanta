using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Graphics.Display;
using Windows.Media.Capture;
using Windows.System.Display;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LaunchApp.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            PreviewControl.FaceDetected += PreviewControl_FaceDetected;
            Loaded += MainPage_Loaded;
        }

        DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
        private void MainPage_Loaded(Object sender, RoutedEventArgs e)
        {
            timer.Tick += (s, args) =>
            {
                if (VisualStateGroup.CurrentState == VisualStateAdmin)
                {
                    return;
                }
                if (timer.IsEnabled)
                {
                    timer.Stop();
                }
                if (VisualStateGroup.CurrentState != VisualStateNormal)
                {
                    VisualStateManager.GoToState(this, VisualStateIdle.Name, true);
                }
            };
        }

        private void PreviewControl_FaceDetected(Object sender, CameraControl.FaceDetectedEventArgsEx e)
        {
            if (VisualStateGroup.CurrentState == VisualStateAdmin)
            {
                return;
            }
            if (e.Faces.Any() && VisualStateGroup.CurrentState != VisualStateNormal)
            {
                if (timer.IsEnabled)
                {
                    timer.Stop();
                }
                VisualStateManager.GoToState(this, VisualStateNormal.Name, true);
            }
            else
            {
                if (!timer.IsEnabled)
                {
                    timer.Start();
                }
            }
        }

        private void BackgroundList_ItemClick(Object sender, ItemClickEventArgs e)
        {
            // TODO
        }

        private void SettingsButton_Click(Object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, VisualStateAdmin.Name, true);
        }
    }
}
