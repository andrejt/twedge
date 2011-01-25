using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;
using System.IO;

namespace Twedge
{
    public partial class MainPage : UserControl
    {
        HttpWebRequest client;
        private DispatcherTimer timer;
        private DispatcherTimer timer2;

        public static readonly XNamespace Xmlns = "http://www.w3.org/2005/Atom";

        private DateTime lastTweetTime;

        public MainPage()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(App.SecondsToRefresh);
            timer.Tick += new EventHandler(OnTick);

            timer2 = new DispatcherTimer();
            timer2.Interval = TimeSpan.FromSeconds(App.SecondsToRotate);
            timer2.Tick += new EventHandler(OnTick2);

            infoLabel.Visibility = App.HideInfo ? Visibility.Collapsed : Visibility.Visible;
            Statuses = new ObservableCollection<TwitterStatus>();
            DataContext = this;
            Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        public string DateFormat
        {
            get { return "ddd, d.M H:mm"; }
        }
        void OnTick(object sender, EventArgs e)
        {
            try
            {
            Uri uri = new Uri("http://search.twitter.com/search.atom?q=" + App.HashTag.Replace("#", "%23"), UriKind.Absolute);
            client = WebRequest.CreateHttp(uri);
            client.BeginGetResponse(OnLoaded, this);

            }
            catch (Exception)
            {
            }
        }

        void OnTick2(object sender, EventArgs e)
        {
            try
            {
                TwitterStatus newStatus = newStatuses.Count > 0 ? newStatuses.Peek() : null;
                TwitterStatus firstStatus = Statuses.FirstOrDefault();
                TwitterStatus lastStatus = Statuses.LastOrDefault();
                TwitterStatus statusToAdd = null;

                if (newStatus != null && firstStatus != null && lastStatus != null)
                {
                    // add
                    if (firstStatus.Published >= lastStatus.Published)
                    {
                        statusToAdd = newStatuses.Dequeue();
                    }
                }

                if (statusToAdd == null && lastStatus != null)
                {
                    statusToAdd = lastStatus;
                }

                if (statusToAdd == null)
                {
                    return;
                }

                TwitterStatus first = Statuses.FirstOrDefault();
                if (first != null)
                {
                    statusToAdd.BackgroundColor = first.BackgroundColor == App.Theme.BackgroundColor ? App.Theme.AlternatingColor : App.Theme.BackgroundColor;
                }

                if (lastStatus != null)
                {
                    Statuses.Remove(lastStatus);
                }
                lastTweetTime = statusToAdd.Published;
                Statuses.Insert(0, statusToAdd);
            }
            catch (Exception)
            {
            }
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            OnTick(this, EventArgs.Empty);
            timer.Start();
            timer2.Start();
        }

        private void OnLoaded(IAsyncResult asyncRes)
        {
            if (!client.HaveResponse)
            {
                return;
            }
            HttpWebResponse httpResponse;

            try
            {
                httpResponse = (HttpWebResponse) client.EndGetResponse(asyncRes);
            }
            catch
            {
//                ((MainPage)asyncRes.AsyncState).Dispatcher.BeginInvoke(() => errorText.Text = "Napaka, brez panike!");
                return;
            }

            if (httpResponse.StatusCode != HttpStatusCode.OK)
            {
                return;
            }

            try
            {
                Configuration theme = App.Theme;
                IEnumerable<TwitterStatus> data;
                using (Stream stream = httpResponse.GetResponseStream())
                {
                    XDocument feed = XDocument.Load(stream);
                    data = from item in feed.Root.Elements(Xmlns + "entry")
                           select new TwitterStatus(item);
                }
                ((MainPage) asyncRes.AsyncState).Dispatcher.BeginInvoke(() => ParseFeed(data));
            }
            catch 
            {
//                ((MainPage)asyncRes.AsyncState).Dispatcher.BeginInvoke(() => errorText.Text = "Napaka, brez panike!");
            }

        }

        private Queue<TwitterStatus> newStatuses = new Queue<TwitterStatus>();

        private void ParseFeed(IEnumerable<TwitterStatus> data)
        {
            try
            {
            bool isAlternating = false;
            if (Statuses.Count == 0)
            {
                foreach (TwitterStatus status in data.OrderByDescending(s => s.Published).Take(App.MaxItems))
                {
                    status.BackgroundColor = isAlternating ? App.Theme.AlternatingColor:App.Theme.BackgroundColor;
                    Statuses.Add(status);
                    isAlternating = !isAlternating;
                    lastTweetTime = status.Published;
                }
            }
            else
            {
                foreach (TwitterStatus status in data.OrderByDescending(s => s.Published).Take(App.MaxItems).Where(status => !Statuses.Any(s => s.Id == status.Id)))
                {
                    newStatuses.Enqueue(status);
                }
                
            }

            }
            catch (Exception)
            {
            }
        }

        public ObservableCollection<TwitterStatus> Statuses { get; private set; }
    }
}
