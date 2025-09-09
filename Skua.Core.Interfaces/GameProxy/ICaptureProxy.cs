using System.ComponentModel;
using System.Net;

namespace Skua.Core.Interfaces;

public interface ICaptureProxy : INotifyPropertyChanged
{
    /// <summary>
    /// The destination server for the proxy to relay traffic to and from.
    /// </summary>
    IPEndPoint? Destination { get; set; }

    /// <summary>
    /// The list of packet interceptors.
    /// </summary>
    List<IInterceptor> Interceptors { get; }

    /// <summary>
    /// Indicates whether the proxy is running or not.
    /// </summary>
    bool Running { get; }

    /// <summary>
    /// Starts the capture proxy.
    /// </summary>
    void Start();

    /// <summary>
    /// Stops the capture proxy.
    /// </summary>
    void Stop();
}