using System.ComponentModel;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Skua.Core.Interfaces;

public delegate void FlashCallHandler(string function, params object[] args);
public delegate void FlashErrorHandler(Exception e, string function, params object[] args);
public interface IFlashUtil
{
    event FlashCallHandler? FlashCall;
    event FlashErrorHandler? FlashError;

    /// <summary>
    /// Make a flash call for the specified <paramref name="function"/>.
    /// </summary>
    /// <param name="function">Function to be called in the SWF.</param>
    /// <param name="args">Parameters of the called <paramref name="function"/></param>
    /// <returns>A <see cref="string"/> representation of the return value.</returns>
    string? Call(string function, params object[] args);
    /// <summary>
    /// Make a flash call for the specified <paramref name="function"/>.
    /// </summary>
    /// <typeparam name="T"><see cref="Type"/> to convert the return value.</typeparam>
    /// <param name="function">Function to be called in the SWF.</param>
    /// <param name="args">Parameters of the called <paramref name="function"/></param>
    /// <returns>An object of the specified <typeparamref name="T"/> or <see langword="null"/></returns>
    T? Call<T>(string function, params object[] args);
    /// <summary>
    /// Make a flash call for the specified <paramref name="function"/>.
    /// </summary>
    /// <param name="function">Function to be called in the SWF.</param>
    /// <param name="type"><see cref="Type"/> that the return value will be converted to.</param>
    /// <param name="args">Parameters of the called <paramref name="function"/></param>
    /// <returns>An <see cref="object"/> of the specified <paramref name="type"/> or <see langword="null"/></returns>
    object? Call(string function, Type type, params object[] args);
    /// <summary>
    /// Converts a <see cref="XElement"/> to <see cref="object"/>.
    /// </summary>
    /// <param name="el">Element to parse.</param>
    /// <returns>An <see cref="object"/> representing the converted <see cref="XElement"/>.</returns>
    object FromFlashXml(XElement el);
    /// <summary>
    /// Raises the <see cref="FlashCall"/> event.
    /// </summary>
    /// <param name="function">Function which was called.</param>
    /// <param name="args">Parameters of the function.</param>
    void OnFlashCall(string function, params object[] args);
    /// <summary>
    /// Raises the <see cref="FlashError"/> event.
    /// </summary>
    /// <param name="e"><see cref="Exception"/> that was thrown.</param>
    /// <param name="function">The function that threw the exception.</param>
    /// <param name="args">Array of the arguments passed to the function.</param>
    void OnFlashError(Exception e, string function, params object[] args);
    /// <summary>
    /// Creates a <see cref="IFlashObject{T}"/> from the specified <paramref name="path"/>.
    /// </summary>
    /// <typeparam name="T"><see cref="Type"/> of the flash object.</typeparam>
    /// <param name="path">Path to get the object from.</param>
    /// <returns></returns>
    IFlashObject<T> CreateFlashObject<T>(string path);
    /// <summary>
    /// Checks if the actionscript object at the given path is null.
    /// </summary>
    /// <param name="path">The path of the object to check.</param>
    /// <returns>True if the object at the given path is null (unset).</returns>
    public bool IsNull(string path)
    {
        return Call<bool>("isNull", path);
    }

    /// <summary>
    /// Gets an actionscript object at the given location as a JSON string.
    /// </summary>
    /// <param name="path">The path of the object to get.</param>
    /// <returns>The value of the object at the given path as a serialzied JSON string.</returns>
    public string? GetGameObject(string path)
    {
        if (path.Contains('['))
        {
            string key = path.Split(new char[] { '"', '[', ']' }, StringSplitOptions.RemoveEmptyEntries).Last();
            string finalPath = path.Split('[')[0];
            return Call("getGameObjectKey", finalPath, key);
        }
        return Call("getGameObject", path);
    }

