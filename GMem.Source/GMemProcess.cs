using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;

public class GMemProcess
{
    private string g_processName;
    private string g_moduleName;
    private sbyte g_bytesRead;

    /// <summary>
    /// Constructs a GMemProcess Object
    /// </summary>
    /// <param name="processName">Process name without .exe</param>
    /// <param name="moduleName">Process module name that you want to read from(type process name if main module)</param>
    public GMemProcess(string processName, string moduleName)
    {
        g_processName = processName;
        g_moduleName = moduleName;
    }

    [DllImport("kernel32")]
    private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int nSize, ref sbyte lpNumberOfBytesRead);
    [DllImport("kernel32")]
    private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, ref sbyte lpNumberOfBytesRead);
    /// <summary>
    /// Creates a ptrObject instance to be used by read<T> function.
    /// </summary>
    /// <param name="ptrAddress">Hex address to read</param>
    /// <param name="ptrOffsets">Offsets(leave null here if there no offsets)</param>
    /// <returns>Ready to use ptrObject</returns>
    public ptrObject create_ptr_object(int ptrAddress, int[] ptrOffsets)
    {
        // Get the processes that has the given name
        Process[] processList = Process.GetProcessesByName(g_processName);
        if (processList.Length != 0)
        {
            Process gmProcess = processList[0];
            ProcessModule gmModule;
            // If the module name and process name is the same, its just main module we don't need to call get_module_by_name
            if (g_processName == g_moduleName)
                gmModule = gmProcess.MainModule;
            else
                gmModule = get_module_by_name(gmProcess);
            if (gmModule != null)
            {
                // No need to calculate anything if its a direct address.
                if (ptrOffsets != null)
                {
                    byte[] ptrBuffer = new byte[sizeof(int)];
                    /*
                    A very simple explanation of what is going on here, 
                    to reach the address that holds the value, you need to execute; 
                    1) modulebaseaddress + pointer address = calculated address
                    2) for every offset, read calculated address and add the offset to the value that you get from read.
                    Final result is the address that holds the value.
                    */
                    int calculatedAdr = gmModule.BaseAddress.ToInt32() + ptrAddress;
                    foreach (int offset in ptrOffsets)
                    {
                        ReadProcessMemory(gmProcess.Handle, (IntPtr)calculatedAdr, ptrBuffer, sizeof(int), ref g_bytesRead);
                        calculatedAdr = BitConverter.ToInt32(ptrBuffer, 0) + offset;
                    }
                    return new ptrObject() { calculatedAddress = (IntPtr)calculatedAdr, processHandle = gmProcess.Handle };
                }
            }
            else
                return new ptrObject() { calculatedAddress = (IntPtr)ptrAddress, processHandle = gmProcess.Handle };
        }
        return null;
    }
    /// <summary>
    /// Read the address and converts it to the given data type.
    /// </summary>
    /// <typeparam name="T">Returning Type</typeparam>
    /// <param name="ptrObj">Calculated ptrObject</param>
    /// <returns>Value that the given address holds.</returns>
    public T read<T>(ptrObject ptrObj)
    {
        // Get the size of T.
        int sizeoft = Marshal.SizeOf(typeof(T));
        byte[] dataBuffer = new byte[sizeoft];
        ReadProcessMemory(ptrObj.processHandle, ptrObj.calculatedAddress, dataBuffer, sizeoft, ref g_bytesRead);
        // Check here for more info about this if statement https://msdn.microsoft.com/en-us/library/system.bitconverter.islittleendian(v=vs.110).aspx
        if (!BitConverter.IsLittleEndian)
            Array.Reverse(dataBuffer);
        // Rest just returning converted data.
        if (typeof(T) == typeof(int))
            return (T)(object)BitConverter.ToInt32(dataBuffer, 0);
        else if (typeof(T) == typeof(float))
            return (T)(object)BitConverter.ToSingle(dataBuffer, 0);
        else if (typeof(T) == typeof(double))
            return (T)(object)BitConverter.ToDouble(dataBuffer, 0);
        else if (typeof(T) == typeof(short))
            return (T)(object)BitConverter.ToInt16(dataBuffer, 0);
        else if (typeof(T) == typeof(long))
            return (T)(object)BitConverter.ToInt64(dataBuffer, 0);
        else if (typeof(T) == typeof(byte))
            return (T)(object)dataBuffer[0];
        throw new InvalidCastException("The data type you have entered is not valid.");
    }

    /// <summary>
    /// Read the address and converts it to the given data type.
    /// </summary>
    /// <typeparam name="T">Returning Type</typeparam>
    /// <param name="ptrObj">Calculated ptrObject <see cref="create_ptr_object(int, int[])"/> function.</param>
    /// <param name="length">Size of Array</param>
    /// <returns>Value that the given address holds.</returns>
    public T read<T>(ptrObject ptrObj, int length)
    {
        // Since we need length to read strings or AOB's, i needed to seperate these types here in an overload.
        byte[] dataBuffer = new byte[length];
        ReadProcessMemory(ptrObj.processHandle, ptrObj.calculatedAddress, dataBuffer, dataBuffer.Length, ref g_bytesRead);
        if (!BitConverter.IsLittleEndian)
            Array.Reverse(dataBuffer);
        if (typeof(T) == typeof(string))
            return (T)(object)Encoding.UTF8.GetString(dataBuffer);
        else if (typeof(T) == typeof(byte[]))
            return (T)(object)dataBuffer;
        /* string[] is completely arbitrary. Its literally just the same as byte[] 
           but instead of numeric bytes you see hexadecimal values as strings.
         */
        else if (typeof(T) == typeof(string[])) 
            return (T)(object)(BitConverter.ToString(dataBuffer).Split('-'));
        throw new InvalidOperationException("The data type you have entered is not valid.");
    }

    /// <summary>
    /// Writes the given value to the address that was calculated.
    /// </summary>
    /// <typeparam name="T">Data type to write</typeparam>
    /// <param name="ptrObj">Calculated ptrObject <see cref="create_ptr_object(int, int[])"/> function.</param>
    /// <param name="value">Data to write.</param>
    /// <returns>True on success.</returns>
    public bool write<T>(ptrObject ptrObj, object value)
    {
        int sizeoft;
        if (typeof(T) == typeof(string)) sizeoft = value.ToString().Length;
        else if (typeof(T) == typeof(byte[])) sizeoft = ((byte[])value).Length;
        else if (typeof(T) == typeof(string[])) sizeoft = ((string[])value).Length;
        else sizeoft = Marshal.SizeOf(typeof(T));
        return WriteProcessMemory(ptrObj.processHandle, ptrObj.calculatedAddress, prepWriteData<T>(value), sizeoft, ref g_bytesRead);
    }

    private byte[] prepWriteData<T>(object value)
    {
        // This functions works great but looks really bad. Will do something about it after i get everything done.
        if (typeof(T) == typeof(int))
            return BitConverter.IsLittleEndian ? BitConverter.GetBytes(Convert.ToInt32(value)) : BitConverter.GetBytes(Convert.ToInt32(value)).Reverse().ToArray();
        else if (typeof(T) == typeof(float))
            return BitConverter.IsLittleEndian ? BitConverter.GetBytes(Convert.ToSingle(value)) : BitConverter.GetBytes(Convert.ToSingle(value)).Reverse().ToArray();
        else if (typeof(T) == typeof(double))
            return BitConverter.IsLittleEndian ? BitConverter.GetBytes(Convert.ToDouble(value)) : BitConverter.GetBytes(Convert.ToDouble(value)).Reverse().ToArray();
        else if (typeof(T) == typeof(byte))
            return BitConverter.IsLittleEndian ? BitConverter.GetBytes(Convert.ToByte(value)) : BitConverter.GetBytes(Convert.ToByte(value)).Reverse().ToArray();
        else if (typeof(T) == typeof(string))
            return BitConverter.IsLittleEndian ? Encoding.UTF8.GetBytes(value.ToString()) : Encoding.UTF8.GetBytes(value.ToString()).Reverse().ToArray();
        else if (typeof(T) == typeof(byte[]))
            return BitConverter.IsLittleEndian ? (byte[])value : ((byte[])value).Reverse().ToArray();
        else if (typeof(T) == typeof(string[]))
            return BitConverter.IsLittleEndian ? stringToAOB((string[])value) : stringToAOB((string[])value).Reverse().ToArray();
        else
            throw new InvalidCastException("The data type you have entered is not valid.");
    }

    private byte[] stringToAOB(string[] dataBuffer)
    {
        byte[] fixedBytes = new byte[dataBuffer.Length];
        for (int i = 0; i < dataBuffer.Length; i++) fixedBytes[i] = Convert.ToByte(dataBuffer[i], 16);
        return fixedBytes;
    }

    private ProcessModule get_module_by_name(Process prcs)
    {
        foreach (ProcessModule item in prcs.Modules)
        {
            if (item.ModuleName == g_moduleName)
                return item;
        }
        return null;
    }
}