#pragma once

#include <string>
#include <memory>

template <typename T>
struct TextFile
{
    std::string name;
    std::unique_ptr<T> gen1 = nullptr;
    std::unique_ptr<T> gen2 = nullptr;
    std::unique_ptr<T> gen3 = nullptr;
    std::unique_ptr<T> gen4 = nullptr;
    std::unique_ptr<T> gen5 = nullptr;
    std::unique_ptr<T> gen6 = nullptr;
    std::unique_ptr<T> gen7 = nullptr;
    std::unique_ptr<T> gen8 = nullptr;
    T data; // The main T data (equivalent to T& in TypeScript)
};
