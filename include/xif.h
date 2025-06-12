/***************************************************************
**
** TBReAI Header File
**
** File         :  xif.h
** Module       :  XIF
** Author       :  SH
** Created      :  2025-04-16 (YYYY-MM-DD)
** License      :  MIT
** Description  :  Cross-Platform Interface Framework Root Header
**
***************************************************************/

#ifndef XIF_H
#define XIF_H

#ifdef __cplusplus
extern "C" {
#endif

/***************************************************************
** MARK: INCLUDES
***************************************************************/

#include <stdint.h>
#include <stddef.h>
#include <stdbool.h>

/***************************************************************
** MARK: CONSTANTS & MACROS
***************************************************************/

/***************************************************************
** MARK: TYPEDEFS
***************************************************************/

#ifndef TBRERT_H

    typedef struct
    {
        float x;           /* x coordinate */
        float y;           /* y coordinate */
    } vector2_t;

    typedef struct
    {
        float x;           /* x coordinate */
        float y;           /* y coordinate */
        float z;           /* z coordinate */
    } vector3_t;

    typedef struct
    {
        float x;           /* x coordinate */
        float y;           /* y coordinate */
        float z;           /* z coordinate */
        float w;           /* w coordinate */
    } vector4_t;

#endif /* TBRERT_H */

typedef struct
{
    uint64_t timestamp; /* ms */
    size_t width;       /* width in pixels */
    size_t height;      /* height in pixels */
    size_t channels;    /* bytes per pixel */
    void* data;         /* pointer to image data */
} xif_image_t;

typedef struct 
{
    uint64_t timestamp; /* ms */
    size_t num_points;  /* number of points in the point cloud */
    vector4_t* points;  /* pointer to array of points */
} xif_pointcloud_t;

typedef struct 
{
    uint64_t timestamp;             /* ms */
    vector3_t orientation;          /* euler angles, radians */
    vector3_t angular_velocity;     /* radians */
    vector3_t linear_acceleration;  /* m/s^2 */
} xif_imu_t;

typedef struct 
{
    uint64_t timestamp;         /* ms */
    uint8_t ami_state;          /* AMI state */
    
    int32_t front_axle_trq; 
    uint32_t front_axle_trq_request;
    uint32_t front_axle_trq_max;
    
    int32_t rear_axle_trq;
    uint32_t rear_axle_trq_request;
    uint32_t rear_axle_trq_max;

    int32_t steer_angle;
    uint32_t steer_angle_max;
    int32_t steer_angle_request;

    uint32_t hyd_press_f_pct;
    uint32_t hyd_press_f_req_pct;
    uint32_t hyd_press_r_pct;
    uint32_t hyd_press_r_req_pct;    

    uint32_t front_left_rpm;
    uint32_t front_right_rpm;
    uint32_t rear_left_rpm;
    uint32_t rear_right_rpm;
} xif_vehicle_data_t;

typedef struct 
{
    uint8_t mission_status;
    uint32_t lap_counter;
    
    uint32_t front_axle_trq_request; 
    uint32_t front_motor_speed_max;

    uint32_t rear_axle_trq_request;
    uint32_t rear_motor_speed_max;

    int32_t steer_request_deg;

    uint32_t hyd_press_f_req_pct;
    uint32_t hyd_press_r_req_pct;
} xif_vehicle_control_t;

typedef enum
{
    XIF_OBJECT_TYPE_BLUE_CONE,
    XIF_OBJECT_TYPE_YELLOW_CONE,
    XIF_OBJECT_TYPE_SMALL_ORANGE_CONE,
    XIF_OBJECT_TYPE_LARGE_ORANGE_CONE,
    XIF_OBJECT_TYPE_OTHER
} xif_object_type_t;

typedef struct
{
    vector3_t position;     /* position in world coordinates */
    vector3_t size;         /* size of the object (length, width, height) */
    vector3_t orientation;  /* orientation in euler angles (roll, pitch, yaw) */
    xif_object_type_t type; /* type of the object */
} xif_reference_object_t;

typedef struct
{
    uint64_t timestamp; /* ms */
    size_t num_objects; /* number of objects in the perception */
    xif_reference_object_t* objects; /* pointer to array of objects */
} xif_reference_perception_t;

typedef struct 
{
    uint64_t timestamp; /* ms */
    vector3_t position; /* position in world coordinates */
    vector3_t orientation; /* orientation in euler angles (roll, pitch, yaw) */
} xif_reference_pose_t;

typedef void (*xif_timestep_callback_t)(const uint64_t time_ms);
typedef void (*xif_image_callback_t)(const xif_image_t* image);
typedef void (*xif_pointcloud_callback_t)(const xif_pointcloud_t* pointcloud);
typedef void (*xif_imu_callback_t)(const xif_imu_t* imu_data);
typedef void (*xif_vehicle_data_callback_t)(const xif_vehicle_data_t* vehicle_data);
typedef void (*xif_vehicle_control_callback_t)(const xif_vehicle_control_t* vehicle_control);

typedef void (*xif_reference_perception_callback_t)(const xif_reference_perception_t* perception);
typedef void (*xif_reference_pose_callback_t)(const xif_reference_pose_t* pose);

/***************************************************************
** MARK: FUNCTION DEFS
***************************************************************/

#ifdef __cplusplus
}
#endif

#endif /* XIF_H */



