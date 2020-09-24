using System;
using System.IO;
using System.Reflection;

namespace MonicaPlatform.TechLog.Module.Core
{
    public static class DorectoryExtensions
    {
        public static string PathApplication
        {
            get => Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
        }
    }
}
