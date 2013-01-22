using System.Diagnostics;
using System.Reflection;

namespace Core.Utils
{
    public static class RuntimeInfo
    {
        private static int processId;
        private static string processName;

        private static string assemblyName;

        static RuntimeInfo()
        {
            UpdateRuntimeInfo();
        }

        public static int ProcessId
        {
            get
            {
                return processId;
            }
        }

        public static string ProcessName
        {
            get
            {
                return processName;
            }
        }

        public static string AssemblyName
        {
            get
            {
                return assemblyName;
            }
        }

        private static void UpdateRuntimeInfo()
        {
            if (processName == null)
            {
                Process process = Process.GetCurrentProcess();
                processId = process.Id;
                processName = process.ProcessName;
            }

            if (assemblyName == null)
            {
                Assembly assembly = Assembly.GetEntryAssembly();
                assemblyName = System.IO.Path.GetFileName(assembly.Location);
            }
        }
    }
}
