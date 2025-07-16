#include "AnyObject.h"

#include <stdexcept>
#include <memory>

AnyObject::AnyObject(const rapidjson::Value& value)
{
   if (value.IsObject())
   {
       // Use a rapidjson::Document to create a compatible allocator
       rapidjson::Document::AllocatorType& allocator = *(new rapidjson::Document::AllocatorType());
       this->data = std::make_unique<rapidjson::Value>(value, allocator);
   }
   else
   {
       throw std::invalid_argument("AnyObject must be initialized with a JSON object");
   }
}

AnyObject::AnyObject(const AnyObject& other)
{
    if (other.data)
    {
        // Use a rapidjson::Document to provide an allocator for the copy
        rapidjson::Document tempDoc;
        this->data = std::make_unique<rapidjson::Value>(*other.data, tempDoc.GetAllocator());
    }
    else
    {
        this->data = nullptr;
    }
}

DataType AnyObject::get_data_type() const
{
   return DataType::SCRIPTS;
}

std::unique_ptr<IDexData> AnyObject::clone() const
{
   return std::make_unique<AnyObject>(*this);
}