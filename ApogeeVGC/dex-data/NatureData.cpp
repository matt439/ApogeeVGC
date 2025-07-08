#include "NatureData.h"

NatureData::NatureData(const std::string& name, const std::optional<StatIDExceptHP>& plus,
    const std::optional<StatIDExceptHP>& minus)
    : name(name), plus(plus), minus(minus)
{
}

NatureData::NatureData(const BasicEffectData& basic_effect_data,
    const std::optional<StatIDExceptHP>& plus,
    const std::optional<StatIDExceptHP>& minus)
    : name(basic_effect_data.name), plus(plus), minus(minus)
{
}