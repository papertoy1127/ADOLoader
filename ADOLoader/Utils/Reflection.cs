using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace ADOLoader.Utils {
    public static class Reflection {
        internal static Dictionary<Type, Dictionary<string, FieldInfo>> fields =
            new();

        internal static Dictionary<Type, Dictionary<string, PropertyInfo>> properties =
            new();

        internal static Dictionary<Type, Dictionary<string, IEnumerable<(Type[], MethodInfo)>>> methods =
            new();

        private const int FIELD = 0;
        private const int PROPERTY = 1;
        private const int PROPERTYGETONLY = 2;
        private const int PROPERTYSETONLY = 3;
        private const int METHOD = 4;

        internal static bool CheckField(Type type, string member) {
            var field = type.GetField(member, AccessTools.all);
            if (field != null) {
                fields[type][member] = field;
                return true;
            }

            return false;
        }

        internal static int? CheckProperty(Type type, string member) {
            var property = type.GetProperty(member, AccessTools.all);
            if (property != null) {
                if (property.CanRead) {
                    if (property.CanWrite) {
                        properties[type][member] = property;
                        return PROPERTY;
                    }

                    properties[type][member] = property;
                    return PROPERTYGETONLY;
                }

                properties[type][member] = property;
                return PROPERTYSETONLY;
            }

            return null;
        }

        internal static bool CheckMethod(Type type, string member) {
            
            var methodInfos = type.GetMethods(AccessTools.all).Where(info => info.Name == member);
            var enumerable = methodInfos as MethodInfo[] ?? methodInfos.ToArray();
            if (enumerable.Any()) {
                methods[type][member] = enumerable.Select(info =>
                    (info.GetParameters().Select(param => param.ParameterType).ToArray(), info));
                return true;
            }

            return false;
        }

        public static T get<T>(this object instance, string varName) {
            var type = instance.GetType();
            if (!fields.ContainsKey(type)) {
                fields[type] = new Dictionary<string, FieldInfo>();
            }
            if (!properties.ContainsKey(type)) {
                properties[type] = new Dictionary<string, PropertyInfo>();
            }

            if (!fields[type].ContainsKey(varName) && !properties[type].ContainsKey(varName)) {
                if (!CheckField(type, varName)) {
                    if (CheckProperty(type, varName) is null)
                        throw new MemberNotFoundException(varName);
                }
            }
            
            if (fields[type].ContainsKey(varName)) {
                var result = fields[type][varName].GetValue(instance);
                if (result is T res) return res;
                throw new InvalidCastException(varName);
            }

            if (properties[type][varName].CanRead) {
                var result = properties[type][varName].GetValue(instance);
                if (result is T res) return res;
                throw new InvalidCastException(varName);
            }

            throw new InvalidOperationException(varName);
        }

        public static void set<T>(this object instance, string varName, T value) => instance.set<T>(varName)(value);
        public static Action<T> set<T>(this object instance, string varName) {
            var type = instance.GetType();
            if (!fields.ContainsKey(type)) {
                fields[type] = new Dictionary<string, FieldInfo>();
            }
            if (!properties.ContainsKey(type)) {
                properties[type] = new Dictionary<string, PropertyInfo>();
            }

            if (!fields[type].ContainsKey(varName) && !properties[type].ContainsKey(varName)) {
                if (!CheckField(type, varName)) {
                    if (CheckProperty(type, varName) is null)
                        throw new MemberNotFoundException(varName);
                }
            }

            if (fields[type].ContainsKey(varName)) {
                var info = fields[type][varName];
                if (info.FieldType == typeof(T))
                    return value => info.SetValue(instance, value);
                throw new InvalidCastException(varName);
            }
            else {
                var info = properties[type][varName];
                if (!info.CanWrite) throw new InvalidOperationException();
                if (info.PropertyType == typeof(T))
                    return value => info.SetValue(instance, value);
                throw new InvalidCastException(varName);

            }
        }

        public delegate T InvokableMethod<out T>(params object[] args);
        
        public static InvokableMethod<T> invoke<T>(this object instance, string methodName) {
            var type = instance.GetType();
            if (!methods.ContainsKey(type)) {
                methods[type] = new Dictionary<string, IEnumerable<(Type[], MethodInfo)>>();
            }

            if (!methods[type].ContainsKey(methodName)) {
                if (!CheckMethod(type, methodName)) {
                    throw new MemberNotFoundException(methodName);
                }
            }

            var methodInfos = methods[type][methodName];
            return args => {
                foreach (var methodInfo in methodInfos) {
                    if (methodInfo.Item1.CheckType(args)) {
                        if (typeof(T) != typeof(object) && methodInfo.Item2.ReturnType != typeof(T))
                            throw new InvalidCastException(methodName);
                        return (T) methodInfo.Item2.Invoke(instance, args);
                    }
                }
                
                throw new MemberNotFoundException(methodName);
            };
        }
    }
}