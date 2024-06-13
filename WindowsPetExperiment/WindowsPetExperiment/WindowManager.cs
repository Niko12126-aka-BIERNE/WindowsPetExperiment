using System.Runtime.InteropServices;

namespace WindowsPetExperiment
{
    internal class WindowManager
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd); // Check if window is minimized

        [DllImport("user32.dll")]
        private static extern bool IsZoomed(IntPtr hWnd); // Check if window is maximized

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public struct LINE(Point point1, Point point2)
        {
            public Point Point1 = point1;
            public Point Point2 = point2;
        }

        public static LINE? GetNewFocusedWindowTopLine()
        {
            IntPtr focusedWindowPtr = GetForegroundWindow();

            if (focusedWindowPtr == Form1.PetHandle || focusedWindowPtr == Form1.HomeHandle)
            {
                return null;
            }

            if (IsZoomed(focusedWindowPtr))
            {
                return null;
            }

            if (!IsWindowVisible(focusedWindowPtr) || IsIconic(focusedWindowPtr))
            {
                return null;
            }

            return GetTopLine(focusedWindowPtr);
        }

        public static LINE GetTopLine(IntPtr homeHandle)
        {
            GetWindowRect(homeHandle, out RECT focusedWindowRectangle);

            LINE topLine = new(new Point(focusedWindowRectangle.Left, focusedWindowRectangle.Top), new Point(focusedWindowRectangle.Right, focusedWindowRectangle.Top));

            return topLine;
        }
    }
}
