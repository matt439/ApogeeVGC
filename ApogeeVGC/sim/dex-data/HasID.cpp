#include "HasID.h"

HasID::HasID(const std::string* id)
	: id(id), userid(nullptr), roomid(nullptr) {
}

HasID::HasID(const std::string* userid, const std::string* roomid)
	: id(nullptr), userid(userid), roomid(roomid) {
}

HasID::HasID(const std::string* id, const std::string* userid, const std::string* roomid)
	: id(id), userid(userid), roomid(roomid) {
}