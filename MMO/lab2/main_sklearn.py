import random
import csv
import numpy as np
import sklearn as sk
from sklearn.impute import SimpleImputer
from sklearn.tree import DecisionTreeClassifier
from itertools import product


def main():
    data = np.genfromtxt('heart_data.csv', delimiter=',',
                         skip_header=True)

    X = data[:, :-1]
    Y = data[:, -1]

    X_train, X_test, Y_train, Y_test = sk.model_selection.train_test_split(
        X, Y, test_size=0.3)

    imp = SimpleImputer()
    imp.fit(X_train)

    X_train = imp.transform(X_train)

    with open('result.csv', 'w', newline='', encoding='utf-8') as fileOut:
        writer = csv.writer(fileOut)
        header = ["accuracy", "max depth", "max leaf nodes"]
        writer.writerow(header)
        # print(header)
        for max_depth, max_leaf_nodes in product(range(10, 100, 10), range(10, 100, 10)):
            clf = DecisionTreeClassifier(
                max_depth=max_depth, max_leaf_nodes=max_leaf_nodes)
            clf.fit(X_train, Y_train)

            X_test = imp.transform(X_test)
            predicted = clf.predict(X_test)

            result_row = [sk.metrics.accuracy_score(
                Y_test, predicted), max_depth, max_leaf_nodes]

            writer.writerow(result_row)
            print(result_row)


if __name__ == "__main__":
    main()
