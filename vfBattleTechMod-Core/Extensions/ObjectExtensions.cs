using System;
using System.Collections.Generic;
using System.Reflection;
using vfBattleTechMod_Core.Extensions.ArrayExtensions;

namespace vfBattleTechMod_Core.Extensions
{
    public static class ObjectExtensions
    {
        private static readonly MethodInfo CloneMethod = typeof(object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);

        public static bool IsPrimitive(this Type type)
        {
            if (type == typeof(string))
            {
                return true;
            }

            return type.IsValueType & type.IsPrimitive;
        }

        public static object Copy(this object originalObject)
        {
            return ObjectExtensions.InternalCopy(originalObject, new Dictionary<object, object>(new ReferenceEqualityComparer()));
        }

        private static object InternalCopy(object originalObject, IDictionary<object, object> visited)
        {
            if (originalObject == null)
            {
                return null;
            }

            var typeToReflect = originalObject.GetType();
            if (typeToReflect.IsPrimitive())
            {
                return originalObject;
            }

            if (visited.ContainsKey(originalObject))
            {
                return visited[originalObject];
            }

            if (typeof(Delegate).IsAssignableFrom(typeToReflect))
            {
                return null;
            }

            var cloneObject = ObjectExtensions.CloneMethod.Invoke(originalObject, null);
            if (typeToReflect.IsArray)
            {
                var arrayType = typeToReflect.GetElementType();
                if (arrayType.IsPrimitive() == false)
                {
                    var clonedArray = (Array)cloneObject;
                    clonedArray.ForEach((array, indices) => array.SetValue(ObjectExtensions.InternalCopy(clonedArray.GetValue(indices), visited), indices));
                }
            }

            visited.Add(originalObject, cloneObject);
            ObjectExtensions.CopyFields(originalObject, visited, cloneObject, typeToReflect);
            ObjectExtensions.RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);
            return cloneObject;
        }

        private static void RecursiveCopyBaseTypePrivateFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect)
        {
            if (typeToReflect.BaseType != null)
            {
                ObjectExtensions.RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.BaseType);
                ObjectExtensions.CopyFields(originalObject, visited, cloneObject, typeToReflect.BaseType, BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate);
            }
        }

        private static void CopyFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy, Func<FieldInfo, bool> filter = null)
        {
            foreach (var fieldInfo in typeToReflect.GetFields(bindingFlags))
            {
                if ((filter != null) && (filter(fieldInfo) == false))
                {
                    continue;
                }

                if (fieldInfo.FieldType.IsPrimitive())
                {
                    continue;
                }

                var originalFieldValue = fieldInfo.GetValue(originalObject);
                var clonedFieldValue = ObjectExtensions.InternalCopy(originalFieldValue, visited);
                fieldInfo.SetValue(cloneObject, clonedFieldValue);
            }
        }

        public static T Copy<T>(this T original)
        {
            return (T)((object)original).Copy();
        }
    }

    public class ReferenceEqualityComparer : EqualityComparer<object>
    {
        public override bool Equals(object x, object y)
        {
            return object.ReferenceEquals(x, y);
        }

        public override int GetHashCode(object obj)
        {
            if (obj == null)
            {
                return 0;
            }

            return obj.GetHashCode();
        }
    }

    namespace ArrayExtensions
    {
        public static class ArrayExtensions
        {
            public static void ForEach(this Array array, Action<Array, int[]> action)
            {
                if (array.LongLength == 0)
                {
                    return;
                }

                var walker = new ArrayTraverse(array);
                do
                {
                    action(array, walker.Position);
                } while (walker.Step());
            }
        }

        internal class ArrayTraverse
        {
            private readonly int[] maxLengths;
            public int[] Position;

            public ArrayTraverse(Array array)
            {
                this.maxLengths = new int[array.Rank];
                for (var i = 0; i < array.Rank; ++i)
                {
                    this.maxLengths[i] = array.GetLength(i) - 1;
                }

                this.Position = new int[array.Rank];
            }

            public bool Step()
            {
                for (var i = 0; i < this.Position.Length; ++i)
                {
                    if (this.Position[i] < this.maxLengths[i])
                    {
                        this.Position[i]++;
                        for (var j = 0; j < i; j++)
                        {
                            this.Position[j] = 0;
                        }

                        return true;
                    }
                }

                return false;
            }
        }
    }
}