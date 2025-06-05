/*
**  XIF Server.cs
*/

using System;
using System.Runtime.InteropServices;

using XIF.Comms;
using XIF.Comms.Common;

using NetMQ;

namespace XIF
{
    public static class ServerStatic
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void VehicleControlCallback(IntPtr vehicleControl);

        private static VehicleControlCallback? vehicleControlCallback;

        private static MqPublisher? serverPublisher;
        private static MqSubscription? vehicleControlSubscription;
        private static MqSubscription? clientTimestepSubscription;

        private static SharedMemory? imageShm;
        private static SharedMemory? pointcloudShm;

        [UnmanagedCallersOnly(EntryPoint = "xifs_init")]
        public static bool Init()
        {
            serverPublisher = new MqPublisher(PortDefinition.SERVER_PORT);

            vehicleControlSubscription = new MqSubscription(PortDefinition.CLIENT_PORT, "/client/vehicle_control");
            clientTimestepSubscription = new MqSubscription(PortDefinition.CLIENT_PORT, "/client/timestep");

            vehicleControlCallback = null;

            imageShm = new SharedMemory("XIF.IMAGE");
            pointcloudShm = new SharedMemory("XIF.POINCLOUD");

            return true;
        }

        [UnmanagedCallersOnly(EntryPoint = "xifc_set_vehicle_control_callback")]
        public static void SetVehicleControlCallback(IntPtr callbackPtr)
        {
            if (callbackPtr == IntPtr.Zero)
            {
                return;
            }

            vehicleControlCallback = Marshal.GetDelegateForFunctionPointer<VehicleControlCallback>(callbackPtr);
        }

        [UnmanagedCallersOnly(EntryPoint = "xifs_transmit_timestep")]
        public static void TransmitTimestep(UInt64 timeMs)
        {
            Timestep timestep = new Timestep();
            timestep.TimeMs = timeMs;

            serverPublisher!.Transmit("/server/timestep", timestep);
        }
        
        [UnmanagedCallersOnly(EntryPoint = "xifs_transmit_image")]
        public static unsafe void TransmitImage(Image image)
        {
            if (image.Data == IntPtr.Zero)
            {
                return;
            }

            imageShm!.WriteBuffer((byte*)image.Data, (int)image.Width *  (int)image.Height * (int)image.Channels);

            serverPublisher!.Transmit("/server/image", image);
        }

        [UnmanagedCallersOnly(EntryPoint = "xifs_transmit_pointcloud")]
        public static unsafe void TransmitPointcloud(Pointcloud pointcloud)
        {
            if (pointcloud.Points == IntPtr.Zero)
            {
                return;
            }

            pointcloudShm!.WriteBuffer((byte*)pointcloud.Points, (int)pointcloud.NumPoints * Marshal.SizeOf<Vector4>());

            serverPublisher!.Transmit("/server/pointcloud", pointcloud);
        }

        [UnmanagedCallersOnly(EntryPoint = "xifs_transmit_imu")]
        public static void TransmitImu(Imu imu)
        {
            serverPublisher!.Transmit("/server/imu", imu);
        }

        [UnmanagedCallersOnly(EntryPoint = "xifs_transmit_vehicle_data")]
        public static void TransmitVehicleData(VehicleData vehicleData)
        {
            serverPublisher!.Transmit("/server/vehicle_data", vehicleData);
        }

        [UnmanagedCallersOnly(EntryPoint = "xifs_transmit_reference_perception")]
        public static void TransmitReferencePerception(ReferenceObjectsFixed referenceFixed)
        {

            ReferenceObjects referencePerception = new ReferenceObjects();
            referencePerception.TimeMs = referenceFixed.TimeMs;

            /* Convert IntPtr to array of ReferenceObject */
            int count = (int)referenceFixed.NumObjects;
            ReferenceObject[] objects = new ReferenceObject[count];

            if (referenceFixed.Objects != IntPtr.Zero && count > 0)
            {
                int size = Marshal.SizeOf<ReferenceObject>();
                for (int i = 0; i < count; i++)
                {
                    IntPtr ptr = IntPtr.Add(referenceFixed.Objects, i * size);
                    objects[i] = Marshal.PtrToStructure<ReferenceObject>(ptr)!;
                }
            }
            
            referencePerception.Objects = objects;
            serverPublisher!.Transmit("/server/reference_perception", referencePerception);
        }

        [UnmanagedCallersOnly(EntryPoint = "xifs_transmit_reference_pose")]
        public static void TransmitReferencePose(ReferencePose referencePose)
        {
            serverPublisher!.Transmit("/server/reference_pose", referencePose);
        }

    }
}
