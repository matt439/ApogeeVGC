#pragma once

#include <string>

struct HasID
{
	const std::string* id = nullptr; // optional
	const std::string* userid = nullptr; // optional
	const std::string* roomid = nullptr; // optional

	HasID() = default;
	HasID(const HasID&) = default;
	HasID(const std::string* id);
	HasID(const std::string* userid, const std::string* roomid);
	HasID(const std::string* id, const std::string* userid, const std::string* roomid);
};