using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Twedge
{
    public partial class App : Application
    {

        public App()
        {
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {

            //string culture = e.InitParams.ContainsKey("culture") ? e.InitParams["width"] : "sl-SI";

            //Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture; // new CultureInfo("sl-SI");
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("sl-SI");

            //int width = e.InitParams.ContainsKey("width") ? int.Parse(e.InitParams["width"]) : 200;
            //int height = e.InitParams.ContainsKey("height") ? int.Parse(e.InitParams["width"]) : 300;

            string backColor = e.InitParams.ContainsKey("backgroundColor")
                                   ? e.InitParams["backgroundColor"]
                                   : "#1c3546";
            string alternatingColor = e.InitParams.ContainsKey("alternatingColor")
                                          ? e.InitParams["alternatingColor"]
                                          : "#092233";
            string borderColor = e.InitParams.ContainsKey("borderColor")
                                     ? e.InitParams["borderColor"]
                                     : "#38516a";
            string foreColor = e.InitParams.ContainsKey("foregroundColor")
                                   ? e.InitParams["foregroundColor"]
                                   : "#ebebeb";
            string linkColor = e.InitParams.ContainsKey("linkColor")
                                   ? e.InitParams["linkColor"]
                                   : "#3698c6";
            string usernameColor = e.InitParams.ContainsKey("usernameColor")
                                   ? e.InitParams["usernameColor"]
                                   : "#62879d";
            string timeColor = e.InitParams.ContainsKey("timeColor")
                                   ? e.InitParams["timeColor"]
                                   : "#7dafcf";

            SecondsToRefresh = e.InitParams.ContainsKey("secondsToRefresh")
                                   ? int.Parse(e.InitParams["secondsToRefresh"])
                                   : 30;
            SecondsToRotate = e.InitParams.ContainsKey("secondsToRotate")
                                   ? int.Parse(e.InitParams["secondsToRotate"])
                                   : 4;
            HideInfo = e.InitParams.ContainsKey("hideInfo")
                                   ? bool.Parse(e.InitParams["hideInfo"])
                                   : false;
            MaxItems = e.InitParams.ContainsKey("maxItems") ? int.Parse(e.InitParams["maxItems"]) : 30;
            HashTag = e.InitParams.ContainsKey("hashTag") ? e.InitParams["hashTag"] : "#ntk10";

            theme = (Configuration) Resources["Theme"];

            theme.UsernameColor = HexToColor(usernameColor, Colors.Blue);
            theme.TimeColor = HexToColor(timeColor, Colors.Black);
            theme.BackgroundColor = HexToColor(backColor, Colors.Transparent);
            theme.AlternatingColor = HexToColor(alternatingColor, Colors.Transparent);
            theme.ForegroundColor = HexToColor(foreColor, Colors.Black);
            theme.LinkColor = HexToColor(linkColor, Colors.Blue);
            theme.BorderColor = HexToColor(borderColor, Colors.Black);

            this.RootVisual = new MainPage();
        }

        private static Configuration theme;

        private Color HexToColor(string borderColor, Color defaultColor)
        {
            try
            {
                    //Convert.ToByte(borderColor.Substring(1, 2), 16),
                return Color.FromArgb(255,
                    Convert.ToByte(borderColor.Substring(1, 2), 16),
                    Convert.ToByte(borderColor.Substring(3, 2), 16),
                    Convert.ToByte(borderColor.Substring(5, 2), 16));
            }
            catch
            {
                return defaultColor;
            }
        }

        public static Configuration Theme { get { return theme;} }

        public static string HashTag { get; set; }
        public static int SecondsToRefresh { get; set; }
        public static int SecondsToRotate { get; set; }
        public static int MaxItems { get; set; }
        public static bool HideInfo { get; set; }

        private void Application_Exit(object sender, EventArgs e)
        {

        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            if (!System.Diagnostics.Debugger.IsAttached)
            {

                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
            }
        }

        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }
    }
}
