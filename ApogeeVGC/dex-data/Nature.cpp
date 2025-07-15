#include "Nature.h"

//Nature::Nature() : BasicEffect(BasicEffectData("", false)), NatureData(NatureData())
//{
//	fullname = "nature: " + BasicEffect::name;
//	effect_type = EffectType::NATURE;
//	gen = 3;
//}
//
//Nature::Nature(const BasicEffect& basic_effect, const NatureData& nature_data) :
//	BasicEffect(basic_effect), NatureData(nature_data)
//{
//	fullname = "nature: " + NatureData::name;
//	effect_type = EffectType::NATURE;
//	gen = 3;
//}

Nature::Nature(
	const std::string& name,
	const std::string& real_move,
	const std::string& full_name,
	bool exists,
	int num,
	const std::string& short_desc,
	const std::string& desc,
	NonStandard is_nonstandard,
	bool no_copy,
	bool affects_fainted,
	const std::string& source_effect) :
	//// optional
	//std::unique_ptr<int> duration,
	//std::unique_ptr<std::string> status,
	//std::unique_ptr<std::string> weather,
	//std::unique_ptr<std::function<int(Battle*, Pokemon*, Pokemon*, Effect*)>> duration_callback,
	//std::unique_ptr<bool> infiltrates,
	//std::unique_ptr<StatIDExceptHP> plus,
	//std::unique_ptr<StatIDExceptHP> minus) :
	BasicEffect(name, real_move, full_name, EffectType::NATURE, exists, num, 3,
		short_desc, desc, is_nonstandard, no_copy, affects_fainted, source_effect),
		//std::move(duration), std::move(status), std::move(weather),
		//std::move(duration_callback), std::move(infiltrates)),
	NatureData(name)//, std::move(plus), std::move(minus))
{
	fullname = "nature: " + name;
}

Nature::Nature(const Nature& other) :
	BasicEffect(other),
	NatureData(other)
{
}