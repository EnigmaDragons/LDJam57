using UnityEngine;
using System;

public static class ResourceUtils
{
    /// <summary>
    /// Loads a resource and throws a detailed exception if the resource is not found.
    /// </summary>
    /// <typeparam name="T">The type of resource to load</typeparam>
    /// <param name="path">The path to the resource</param>
    /// <returns>The loaded resource</returns>
    /// <exception cref="InvalidOperationException">Thrown when the resource cannot be found</exception>
    public static T LoadOrThrow<T>(string path) where T : UnityEngine.Object
    {
        var resource = Resources.Load<T>(path);
        if (resource == null)
        {
            throw new InvalidOperationException($"Failed to load resource of type {typeof(T).Name} at path '{path}'. " +
                "Make sure the resource exists in a Resources folder and the path is correct.");
        }
        return resource;
    }
}
