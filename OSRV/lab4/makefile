.PHONY: run1

CXX=g++
CXXFLAGS=-std=c++0x -g

OBJECTS=/root/labs/plates.o
LIBS=-lvg

all: part1 part2 part3 part4

part1:
	$(CXX) $(CXXFLAGS) lab4_1.cpp $(OBJECTS) -o $@.out $(LIBS)
	
part2:
	$(CXX) $(CXXFLAGS) lab4_2.cpp $(OBJECTS) -o $@.out $(LIBS)

part3:
	$(CXX) $(CXXFLAGS) lab4_3.cpp $(OBJECTS) -o $@.out $(LIBS)

part4:
	$(CXX) $(CXXFLAGS) lab4_4.cpp $(OBJECTS) -o $@.out $(LIBS)

run1: part1
	./$<.out

run2: part2
	./$<.out

run3: part3
	./$<.out

run4: part4
	./$<.out