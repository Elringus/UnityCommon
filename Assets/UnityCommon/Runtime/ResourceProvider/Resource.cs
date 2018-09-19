﻿using UnityEngine;

namespace UnityCommon
{
    /// <summary>
    /// Represents a resource object stored at the specified path. 
    /// </summary>
    public class Resource
    {
        public string Path { get; private set; }
        public object Object { get; set; }
        public bool IsValid => Object != null;
        public bool IsUnityObject => Object is Object;
        public bool IsUnloadable => !(Object is GameObject || Object is Component || Object is AssetBundle);
        public Object AsUnityObject => Object as Object;

        public Resource (string path, object obj = null)
        {
            Path = path;
            Object = obj;
        }
    }

    /// <summary>
    /// A strongly typed version of the <see cref="Resource"/>.
    /// </summary>
    /// <typeparam name="T">Type of the resource object.</typeparam>
    public class Resource<T> : Resource
    {
        public new T Object { get { return CastObject(base.Object); } set { base.Object = value; } }

        public Resource (string path, T obj = default(T)) : base(path, obj) { }

        private T CastObject (object resourceObject)
        {
            if (resourceObject == null)
            {
                Debug.LogError(string.Format("Resource '{0}' is null.", Path, typeof(T).Name));
                return default(T);
            }

            if (!typeof(T).IsAssignableFrom(resourceObject.GetType()))
            {
                Debug.LogError(string.Format("Resource '{0}' is not of type '{1}'.", Path, typeof(T).Name));
                return default(T);
            }

            return (T)resourceObject;
        }
    }
}
