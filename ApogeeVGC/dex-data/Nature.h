#pragma once

#include "NatureData.h"
#include "BasicEffect.h"

struct Nature : public BasicEffect, public NatureData
{
    Nature();
    Nature(const Nature&) = default;
    Nature(const BasicEffect& basic_effect, const NatureData& nature_data);
};