using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using System.Reflection;
using System.IO;
using System.Configuration;
using System.Globalization;

namespace QueueService {

    public static class Util {

        public static Object TryGetSetting(ApplicationSettingsBase setting, String name)
        {
            try
            {
                return setting[name];
            }
            catch
            {
                return null;
            }
        }

        public static T GetValueOrDefault<T>(string name, Func<T> value, T defaultValue, bool mandatory)
        {
            try
            {
                T val = value();

                if (val == null && mandatory)
                    throw new ArgumentException(String.Format("The value name: {0} is mandatory", name));

                return val == null ? defaultValue : val;
            }
            catch
            {
                if (mandatory)
                    throw new ArgumentException(String.Format("The value name: {0} is mandatory", name));
                return defaultValue;
            }
        }

        public static void TryExecute(Action action)
        {
            try
            {
                action();
            }
            catch { }
        }

        public static IEnumerable<TTarget> ConvertListFromTo<TSource, TTarget>(
        IEnumerable<TSource> source, Func<TSource, TTarget> convert)
        {

            List<TTarget> target = new List<TTarget>();
            foreach (var item in source)
            {
                target.Add(convert(item));
            }
            return target;
        }

        public static T ThrowExceptionIfNull<T>(T o, Func<Exception> exception)
        {
            if (o == null)
                throw exception();
            return o;
        }
    }
}
