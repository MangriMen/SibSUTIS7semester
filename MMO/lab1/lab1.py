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
    return np.array(data)


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
    return np.array(trainData), np.array(testData)


def reactangleCore(u):
    return 1/2 if abs(u) <= 1 else 0


def dist(a, b):
    return math.sqrt((a[0] - b[0])**2 + (a[1] - b[1])**2)


def classifyPWRWS(trainData, testData, k, numberOfClasses):
    testLabels = []
    for testPoint in testData:
        testDist = []
        for i in np.ndindex(trainData.shape[0]):
            distU = dist(testPoint, [trainData[i][0], trainData[i][1]])
            window = dist(testPoint, [trainData[k+1][0], trainData[k+1][1]])

            testDist.append((reactangleCore(distU/window), trainData[i][2]))

        stat = [0 for i in range(numberOfClasses)]
        filteredTestDist = np.array([x for x in testDist if x[0] >= 0.5])

        for d in filteredTestDist:
            stat[int(d[1])] += 1

        testLabels.append(
            sorted(zip(stat, range(numberOfClasses)), reverse=True)[0][1])
    return testLabels
