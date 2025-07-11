#pragma once


#include "Format.h"

class DexFormats
{
public:
	int x;

	// Get by Format (returns reference)
	const Format& get_format(const Format& item) const;

	// Get by name (string)
	const Format& get_format(const std::string& name);

	// Get by ID
	const Format& get_format_by_id(const ID& id);

	// Get all formats
	const std::vector<Format>& get_format_items();

	//get(name ? : string | Format, isTrusted = false) : Format{
	//	if (name&& typeof name != = 'string') return name;

	//	name = (name || '').trim();
	//	let id = toID(name);

	//	if (!name.includes('@@@')) {
	//		const ruleset = this.rulesetCache.get(id);
	//		if (ruleset) return ruleset;
	//	}

	//	if (this.dex.getAlias(id)) {
	//		id = this.dex.getAlias(id)!;
	//		name = id;
	//	}
	//	if (this.dex.data.Rulesets.hasOwnProperty(DEFAULT_MOD + id)) {
	//		id = (DEFAULT_MOD + id) as ID;
	//	}
	//	let supplementaryAttributes : AnyObject | null = null;
	//	if (name.includes('@@@')) {
	//		if (!isTrusted) {
	//			try {
	//				name = this.validate(name);
	//				isTrusted = true;
	//			}
	// catch {}
	//}
	//const [newName, customRulesString] = name.split('@@@', 2);
	//name = newName.trim();
	//id = toID(name);
	//if (isTrusted && customRulesString) {
	//	supplementaryAttributes = {
	//		customRules: customRulesString.split(','),
	//		searchShow : false,
	//	};
	//}
	//}
	//let effect;
	//if (this.dex.data.Rulesets.hasOwnProperty(id)) {
	//	effect = new Format({ name, ...this.dex.data.Rulesets[id] as any, ...supplementaryAttributes });
	//}
	//else {
	// effect = new Format({ id, name, exists: false });
	//}
	//return effect;
	//}
};