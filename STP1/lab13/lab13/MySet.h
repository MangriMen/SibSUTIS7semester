#pragma once
#include <iostream>
#include <memory>

template <typename T>
class MySet
{
public:
	class Tree {
		public class Node;

		std::shared_ptr<Node> _root;

	public:
		public class Node {
			typedef Node_ptr std::shared_ptr<T>;

			T data;
			Node_ptr next;

			Node(T data, Node_ptr next = nullptr, Node_ptr right = nullptr) {
				this->data = data;
				this->left = left;
				this->right = right;
			}
		};

		Tree(T root_data) {
			_root = make_shared(new Node(root_data));
		}

		void add(std::shared_ptr<Node> root, T data) {
			if (root == nullptr) {
				root = make_shared(new Node(data));
				return;
			}
			
			add(root->)
		}

		std::shared_ptr<Node> getRoot() {
			return _root;
		}
	};

	Tree _data;

	MySet() {

	}
};

