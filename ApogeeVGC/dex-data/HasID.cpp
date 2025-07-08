#include "HasID.h"

HasID::HasID(const std::string& id) : id(id)
{
}

HasID::HasID(const std::optional<std::string>& id) : id(id)
{
}

HasID::HasID(const std::optional<std::string>& userid, const std::optional<std::string>& roomid)
    : userid(userid), roomid(roomid)
{
}

HasID::HasID(const std::optional<std::string>& id, const std::optional<std::string>& userid,
    const std::optional<std::string>& roomid)
    : id(id), userid(userid), roomid(roomid)
{
}