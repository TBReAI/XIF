/*
**  XIF Comms Common.cs
**
**  Structure defintions for ZeroMQ messages.
*/

using System;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace XIF.Comms.Common
{
    [JsonSerializable(typeof(XIF.Comms.Common.Timestep))]
    [JsonSerializable(typeof(XIF.Comms.Common.Image))]
    [JsonSerializable(typeof(XIF.Comms.Common.Pointcloud))]
    [JsonSerializable(typeof(XIF.Comms.Common.Imu))]
    [JsonSerializable(typeof(XIF.Comms.Common.VehicleData))]
    [JsonSerializable(typeof(XIF.Comms.Common.VehicleControl))]
    [JsonSerializable(typeof(XIF.Comms.Common.ReferenceObjects))]
    [JsonSerializable(typeof(XIF.Comms.Common.ReferencePose))]
    public partial class ApplicationJsonContext : JsonSerializerContext
    {

    }

    public enum PortDefinition
    {
        SERVER_PORT = 7823,
        CLIENT_PORT = 7824
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vector2
    {
        public float x { get; set; }
        public float y { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vector3
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vector4
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public float w { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Timestep
    {
        public UInt64 TimeMs { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Image
    {
        public UInt64 TimeMs { get; set; }
        public UInt64 Width { get; set; }
        public UInt64 Height { get; set; }
        public UInt64 Channels { get; set; } // 3 for RGB, 4 for RGBA
        public UInt64 Data { get; set; } // pointer to the image data buffer
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Pointcloud
    {
        public UInt64 TimeMs { get; set; }
        public UInt64 NumPoints { get; set; }
        public UInt64 Points { get; set; } // pointer to the point cloud data buffer
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Imu
    {
        public UInt64 TimeMs { get; set; }
        public Vector3 Orientation { get; set; } // euler angles in radians
        public Vector3 AngularVelocity { get; set; } // rad/s
        public Vector3 LinearAcceleration { get; set; } // m/s^2
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VehicleData
    {
        public UInt64 TimeMs { get; set; }
        public byte AmiState { get; set; }

        public Int32 FrontAxleTrq { get; set; } // Nm
        public UInt32 FrontAxleTrqRequest { get; set; } // Nm
        public UInt32 FrontAxleTrqMax { get; set; } // Nm

        public Int32 RearAxleTrq { get; set; } // Nm
        public UInt32 RearAxleTrqRequest { get; set; } // Nm
        public UInt32 RearAxleTrqMax { get; set; } // Nm

        public Int32 SteerAngle { get; set; } // deci-degrees
        public UInt32 SteerAngleMax { get; set; } // deci-degrees
        public Int32 SteerAngleRequest { get; set; } // deci-degrees

        public UInt32 HydPressFPct { get; set; } // percentage
        public UInt32 HydPressFReqPct { get; set; } // percentage

        public UInt32 HydPressRPct { get; set; } // percentage
        public UInt32 HydPressRReqPct { get; set; } // percentage

        public UInt32 FrontLeftRpm { get; set; }
        public UInt32 FrontRightRpm { get; set; }
        public UInt32 RearLeftRpm { get; set; }
        public UInt32 RearRightRpm { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VehicleControl
    {
        public byte MissionStatus { get; set; }
        public UInt32 LapCounter { get; set; }

        public UInt32 FrontAxleTrqRequest { get; set; } // Nm
        public UInt32 FrontMotorSpeedMax { get; set; } // RPM

        public UInt32 RearAxleTrqRequest { get; set; } // Nm
        public UInt32 RearMotorSpeedMax { get; set; } // RPM

        public Int32 SteerAngleRequest { get; set; } // deci-degrees

        public UInt32 HydPressFReqPct { get; set; } // percentage
        public UInt32 HydPressRReqPct { get; set; } // percentage
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ReferenceObject
    {
        public Vector3 Position { get; set; } // in meters  
        public Vector3 Size { get; set; } // in meters (length, width, height)
        public Vector3 Orientation { get; set; } // euler angles in radians
        public int Type { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ReferenceObjects
    {
        public UInt64 TimeMs { get; set; }
        public ReferenceObject[] Objects { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ReferenceObjectsFixed
    {
        public UInt64 TimeMs { get; set; }
        public UInt64 NumObjects { get; set; }
        public UInt64 Objects { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ReferencePose
    {
        public UInt64 TimeMs { get; set; }
        public Vector3 Position { get; set; } // in meters
        public Vector3 Orientation { get; set; } // euler angles in radians
    }

    public enum AmiState
    {
        AMI_NOT_SELECTED = 0,
        AMI_ACCELERATION = 1,
        AMI_SKIDPAD = 2,
        AMI_AUTOCROSS = 3,
        AMI_TRACK_DRIVE = 4,
        AMI_STATIC_INSPECTION_A = 5,
        AMI_STATIC_INSPECTION_B = 6,
        AMI_AUTONOMOUS_DEMO = 7
    }

    public enum MissionStatus
    {
        MISSION_NOT_SELECTED = 0,
        MISSION_SELECTED = 1,
        MISSION_RUNNING = 2,
        MISSION_FINISHED = 3
    }

    public enum ObjectType
    {
        OBJECT_TYPE_BLUE_CONE,
        OBJECT_TYPE_YELLOW_CONE,
        OBJECT_TYPE_SMALL_ORANGE_CONE,
        OBJECT_TYPE_LARGE_ORANGE_CONE,
        OBJECT_TYPE_OTHER
    }
}

