import random
import math
import pylab as pl
import numpy as np
from matplotlib.colors import ListedColormap
import csv


def readCSVData(filename):
    data = []
    count = 0
    with open(filename, 'r') as file_in:
        reader = csv.reader(file_in)
        for row in reader:
            if count == 0:
                count += 1
                continue
            data.append([int(x) for x in row])
    return data


def showData(trainData):
    classColormap = ListedColormap(['#FF0000', '#00FF00'])
    pl.scatter([trainData[i][0] for i in range(len(trainData))],
               [trainData[i][1] for i in range(len(trainData))],
               c=[trainData[i][2]+1 for i in range(len(trainData))],
               cmap=classColormap)
    pl.show()


def splitTrainTest(data, testPercent):
    trainData = []
    testData = []
    for row in data:
        if random.random() < testPercent:
            testData.append(row)
        else:
            trainData.append(row)
    return trainData, testData


def classifyKNN(trainData, testData, k, numberOfClasses):
    # Euclidean distance between 2-dimensional point
    def dist(a, b):
        return math.sqrt((a[0] - b[0])**2 + (a[1] - b[1])**2)

    def distWindow(a, b, distance):
        distance = math.sqrt((a[0] - b[0])**2 + (a[1] - b[1])**2)
        if (abs(distance) > 1):
            return 0
        return 1
    testLabels = []
    for testPoint in testData:
        # Claculate distances between test point and all of the train points
        testDist = [[dist(testPoint, [trainData[i][0], trainData[i][1]]), trainData[i][2]]
                    for i in range(len(trainData))]
        # How many points of each class among nearest K
        stat = [0 for i in range(numberOfClasses)]
        for d in sorted(testDist)[0:k]:
            stat[d[1]] += 1
        # Assign a class with the most number of occurences among K nearest neighbours
        testLabels.append(
            sorted(zip(stat, range(numberOfClasses)), reverse=True)[0][1])
    return testLabels


def classifyPWRWS(z):

    pass


def main():
    data = readCSVData("data1.csv")
    # print(data)
    # showData(data[:100])

    trainData, testData = splitTrainTest(data[:100], 0.333333)
    classifyKNN(trainData, testData, 2, 2)


if __name__ == "__main__":
    main()
