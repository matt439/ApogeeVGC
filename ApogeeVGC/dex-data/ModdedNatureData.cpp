#include "ModdedNatureData.h"

ModdedNatureData::ModdedNatureData(
	std::unique_ptr<StatIDExceptHP> plus,
	std::unique_ptr<StatIDExceptHP> minus) :
	NatureData("", std::move(plus), std::move(minus))
{
}