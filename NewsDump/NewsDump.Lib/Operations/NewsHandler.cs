﻿using NewsDump.Lib.Operations.Sites;
using NewsDump.Lib.Operations.Sites.Interface;
using System.Collections.Generic;

namespace NewsDump.Lib.Operations
{
    public static class NewsHandler
    {
        public static void Run()
        {
            var services = RegisterServices();

            services.ForEach(x => x.RunAndSave());

        }

        private static List<IDumper> RegisterServices()
        {
            var serviceList = new List<IDumper>();
            serviceList.Add(new FarsnewsDumper());
            serviceList.Add(new IribDumper());
            serviceList.Add(new SookDumper());
            serviceList.Add(new IrnaDumper());
            serviceList.Add(new MehrnewsDumper());
            serviceList.Add(new TasnimnewsDumper());
            serviceList.Add(new YjcDumper());
            serviceList.Add(new PgnewsDumper());
            return serviceList;
        }
    }
}
