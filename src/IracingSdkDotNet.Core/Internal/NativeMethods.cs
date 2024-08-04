using System;
using System.Runtime.InteropServices;

namespace IracingSdkDotNet.Core.Internal;

internal static partial class NativeMethods
{
    /// <summary>
    /// Opens an existing named event object. See <see href="https://learn.microsoft.com/en-us/windows/win32/api/synchapi/nf-synchapi-openeventw"/>.
    /// </summary>
    /// <param name="dwDesiredAccess">The access to the event object. The function fails if the security descriptor of the specified object does not permit the requested access for the calling process. For a list of access rights, see <see href="https://learn.microsoft.com/en-us/windows/desktop/Sync/synchronization-object-security-and-access-rights"/>.</param>
    /// <param name="bInheritHandle">If this value is <see langword="true"/>, processes created by this process will inherit the handle. Otherwise, the processes do not inherit this handle.</param>
    /// <param name="lpName">The name of the event to be opened. Name comparisons are case sensitive.</param>
    /// <returns>If the function succeeds, the return value is a handle to the event object. If the function fails, the return value is <see langword="null"/>.</returns>
#if NET8_0_OR_GREATER
    [LibraryImport("Kernel32.dll", StringMarshalling = StringMarshalling.Utf16)]
    public static partial IntPtr OpenEvent(uint dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, string lpName);
#else
    [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr OpenEvent(uint dwDesiredAccess, bool bInheritHandle, string lpName);
#endif

    /// <summary>
    /// Defines a new window message that is guaranteed to be unique throughout the system. The message value can be used when sending or posting messages.
    /// </summary>
    /// <param name="lpString">The message to be registered.</param>
    /// <returns>If the message is successfully registered, the return value is a message identifier in the range 0xC000 through 0xFFFF. If the function fails, the return value is zero.</returns>
#if NET8_0_OR_GREATER
    [LibraryImport("user32.dll", StringMarshalling = StringMarshalling.Utf16)]
    public static partial uint RegisterWindowMessage(string lpString);
#else
    [DllImport("user32.dll")]
    public static extern uint RegisterWindowMessage(string lpString);
#endif

    /// <summary>
    /// Places (posts) a message in the message queue associated with the thread that created the specified window and returns without waiting for the thread to process the message.
    /// </summary>
    /// <param name="hWnd">A handle to the window whose window procedure is to receive the message. The following values have special meanings.</param>
    /// <param name="Msg">The message to be posted.</param>
    /// <param name="wParam">Additional message-specific information.</param>
    /// <param name="lParam">Additional message-specific information.</param>
    /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
#if NET8_0_OR_GREATER
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
#else
    [DllImport("user32.dll")]
    public static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
#endif
}
