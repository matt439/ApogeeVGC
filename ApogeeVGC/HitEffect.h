#pragma once

#include "GlobalTypes.h"
#include "MoveEventMethods.h"

struct HitEffect
{  
   // Optional function, type depends on MoveEventMethods::on_hit signature  
   std::optional<OnHitFunc> on_hit;

   // set pokemon conditions  
   std::optional<SparseBoostsTable> boosts;
   std::optional<std::string> status;  
   std::optional<std::string> volatile_status;  

   // set side/slot conditions  
   std::optional<std::string> side_condition;  
   std::optional<std::string> slot_condition;  

   // set field conditions  
   std::optional<std::string> pseudo_weather;  
   std::optional<std::string> terrain;  
   std::optional<std::string> weather;  
};