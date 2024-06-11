using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
