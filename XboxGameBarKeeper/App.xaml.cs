using Microsoft.Gaming.XboxGameBar;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace XboxGameBarKeeper
{
    sealed partial class App : Application
    {
        private XboxGameBarWidget _root = null;
        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            XboxGameBarWidgetActivatedEventArgs widgetArgs = null;

            if (args.Kind == ActivationKind.Protocol)
            {
                var protocolArgs = args as IProtocolActivatedEventArgs;
                string protocolScheme = protocolArgs.Uri.Scheme;

                if (protocolScheme.Equals("ms-gamebarwidget"))
                {
                    widgetArgs = args as XboxGameBarWidgetActivatedEventArgs;
                }
            }
            if (widgetArgs != null)
            {
                if (widgetArgs.IsLaunchActivation)
                {
                    var rootFrame = new Frame();
                    Window.Current.Content = rootFrame;
                    _root = new XboxGameBarWidget(widgetArgs, Window.Current.CoreWindow, rootFrame);
                    rootFrame.Navigate(typeof(MainPage));
                    Window.Current.Closed += RootClose;
                    Window.Current.Activate();
                }
            }
        }

        private void RootClose(object sender, Windows.UI.Core.CoreWindowEventArgs e)
        {
            _root = null;
            Window.Current.Closed -= RootClose;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
            {
                rootFrame = new Frame();
                Window.Current.Content = rootFrame;
            }
            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                Window.Current.Activate();
            }
        }
    }
}
