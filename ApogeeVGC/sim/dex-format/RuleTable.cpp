#include "RuleTable.h"

bool RuleTable::is_banned(const std::string& thing) const
{
	return false; // TODO implement this
}

bool RuleTable::is_banned_species(const Species& species) const
{
	return false; // TODO implement this
}

bool RuleTable::is_restricted(const std::string& thing) const
{
	return false; // TODO implement this
}

bool RuleTable::is_restricted_species(const Species& species) const
{
	return false; // TODO implement this
}

std::vector<std::string> RuleTable::get_tag_rules() const
{
	return tag_rules;
}

std::string* RuleTable::check(const std::string& thing,
	const std::unordered_map<std::string, bool>* set_has)
{
	// TODO implement this
	return nullptr; // placeholder
}

std::string* RuleTable::get_reason(const std::string& key)
{
	return nullptr; // TODO implement this
}

std::string RuleTable::blame(const std::string& key) const
{
	// TODO implement this
	return ""; // placeholder
}

int RuleTable::get_complex_ban_index(const std::vector<ComplexBan>& complex_bans,
	const std::string& rule) const
{
	// TODO implement this
	return -1; // placeholder
}

void RuleTable::add_complex_ban(const std::string& rule, const std::string& source, int limit,
	const std::vector<std::string>& bans)
{
	// TODO implement this
}

void RuleTable::add_complex_team_ban(const std::string& rule, const std::string& source, int limit,
	const std::vector<std::string>& bans)
{
	// TODO implement this
}

/** After a RuleTable has been filled out, resolve its hardcoded numeric properties */
void RuleTable::resolve_numbers(const Format& format, IModdedDex* dex)
{
	// TODO implement this
}

bool RuleTable::has_complex_bans() const
{
	return false; // TODO implement this
}