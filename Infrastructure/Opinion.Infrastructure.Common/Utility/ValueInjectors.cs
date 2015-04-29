using System;
using System.Collections.Generic;
using System.Linq;
using Omu.ValueInjecter;

namespace Opinion.Infrastructure.Common.Utility
{
    public static class ValueInjectors
    {
        public static T GetInjectedValues<T, U>(ref U q)
            where T : new()
            where U : class
        {
            T flatViewModel = new T();
            flatViewModel.InjectFrom<FlatLoopValueInjection>(q)

                .InjectFrom<UnflatLoopValueInjection>(q)
                .InjectFrom<NullablesToNormal>(q)
                .InjectFrom<NormalToNullables>(q)
                .InjectFrom<IntToEnum>(q)
                .InjectFrom<EnumToInt>(q);

            return flatViewModel;
        }

        public static IEnumerable<T> GetInjectedValues<T, U>(IEnumerable<U> q)
            where T : new()
            where U : class
        {
            foreach (var item in q)
            {
                var refitem = item as U;
                yield return GetInjectedValues<T, U>(ref refitem);
            }
        }
    }
}
