#include "assign_missing_fields.h"

template<typename ValueType>
void assign_missing_fields(std::map<std::string, ValueType>& self, const std::map<std::string, ValueType>& data)
{
    for (const auto& [k, v] : data)
    {
        if (self.find(k) != self.end()) continue;
        self[k] = v;
    }
}