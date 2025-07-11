#pragma once

#include "../dex-format/DexFormats.h"
#include "../dex-abilities/DexAbilities.h"
#include "../dex-items/DexItems.h"
#include "../dex-moves/DexMoves.h"
#include "../dex-species/DexSpecies.h"
#include "../dex-conditions/DexConditions.h"
#include "../dex-data/DexNatures.h"
#include "../dex-data/DexTypes.h"
#include "../dex-data/DexStats.h"
#include "../dex-data/to_id.h"
#include "../global-types/StatsTable.h"

#include <rapidjson/document.h>

#include "TextTableData.h"
#include "AliasesTable.h"
#include "DataType.h"
#include "IModdedDex.h"
#include "IDex.h"
#include "Descriptions.h"

#include <filesystem>

class ModdedDex : public IModdedDex
{
public:
	IDex* dex_parent = nullptr;
	std::unordered_map<std::string, ModdedDex>* dexes = nullptr;

    std::string name = "[ModdedDex]";
    bool is_base = false;
    std::string current_mod = "";
    std::filesystem::path data_dir = "";

    //// Utility function alias
    //using ToIDFunc = std::string(*)(const std::string&);
    //ToIDFunc toID = to_id; // from dex-data.h

    int gen = 0;
    std::string parent_mod = "";
    bool mods_loaded = false;

	std::unique_ptr<DexTableData> data_cache = nullptr;
	std::unique_ptr<TextTableData> text_cache = nullptr;

	std::unique_ptr<DexFormats> formats = nullptr;
	std::unique_ptr<DexAbilities> abilities = nullptr;
	std::unique_ptr<DexItems> items = nullptr;
	std::unique_ptr<DexMoves> moves = nullptr;
	std::unique_ptr<DexSpecies> species = nullptr;
	std::unique_ptr<DexConditions> conditions = nullptr;
	std::unique_ptr<DexNatures> natures = nullptr;
	std::unique_ptr<DexTypes> types = nullptr;
	std::unique_ptr<DexStats> stats = nullptr;

	std::unique_ptr<AliasesTable> aliases = nullptr;
    // For fuzzyAliases, use a map from ID to vector of IDs
	std::unique_ptr<std::unordered_map<IDEntry, std::vector<IDEntry>>> fuzzy_aliases = nullptr;

    ModdedDex(IDex* dex_parent, const std::string& mod = "base");

    // Returns the loaded data table for this dex
    DexTableData* get_data() override;

    // Returns the global map of all dexes, ensuring mods are included
    std::unordered_map<std::string, ModdedDex>* get_dexes();

    // Returns a ModdedDex for a given mod name, ensuring data is included
	ModdedDex* get_modded_dex(const std::string& mod = "base") override;

    // Returns a ModdedDex for a given generation, or this if gen is 0
    ModdedDex* get_modded_dex_for_gen(int gen);

    // Returns the correct ModdedDex for a given format
    ModdedDex* get_modded_dex_for_format(const Format& format);

    // Returns the correct data entry for a given type and id, handling inheritance and deep cloning
    template<typename T>
    T& mod_data(DataType data_type, const std::string& id);

    // Returns the name of the dex/effect
    const std::string& effect_to_string() const;

    // Helper to trim whitespace from both ends
    static std::string trim(const std::string& s);

    /*
     * Sanitizes a username or Pokemon nickname
     *
     * Returns the passed name, sanitized for safe use as a name in the PS
     * protocol.
     *
     * Such a string must uphold these guarantees:
     * - must not contain any ASCII whitespace character other than a space
     * - must not start or end with a space character
     * - must not contain any of: | , [ ]
     * - must not be the empty string
     * - must not contain Unicode RTL control characters
     *
     * If no such string can be found, returns the empty string. Calling
     * functions are expected to check for that condition and deal with it
     * accordingly.
     *
     * getName also enforces that there are not multiple consecutive space
     * characters in the name, although this is not strictly necessary for
     * safety.
     */
    static std::string get_name(const std::string& input);

    // TypesManager types_manager = TypesManager();

    /**
     * get_immunity() returns false if the target is immune; true otherwise.
     * Also checks immunity to some statuses.
     */
	bool get_immunity(const std::string& source_type, const std::string& target_type) const;
    bool get_immunity(const std::string& source_type,
        const std::vector<const std::string&>& target_types) const;


    // // Overload for string source and string target
    //bool get_immunity(const std::string& source_type, const std::string& target_type) const;

    //// Overload for string source and vector<string> target
    //bool get_immunity(const std::string& source_type, const std::vector<std::string>& target_types) const;

    //// Overload for object source (with .type) and string/array target
    //template<typename Source>
    //bool get_immunity(const Source& source, const std::vector<std::string>& target_types) const;
    //template<typename Source>
    //bool get_immunity(const Source& source, const std::string& target_type) const;

    // For string source and string target
    int get_effectiveness(const std::string& source_type, const std::string& target_type) const;

    // For string source and vector<string> target
    int get_effectiveness(const std::string&
        source_type, const std::vector<std::string>& target_types) const;

    //// For object source (with .type) and string/array target
    //template<typename Source>
    //int get_effectiveness(const Source& source, const std::string& target_type) const;
    //template<typename Source>
    //int get_effectiveness(const Source& source, const std::vector<std::string>& target_types) const;

	Descriptions get_descriptions(const std::string& table, const std::string& id) override;

	ActiveMove& get_active_move(const std::string& id);
	ActiveMove& get_active_move(const Move& move);

    StatsTable get_hidden_power();

    std::vector<AnyObject> data_search(const std::string & target, 
        const std::vector<std::string>&search_in = {}, bool is_inexact = false);



    //// Helper to map DataType to string key
    //std::string data_type_to_key(DataType data_type);

    // Returns a pointer to the relevant RapidJSON value, or nullptr if not found/invalid
    const rapidjson::Value* load_data_file(const std::string& base_path, DataType data_type);

    // Returns a pointer to the relevant RapidJSON value, or nullptr if not found/invalid
    const rapidjson::Value* load_text_file(const std::string& name, const std::string& export_name);

    ModdedDex* include_mods();

    ModdedDex* include_mod_data();

    ModdedDex* include_data();

    TextTableData& load_text_data();

	// Returns a pointer to the alias for the given ID, or nullptr if not found
    std::string* get_alias(const ID& id) override;

    AliasesTable& load_aliases();

	void load_data();

    ModdedDex* include_formats();

	// Extra functions for IDex interface so that other classes can use it
	// without being exposed to ModdedDex specifics
    DexTableData* get_data_cache() override;
    int get_gen() const override;

	const std::string& get_parent_mod() const override;

};