using System;
using System.Collections.Generic;
using System.Linq;
using Omu.ValueInjecter;
using System.Collections;

namespace Opinion.Infrastructure.Common.Utility
{
    //Tomado de http://valueinjecter.codeplex.com/wikipage?title=Deep%20Cloning&referringTitle=Home
    public class CloneInjection : ConventionInjection
    {
        protected override bool Match(ConventionInfo c)
        {
            return c.SourceProp.Name == c.TargetProp.Name && c.SourceProp.Value != null;
        }

        protected override object SetValue(ConventionInfo c)
        {
            //for value types and string just return the value as is
            if (c.SourceProp.Type.IsValueType || c.SourceProp.Type == typeof(string))
                return c.SourceProp.Value;

            //handle arrays
            if (c.SourceProp.Type.IsArray)
            {
                var arr = c.SourceProp.Value as Array;
                var clone = arr.Clone() as Array;

                for (int index = 0; index < arr.Length; index++)
                {
                    var a = arr.GetValue(index);
                    if (a.GetType().IsValueType || a.GetType() == typeof(string)) continue;
                    clone.SetValue(Activator.CreateInstance(a.GetType()).InjectFrom<CloneInjection>(a), index);
                }
                return clone;
            }


            if (c.SourceProp.Type.IsGenericType)
            {
                //handle IEnumerable<> also ICollection<> IList<> List<>
                if (c.SourceProp.Type.GetGenericTypeDefinition().GetInterfaces().Contains(typeof(IEnumerable)))
                {
                    var t = c.SourceProp.Type.GetGenericArguments()[0];
                    if (t.IsValueType || t == typeof(string)) return c.SourceProp.Value;

                    var tlist = typeof(List<>).MakeGenericType(t);
                    var list = Activator.CreateInstance(tlist);

                    var addMethod = tlist.GetMethod("Add");
                    foreach (var o in c.SourceProp.Value as IEnumerable)
                    {
                        var e = Activator.CreateInstance(t).InjectFrom<CloneInjection>(o);
                        addMethod.Invoke(list, new[] { e }); // in 4.0 you can use dynamic and just do list.Add(e);
                    }
                    return list;
                }

                //unhandled generic type, you could also return null or throw
                return c.SourceProp.Value;
            }

            //for simple object types create a new instace and apply the clone injection on it
            return Activator.CreateInstance(c.SourceProp.Type)
                .InjectFrom<CloneInjection>(c.SourceProp.Value);
        }
    }
    
    public class IgnoreAudit : ConventionInjection
    {
        private static readonly string[] AuditNames = typeof(IAudit).GetProperties().Select(p => p.Name).ToArray();

        protected override bool Match(ConventionInfo c)
        {
            var result = AuditNames.Contains(c.TargetProp.Name);
            return result;
        }

        protected override object SetValue(ConventionInfo c)
        {
            // don't override these values
            return c.TargetProp.Value;
        }
    }

    // all below are from http://valueinjecter.codeplex.com/wikipage?title=Useful%20injections&referringTitle=Home
    public class EnumToInt : ConventionInjection
    {
        protected override bool Match(ConventionInfo c)
        {
            return c.SourceProp.Name == c.TargetProp.Name &&
                c.SourceProp.Type.IsSubclassOf(typeof(Enum)) && c.TargetProp.Type == typeof(int);
        }
    }

    public class IntToEnum : ConventionInjection
    {
        protected override bool Match(ConventionInfo c)
        {
            return c.SourceProp.Name == c.TargetProp.Name &&
                c.SourceProp.Type == typeof(int) && c.TargetProp.Type.IsSubclassOf(typeof(Enum));
        }
    }

    //e.g. int? -> int
    public class NullablesToNormal : ConventionInjection
    {
        protected override bool Match(ConventionInfo c)
        {
            return c.SourceProp.Name == c.TargetProp.Name &&
                    Nullable.GetUnderlyingType(c.SourceProp.Type) == c.TargetProp.Type;
        }
    }

    //e.g. int -> int?
    public class NormalToNullables : ConventionInjection
    {
        protected override bool Match(ConventionInfo c)
        {
            return c.SourceProp.Name == c.TargetProp.Name &&
                    c.SourceProp.Type == Nullable.GetUnderlyingType(c.TargetProp.Type);
        }
    }  
}