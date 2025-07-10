#pragma once

#include "NatureData.h"
#include "BasicEffect.h"

struct Nature : public BasicEffect, public NatureData
{
	// Nature() = default;
    // Nature(const BasicEffect& basic_effect, const NatureData& nature_data);

    Nature(
        const std::string& name,
        const std::string& real_move = "",
        const std::string& full_name = "",
        bool exists = true,
        int num = 0,
        const std::string& short_desc = "",
        const std::string& desc = "",
        NonStandard is_nonstandard = NonStandard::NONE,
        bool no_copy = false,
        bool affects_fainted = false,
        const std::string& source_effect = "",
        // optional
        std::unique_ptr<int> duration = nullptr,
        std::unique_ptr<std::string> status = nullptr,
        std::unique_ptr<std::string> weather = nullptr,
        std::unique_ptr<std::function<int(Battle*, Pokemon*, Pokemon*, Effect*)>> duration_callback = nullptr,
        std::unique_ptr<bool> infiltrates = nullptr,
        std::unique_ptr<StatIDExceptHP> plus = nullptr,
        std::unique_ptr<StatIDExceptHP> minus = nullptr);
};