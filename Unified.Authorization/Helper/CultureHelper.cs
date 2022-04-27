using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Unified.Authorization.Helper
{
    public static class CultureHelper
    {
        public static bool IsRighToLeft()
        {
            //return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft;
            return Thread.CurrentThread.CurrentUICulture.TextInfo.IsRightToLeft;

        }
        public static string GetCurrentCulture()
        {
            // return Thread.CurrentThread.CurrentCulture.Name;
            return Thread.CurrentThread.CurrentUICulture.Name;
        }

        public static int GetLanguageValue()
        {
            return Thread.CurrentThread.CurrentUICulture.TextInfo.IsRightToLeft ? 1 : 0;
        }

    }
}
