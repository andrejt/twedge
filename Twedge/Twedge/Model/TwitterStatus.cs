using System;
using System.Linq;
using System.Windows.Media;
using System.Xml.Linq;

namespace Twedge
{
    public class TwitterStatus
    {
        public string Id { get; set; }
        public DateTime Published { get; set; }
        public string Title { get; set; }
        public Uri PhotoUri { get; set; }
        public string Author { get; set; }
        public Uri AuthorUri { get; set; }
        public Color BackgroundColor { get; set; }

        public static bool IsAlternating;
        public TwitterStatus(XElement item)
        {
            try
            {

                Id = item.Element(MainPage.Xmlns + "id").Value;
                Title = item.Element(MainPage.Xmlns + "title").Value;
                Author =
                    item.Element(MainPage.Xmlns + "author").Element(MainPage.Xmlns + "name").Value.Split(' ')[0];
                Published = DateTime.Parse(item.Element(MainPage.Xmlns + "updated").Value);
                PhotoUri =
                    new Uri(item.Elements(MainPage.Xmlns + "link").ElementAt(1).Attribute("href").Value);
                AuthorUri = new Uri(item.Element(MainPage.Xmlns + "author").Element(MainPage.Xmlns + "uri").Value);

                IsAlternating = !IsAlternating;
            }
            catch
            {
            }
        }
    }
}