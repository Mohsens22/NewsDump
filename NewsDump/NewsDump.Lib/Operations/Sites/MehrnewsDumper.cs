using NewsDump.Lib.Model;
using NewsDump.Lib.Operations.Sites.Interface;
using NewsDump.Lib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewsDump.Lib.Operations.Sites
{
    class MehrnewsDumper : DumperBase, IDumper
    {
        public News ExtractNews(string html)
        {
            throw new NotImplementedException();
        }

        

        public void RunAndSave()
        {
            var xml = Get(Constants.MehrnewsRss);
            var feed = GetFeed(xml);

            foreach (var item in feed.Items)
            {
                var html = Get(item.Links.FirstOrDefault().Uri.ToString());

                var news = ExtractNews(html);
                
            }

        }
    }
}