using CoreHook;
using System;
using System.Runtime.InteropServices;

namespace Skua.WPF.Flash;

public class EoLHook
{
    private static LocalHook? _hook;

    public static bool IsHooked => _hook is not null;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern void GetSystemTime(IntPtr lpSystemTime);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
    private delegate void GetSystemTimeDelegate(IntPtr lpSystemTime);

    private static unsafe void GetSystemTimeHooked(IntPtr lpSystemTime)
    {
        GetSystemTime(lpSystemTime);
        _SYSTEMTIME* ptr = (_SYSTEMTIME*)lpSystemTime;
        ptr->wYear = 2020;
    }

    public static void Hook()
    {
        _hook = LocalHook.Create(LocalHook.GetProcAddress("kernel32.dll", "GetSystemTime"), new GetSystemTimeDelegate(GetSystemTimeHooked), null);
        _hook.ThreadACL.SetInclusiveACL(new int[1]);
    }

    public static void Unhook()
    {
        _hook?.Dispose();
        _hook = null;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct _SYSTEMTIME
    {
        public ushort wYear;
        public ushort wMonth;
        public ushort wDayOfWeek;
        public ushort wDay;
        public ushort wHour;
        public ushort wMinute;
        public ushort wSecond;
        public ushort wMilliseconds;
    }
}