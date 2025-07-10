#include "HitEffect.h"

HitEffect::HitEffect(std::unique_ptr<OnHitFunc> on_hit,
	std::unique_ptr<SparseBoostsTable> boosts,
	std::unique_ptr<std::string> status,
	std::unique_ptr<std::string> volatile_status,
	std::unique_ptr<std::string> side_condition,
	std::unique_ptr<std::string> slot_condition,
	std::unique_ptr<std::string> pseudo_weather,
	std::unique_ptr<std::string> terrain,
	std::unique_ptr<std::string> weather) :
	on_hit(std::move(on_hit)),
	boosts(std::move(boosts)),
	status(std::move(status)),
	volatile_status(std::move(volatile_status)),
	side_condition(std::move(side_condition)),
	slot_condition(std::move(slot_condition)),
	pseudo_weather(std::move(pseudo_weather)),
	terrain(std::move(terrain)),
	weather(std::move(weather))
{
}