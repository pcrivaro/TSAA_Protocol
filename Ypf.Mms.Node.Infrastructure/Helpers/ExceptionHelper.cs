
using System;
using System.IO;

namespace Ypf.Mms.Node.Infrastructure.Helpers
{
    public static class ExceptionHelper
    {

        public static void Manager(Exception exc)
        {
            File.AppendAllText("exception.txt", exc.GetBaseException().Message);
        }

      
    }
}
