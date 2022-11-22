using System.Runtime.InteropServices;
using Windows.Storage.Pickers;
using WinRT;

namespace kp.Helpers;

[ComImport]
[Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IInitializeWithWindow
{
    void Initialize(IntPtr hwnd);
}
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("EECDBF0E-BAE9-4CB6-A68E-9598E1CB57BB")]
internal interface IWindowNative
{
    IntPtr WindowHandle
    {
        get;

    }
}

public static class FileSavePickerExtension
{
    public static void InitializeWithWindow(this FileSavePicker savePicker, WindowEx window)
    {
        var hwnd = window.As<IWindowNative>().WindowHandle;
        var initializeWithWindow = savePicker.As<IInitializeWithWindow>();
        initializeWithWindow.Initialize(hwnd);
    }
}
