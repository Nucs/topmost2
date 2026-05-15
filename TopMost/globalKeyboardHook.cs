using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Utilities
{
    /// <summary>
    /// A class that manages a global low level keyboard hook.
    /// </summary>
    public class GlobalKeyboardHook : IDisposable
    {
        private delegate IntPtr KeyboardHookProc(int code, IntPtr wParam, IntPtr lParam);

        #pragma warning disable CS0649
        [StructLayout(LayoutKind.Sequential)]
        private struct KeyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public IntPtr dwExtraInfo;
        }
        #pragma warning restore CS0649

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_SYSKEYUP = 0x105;

        private IntPtr hhook = IntPtr.Zero;
        private KeyboardHookProc callbackDelegate;
        private bool disposed;

        public event KeyEventHandler KeyDown;
        public event KeyEventHandler KeyUp;

        public GlobalKeyboardHook()
        {
            Hook();
        }

        ~GlobalKeyboardHook()
        {
            Dispose(false);
        }

        public void Hook()
        {
            if (callbackDelegate != null)
                throw new InvalidOperationException("Can't hook more than once");

            callbackDelegate = HookProc;

            using (Process currentProcess = Process.GetCurrentProcess())
            using (ProcessModule currentModule = currentProcess.MainModule)
            {
                IntPtr moduleHandle = GetModuleHandle(currentModule.ModuleName);
                hhook = SetWindowsHookEx(WH_KEYBOARD_LL, callbackDelegate, moduleHandle, 0);
            }

            if (hhook == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        public void Unhook()
        {
            if (callbackDelegate == null)
                return;

            if (hhook != IntPtr.Zero && !UnhookWindowsHookEx(hhook))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            hhook = IntPtr.Zero;
            callbackDelegate = null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            try
            {
                Unhook();
            }
            catch
            {
                if (disposing)
                    throw;
            }

            disposed = true;
        }

        private IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code >= 0)
            {
                KeyboardHookStruct hookInfo = Marshal.PtrToStructure<KeyboardHookStruct>(lParam);
                Keys key = (Keys)hookInfo.vkCode;
                KeyEventArgs keyEventArgs = new KeyEventArgs(key);
                int message = wParam.ToInt32();

                if (message == WM_KEYDOWN || message == WM_SYSKEYDOWN)
                {
                    KeyDown?.Invoke(this, keyEventArgs);
                }
                else if (message == WM_KEYUP || message == WM_SYSKEYUP)
                {
                    KeyUp?.Invoke(this, keyEventArgs);
                }

                if (keyEventArgs.Handled)
                    return new IntPtr(1);
            }

            return CallNextHookEx(hhook, code, wParam, lParam);
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
