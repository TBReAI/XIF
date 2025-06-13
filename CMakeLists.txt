cmake_minimum_required(VERSION 3.10)

project(XIF)

#if ()
    add_subdirectory(ZMQ)
#else()
    #add_subdirectory(ROS2)
#endif()

set(XIF_INCLUDE_DIRS ${CMAKE_CURRENT_LIST_DIR}/include)