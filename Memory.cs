using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;
namespace AssaultCubeExternal
{
    internal static class Memory
    {

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] buffer, int size, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] buffer, int size, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        private static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);


        public static IntPtr ProcessHandle;
        public static Process process;

        private static int m_iBytesRead = 0;
        private static int m_iBytesWrite = 0;

        //https://docs.microsoft.com/en-us/windows/win32/memory/memory-protection-constants
        private const uint PAGE_EXECUTE_READWRITE = 0x40;
        public static void PatchEx(int Address, byte[] Write)
        {
            UIntPtr size = (UIntPtr)Write.Length;
            VirtualProtectEx(ProcessHandle, (IntPtr)Address, size, PAGE_EXECUTE_READWRITE, out uint oldprotect);
            WriteMemory<byte[]>(Address, Write);
            VirtualProtectEx(ProcessHandle, (IntPtr)Address, size, oldprotect, out uint _);
        }

        public static void SetProcess(Process _process)
        {
            process = _process;
            ProcessHandle = Memory.OpenProcess(0x0008 | 0x0010 | 0x0020, false, _process.Id);
        }

        public static void NopEx(int address, int size)
        {
            byte[] nopArray = Enumerable.Repeat((byte)0x90, size).ToArray();

            PatchEx(address, nopArray);
        }

                                                                                                             //         Processes should not have dlls with the same name anyway :p
        public static IntPtr GetModuleBaseAddress(string moduleName) => process.Modules.Cast<ProcessModule>().First(x => x.ModuleName.ToLower() == moduleName.ToLower()).BaseAddress;


        public static T ReadMemory<T>(int Adress) where T : struct
        {
            int ByteSize = Marshal.SizeOf(typeof(T));
            byte[] buffer = new byte[ByteSize];
            ReadProcessMemory((int)ProcessHandle, Adress, buffer, buffer.Length, ref m_iBytesRead);

            return ByteArrayToStructure<T>(buffer);
        }

        public static byte[] ReadMemoryBytes(int Adress, int bytesToRead)
        {
            byte[] buffer = new byte[bytesToRead];
            ReadProcessMemory((int)ProcessHandle, Adress, buffer, buffer.Length, ref m_iBytesRead);

            return buffer;
        }

        public static void WriteMemory<T>(int Adress, object Value)
        {
            //If 'value' is already a byte array, no need to convert it.
            byte[] buffer;
            if (typeof(T) == typeof(byte[]))
                buffer = (byte[])Value;
            else
                buffer = StructureToByteArray(Value);

            WriteProcessMemory((int)ProcessHandle, Adress, buffer, buffer.Length, out m_iBytesWrite);
        }

        public static string ReadString(int baseAddress, int size, Encoding textEncoding = null)
        {
            if (textEncoding == null)
                textEncoding = Encoding.ASCII;

            //create buffer for string
            byte[] buffer = new byte[size];


            ReadProcessMemory((int)ProcessHandle, baseAddress, buffer, size, ref m_iBytesWrite);

            //encode bytes to ASCII
            return textEncoding.GetString(buffer);
        }
        public static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                return (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                handle.Free();
            }
        }

        public static byte[] StructureToByteArray(object obj)
        {
            int len = Marshal.SizeOf(obj);

            byte[] arr = new byte[len];

            IntPtr ptr = Marshal.AllocHGlobal(len);

            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, arr, 0, len);
            Marshal.FreeHGlobal(ptr);

            return arr;
        }
    }
}