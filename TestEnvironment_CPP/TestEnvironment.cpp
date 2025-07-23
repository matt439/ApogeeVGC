#include <variant>
#include <iostream>
#include <memory>
#include <functional>

class A
{
public:
    int x = 0;
};


int main()
{
 /*   class B;

	std::variant<A*, B*> v;
*/


	//A* a = 0;

	//if (a == nullptr)
	//{
	//	std::cout << "a is null" << std::endl;
	//}
	//else
	//{
	//	std::cout << "a is not null" << std::endl;
	//}


	//std::unique_ptr<int> p1 = nullptr;
	//std::unique_ptr<int> p2 = std::make_unique<int>(5);

	//p2 = std::move(p1); // p2 now owns the nullptr


	//if (p2 == nullptr)
	//	std::cout << "p2 is null" << std::endl;

	std::function<int(int)> f = nullptr;

	if (f == nullptr)
		std::cout << "f is null" << std::endl;

    std::cout << "Completed.";
	auto y = std::cin.get();
}

