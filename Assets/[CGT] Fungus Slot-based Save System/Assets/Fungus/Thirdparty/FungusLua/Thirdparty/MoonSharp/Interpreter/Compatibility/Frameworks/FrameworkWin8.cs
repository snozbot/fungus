#if NETFX_CORE && !DOTNET_CORE

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
			return ei.AddMethod;
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
			return type.GetTypeInfo().GenericTypeArguments;
		}

		public override MethodInfo GetGetMethod(PropertyInfo pi)
		{
			return pi.GetMethod;
		}

		public override Type GetInterface(Type type, string name)
		{
			return type.GetTypeInfo().ImplementedInterfaces.FirstOrDefault(t => t.Name == name);
		}

		public override Type[] GetInterfaces(Type t)
		{
			return SafeArray(GetTypeInfoFromType(t).ImplementedInterfaces);
		}


		public override MethodInfo GetMethod(Type type, string name)
		{
			return GetTypeInfoFromType(type).GetDeclaredMethod(name);
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
			return GetTypeInfoFromType(type).GetDeclaredProperty(name);
		}

		public override MethodInfo GetRemoveMethod(EventInfo ei)
		{
			return ei.RemoveMethod;
		}

		public override MethodInfo GetSetMethod(PropertyInfo pi)
		{
			return pi.SetMethod;
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
			if (o == null)
				return false;

			return t.GetTypeInfo().IsAssignableFrom(o.GetType().GetTypeInfo());
		}

		public override bool StringContainsChar(string str, char chr)
		{
			return str.Contains(chr);
		}

		private static MethodInfo GetMethodEx(Type t, string name, Type[] parameters)
		{
			var ti = t.GetTypeInfo();
			var methods = ti.GetDeclaredMethods(name);
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
		}


		public override MethodInfo GetMethod(Type resourcesType, string name, Type[] types)
		{
			return GetMethodEx(resourcesType, name, types);
		}

		public override Type[] GetAssemblyTypes(Assembly asm)
		{
			return SafeArray(asm.ExportedTypes);
		}

	}
}
#endif
