using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Globalization;

namespace MarkerMetro.Unity.WinLegacy.Reflection
{
    [Flags]
    public enum BindingFlags
    {
        Default,
        Public,
        Instance,
        InvokeMethod,
        NonPublic,
        Static,
        FlattenHierarchy,
        DeclaredOnly,
        IgnoreCase
    }

    public enum TypeCode
    {
        Byte,
        Int16,
        Int32,
        Int64,
        SByte,
        UInt16,
        UInt32,
        UInt64,
        Single,
        Double,
        Char,
        Boolean,
        String,
        DateTime,
        Decimal,
        Empty,
        DBNull, // Never used
        Object
    }

    public static class ReflectionExtensions
    {

        private static readonly Dictionary<Type, TypeCode> _typeCodeTable =
           new Dictionary<Type, TypeCode>()
            {
                    { typeof( Boolean ), TypeCode.Boolean },
                    { typeof( Char ), TypeCode.Char },
                    { typeof( Byte ), TypeCode.Byte },
                    { typeof( Int16 ), TypeCode.Int16 },
                    { typeof( Int32 ), TypeCode.Int32 },
                    { typeof( Int64 ), TypeCode.Int64 },
                    { typeof( SByte ), TypeCode.SByte },
                    { typeof( UInt16 ), TypeCode.UInt16 },
                    { typeof( UInt32 ), TypeCode.UInt32 },
                    { typeof( UInt64 ), TypeCode.UInt64 },
                    { typeof( Single ), TypeCode.Single },
                    { typeof( Double ), TypeCode.Double },
                    { typeof( DateTime ), TypeCode.DateTime },
                    { typeof( Decimal ), TypeCode.Decimal },
                    { typeof( String ), TypeCode.String },
            };


        public static bool IsValueType(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().IsValueType;
#else
            return type.IsValueType;   
#endif
        }

        public static bool IsGenericType(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().IsGenericType;
#else
            return type.IsGenericType;
#endif
        }

        public static object[] GetCustomAttributes(this Type type, bool inherit)
        {
#if NETFX_CORE
            var customAttributes = type.GetTypeInfo().GetCustomAttributes(inherit);
            if (customAttributes == null) return null;
            return customAttributes.ToArray();
#else
            throw new NotImplementedException();
#endif
        }

        public static object[] GetCustomAttributes(this Type type, Type attrType)
        {
#if NETFX_CORE
            var customAttributes = type.GetTypeInfo().GetCustomAttributes(attrType);
            if (customAttributes == null) return null;
            return customAttributes.ToArray();
#else
            throw new NotImplementedException();
#endif
        }

        public static object[] GetCustomAttributes(this Type type, Type attrType, bool inherit)
        {
            
#if NETFX_CORE
            var customAttributes = type.GetTypeInfo().GetCustomAttributes(attrType, inherit);
            if (customAttributes == null) return null;
            return customAttributes.ToArray();
#else
            throw new NotImplementedException();
#endif
        }

        public static Assembly GetAssembly(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().Assembly;
#else
            throw new NotImplementedException();
#endif
        }

        public static bool IsAbstract(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().IsAbstract;
#else
            return type.IsAbstract;
#endif
        }

        public static bool IsDefined(this Type type, Type attributeType)
        {
#if NETFX_CORE

            return type.GetTypeInfo().IsDefined(attributeType);
#else
            throw new NotImplementedException();
#endif
        }

        public static bool IsDefined(this Type type, Type attributeType, bool inherit)
        {
#if NETFX_CORE

            return type.GetTypeInfo().IsDefined(attributeType, inherit);
#else
            throw new NotImplementedException();
#endif
        }

        public static bool IsClass(this Type type)
        {
#if NETFX_CORE

            return type.GetTypeInfo().IsClass;
#else
            return type.IsClass;
#endif
        }

        public static bool IsEnum(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().IsEnum;
#else
            return type.IsEnum;
#endif
        }

        public static bool IsPublic(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().IsPublic;
#else
            return type.IsPublic;
#endif
        }

        public static bool IsVisible(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().IsVisible;
#else
            return type.IsVisible;
#endif
        }

        public static ConstructorInfo GetParameterlessConstructor(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().DeclaredConstructors.FirstOrDefault(c => !c.GetParameters().Any());
#else
            throw new NotImplementedException();
#endif
        }

        public static ConstructorInfo[] GetConstructors(this Type type)
        {
#if NETFX_CORE
            var decConstructors = type.GetTypeInfo().DeclaredConstructors;
            if (decConstructors == null) return null;
            return decConstructors.ToArray();
#else
            throw new NotImplementedException();
#endif
        }

        public static Type GetInterface(this Type type, string name)
        {
            return GetInterface(type, name, false);
        }

