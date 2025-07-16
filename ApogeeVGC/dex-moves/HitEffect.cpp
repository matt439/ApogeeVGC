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

HitEffect::HitEffect(const HitEffect& other) :
	on_hit(other.on_hit ? std::make_unique<OnHitFunc>(*other.on_hit) : nullptr),
	boosts(other.boosts ? std::make_unique<SparseBoostsTable>(*other.boosts) : nullptr),
	status(other.status ? std::make_unique<std::string>(*other.status) : nullptr),
	volatile_status(other.volatile_status ? std::make_unique<std::string>(*other.volatile_status) : nullptr),
	side_condition(other.side_condition ? std::make_unique<std::string>(*other.side_condition) : nullptr),
	slot_condition(other.slot_condition ? std::make_unique<std::string>(*other.slot_condition) : nullptr),
	pseudo_weather(other.pseudo_weather ? std::make_unique<std::string>(*other.pseudo_weather) : nullptr),
	terrain(other.terrain ? std::make_unique<std::string>(*other.terrain) : nullptr),
	weather(other.weather ? std::make_unique<std::string>(*other.weather) : nullptr)
{
}