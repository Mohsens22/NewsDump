using HtmlAgilityPack;
using NewsDump.Lib.Data;
using NewsDump.Lib.Model;
using NewsDump.Lib.Operations.Sites.Interface;
using NewsDump.Lib.Util;
using Olive;
using System;
using System.Linq;

namespace NewsDump.Lib.Operations.Sites
{
    class YjcDumper : DumperBase, IDumper
    {
        Repository<News> _repo = Repository.Of<News>();
        public News ExtractNews(string html, Uri baseUri)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var printButtton = htmlDoc.DocumentNode.GetElementsWithClass("a", "news_print_botton")?.FirstOrDefault();
            var printValue = printButtton.GetAttributeValue("onclick", null).GetItemsWithinQuotes()?.FirstOrDefault(x => x.Contains("/fa/print"));

            var printUri = $"http://{baseUri.Host}{printValue}";
            var printHtml = Get(printUri);

            var printDoc = new HtmlDocument();
            printDoc.LoadHtml(printHtml);

            var body = printDoc.DocumentNode.GetElementsWithClass("div", "body")?.FirstOrDefault();
            var paragraphs = body.ChildNodes.Where(x => x.Name == "p");
            var text = string.Join(Environment.NewLine, paragraphs.Select(x => x.InnerText.HtmlDecode().Trim()));

            if (text.IsEmpty())
            {
                //Validate for trivia character
                if (!body.InnerText.HtmlDecode().StartsWith("{$"))
                {
                    text = body.InnerText.HtmlDecode().Trim();
                }
            }

            return new News { NewsBody = text };
        }



        public void RunAndSave()
        {
            EventBus.Notify("YJC dumper running", "Info");

            var xml = Get(Constants.YjcRss);
            var feed = GetFeed(xml);

            foreach (var item in feed.Items)
            {
                try
                {
                    //Validate Uri
                    if (item.Links.None())
                    {
                        EventBus.Notify("This feed has no links", "Alert");
                        continue;
                    }

                    //Run operation for new items only
                    if (item.NewsExists(_repo))
                    {
                        continue;
                    }

                    var html = Get(item.GetUri().ToString());


                    var news = ExtractNews(html, item.GetUri());




                    //Set data from feed
                    news = SetNewsFromFeed(news, item);

                    if (news.NewsIntro.IsEmpty() && news.NewsBody.HasValue())
                    {
                        news.NewsIntro = news.NewsBody.Take(0, 100) + "...";
                    }


                    //Save in database
                    news.SaveNewsInDatabase(_repo);
                }
                catch (Exception ex)
                {

                    EventBus.Notify("Retrying, YJC...", "Info");
                    EventBus.Log(ex.Message, "retrying in yjc");
                }


            }

            EventBus.Notify("YJC dumper exiting", "Info");

        }


    }
}
