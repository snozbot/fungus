#if DOTNET_CORE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MoonSharp.Interpreter.Compatibility.Frameworks
{
	class FrameworkCurrent : FrameworkReflectionBase
	{
		public override TypeInfo GetTypeInfoFromType(Type t)
		{
			return t.GetTypeInfo();
		}

		private T[] SafeArray<T>(IEnumerable<T> prop) 
		{
			return prop != null ? prop.ToArray() : new T[0];
		}

		public override MethodInfo GetAddMethod(EventInfo ei)
		{
			return ei.GetAddMethod(true);
		}

		public override ConstructorInfo[] GetConstructors(Type type)
		{
			return SafeArray(GetTypeInfoFromType(type).DeclaredConstructors);
		}

		public override EventInfo[] GetEvents(Type type)
		{
			return SafeArray(GetTypeInfoFromType(type).DeclaredEvents);
		}

		public override FieldInfo[] GetFields(Type type)
		{
			return SafeArray(GetTypeInfoFromType(type).DeclaredFields);
		}

		public override Type[] GetGenericArguments(Type type)
		{
			return type.GetTypeInfo().GetGenericArguments();
		}

		public override MethodInfo GetGetMethod(PropertyInfo pi)
		{
			return pi.GetGetMethod(true);
		}

		public override Type GetInterface(Type type, string name)
		{
			return GetTypeInfoFromType(type).GetInterface(name);
		}

		public override Type[] GetInterfaces(Type t)
		{
			return GetTypeInfoFromType(t).GetInterfaces();
		}


		public override MethodInfo GetMethod(Type type, string name)
		{
			return GetTypeInfoFromType(type).GetMethod(name);
		}

		public override MethodInfo[] GetMethods(Type type)
		{
			return SafeArray(GetTypeInfoFromType(type).DeclaredMethods);
		}

		public override Type[] GetNestedTypes(Type type)
		{
			return SafeArray(GetTypeInfoFromType(type).DeclaredNestedTypes.Select(ti => ti.AsType()));
		}

		public override PropertyInfo[] GetProperties(Type type)
		{
			return SafeArray(GetTypeInfoFromType(type).DeclaredProperties);
		}

		public override PropertyInfo GetProperty(Type type, string name)
		{
			return GetTypeInfoFromType(type).GetProperty(name);
		}

		public override MethodInfo GetRemoveMethod(EventInfo ei)
		{
			return ei.GetRemoveMethod(true);
		}

		public override MethodInfo GetSetMethod(PropertyInfo pi)
		{
			return pi.GetSetMethod(true);
		}


		public override bool IsAssignableFrom(Type current, Type toCompare)
		{
			return current.GetTypeInfo().IsAssignableFrom(toCompare.GetTypeInfo());
		}

		public override bool IsDbNull(object o)
		{
			return o != null && o.GetType().FullName.StartsWith("System.DBNull");
		}

		public override bool IsInstanceOfType(Type t, object o)
		{
			return t.GetTypeInfo().IsInstanceOfType(o);
		}

		public override bool StringContainsChar(string str, char chr)
		{
			return str.Contains(chr);
		}

		public  override MethodInfo GetMethod(Type resourcesType, string name, Type[] types)
		{
			return resourcesType.GetTypeInfo().GetMethod(name, types);
		}

		public override Type[] GetAssemblyTypes(Assembly asm)
		{
			return asm.GetExportedTypes();
		}

	}
}
#endif
