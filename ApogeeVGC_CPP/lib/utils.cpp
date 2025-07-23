#include "utils.h"

using namespace rapidjson;

Value deep_clone(const Value& value, Document::AllocatorType& allocator)
{
    if (value.IsNull() || value.IsBool() || value.IsInt() || value.IsUint() ||
        value.IsInt64() || value.IsUint64() || value.IsDouble() || value.IsString())
    {
        // For primitive types, just copy
        return Value(value, allocator);
    }
    if (value.IsArray())
    {
        Value arr(kArrayType);
        for (auto& v : value.GetArray())
        {
            arr.PushBack(deep_clone(v, allocator), allocator);
        }
        return arr;
    }
    if (value.IsObject())
    {
        Value obj(kObjectType);
        for (auto it = value.MemberBegin(); it != value.MemberEnd(); it++)
        {
            obj.AddMember(Value(it->name, allocator),
                deep_clone(it->value, allocator),
                allocator);
        }
        return obj;
    }
    // Should not reach here
    return Value();
}
