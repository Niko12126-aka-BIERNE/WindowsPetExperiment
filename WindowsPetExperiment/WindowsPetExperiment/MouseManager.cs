using System.Runtime.InteropServices;

namespace WindowsPetExperiment
{
    internal class MouseManager
    {
        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out Point lpPoint);

        public static Point GetMouseLocation()
        {
            GetCursorPos(out Point lpPoint);

            return lpPoint;
        }
    }
}
