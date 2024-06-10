using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using static WindowsPetExperiment.WindowManager;
using System.Runtime.CompilerServices;

namespace WindowsPetExperiment
{
    internal class WindowManager
    {
        public static IntPtr FocusedWindowPtr { get; private set; }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsIconic(IntPtr hWnd); // Check if window is minimized

        // Define the RECT structure to represent a rectangle
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

            if (focusedWindowPtr == Form1.PetHandle)
            {
                return null;
            }

            if (focusedWindowPtr == FocusedWindowPtr)
            {
                return null;
            }

            FocusedWindowPtr = focusedWindowPtr;

            if (!IsWindowVisible(focusedWindowPtr) || IsIconic(focusedWindowPtr))
            {
                return null;
            }

            GetWindowRect(focusedWindowPtr, out RECT focusedWindowRectangle);

            LINE topLine = new(new Point(focusedWindowRectangle.Left, focusedWindowRectangle.Top), new Point(focusedWindowRectangle.Right, focusedWindowRectangle.Top));

            return topLine;
        }
    }
}
