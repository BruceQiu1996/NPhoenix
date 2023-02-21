using System;
using System.IO;
using System.Runtime.InteropServices;

namespace LeagueOfLegendsBoxer.Helpers
{
    public class InjectDllHelper
    {
        const int All = 0x001F0FFF;
        [Flags]
        public enum AllocationType
        {
            Commit = 0x00001000
        }

        [Flags]
        public enum MemoryProtection
        {
            ExecuteReadWrite = 0x0040
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
        private delegate uint call_NtCreateThreadEx(out IntPtr hThread, uint DesiredAccess, IntPtr ObjectAttributes, IntPtr ProcessHandle, IntPtr lpStartAddress, IntPtr lpParameter, bool CreateSuspended, uint StackZeroBits, uint SizeOfStackCommit, uint SizeOfStackReserve, IntPtr lpBytesBuffer);

        public void Inject(string binPath, int processId)
        {
            using (FileStream fs = new FileStream(binPath, FileMode.Open, FileAccess.Read))
            {
                UInt32 imageSize = (UInt32)fs.Length;
                BinaryReader br = new BinaryReader(fs);
                byte[] Xpayload = br.ReadBytes((int)fs.Length);
                IntPtr DLLFile = LoadLibrary(@"c:\\windows\\system32\\kernelbase.dll");
                IntPtr DLLFileNt = LoadLibrary(@"c:\windows\system32\ntdll.dll");

                /// step1
                IntPtr FunctionCall_01 = GetProcAddress(DLLFile, "OpenProcess");
                call_OpenProces FunctionCall_01_Del = (call_OpenProces)Marshal.GetDelegateForFunctionPointer(FunctionCall_01, typeof(call_OpenProces));
                IntPtr Result_01 = FunctionCall_01_Del(All, false, processId);
                /// step2
                IntPtr FunctionCall_02 = GetProcAddress(DLLFile, "VirtualAllocEx");
                call_VirtualAllocEx FunctionCall_02_Del = (call_VirtualAllocEx)Marshal.GetDelegateForFunctionPointer(FunctionCall_02, typeof(call_VirtualAllocEx));
                IntPtr Result_02 = FunctionCall_02_Del(Result_01, IntPtr.Zero, (uint)Xpayload.Length, AllocationType.Commit, MemoryProtection.ExecuteReadWrite);
                /// step3
                UIntPtr _out = UIntPtr.Zero;
                IntPtr FunctionCall_03 = GetProcAddress(DLLFile, "WriteProcessMemory");
                call_WriteProcessMemory FunctionCall_03_Del = (call_WriteProcessMemory)Marshal.GetDelegateForFunctionPointer(FunctionCall_03, typeof(call_WriteProcessMemory));
                bool Result_03 = FunctionCall_03_Del(Result_01, Result_02, Xpayload, (uint)Xpayload.Length, out _out);
                /// step4
                /// NTDLL.DLL API  
                uint Result_04_1 = 0;
                IntPtr ops = IntPtr.Zero;
                IntPtr FunctionCall_04 = GetProcAddress(DLLFileNt, "NtCreateThreadEx");
                call_NtCreateThreadEx FunctionCall_04_Del = (call_NtCreateThreadEx)Marshal.GetDelegateForFunctionPointer(FunctionCall_04, typeof(call_NtCreateThreadEx));
                Result_04_1 = FunctionCall_04_Del(out ops, 0x1FFFFF, IntPtr.Zero, Result_01, Result_02, IntPtr.Zero, false, 0, 0, 0, IntPtr.Zero);

                // CloseHandle(Result_04);
                CloseHandle(Result_01);
                FreeLibrary(DLLFile);
                FreeLibrary(DLLFileNt);
            }
        }
    }
}
