using System;
using System.Reflection;


namespace Ypf.Mms.Node.Infrastructure.Helpers
{
    public static class ActivatorHelper
    {

        public static T CreateInstance<T>(string type)
        {
            return CreateInstance<T>(Type.GetType(type));
        }


        public static T CreateInstance<T>(Type type)
        {
            return (T)Activator.CreateInstance(type);
        }

    }
}
