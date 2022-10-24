all: single multiple cache

single:
	g++ -std=c++0x sp.cpp -o sp.out -lvg

multiple:
	g++ -std=c++0x mp_send.cpp -o mp_send.out -lvg
	g++ -std=c++0x mp_receive.cpp -o mp_receive.out -lvg

cache:
	g++ -std=c++0x cache.cpp -o cache.out -lvg