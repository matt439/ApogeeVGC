#pragma once

#include <optional>
#include <string>

struct HasID
{
	std::optional<std::string> id = std::nullopt;
	std::optional<std::string> userid = std::nullopt;
	std::optional<std::string> roomid = std::nullopt;

	HasID() = default;
	HasID(const HasID&) = default;
	HasID(const std::string& id);
	HasID(const std::optional<std::string>& id);
	HasID(const std::optional<std::string>& userid, const std::optional<std::string>& roomid);
	HasID(const std::optional<std::string>& id, const std::optional<std::string>& userid,
		const std::optional<std::string>& roomid);
};