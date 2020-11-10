cmake_minimum_required(VERSION 2.8.9)

set(CMAKE_CXX_COMPILER "clang++")

project(ehata)

# C++14
set(CMAKE_CXX_STANDARD 14)

# Headers
include_directories(include)

# SRC
file(GLOB SOURCES "src/*.cpp")

# Shared Library
add_library(ehata SHARED ${SOURCES})

install(
    TARGETS ehata
    LIBRARY DESTINATION "lib${LIB_SUFFIX}"
    COMPONENT ehata
)

install(
    FILES "${CMAKE_CURRENT_SOURCE_DIR}/include/ehata.h"
    DESTINATION "include/ehata"
    COMPONENT ehata-dev
)
