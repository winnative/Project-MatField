using Microsoft.UI;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Project_MatField.Kernel
{
    public static class Win32Kernel
    {
        public enum BackDropType
        {
            Mica,
            MicaAlt,
            Acrylic
        }

        public static double GetScaleFactor(this Window window)
        {
            var effectiveDpi = GetDpiForWindow(GetWindowHandleForCurrentWindow(window));
            var scaleFactor = effectiveDpi / 96.0;
            return scaleFactor;
        }

        public static void ResizeAndMove(this Window window, int width, int height, int x = 0, int y = 0)
        {
            IntPtr hWnd = GetWindowHandleForCurrentWindow(window);
            var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            var effectiveDpi = GetDpiForWindow(hWnd);
            var scaleFactor = effectiveDpi / 96.0;

            if (width is 0 && height is 0)
            {
                width = 800;
                height = 600;
            }

            width = (int)(width * scaleFactor);
            height = (int)(height * scaleFactor);

            if (x is 0 && y is 0)
            {
                appWindow.Resize(new(width, height));
                GoToCenterScreen(window);
            }
            else
            {
                appWindow.MoveAndResize(new(x, y, width, height));
            }
        }
        public static void ChangeFlowDirection(WindowsDirectionLayout layout)
        {
            SetProcessDefaultLayout((IntPtr)layout);
        }

        public static void ApplyMinSize(this Window window, int minHeight = 600, int minWidth = 840, int maxHeight = 0, int maxWidth = 0)
        {
            MinWindowHeight = minHeight;
            MinWindowWidth = minWidth;
            MaxWindowHeight = maxHeight;
            MaxWindowWidth = maxWidth;
            RegisterWindowMinMax(window);
        }

        public static void Collect()
        {
            GC.Collect(GC.MaxGeneration);
            GC.WaitForPendingFinalizers();
            SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle,
                (UIntPtr)0xFFFFFFFF, (UIntPtr)0xFFFFFFFF);
        }

        private static WinProc newWndProc = null;
        private static IntPtr oldWndProc = IntPtr.Zero;
        private delegate IntPtr WinProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll")]
        internal static extern int GetDpiForWindow(IntPtr hwnd);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetProcessWorkingSetSize(IntPtr process,
        UIntPtr minimumWorkingSetSize, UIntPtr maximumWorkingSetSize);

        [DllImport("User32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(IntPtr hWnd, WindowLongIndexFlags nIndex, WinProc newProc);

        [DllImport("User32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, WindowLongIndexFlags nIndex, WinProc newProc);

        [DllImport("User32.dll")]
        private static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll")]
        internal static extern bool SetProcessDefaultLayout(IntPtr dwDefaultLayout);

        [DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#138")]
        public static extern bool ShouldSystemUseDarkMode();

        public static int MinWindowWidth { get; set; } = 500;
        public static int MinWindowHeight { get; set; } = 600;
        public static int MaxWindowWidth { get; set; } = 0;
        public static int MaxWindowHeight { get; set; } = 0;

        private static void RegisterWindowMinMax(Window window)
        {
            var hwnd = GetWindowHandleForCurrentWindow(window);

            newWndProc = new WinProc(WndProc);
            oldWndProc = SetWindowLongPtr(hwnd, WindowLongIndexFlags.GWL_WNDPROC, newWndProc);
        }

        private static IntPtr GetWindowHandleForCurrentWindow(object target) =>
            WinRT.Interop.WindowNative.GetWindowHandle(target);

        private static IntPtr WndProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam)
        {
            switch (Msg)
            {
                case WindowMessage.WM_GETMINMAXINFO:
                    var dpi = GetDpiForWindow(hWnd);
                    var scalingFactor = (float)dpi / 96;

                    var minMaxInfo = Marshal.PtrToStructure<MINMAXINFO>(lParam);
                    minMaxInfo.ptMinTrackSize.x = (int)(MinWindowWidth * scalingFactor);
                    minMaxInfo.ptMinTrackSize.y = (int)(MinWindowHeight * scalingFactor);
                    if (MaxWindowHeight is not 0 && MaxWindowWidth is not 0)
                    {
                        minMaxInfo.ptMaxTrackSize.x = (int)(MaxWindowWidth * scalingFactor);
                        minMaxInfo.ptMaxTrackSize.y = (int)(MaxWindowHeight * scalingFactor);
                    }

                    Marshal.StructureToPtr(minMaxInfo, lParam, true);
                    break;

            }
            return CallWindowProc(oldWndProc, hWnd, Msg, wParam, lParam);
        }

        private static IntPtr SetWindowLongPtr(IntPtr hWnd, WindowLongIndexFlags nIndex, WinProc newProc)
        {
            if (IntPtr.Size == 8)
                return SetWindowLongPtr64(hWnd, nIndex, newProc);
            else
                return new IntPtr(SetWindowLong32(hWnd, nIndex, newProc));
        }

        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        }

        [Flags]
        private enum WindowLongIndexFlags : int
        {
            GWL_WNDPROC = -4,
        }

        private enum WindowMessage : int
        {
            WM_GETMINMAXINFO = 0x0024,
        }

        public enum WindowsDirectionLayout : int
        {
            LAYOUT_RTL = 0x00000001,
            LAYOUT_LTR = 0
        }

        public static void GoToCenterScreen(this Window window)
        {
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            Microsoft.UI.Windowing.DisplayArea displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(windowId, Microsoft.UI.Windowing.DisplayAreaFallback.Nearest);
            if (displayArea is not null)
            {
                var CenteredPosition = appWindow.Position;
                CenteredPosition.X = ((displayArea.WorkArea.Width - appWindow.Size.Width) / 2);
                CenteredPosition.Y = ((displayArea.WorkArea.Height - appWindow.Size.Height) / 2);
                appWindow.Move(CenteredPosition);
            }
        }

        public static void SetIcon(this Window win)
        {
            IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(win);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            appWindow.SetIcon(@"Images\eenoLogo.ico");
        }
    }
}
