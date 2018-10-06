﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnityCommon
{
    /// <summary>
    /// Allows working with resources using a prioritized providers list.
    /// </summary>
    public abstract class ResourceLoader
    {
        public abstract bool IsLoadingAny { get; }

        protected List<IResourceProvider> Providers { get; }
        protected string Prefix { get; }

        public ResourceLoader (List<IResourceProvider> providersList, string resourcePathPrefix = null)
        {
            Providers = providersList;
            Prefix = resourcePathPrefix;
        }

        public abstract Task PreloadAsync (string path, bool isFullPath = false);
        public abstract bool IsLoadedByProvider (string path, bool isFullPath = false);
        public abstract bool IsLoaded (string path, bool isFullPath = false);
        public abstract Task UnloadAsync (string path, bool isFullPath = false);
        public abstract Task UnloadAllAsync ();
    }

    /// <summary>
    /// Allows working with resources of specific type using a prioritized providers list.
    /// </summary>
    public class ResourceLoader<TResource> : ResourceLoader where TResource : class
    {
        public override bool IsLoadingAny => loadCounter > 0;
        public Dictionary<string, TResource> LoadedResources { get; }

        private int loadCounter;

        public ResourceLoader (List<IResourceProvider> providersList, string resourcePathPrefix = null)
            : base(providersList, resourcePathPrefix)
        {
            LoadedResources = new Dictionary<string, TResource>();
        }

        public virtual async Task<TResource> LoadAsync (string path, bool isFullPath = false)
        {
            IncrementLoadCounter();
            if (!isFullPath) path = BuildFullPath(path);

            var resource = await Providers.LoadResourceAsync<TResource>(path);
            AddLoadedResource(resource);

            DecrementLoadCounter();
            return resource != null && resource.IsValid ? resource.Object : null;
        }

        public virtual async Task<IEnumerable<TResource>> LoadAllAsync (string path = null, bool isFullPath = false)
        {
            IncrementLoadCounter();
            if (!isFullPath) path = BuildFullPath(path);

            var resources = await Providers.LoadResourcesAsync<TResource>(path);
            AddLoadedResources(resources);

            DecrementLoadCounter();
            return resources.Select(r => r != null && r.IsValid ? r.Object : null);
        }

        public virtual async Task<IEnumerable<Resource<TResource>>> LocateResourcesAsync (string path, bool isFullPath = false)
        {
            if (!isFullPath) path = BuildFullPath(path);
            return await Providers.LocateResourcesAsync<TResource>(path);
        }

        public override async Task PreloadAsync (string path, bool isFullPath = false)
        {
            await LoadAsync(path, isFullPath);
        }

        public override async Task UnloadAsync (string path, bool isFullPath = false)
        {
            if (!isFullPath) path = BuildFullPath(path);

            await Providers.UnloadResourceAsync(path);

            RemoveLoadedResource(path);
        }

        public override async Task UnloadAllAsync ()
        {
            var resources = LoadedResources.Keys.ToArray();
            for (int i = 0; i < 0; i++)
                await UnloadAsync(resources[i], true);
        }

        /// <summary>
        /// Whether a resource with the provided path is loaded by any of the available providers.
        /// </summary>
        public override bool IsLoadedByProvider (string path, bool isFullPath = false)
        {
            if (!isFullPath) path = BuildFullPath(path);
            return Providers.ResourceLoaded(path);
        }

        /// <summary>
        /// Whether a resource with the provided path is already loaded 
        /// and can be instantly retrieved via <see cref="GetLoaded(string, bool)"/>.
        /// </summary>
        public override bool IsLoaded (string path, bool isFullPath = false)
        {
            if (!isFullPath) path = BuildFullPath(path);
            return LoadedResources.ContainsKey(path);
        }

        /// <summary>
        /// Returns a loaded resource with the provided path.
        /// In case the resource is not loaded, will return null.
        /// </summary>
        public virtual TResource GetLoaded (string path, bool isFullPath = false)
        {
            if (!isFullPath) path = BuildFullPath(path);
            if (!IsLoaded(path, true)) return default(TResource);
            return LoadedResources[path];
        }

        protected virtual string BuildFullPath (string path)
        {
            if (!string.IsNullOrWhiteSpace(Prefix))
            {
                if (!string.IsNullOrWhiteSpace(path)) return $"{Prefix}/{path}";
                else return Prefix;
            }
            else return path;
        }

        protected virtual void AddLoadedResources (IEnumerable<Resource<TResource>> resources)
        {
            foreach (var resource in resources)
                if (resource != null && resource.IsValid)
                    AddLoadedResource(resource);
        }

        protected virtual void AddLoadedResource (Resource<TResource> resource)
        {
            if (resource != null && resource.IsValid)
                LoadedResources[resource.Path] = resource.Object;
        }

        protected virtual void RemoveLoadedResource (string resourcePath)
        {
            if (LoadedResources.ContainsKey(resourcePath))
                LoadedResources.Remove(resourcePath);
        }

        protected void IncrementLoadCounter () => loadCounter++;
        protected void DecrementLoadCounter () => loadCounter--;
    }
}