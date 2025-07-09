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

#include <rapidjson/document.h>

#include "TextTableData.h"
#include "AliasesTable.h"
#include "DataType.h"
#include "IModdedDex.h"
#include "TypesManager.h"

class ModdedDex : public IModdedDex
{
public:
    // Data type/class references (could be type aliases or static methods)
    // For C++, these are typically not needed, but you can provide static access if desired.

    // Metadata
    std::string name = "[ModdedDex]";
    bool is_base = false;
    std::string current_mod = "";
    std::string data_dir = "";

    // Utility function alias
    using ToIDFunc = std::string(*)(const std::string&);
    ToIDFunc toID = to_id; // from dex-data.h

    // State
    int gen = 0;
    std::string parent_mod = "";
    bool mods_loaded = false;

    // Data caches
	std::optional<DexTableData> data_cache = std::nullopt;
	std::optional<TextTableData> text_cache = std::nullopt;

    // Utility functions (placeholders, define as needed)
    // For deepClone, deepFreeze, Multiset, use std::function or static methods
    // std::function<ReturnType(Args...)> deepClone;
    // std::function<ReturnType(Args...)> deepFreeze;
    // MultisetType Multiset;

	std::unique_ptr<DexFormats> formats = nullptr;
	std::unique_ptr<DexAbilities> abilities = nullptr;
	std::unique_ptr<DexItems> items = nullptr;
	std::unique_ptr<DexMoves> moves = nullptr;
	std::unique_ptr<DexSpecies> species = nullptr;
	std::unique_ptr<DexConditions> conditions = nullptr;
	std::unique_ptr<DexNatures> natures = nullptr;
	std::unique_ptr<DexTypes> types = nullptr;
	std::unique_ptr<DexStats> stats = nullptr;

    // Aliases
	std::optional<AliasesTable> aliases = std::nullopt;
    // For fuzzyAliases, use a map from ID to vector of IDs
	std::optional<std::unordered_map<IDEntry, std::vector<IDEntry>>> fuzzy_aliases = std::nullopt;

    ModdedDex(const std::string& mod = "base");

    // Returns the loaded data table for this dex
    DexTableData& data() override;

    // Returns the global map of all dexes, ensuring mods are included
    static std::unordered_map<std::string, ModdedDex>& get_dexes();

    // Returns a ModdedDex for a given mod name, ensuring data is included
    static ModdedDex& mod(const std::string& mod = "base");

    // Returns a ModdedDex for a given generation, or this if gen is 0
    ModdedDex& for_gen(int gen);

    // Returns the correct ModdedDex for a given format
    ModdedDex& for_format(const Format& format);

    // Returns the correct data entry for a given type and id, handling inheritance and deep cloning
    template<typename T>
    T& mod_data(DataType data_type, const std::string& id);

    // Returns the name of the dex/effect
    std::string effect_to_string() const;

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

    TypesManager types_manager = TypesManager();

    /**
     * get_immunity() returns false if the target is immune; true otherwise.
     * Also checks immunity to some statuses.
     */
     // Overload for string source and string target
    bool get_immunity(const std::string& source_type, const std::string& target_type) const;

    // Overload for string source and vector<string> target
    bool get_immunity(const std::string& source_type, const std::vector<std::string>& target_types) const;

    // Overload for object source (with .type) and string/array target
    template<typename Source>
    bool get_immunity(const Source& source, const std::vector<std::string>& target_types) const;
    template<typename Source>
    bool get_immunity(const Source& source, const std::string& target_type) const;

    // For string source and string target
    int get_effectiveness(const std::string& source_type, const std::string& target_type) const;

    // For string source and vector<string> target
    int get_effectiveness(const std::string& source_type, const std::vector<std::string>& target_types) const;

    // For object source (with .type) and string/array target
    template<typename Source>
    int get_effectiveness(const Source& source, const std::string& target_type) const;
    template<typename Source>
    int get_effectiveness(const Source& source, const std::vector<std::string>& target_types) const;

    // Helper to map DataType to string key
    std::string data_type_to_key(DataType data_type);

    // Returns a pointer to the relevant RapidJSON value, or nullptr if not found/invalid
    const rapidjson::Value* load_data_file(const std::string& base_path, DataType data_type);

    // Returns a pointer to the relevant RapidJSON value, or nullptr if not found/invalid
    const rapidjson::Value* load_text_file(const std::string& name, const std::string& export_name);

    void include_mods();

    // TODO
    // includeModData
    // includeData

    TextTableData& load_text_data();

    std::optional<std::string> get_alias(const std::string& id) override;

    AliasesTable& load_aliases();

    std::optional<DexTableData> get_data_cache() override;
    int get_gen() const override;

};