#include "TeamValidator.h"

//#include "../dex-format/Format.h"
//#include "../dex/IModdedDex.h"
//#include "../dex-format/RuleTable.h"
//#include <stdexcept>

TeamValidator::TeamValidator(const std::string& format_name, IModdedDex* dex_instance)
	: dex(dex_instance)
{
	//if (!dex) {
	//	throw std::runtime_error("Dex instance is null");
	//}
	//// Get the format by name
	//format = dex->get_format(format_name);
	//if (format.id.empty()) {
	//	throw std::runtime_error("Format not found: " + format_name);
	//}
	//// Initialize modded_dex
	//modded_dex = dex->get_modded_dex_for_format(format);
	//if (!modded_dex) {
	//	throw std::runtime_error("ModdedDex not found for format: " + format_name);
	//}
	//gen = modded_dex->gen;
	//rule_table = modded_dex->formats->get_rule_table(&format);
	//min_source_gen = rule_table.min_source_gen;
}

TeamValidator::TeamValidator(const Format& format_instance, IModdedDex* dex_instance)
{

}