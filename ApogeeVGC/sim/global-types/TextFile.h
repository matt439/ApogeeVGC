#pragma once

#include "ConditionTextData.h"
#include "MoveTextData.h"
#include "BasicTextData.h"
#include <string>
#include <memory>

template <typename T>
struct TextFile
{
    std::string name = "";
    std::unique_ptr<T> gen1 = nullptr;
    std::unique_ptr<T> gen2 = nullptr;
    std::unique_ptr<T> gen3 = nullptr;
    std::unique_ptr<T> gen4 = nullptr;
    std::unique_ptr<T> gen5 = nullptr;
    std::unique_ptr<T> gen6 = nullptr;
    std::unique_ptr<T> gen7 = nullptr;
    std::unique_ptr<T> gen8 = nullptr;
    std::unique_ptr<T> data = nullptr;

	TextFile() = default;
    TextFile(const std::string& name);
    TextFile(const TextFile& other);
};

//struct ConditionTextData;

//TextFile<ConditionTextData>::TextFile(const std::string& name);
TextFile<ConditionTextData>::TextFile(const TextFile& other);

//struct MoveTextData;

//TextFile<MoveTextData>::TextFile(const std::string& name);
TextFile<MoveTextData>::TextFile(const TextFile& other);

TextFile<BasicTextData>::TextFile(const TextFile& other);
