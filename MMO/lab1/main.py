import time
from lab1 import *


def main():
    data = readCSVData("data1.csv")[:10000]

    trainData, testData = splitTrainTest(data, 0.333333)

    start_time = time.time()
    testLabels = classifyPWRWS(trainData, testData, 2, 2)
    print("%s seconds" % (time.time() - start_time))

    concurrency = 0
    for testDataClass, testLabel in zip(testData, testLabels):
        concurrency += 1 if testDataClass[-1] == testLabel else 0
    print(f'Accuracy: {concurrency/len(testLabels) * 100}%')

    result = [[testData[i][0], testData[i][1], testLabels[i]]
              for i in range(len(testData))]
    showData(result)


if __name__ == "__main__":
    main()
