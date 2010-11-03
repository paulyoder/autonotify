using System;

namespace AutoNotify.Extensions
{
    public static class Extensions
    {
        public static T Tap<T>(this T target, Action<T> tap)
        {
            tap(target);
            return target;
        }
    }
}