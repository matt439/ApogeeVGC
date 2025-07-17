#include "./sim/dex/Dex.h"
#include <memory>

int main()
{
	std::unique_ptr<Dex> dex = std::make_unique<Dex>();

	return 0;
}