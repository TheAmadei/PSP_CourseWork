using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Controller
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Win32Control : HwndHost
    {
        /// <summary>
        /// Уникальный номер экземпляра окна программы.
        /// </summary>
        protected IntPtr Hwnd { get; private set; }
        /// <summary>
        /// Инициализирование Hwnd.
        /// </summary>
        protected bool HwndInitialized { get; private set; }

        private const string WindowClass = "HwndWrapper";
        /// <summary>
        /// Конструктор класса Win32Control.
        /// </summary>
        protected Win32Control()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }
        /// <summary>
        /// Метод, при загрузке окна.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="routedEventArgs"></param>
        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Initialize();
            HwndInitialized = true;

            Loaded -= OnLoaded;
        }
        /// <summary>
        /// Метод, при зактрытии окна.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="routedEventArgs"></param>
        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Uninitialize();
            HwndInitialized = false;

            Unloaded -= OnUnloaded;

            Dispose();
        }
        /// <summary>
        /// Инициализирование.
        /// </summary>
        protected abstract void Initialize();
        /// <summary>
        /// Деинициализирование.
        /// </summary>
        protected abstract void Uninitialize();
        /// <summary>
        /// Изменение.
        /// </summary>
        protected abstract void Resized();
        /// <summary>
        /// Выстраивание главного окна.
        /// </summary>
        /// <param name="hwndParent"></param>
        /// <returns></returns>
        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            var wndClass = new NativeMethods.WndClassEx();
            wndClass.cbSize = (uint)Marshal.SizeOf(wndClass);
            wndClass.hInstance = NativeMethods.GetModuleHandle(null);
            wndClass.lpfnWndProc = NativeMethods.DefaultWindowProc;
            wndClass.lpszClassName = WindowClass;
            wndClass.hCursor = NativeMethods.LoadCursor(IntPtr.Zero, NativeMethods.IDC_ARROW);
            NativeMethods.RegisterClassEx(ref wndClass);

            Hwnd = NativeMethods.CreateWindowEx(
                0, WindowClass, "", NativeMethods.WS_CHILD | NativeMethods.WS_VISIBLE,
                0, 0, (int)Width, (int)Height, hwndParent.Handle, IntPtr.Zero, IntPtr.Zero, 0);

            return new HandleRef(this, Hwnd);
        }
        /// <summary>
        /// При закрытии окна.
        /// </summary>
        /// <param name="hwnd"></param>
        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            NativeMethods.DestroyWindow(hwnd.Handle);
            Hwnd = IntPtr.Zero;
        }
        /// <summary>
        /// При изменении размеров рендера.
        /// </summary>
        /// <param name="sizeInfo"></param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            UpdateWindowPos();

            base.OnRenderSizeChanged(sizeInfo);

            if (HwndInitialized)
                Resized();
        }
    }
    /// <summary>
    /// Собственный метод.
    /// </summary>
    internal class NativeMethods
    {
        public const int WS_CHILD = 0x40000000;
        public const int WS_VISIBLE = 0x10000000;

        public const int IDC_ARROW = 32512;

        [StructLayout(LayoutKind.Sequential)]
        public struct WndClassEx
        {
            public uint cbSize;
            public uint style;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public WndProc lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            public string lpszMenuName;
            public string lpszClassName;
            public IntPtr hIconSm;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        public delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        public static readonly WndProc DefaultWindowProc = DefWindowProc;

        [DllImport("user32.dll", EntryPoint = "CreateWindowEx", CharSet = CharSet.Auto)]
        public static extern IntPtr CreateWindowEx(
            int exStyle,
            string className,
            string windowName,
            int style,
            int x, int y,
            int width, int height,
            IntPtr hwndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            [MarshalAs(UnmanagedType.AsAny)] object pvParam);

        [DllImport("user32.dll", EntryPoint = "DestroyWindow", CharSet = CharSet.Auto)]
        public static extern bool DestroyWindow(IntPtr hwnd);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string module);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.U2)]
        public static extern short RegisterClassEx([In] ref WndClassEx lpwcx);

        [DllImport("user32.dll")]
        public static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);
        // ReSharper restore InconsistentNaming
    }
}
