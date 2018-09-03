using System.Diagnostics;
using System.Linq;

namespace Helpers
{
    public static class Globals
    {
        private static Process process;
        public static Process Process => process ?? (process = Process.GetProcessesByName("Wow").FirstOrDefault());
    }
}