    /// <summary>
    /// Gets an actionscript object at the given location and deserializes it as JSON to the given type.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the object to.</typeparam>
    /// <param name="path">The path of the object to get (i.e. world.myAvatar.sta.$tha will get your haste stat).</param>
    /// <param name="def">The default value to return if the call/deserialization fails.</param>
    /// <returns>The deserialized value of the object at the given path.</returns>
    public T? GetGameObject<T>(string path, T? def = default)
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(GetGameObject(path));
        }
        catch
        {
            return def;
        }
    }

    /// <summary>
    /// Gets a static actionscript object at the given location as a JSON string.
    /// </summary>
    /// <param name="path">The path of the object to get.</param>
    /// <returns>The value of the object at the given path as a serialzied JSON string.</returns>
    public string? GetGameObjectStatic(string path)
    {
        return Call("getGameObjectS", path);
    }

    /// <summary>
    /// Gets a static actionscript object at the given location and deserializes it as JSON to the given type.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the object to.</typeparam>
    /// <param name="path">The path of the object to get.</param>
    /// <param name="def">The default value to return if the call/deserialization fails.</param>
    /// <returns>The deserialized value of the object at the given path.</returns>
    public T? GetGameObjectStatic<T>(string path, T? def = default)
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(GetGameObjectStatic(path));
        }
        catch
        {
            return def;
        }
    }

    /// <summary>
    /// Sets the value of the actionscript object at the given path.
    /// </summary>
    /// <param name="path">The path of the object to set.</param>
    /// <param name="value">The value to set the object to. This can be a string, any number type or a bool.</param>
    public void SetGameObject(string path, object value)
    {
        if (path.Contains('['))
        {
            string key = path.Split(new char[] { '"', '[', ']' }, StringSplitOptions.RemoveEmptyEntries).Last();
            string finalPath = path.Split('[')[0];
            Call("setGameObjectKey", finalPath, key, value);
            return;
        }
        Call("setGameObject", path, value);
    }

    /// <summary>
    /// Calls the actionscript object with the given name at the given location.
    /// </summary>
    /// <param name="path">The path to the object and its function name.</param>
    /// <param name="args">The arguments to pass to the function.</param>
    /// <returns>The value of the object returned by calling the function as a serialzied JSON string.</returns>
    public string? CallGameFunction(string path, params object[] args)
    {
        return args.Length > 0 ? Call("callGameFunction", new object[] { path }.Concat(args).ToArray()) : Call("callGameFunction0", path);
    }

    /// <summary>
    /// Calls the actionscript object with the given name at the given location.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the return of the function as.</typeparam>
    /// <param name="path">The path to the object and its function name.</param>
    /// <param name="args">The arguments to pass to the function.</param>
    /// <returns>The deserialized value of the object returned by the function.</returns>
    public T? CallGameFunction<T>(string path, params object[] args)
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(CallGameFunction(path, args));
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// Gets the actionscript object of the array at the given path at the given index in that array.
    /// </summary>
    /// <param name="path">The path to the array.</param>
    /// <param name="index">The index in the array to get the object from.</param>
    /// <returns>The value of the object at the given index in the array as a serialzied JSON string.</returns>
    public string? GetArrayObject(string path, int index)
    {
        return Call("getArrayObject", path, index);
    }

    /// <summary>
    /// Gets the actionscript object of the array at the given path at the given index in that array.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the object in the array as.</typeparam>
    /// <param name="path">The path to the array.</param>
    /// <param name="index">The index in the array to get the object from.</param>
    /// <param name="def">The default value to return if the call/deserialization fails.</param>
    /// <returns>The deserialized value of the object at the given index in the array.</returns>
    public T? GetArrayObject<T>(string path, int index, T? def = default)
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(GetArrayObject(path, index));
        }
        catch
        {
            return def;
        }
    }

    /// <summary>
    /// Selects the members of each object in the array at the given path and puts them into a new array and returns them.
    /// </summary>
    /// <typeparam name="T">The type of object to deserialize the contents of the array as.</typeparam>
    /// <param name="path">The path to the array.</param>
    /// <param name="selector">The name of the field to use to populate the new array.</param>
    /// <returns>A list of deserialized objects from the selected array.</returns>
    public List<T> SelectArrayObjects<T>(string path, string selector)
    {
        try
        {
            return JsonConvert.DeserializeObject<List<T>>(Call("selectArrayObjects", path, selector));
        }
        catch
        {
            return new List<T>();
        }
    }
}
