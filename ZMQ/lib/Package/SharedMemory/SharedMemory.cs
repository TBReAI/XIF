/*
**  XIF SharedMemory.cs
*/

using System;
using System.IO;
using System.IO.MemoryMappedFiles;

using System.Runtime.InteropServices;

namespace XIF
{
    public class SharedMemory
    {
        private long? mmfSize;
        private MemoryMappedFile? mmf;
        private MemoryMappedViewAccessor? accessor;
        private string fileName;

        public SharedMemory(string file)
        {
            mmfSize = null;
            mmf = null;
            accessor = null;
            
            fileName = file;
        }

        public unsafe void WriteBuffer(byte* dataPtr, long dataLength)
        {
            CreateMmf(dataLength);

            /* Get access to shared memory through the accessor */
            byte* sharedMemoryPtr = null;

            try
            {
                /* Lock the memory and get a pointer to the shared memory */
                accessor!.SafeMemoryMappedViewHandle.AcquirePointer(ref sharedMemoryPtr);

                if (sharedMemoryPtr == null)
                {
                    throw new InvalidOperationException("Failed to acquire shared memory pointer.");
                }

                /* Copy data directly from the native pointer to shared memory */
                Buffer.MemoryCopy(dataPtr, sharedMemoryPtr, dataLength, dataLength);
            }
            finally
            {
                /* Release the pointer after copying */
                if (sharedMemoryPtr != null)
                {
                    accessor!.SafeMemoryMappedViewHandle.ReleasePointer();
                }
            }
        }

        /* Read image data from shared memory but uses opencv2 so can't use in Unity */
        public unsafe byte* GetSharedMemoryPointer(long dataLength)
        {
            OpenMmf(dataLength);

            /* Get access to shared memory through the accessor */
            byte* sharedMemoryPtr = null;

            /* Lock the memory and get a pointer to the shared memory */
            accessor!.SafeMemoryMappedViewHandle.AcquirePointer(ref sharedMemoryPtr);

            if (sharedMemoryPtr == null)
            {
                throw new InvalidOperationException("Failed to acquire shared memory pointer.");
            }

            return sharedMemoryPtr;
        }

        public unsafe void ReleaseSharedMemoryPointer()
        {
            /* Release the pointer after reading */
            accessor!.SafeMemoryMappedViewHandle.ReleasePointer();
        }
   
        private void CreateMmf(long memorySize)
        {
            if (mmf == null || memorySize != mmfSize)
            {
               string filePath = GetCrossPlatformFilePath(fileName);

                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.OpenOrCreate, fileName, memorySize);
                }
                else
                {
                    mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.OpenOrCreate, null, memorySize);
                }
                
                accessor = mmf.CreateViewAccessor();

                mmfSize = memorySize;
            }
        }

        private void OpenMmf(long memorySize)
        {
            if (mmf == null || memorySize != mmfSize)
            {
                string filePath = GetCrossPlatformFilePath(fileName);

                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    mmf = MemoryMappedFile.OpenExisting(fileName);
                }
                else
                {
                    mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.Open, null, memorySize);
                }

                accessor = mmf.CreateViewAccessor();

                mmfSize = memorySize;
            }
        }

        /* Determine a cross-platform file path for the MMF */
        private string GetCrossPlatformFilePath(string fileName)
        {
            string directory;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                directory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            }
            else if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                directory = "/var/tmp";  /* Safer than /tmp for shared access */
            }
            else
            {
                throw new PlatformNotSupportedException("Unsupported platform.");
            }

            return Path.Combine(directory, fileName);
        }
    }
}
