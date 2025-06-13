/*
**  XIF Client.cs
*/

using System;
using System.Runtime.InteropServices;

using XIF.Comms;
using XIF.Comms.Common;

using NetMQ;

namespace XIF
{
    public static class ClientStatic
    {

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void TimestepCallback(UInt64 timeMs);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ImageCallback(IntPtr image);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void PointcloudCallback(IntPtr pointcloud);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ImuCallback(IntPtr imuData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void VehicleDataCallback(IntPtr vehicleData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ReferencePerceptionCallback(IntPtr referencePerception);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ReferencePoseCallback(IntPtr referencePose);

        private static TimestepCallback? timestepCallback;
        private static ImageCallback? imageCallback;
        private static PointcloudCallback? pointcloudCallback;
        private static ImuCallback? imuCallback;
        private static VehicleDataCallback? vehicleDataCallback;
        private static ReferencePerceptionCallback? referencePerceptionCallback;
        private static ReferencePoseCallback? referencePoseCallback;

        private static MqPublisher? clientPublisher;
        private static MqSubscription? timestepSubscription;
        private static MqSubscription? imageSubscription;
        private static MqSubscription? pointcloudSubscription;
        private static MqSubscription? imuSubscription;
        private static MqSubscription? vehicleDataSubscription;
        private static MqSubscription? referencePerceptionSubscription;
        private static MqSubscription? referencePoseSubscription;

        private static SharedMemory? imageShm;
        private static SharedMemory? pointcloudShm;

        [UnmanagedCallersOnly(EntryPoint = "xifc_init")]
        public static bool Init()
        {
            clientPublisher = new MqPublisher(PortDefinition.CLIENT_PORT);

            timestepSubscription = new MqSubscription(PortDefinition.SERVER_PORT, "/server/timestep");
            imageSubscription = new MqSubscription(PortDefinition.SERVER_PORT, "/server/image");
            pointcloudSubscription = new MqSubscription(PortDefinition.SERVER_PORT, "/server/pointcloud");
            imuSubscription = new MqSubscription(PortDefinition.SERVER_PORT, "/server/imu");
            vehicleDataSubscription = new MqSubscription(PortDefinition.SERVER_PORT, "/server/vehicle_data");
            referencePerceptionSubscription = new MqSubscription(PortDefinition.SERVER_PORT, "/server/reference_perception");
            referencePoseSubscription = new MqSubscription(PortDefinition.SERVER_PORT, "/server/reference_pose");

            timestepCallback = null;
            imageCallback = null;
            pointcloudCallback = null;
            imuCallback = null;
            vehicleDataCallback = null;
            referencePerceptionCallback = null;
            referencePoseCallback = null;

            imageShm = new SharedMemory("XIF.IMAGE");
            pointcloudShm = new SharedMemory("XIF.POINCLOUD");

            Console.WriteLine($"XIF Client initialized on port {(int)PortDefinition.CLIENT_PORT}");
            return true;
        }

        public static unsafe void UpdateClient()
        {
            Timestep timestep = new Timestep();
            if (timestepSubscription!.TryReceive(ref timestep))
            {
                if (timestepCallback != null)
                {
                    timestepCallback(timestep.TimeMs);
                }
            }

            Imu imu = new Imu();
            if (imuSubscription!.TryReceive(ref imu))
            {
                IntPtr imuPtr = Marshal.AllocHGlobal(Marshal.SizeOf(imu));
                Marshal.StructureToPtr(imu, imuPtr, false);

                if (imuCallback != null)
                {
                    imuCallback(imuPtr);
                }

                Marshal.FreeHGlobal(imuPtr);
            }

            Image image = new Image();
            if (imageSubscription!.TryReceive(ref image))
            {
                int size = (int)(image.Width * image.Height * image.Channels);
                byte* imageData = imageShm.GetSharedMemoryPointer(size);

                image.Data = (UInt64)imageData;

                IntPtr imagePtr = Marshal.AllocHGlobal(Marshal.SizeOf(image));
                Marshal.StructureToPtr(image, imagePtr, false);

                if (imageCallback != null)
                {
                    imageCallback(imagePtr);
                }

                Marshal.FreeHGlobal(imagePtr);

                imageShm.ReleaseSharedMemoryPointer();
            }

            Pointcloud pointcloud = new Pointcloud();
            if (pointcloudSubscription!.TryReceive(ref pointcloud))
            {
                int size = (int)((int)pointcloud.NumPoints * Marshal.SizeOf<Vector4>());
                Vector4* pointData = (Vector4*)pointcloudShm.GetSharedMemoryPointer(size);

                pointcloud.Points = (UInt64)pointData;

                IntPtr pointcloudPtr = Marshal.AllocHGlobal(Marshal.SizeOf(pointcloud));
                Marshal.StructureToPtr(pointcloud, pointcloudPtr, false);

                if (pointcloudCallback != null)
                {
                    pointcloudCallback(pointcloudPtr);
                }

                Marshal.FreeHGlobal(pointcloudPtr);

                pointcloudShm.ReleaseSharedMemoryPointer();
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "xifc_update")]
        public static void Update()
        {
            UpdateClient();
        }

        [UnmanagedCallersOnly(EntryPoint = "xifc_run")]
        public static int Run()
        {
            while (true)
            {
                UpdateClient();
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "xifc_set_timestep_callback")]
        public static void SetTimestepCallback(IntPtr callbackPtr)
        {
            if (callbackPtr == IntPtr.Zero)
            {
                return;
            }

            timestepCallback = Marshal.GetDelegateForFunctionPointer<TimestepCallback>(callbackPtr);
        }

        [UnmanagedCallersOnly(EntryPoint = "xifc_set_image_callback")]
        public static void SetImageCallback(IntPtr callbackPtr)
        {
            if (callbackPtr == IntPtr.Zero)
            {
                return;
            }

            imageCallback = Marshal.GetDelegateForFunctionPointer<ImageCallback>(callbackPtr);
        }

        [UnmanagedCallersOnly(EntryPoint = "xifc_set_pointcloud_callback")]
        public static void SetPointcloudCallback(IntPtr callbackPtr)
        {
            if (callbackPtr == IntPtr.Zero)
            {
                return;
            }

            pointcloudCallback = Marshal.GetDelegateForFunctionPointer<PointcloudCallback>(callbackPtr);
        }

        [UnmanagedCallersOnly(EntryPoint = "xifc_set_imu_callback")]
        public static void SetImuCallback(IntPtr callbackPtr)
        {
            if (callbackPtr == IntPtr.Zero)
            {
                return;
            }

            imuCallback = Marshal.GetDelegateForFunctionPointer<ImuCallback>(callbackPtr);
        }

        [UnmanagedCallersOnly(EntryPoint = "xifc_set_vehicle_data_callback")]
        public static void SetVehicleDataCallback(IntPtr callbackPtr)
        {
            if (callbackPtr == IntPtr.Zero)
            {
                return;
            }

            vehicleDataCallback = Marshal.GetDelegateForFunctionPointer<VehicleDataCallback>(callbackPtr);
        }

        [UnmanagedCallersOnly(EntryPoint = "xifc_set_reference_perception_callback")]
        public static void SetReferencePerceptionCallback(IntPtr callbackPtr)
        {
            if (callbackPtr == IntPtr.Zero)
            {
                return;
            }

            referencePerceptionCallback = Marshal.GetDelegateForFunctionPointer<ReferencePerceptionCallback>(callbackPtr);
        }

        [UnmanagedCallersOnly(EntryPoint = "xifc_set_reference_pose_callback")]
        public static void SetReferencePoseCallback(IntPtr callbackPtr)
        {
            if (callbackPtr == IntPtr.Zero)
            {
                return;
            }

            referencePoseCallback = Marshal.GetDelegateForFunctionPointer<ReferencePoseCallback>(callbackPtr);
        }
    
        [UnmanagedCallersOnly(EntryPoint = "xifc_transmit_vehicle_control")]
        public static void TransmitVehicleControl(VehicleControl vehicleControl)
        {
            clientPublisher!.Transmit("/client/vehicle_control", vehicleControl);
        }
    }
}
