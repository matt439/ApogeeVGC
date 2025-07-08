#include "BasicEffect.h"

#include "to_id.h"

BasicEffect::BasicEffect(const BasicEffectData& data)
{
    name = data.name;
    id = data.real_move ? to_id(*data.real_move) : to_id(name);
    fullname = data.fullname.value_or(name);
    effect_type = data.effect_type.value_or(EffectType::CONDITION);
    exists = data.exists.value_or(!id.empty());
    num = data.num.value_or(0);
    gen = data.gen.value_or(0);
    short_desc = data.short_desc.value_or("");
    desc = data.desc.value_or("");
    is_nonstandard = data.is_nonstandard;
    duration = data.duration;
    no_copy = data.no_copy.value_or(false);
    affects_fainted = data.affects_fainted.value_or(false);
    status = data.status;
    weather = data.weather;
    source_effect = data.source_effect.value_or("");
}

std::string_view BasicEffect::to_string() const
{
    return this->name;
}