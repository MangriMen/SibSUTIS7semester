import random
import csv
import numpy as np
import sklearn as sk
from sklearn.impute import SimpleImputer
from sklearn.tree import DecisionTreeClassifier
from itertools import product
from sklearn.linear_model import LinearRegression
from sklearn import model_selection
from sklearn import preprocessing
import matplotlib.pyplot as plt


def testData(data):
    score = []
    for _ in range(10):
        x = data[:, :-1]
        y = data[:, -1]

        x_train, x_test, y_train, y_test = model_selection.train_test_split(
            x, y, test_size=0.3)

        imputer = SimpleImputer()

        imputer.fit(x_train)

        x_train = imputer.transform(x_train)
        x_train = preprocessing.normalize(x_train)

        x_test = imputer.transform(x_test)
        x_test = preprocessing.normalize(x_test)

        regressor = LinearRegression()
        regressor.fit(x_train, y_train)

        y_predicted = regressor.predict(x_test)

        score.append(sk.metrics.mean_squared_error(y_test, y_predicted))
        # print(sk.metrics.mean_squared_error(y_test, y_predicted))
        # print(sk.metrics.r2_score(y_test, y_predicted))
    avg_score = 0
    for s in score:
        avg_score += s
    avg_score /= 10

    return avg_score


def main():
    general_data = np.genfromtxt(
        'winequalityN.csv', delimiter=',',
        dtype=[str, float, float, float, float, float, float, float, float, float, float, float, float], skip_header=True)

    print(general_data[:1])
    red_wine_data = [x for x in general_data if x[0] == "red"]
    white_wine_data = [x for x in general_data if x[0] == "white"]

    print(f'General: {testData(general_data)}')
    print(f'Red: {testData(red_wine_data)}')
    print(f'White: {testData(white_wine_data)}')


if __name__ == "__main__":
    main()
