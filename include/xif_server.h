/***************************************************************
**
** TBReAI Header File
**
** File         :  xif_server.h
** Module       :  XIF
** Author       :  SH
** Created      :  2025-04-16 (YYYY-MM-DD)
** License      :  MIT
** Description  :  Interface for XIF Servers (i.e Simulators)
**
***************************************************************/

#ifndef XIF_SERVER_H
#define XIF_SERVER_H

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

bool xifs_init(void);
bool xifs_update(void); /* returns true if the timestep is complete */

void xifs_transmit_timestep(uint64_t timestep_ms);
void xifs_transmit_image(xif_image_t image);
void xifs_transmit_pointcloud(xif_pointcloud_t pointcloud);
void xifs_transmit_imu(xif_imu_t imu);
void xifs_transmit_vehicle_data(xif_vehicle_data_t vehicle_data);
void xifs_transmit_reference_perception(xif_reference_perception_t perception);
void xifs_transmit_reference_pose(xif_reference_pose_t pose);

void xifc_set_vehicle_control_callback(xif_vehicle_control_callback_t callback);

#ifdef __cplusplus
}
#endif

#endif /* XIF_SERVER_H */



