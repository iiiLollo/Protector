using Il2CppInterop.Runtime;
using System;
using Unity.Entities;
using System.Runtime.InteropServices;
namespace Protector.Helpers
{
    internal static class StreamingServices
    {


        public unsafe static void Write<T>(this Entity entity, T componentData) where T : struct
        {
            // Get the ComponentType for T
            var ct = new ComponentType(Il2CppType.Of<T>());

            // Marshal the component data to a byte array
            byte[] byteArray = StructureToByteArray(componentData);

            // Get the size of T
            int size = Marshal.SizeOf<T>();

            // Create a pointer to the byte array
            fixed (byte* p = byteArray)
            {
                // Set the component data
                Core.EntityManager.SetComponentDataRaw(entity, ct.TypeIndex, p, size);
            }
        }

        public static byte[] StructureToByteArray<T>(T structure) where T : struct
        {
            int size = Marshal.SizeOf(structure);
            byte[] byteArray = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(structure, ptr, true);
            Marshal.Copy(ptr, byteArray, 0, size);
            Marshal.FreeHGlobal(ptr);

            return byteArray;
        }
    }
}
