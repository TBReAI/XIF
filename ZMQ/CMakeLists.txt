cmake_minimum_required(VERSION 3.10)

project(XIF-ZMQ)

set(DOTNET_PROJECT_DIR ${CMAKE_CURRENT_LIST_DIR})

set(CSHARP_PROJECT ${DOTNET_PROJECT_DIR}/XIF.csproj)
file(GLOB_RECURSE CSHARP_SRC_FILES ${DOTNET_PROJECT_DIR}/lib/*.cs)

if(WIN32)

    set(DOTNET_OUTPUT_DIR ${DOTNET_PROJECT_DIR}/bin/Release/net9.0/win-x64/native)
    set(DOTNET_OUTPUT_DLL ${DOTNET_OUTPUT_DIR}/XIF.dll)
    add_custom_command(
        OUTPUT  ${DOTNET_OUTPUT_DIR}/XIF.dll
        COMMAND dotnet publish ${DOTNET_PROJECT_DIR}/XIF.csproj -c Release -r win-x64
        DEPENDS ${CSHARP_PROJECT} ${CSHARP_SRC_FILES}
        WORKING_DIRECTORY ${DOTNET_PROJECT_DIR}
        COMMENT "Building C# NativeAOT DLL"
    )
else()
    set(DOTNET_OUTPUT_DIR ${DOTNET_PROJECT_DIR}/bin/Release/net9.0/linux-x64/native)
    set(DOTNET_ORIGINAL_DLL ${DOTNET_OUTPUT_DIR}/XIF.so)
    set(DOTNET_OUTPUT_DLL ${DOTNET_OUTPUT_DIR}/libXIF.so)

    add_custom_command(
        OUTPUT  ${DOTNET_OUTPUT_DLL}
        COMMAND dotnet publish ${DOTNET_PROJECT_DIR}/XIF.csproj -c Release -r linux-x64
        COMMAND ${CMAKE_COMMAND} -E rename ${DOTNET_ORIGINAL_DLL} ${DOTNET_OUTPUT_DLL}
        DEPENDS ${CSHARP_PROJECT} ${CSHARP_SRC_FILES}
        WORKING_DIRECTORY ${DOTNET_PROJECT_DIR}
        COMMENT "Building C# NativeAOT DLL"
    )
endif()

add_custom_target(cs ALL
    DEPENDS ${DOTNET_OUTPUT_DLL}
)

set(XIF_DLL ${DOTNET_OUTPUT_DLL} CACHE INTERNAL "Native C# DLL Path")

set(XIF_LIBS 
    ${DOTNET_OUTPUT_DLL}
    CACHE INTERNAL "XIF Library Path"
)

set(XIF_DEPENDS
    cs
    CACHE INTERNAL "XIF Dependencies"
)