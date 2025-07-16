#include "to_id.h"

std::string to_id(const std::string& text)
{
    std::string result;
    for (char c : text)
    {
        if (std::isalnum(static_cast<unsigned char>(c)))
        {
            result += std::tolower(static_cast<unsigned char>(c));
        }
    }
    return result;
}

std::string to_id(int num)
{
    return to_id(std::to_string(num));
}

std::string to_id(const HasID& obj)
{
    if (obj.id) return to_id(*obj.id);
    if (obj.userid) return to_id(*obj.userid);
    if (obj.roomid) return to_id(*obj.roomid);
    return "";
}