#include "TextFile.h"

template <typename T>
TextFile<T>::TextFile(const std::string& name) :
	name(name), data()
{
}

template <typename T>
TextFile<T>::TextFile(const TextFile& other) :
	name(other.name),
	gen1(other.gen1 ? std::make_unique<T>(*other.gen1) : nullptr),
	gen2(other.gen2 ? std::make_unique<T>(*other.gen2) : nullptr),
	gen3(other.gen3 ? std::make_unique<T>(*other.gen3) : nullptr),
	gen4(other.gen4 ? std::make_unique<T>(*other.gen4) : nullptr),
	gen5(other.gen5 ? std::make_unique<T>(*other.gen5) : nullptr),
	gen6(other.gen6 ? std::make_unique<T>(*other.gen6) : nullptr),
	gen7(other.gen7 ? std::make_unique<T>(*other.gen7) : nullptr),
	gen8(other.gen8 ? std::make_unique<T>(*other.gen8) : nullptr),
	data(other.data ? std::make_unique<T>(other.data) : nullptr)
{
}

//TextFile<ConditionTextData>::TextFile(const std::string& name) :
//	TextFile<ConditionTextData>::TextFile(name)
//{
//	// Initialize data if needed
//}


TextFile<ConditionTextData>::TextFile(const TextFile& other) :
	TextFile<ConditionTextData>::TextFile(other.name)
{
	if (other.gen1) gen1 = std::make_unique<ConditionTextData>(*other.gen1);
	if (other.gen2) gen2 = std::make_unique<ConditionTextData>(*other.gen2);
	if (other.gen3) gen3 = std::make_unique<ConditionTextData>(*other.gen3);
	if (other.gen4) gen4 = std::make_unique<ConditionTextData>(*other.gen4);
	if (other.gen5) gen5 = std::make_unique<ConditionTextData>(*other.gen5);
	if (other.gen6) gen6 = std::make_unique<ConditionTextData>(*other.gen6);
	if (other.gen7) gen7 = std::make_unique<ConditionTextData>(*other.gen7);
	if (other.gen8) gen8 = std::make_unique<ConditionTextData>(*other.gen8);
	if (other.data) data = std::make_unique<ConditionTextData>(*other.data);
}


//TextFile<MoveTextData>::TextFile(const std::string& name);

TextFile<MoveTextData>::TextFile(const TextFile& other) :
	TextFile<MoveTextData>::TextFile(other.name)
{
	if (other.gen1) gen1 = std::make_unique<MoveTextData>(*other.gen1);
	if (other.gen2) gen2 = std::make_unique<MoveTextData>(*other.gen2);
	if (other.gen3) gen3 = std::make_unique<MoveTextData>(*other.gen3);
	if (other.gen4) gen4 = std::make_unique<MoveTextData>(*other.gen4);
	if (other.gen5) gen5 = std::make_unique<MoveTextData>(*other.gen5);
	if (other.gen6) gen6 = std::make_unique<MoveTextData>(*other.gen6);
	if (other.gen7) gen7 = std::make_unique<MoveTextData>(*other.gen7);
	if (other.gen8) gen8 = std::make_unique<MoveTextData>(*other.gen8);
	if (other.data) data = std::make_unique<MoveTextData>(*other.data);
}

TextFile<BasicTextData>::TextFile(const TextFile& other) :
	TextFile<BasicTextData>::TextFile(other.name)
{
	if (other.gen1) gen1 = std::make_unique<BasicTextData>(*other.gen1);
	if (other.gen2) gen2 = std::make_unique<BasicTextData>(*other.gen2);
	if (other.gen3) gen3 = std::make_unique<BasicTextData>(*other.gen3);
	if (other.gen4) gen4 = std::make_unique<BasicTextData>(*other.gen4);
	if (other.gen5) gen5 = std::make_unique<BasicTextData>(*other.gen5);
	if (other.gen6) gen6 = std::make_unique<BasicTextData>(*other.gen6);
	if (other.gen7) gen7 = std::make_unique<BasicTextData>(*other.gen7);
	if (other.gen8) gen8 = std::make_unique<BasicTextData>(*other.gen8);
	if (other.data) data = std::make_unique<BasicTextData>(*other.data);
}