        public static Type GetInterface(this Type type, string name, bool ignoreCase)
        {
#if NETFX_CORE
            var interfaces = type.GetTypeInfo().ImplementedInterfaces;

            foreach (var interfaceType in interfaces)
            {
                /*We must compare against the generic type definition*/
                var t = interfaceType.IsGenericType() ? interfaceType.GetGenericTypeDefinition() : interfaceType;

                if (t.Name.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
                    return interfaceType;
                if (t.FullName.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
                    return interfaceType;
            }

            return null;
#else
            throw new NotImplementedException();
#endif
        }

        public static PropertyInfo[] GetProperties(this Type type)
        {
            if (type == null)
                throw new NullReferenceException();
#if NETFX_CORE
            var members = type.GetRuntimeProperties();
            return members == null ? null : members.ToArray();
#else
            throw new NotImplementedException();
#endif
        }

        /**
        * This implementation currently ignores BindingFlags.FlattenHierarchy.
        */
        public static PropertyInfo[] GetProperties(this Type type, BindingFlags flags)
        {
#if NETFX_CORE
            var props = type.GetProperties();
            var result = new List<PropertyInfo>();
            foreach (var prop in props)
                if (TestBindingFlags(type, prop.GetMethod, flags) || TestBindingFlags(type, prop.SetMethod, flags))
                    result.Add(prop);
            return result.ToArray();
#else
            throw new NotImplementedException();
#endif
        }

        /**
         * Attention: This implementation does not throw AmbiguousMatchException when more than one
         * property is found with the specified name and matching the specified binding constraints.
         *
         * This implementation currently ignores BindingFlags.FlattenHierarchy.
         */
        public static PropertyInfo GetProperty(this Type type, string name, BindingFlags bindingAttr)
        {
#if NETFX_CORE
            var props = type.GetProperties(bindingAttr);
            if (!props.Any()) return null;
            return props.FirstOrDefault(f => bindingAttr.HasFlag(BindingFlags.IgnoreCase) ?
                f.Name.ToLower() == name.ToLower() : f.Name == name);
#else
            throw new NotImplementedException();
#endif
        }

        public static PropertyInfo GetProperty(this Type type, string name)
        {
            var props = type.GetProperties();
            if (!props.Any()) return null;
            return props.FirstOrDefault(f => f.Name == name);
        }

        public static MethodInfo[] GetMethods(this Type type)
        {
            if (type == null)
                throw new NullReferenceException();
#if NETFX_CORE
            var members = type.GetRuntimeMethods();
            return members == null ? null : members.ToArray();
#else
            throw new NotImplementedException();
#endif
        }

        /**
         * This implementation currently ignores BindingFlags.FlattenHierarchy.
         */
        public static MethodInfo[] GetMethods(this Type t, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
        {
#if NETFX_CORE
            if (!flags.HasFlag(BindingFlags.Instance) && !flags.HasFlag(BindingFlags.Static)) return null;

            var allMethods = t.GetMethods();
            var resultList = new List<MethodInfo>();
            foreach (var method in allMethods)
            {
                if (TestBindingFlags(t, method, flags))
                    resultList.Add(method);
            }
            return resultList.ToArray();
#else
            throw new NotImplementedException();
#endif
        }

        /**
         * Tests the following BindingFlags:
         * Public, NonPublic, Static, Instance, DeclaredOnly.
         */
        private static bool TestBindingFlags(Type t, MethodInfo method, BindingFlags flags)
        {
#if NETFX_CORE
            var isValid = (flags.HasFlag(BindingFlags.Public) && method.IsPublic)
                || (flags.HasFlag(BindingFlags.NonPublic) && !method.IsPublic);
            isValid &= (flags.HasFlag(BindingFlags.Static) && method.IsStatic) || (flags.HasFlag(BindingFlags.Instance) && !method.IsStatic);
            if (flags.HasFlag(BindingFlags.DeclaredOnly))
                isValid &= method.DeclaringType == t;
            return isValid;
#else
            throw new NotImplementedException();
#endif
        }
        public static MemberInfo[] GetMembers(this Type type)
        {
            if (type == null)
                throw new NullReferenceException();
#if NETFX_CORE
            var ti = type.GetTypeInfo();
            var result = new List<MemberInfo>();
            if (ti.DeclaredMembers != null)
            {
                result.AddRange(ti.DeclaredMembers);
            }
            return result.ToArray();
#else
            throw new NotImplementedException();
#endif
        }

        public static MemberInfo[] GetMembers(this Type t, BindingFlags flags)
        {
#if NETFX_CORE
            if (!flags.HasFlag(BindingFlags.Instance) && !flags.HasFlag(BindingFlags.Static)) return null;
            return t.GetMembers();
#else
            throw new NotImplementedException();
#endif
        }

        public static Object InvokeMember(this Type t, string name, BindingFlags flags, object binder, object target, object[] args)
        {
#if NETFX_CORE
            if (binder != null || target != null)
                throw new ArgumentException("doesn't support binder or target when invoking");
            // We only support invoking a normal method, not a field/property/other member
            var members = t.GetMethods();
            foreach (var m in members)
            {
                if (m.Name.Equals(name))
                {
                    return m.Invoke(t, args);
                }
            }
            return null;
#else
            throw new NotImplementedException();
#endif
        }

        public static FieldInfo[] GetFields(this Type type)
        {
#if NETFX_CORE
            var fields = type.GetRuntimeFields();
            return fields == null ? null : fields.ToArray();
#else
            throw new NotImplementedException();
#endif
        }

        public static FieldInfo[] GetFields(this Type t, BindingFlags flags)
        {
#if NETFX_CORE
            if (!flags.HasFlag(BindingFlags.Instance) && !flags.HasFlag(BindingFlags.Static))
                return null;

            var origFields = t.GetFields();
            var results = new List<FieldInfo>();
            foreach (var field in origFields)
            {
                var isValid = (flags.HasFlag(BindingFlags.Public) && field.IsPublic)
                    || (flags.HasFlag(BindingFlags.NonPublic) && !field.IsPublic);
                isValid &= (flags.HasFlag(BindingFlags.Static) && field.IsStatic) || (flags.HasFlag(BindingFlags.Instance) && !field.IsStatic);
                if (flags.HasFlag(BindingFlags.DeclaredOnly))
                    isValid &= field.DeclaringType == t;

                results.Add(field);
            }
            return results.ToArray();
#else
            throw new NotImplementedException();
#endif
        }

        public static FieldInfo GetField(this Type type, string name)
        {
#if NETFX_CORE
            var fields = type.GetFields();
            if (!fields.Any()) return null;
            return fields.FirstOrDefault(f => f.Name == name);
#else
            throw new NotImplementedException();
#endif
        }

        public static FieldInfo GetField(this Type type, string name, BindingFlags flags)
        {
#if NETFX_CORE
            FieldInfo[] fields = type.GetFields(flags);
            if (fields != null)
                for (int i = 0; i < fields.Length; i++)
                    if (fields[i].Name.Equals(name))
                        return fields[i];
            return null;
#else
            throw new NotImplementedException();
#endif
        }

        public static MethodInfo GetMethod(this Type type, string name)
        {
            return GetMethod(type, name, BindingFlags.Default, null);
        }

        public static MethodInfo GetMethod(this Type type, string name, Type[] types)
        {
            return GetMethod(type, name, BindingFlags.Default, types);
        }

        public static MethodInfo GetMethod(this Type t, string name, BindingFlags flags)
        {
            return GetMethod(t, name, flags, null);
        }
        public static Type GetBaseType(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().BaseType;
#endif
            throw new NotImplementedException();
        }

        public static MethodInfo GetMethod(Type t, string name, BindingFlags flags, Type[] parameters)
        {
#if NETFX_CORE
            var methods = t.GetMethods();
            foreach (var m in methods)
            {
                var plist = m.GetParameters();
                bool match = true;
                foreach (var param in plist)
                {
                    bool valid = true;
                    if (parameters != null)
                    {
                        foreach (var ptype in parameters)
                            valid &= ptype == param.ParameterType;
                    }
                    match &= valid;
                }
                if (match)
                    return m;
            }
            return null;
#else
            throw new NotImplementedException();
#endif
        }

        public static Type[] GetGenericArguments(this Type t)
        {
#if NETFX_CORE
            var ti = t.GetTypeInfo();
            return ti.GenericTypeArguments;
#else
            throw new NotImplementedException();
#endif
        }

        public static bool IsAssignableFrom(this Type current, Type toCompare)
        {
#if NETFX_CORE
            return current.GetTypeInfo().IsAssignableFrom(toCompare.GetTypeInfo());
#else
            throw new NotImplementedException();
#endif
        }

        public static bool IsInterface(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().IsInterface;
#else
            return type.IsInterface;
#endif
        }

        public static bool IsPrimitive(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().IsPrimitive;
#else
            return type.IsPrimitive;
#endif
        }

        /**
         * Missing IsSubclassOf, this works well
         */
        public static bool IsSubclassOf(this Type type, global::System.Type parent)
        {
#if NETFX_CORE
            return parent.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
#else
            throw new NotImplementedException();
#endif
        }

        public static Type[] GetTypes(this Assembly assembly)
        {
#if NETFX_CORE
            if (assembly.DefinedTypes == null) return null;
            if (assembly.DefinedTypes.Count() == 0) return new Type[0];
            var typeInfos = assembly.DefinedTypes.ToArray();
            Type[] types = new Type[typeInfos.Count()];
            for (var x = 0; x < types.Length; x++)
            {
                types[x] = typeInfos[x].AsType();
            }
            return types;

#else
            throw new NotImplementedException();
#endif
        }

        public static Type GetType(this Assembly assembly)
        {
#if NETFX_CORE
            if (assembly.DefinedTypes == null) return null;
            return assembly.GetType();
#else
            throw new NotImplementedException();
#endif
        }

        public static TypeCode GetTypeCode(this Type type)
        {
            if (type == null)
            {
                return TypeCode.Empty;
            }

            TypeCode result;
            if (!_typeCodeTable.TryGetValue(type, out result))
            {
                result = TypeCode.Object;
            }

            return result;
        }

    }
}