using System;
using System.Collections.Generic;
using System.Linq;
using Omu.ValueInjecter;
using System.Collections;

namespace Opinion.Infrastructure.Common.Utility
{
    public static class ObjectExtensions
    {
        public static T ToDomain<T>(this object source, T domain = default(T))
         where T : class, new()
        {
            if (domain == null)
                domain = new T();

            if (source == null)
                return domain;

            domain.InjectFrom(source)
                .InjectFrom<IgnoreAudit>(source)
                .InjectFrom<FlatLoopValueInjection>(source)
                .InjectFrom<UnflatLoopValueInjection>(source)
                .InjectFrom<NullablesToNormal>(source)
                .InjectFrom<NormalToNullables>(source);
            return domain;
        }

        public static T ToViewModel<T>(this object source, T viewmodel = default(T))
            where T : class, new()
        {
            if (viewmodel == null)
                viewmodel = new T();

            if (source == null)
                return viewmodel;

            viewmodel.InjectFrom(source)
                .InjectFrom<FlatLoopValueInjection>(source)
                .InjectFrom<UnflatLoopValueInjection>(source)
                .InjectFrom<NullablesToNormal>(source)
                .InjectFrom<NormalToNullables>(source)
                .InjectFrom<IntToEnum>(source)
                .InjectFrom<EnumToInt>(source);

            return viewmodel;
        }

        public static T Audit<T>(this IAudit<T> target, string user = "system")
        {
            if (!target.CreatedAt.HasValue)
            {
                target.CreatedAt = DateTime.UtcNow;
                target.CreatedBy = user;
            }

            target.UpdatedAt = DateTime.UtcNow;
            target.UpdatedBy = user;

            return (T)target;
        }

        public static bool IsEmpty<T>(this IEnumerable<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            var genericCollection = list as ICollection<T>;
            if (genericCollection != null)
            {
                return genericCollection.Count == 0;
            }

            var nonGenericCollection = list as ICollection;
            if (nonGenericCollection != null)
            {
                return nonGenericCollection.Count == 0;
            }

            return !list.Any();
        }
    }
}
