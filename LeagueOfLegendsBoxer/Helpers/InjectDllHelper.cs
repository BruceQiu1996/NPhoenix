using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace LeagueOfLegendsBoxer.Helpers
{
    public class InjectDllHelper
    {
        public enum ThreadCreationFlags : UInt32
        {
            NORMAL = 0x0,
            CREATE_SUSPENDED = 0x00000004,
            STACK_SIZE_PARAM_IS_A_RESERVATION = 0x00010000
        }

        public enum AllocationType
        {
            NULL = 0x0,
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000
        }

        public enum MemoryProtection : UInt32
        {
            PAGE_EXECUTE = 0x00000010,
            PAGE_EXECUTE_READ = 0x00000020,
            PAGE_EXECUTE_READWRITE = 0x00000040,
            PAGE_EXECUTE_WRITECOPY = 0x00000080,
            PAGE_NOACCESS = 0x00000001,
            PAGE_READONLY = 0x00000002,
            PAGE_READWRITE = 0x00000004,
            PAGE_WRITECOPY = 0x00000008,
            PAGE_GUARD = 0x00000100,
            PAGE_NOCACHE = 0x00000200,
            PAGE_WRITECOMBINE = 0x00000400
        }

        [DllImport("kernelbase.dll")]
        public static extern IntPtr LoadLibrary(string DllFile);

        [DllImport("kernelbase.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr Module);

        [DllImport("kernelbase.dll")]
        public static extern bool CloseHandle(IntPtr hObject);
        [DllImport("kernelbase.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr call_OpenProces(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr call_VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate bool call_WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate uint call_NtCreateThreadEx(out IntPtr hThread,
                                                    uint DesiredAccess,
                                                    IntPtr ObjectAttributes,
                                                    IntPtr ProcessHandle,
                                                    IntPtr lpStartAddress,
                                                    IntPtr lpParameter,
                                                    bool CreateSuspended,
                                                    uint StackZeroBits,
                                                    uint SizeOfStackCommit,
                                                    uint SizeOfStackReserve,
                                                    IntPtr lpBytesBuffer);


        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);
        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, Int32 nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, ThreadCreationFlags dwCreationFlags, out IntPtr lpThreadId);

        public void Run(Process target, string binPath)
        {
            using (FileStream fs = new FileStream(binPath, FileMode.Open, FileAccess.Read))
            {
                UInt32 imageSize = (UInt32)fs.Length;
                BinaryReader br = new BinaryReader(fs);
                byte[] Xpayload = br.ReadBytes((int)fs.Length);
                // allocate some memory for our shellcode
                IntPtr pAddr = VirtualAllocEx(target.Handle, IntPtr.Zero, (UInt32)Xpayload.Length, AllocationType.Commit | AllocationType.Reserve, MemoryProtection.PAGE_EXECUTE_READWRITE);
                WriteProcessMemory(target.Handle, pAddr, Xpayload, Xpayload.Length, out IntPtr lpNumberOfBytesWritten);
                // create the remote thread
                IntPtr hThread = CreateRemoteThread(target.Handle, IntPtr.Zero, 0, pAddr, IntPtr.Zero, ThreadCreationFlags.NORMAL, out hThread);
            }
        }
    }
}