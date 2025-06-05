/***************************************************************
**
** TBReAI Header File
**
** File         :  xif_client.h
** Module       :  XIF
** Author       :  SH
** Created      :  2025-04-16 (YYYY-MM-DD)
** License      :  MIT
** Description  :  Interface for XIF Clients (i.e AI System)
**
***************************************************************/

#ifndef XIF_CLIENT_H
#define XIF_CLIENT_H

#ifdef __cplusplus
extern "C" {
#endif

/***************************************************************
** MARK: INCLUDES
***************************************************************/

#include <stdint.h>
#include <stddef.h>
#include <stdbool.h>

#include "xif.h"

/***************************************************************
** MARK: CONSTANTS & MACROS
***************************************************************/

/***************************************************************
** MARK: TYPEDEFS
***************************************************************/

/***************************************************************
** MARK: FUNCTION DEFS
***************************************************************/

bool xifc_init(void);

/* option between a 'push' or 'pull' API */
void xifc_update(void);
int xifc_run(void);

void xifc_set_timestep_callback(xif_timestep_callback_t callback);
void xifc_set_image_callback(xif_image_callback_t callback);
void xifc_set_pointcloud_callback(xif_pointcloud_callback_t callback);
void xifc_set_imu_callback(xif_imu_callback_t callback);
void xifc_set_vehicle_data_callback(xif_vehicle_data_callback_t callback);
void xifc_set_reference_perception_callback(xif_reference_perception_callback_t callback);
void xifc_set_reference_pose_callback(xif_reference_pose_callback_t callback);

void xifc_transmit_vehicle_control(xif_vehicle_control_t control);

#ifdef __cplusplus
}
#endif

#endif /* XIF_CLIENT_H */



