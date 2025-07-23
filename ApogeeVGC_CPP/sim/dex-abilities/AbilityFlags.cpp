#include "AbilityFlags.h"

AbilityFlags::AbilityFlags(const AbilityFlags& other)
{
	if (other.breakable)
		breakable = std::make_unique<bool>(*other.breakable);

	if (other.cantsuppress)
		cantsuppress = std::make_unique<bool>(*other.cantsuppress);

	if (other.failroleplay)
		failroleplay = std::make_unique<bool>(*other.failroleplay);

	if (other.failskillswap)
		failskillswap = std::make_unique<bool>(*other.failskillswap);

	if (other.noentrain)
		noentrain = std::make_unique<bool>(*other.noentrain);

	if (other.noreceiver)
		noreceiver = std::make_unique<bool>(*other.noreceiver);

	if (other.notrace)
		notrace = std::make_unique<bool>(*other.notrace);

	if (other.notransform)
		notransform = std::make_unique<bool>(*other.notransform);
}