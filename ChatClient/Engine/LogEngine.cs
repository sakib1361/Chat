using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ChatClient.Engine
{
    public static class LogEngine
    {
        public static event EventHandler<string> ErrorOccured;
        public static void Error(Exception ex, [CallerMemberName]string member = "")
        {
            ErrorOccured?.Invoke(null, ex.Message + " => " + member);
        }
    }
}
