using System;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MVVMContactList.Common;

namespace MVVMContactList.Views
{
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            Window.Current.SetTitleBar(CustomTitleBar);
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Windows.UI.Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Windows.UI.Colors.Transparent;
        }

        private void LoginWebView_OnNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            var authUri = args.Uri.ToString();
            if (authUri.Contains("access_token="))
            {
                var accessToken = authUri.Split("access_token=")[1].Split("&")[0];
                Console.WriteLine($"Token is: {accessToken}");

                VkObjects.InitializeApi(accessToken);
                Frame.Navigate(typeof(MainPage));
            }
        }

        private void LoginWebView_NavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            Console.WriteLine("No connection");
            Frame.Navigate(typeof(MainPage));
        }

        private void LoginWebView_OnLoaded(object sender, RoutedEventArgs e)
        {
            LoginWebView.Navigate(new Uri(
                $"https://oauth.vk.com/authorize?client_id={VkConstants.AppId}&display=page&scope={VkConstants.AppScope}&redirect_uri=https://oauth.vk.com/blank.html&response_type=token&v=5.103&state=success"
            ));
        }
    }
}