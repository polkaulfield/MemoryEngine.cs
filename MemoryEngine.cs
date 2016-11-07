using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace MemoryEngine
{
    /// <summary>
    /// TODO: Add documentation here, more info at https://msdn.microsoft.com/en-us/library/b2s063f7.aspx
    /// </summary>
    public class MemoryEngine
    {
        /* CONSTANTS
         *********************************************************************************************/

        // TODO: Minimal comment of this constant   
        private const int PROCESS_ALL_ACCESS = 0x1F0FFF;

        /* IMPORTS
         *********************************************************************************************/

        // TODO: Minimal comment about this method
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(
            int dwDesiredAccess,
            bool bInheritHandle,
            int dwProcessId
        );

        // TODO: Minimal comment about this method
        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(
            int hProcess,
            int lpBaseAddress,
            byte[] lpBuffer,
            int dwSize,
            ref int lpNumberOfBytesRead
        );

        // TODO: Minimal comment about this method
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        // TODO: Minimal comment about this method
        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(
            int hProcess,
            int lpBaseAddress,
            byte[] lpBuffer,
            int dwSize,
            ref int lpNumberOfBytesWritten
        );

        /* FIELDS
         *********************************************************************************************/

        //TODO: Minimal comment of usage
        private int _processHandle;

        //TODO: Minimal comment of usage
        private Process _hookedProcess;

        /* CONSTRUCTOR
         *********************************************************************************************/

        /// <summary>
        /// TODO: Comment of ussage
        /// <param name="pid">TODO: Means of this parameter</param>
        /// </summary>
        public void Hook(int pid)
        {
            _processHandle = OpenProcess(PROCESS_ALL_ACCESS, false, pid).ToInt32();
            _hookedProcess = Process.GetProcessById(pid);
        }

        /* PUBLIC METHODS
         *********************************************************************************************/

        /// <summary>
        /// TODO: Comment of ussage
        /// <param name="address">TODO: Means of this parameter</param>
        /// <param name="bytes">TODO: Means of this parameter</param>
        /// <return>TODO: what this return</return>
        /// </summary>
        public byte[] ReadAddress(int address, int bytes)
        {
            byte[] _buffer = new byte[bytes];
            int _bytesRead = 0;

            ReadProcessMemory(_processHandle, address, _buffer, bytes, ref _bytesRead);

            return _buffer;
        }

        /// <summary>
        /// TODO: Comment of ussage
        /// <param name="address">TODO: Means of this parameter</param>
        /// <param name="offset">TODO: Means of this parameter</param>
        /// <return>TODO: what this return</return>
        /// </summary>
        public int GetAddressFromOffset(int address, int offset)
        {
            byte[] _buffer = new byte[4];

            _buffer = ReadAddress(address + offset, 4);
            address = BitConverter.ToInt32(_buffer, 0);

            return address;
        }

        /// <summary>
        /// TODO: Comment of ussage
        /// <param name="address">TODO: Means of this parameter</param>
        /// <param name="offsets">TODO: Means of this parameter</param>
        /// <return>TODO: what this return</return>
        /// </summary>
        public int GetAddressFromOffsets(int address, int[] offsets)
        {
            byte[] _buffer = new byte[4];

            for (int i = 0; i < offsets.Length; i++)
            {
                _buffer = ReadAddress(address + offsets[i], 4);
                address = BitConverter.ToInt32(_buffer, 0);
            }

            return address;
        }

        /// <summary>
        /// TODO: Comment of ussage
        /// <param name="address">TODO: Means of this parameter</param>
        /// <param name="bytes">TODO: Means of this parameter</param>
        /// <return>TODO: what this return</return>
        /// </summary>
        public int ReadInt(int address, int bytes)
        {
            byte[] _buffer = new byte[bytes];
            int _bytesRead = 0;

            ReadProcessMemory(_processHandle, address, _buffer, bytes, ref _bytesRead);

            return BitConverter.ToInt32(_buffer, 0);
        }

        /// <summary>
        /// TODO: Comment of ussage
        /// <param name="address">TODO: Means of this parameter</param>
        /// <param name="size">TODO: Means of this parameter</param>
        /// <return>TODO: what this return</return>
        /// </summary>
        public string ReadStringUnicode(int address, int size)
        {
            byte[] _buffer = ReadAddress(address, size);


            //trimend to remove extra null characters.
            //Just remember that all strings on memory representation ends with \o character
            string unicode = Encoding.Unicode.GetString(_buffer);
            return unicode.Substring(0, unicode.IndexOf('\0'));
        }

        /// <summary>
        /// TODO: Comment of ussage
        /// <param name="address">TODO: Means of this parameter</param>
        /// <param name="input">TODO: Means of this parameter</param>
        /// <param name="size">TODO: Means of this parameter</param>
        /// </summary>
        public void WriteStringUnicode(int address, string input, int size)
        {
            byte[] _buffer = Encoding.Unicode.GetBytes(input);
            int _bytesWritten = 0;

            WriteProcessMemory(_processHandle, address, _buffer, size, ref _bytesWritten);
        }

        /// <summary>
        /// TODO: Comment of ussage
        /// <param name="address">TODO: Means of this parameter</param>
        /// <param name="input">TODO: Means of this parameter</param>
        /// <param name="size">TODO: Means of this parameter</param>
        /// </summary>
        public void WriteInt(int address, int input, int size)
        {
            byte[] _buffer = BitConverter.GetBytes(input);
            int _bytesWritten = 0;

            WriteProcessMemory(_processHandle, address, _buffer, size, ref _bytesWritten);
        }

        /// <summary>
        /// TODO: Comment of ussage
        /// <param name="address">TODO: Means of this parameter</param>
        /// <param name="input">TODO: Means of this parameter</param>
        /// </summary>
        public void WriteFloat(int address, float input)
        {
            byte[] _buffer = BitConverter.GetBytes(input);
            int _bytesWritten = 0;

            WriteProcessMemory(_processHandle, address, _buffer, _buffer.Length, ref _bytesWritten);
        }

        /// <summary>
        /// TODO: Comment of ussage
        /// <param name="moduleName">TODO: Means of this parameter</param>
        /// <return>TODO: what this return</return>
        /// </summary>
        public int GetModuleAddress(string moduleName)
        {
            foreach (ProcessModule module in _hookedProcess.Modules)
            {
                if (module.ModuleName == moduleName)
                {
                    return module.BaseAddress.ToInt32();
                }
            }

            return 0; //Default return
        }
    }
}
