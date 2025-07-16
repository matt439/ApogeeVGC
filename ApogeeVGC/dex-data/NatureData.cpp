#include "NatureData.h"

//NatureData::NatureData(const std::string& name, const StatIDExceptHP* plus,
//	const StatIDExceptHP* minus)
//	: name(name), plus(plus), minus(minus)
//{
//}
//
//NatureData::NatureData(const BasicEffectData& basic_effect_data,
//	const StatIDExceptHP* plus, const StatIDExceptHP* minus)
//	: name(basic_effect_data.name), plus(plus), minus(minus)
//{
//}

//NatureData::NatureData(const std::string& name) :
//	name(name)
//{
//}


NatureData::NatureData(const std::string& name,
    std::unique_ptr<StatIDExceptHP> plus,
    std::unique_ptr<StatIDExceptHP> minus)
	: name(name), plus(std::move(plus)), minus(std::move(minus))
{
}

NatureData::NatureData(const NatureData& other)
	: name(other.name),
	plus(other.plus ? std::make_unique<StatIDExceptHP>(*other.plus) : nullptr),
	minus(other.minus ? std::make_unique<StatIDExceptHP>(*other.minus) : nullptr)
{
}

DataType NatureData::get_data_type() const
{
	return DataType::NATURES;
}