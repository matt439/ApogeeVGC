#include <variant>
#include <iostream>

class A
{
public:
    int x = 0;
};


int main()
{
    class B;

	std::variant<A*, B*> v;

    std::cout << "Completed.";
	auto y = std::cin.get();
}

