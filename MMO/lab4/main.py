import numpy as np
from keras.datasets import reuters
from keras.models import Sequential
from keras.layers import Dense
from keras.layers.noise import GaussianDropout
from keras.preprocessing.text import Tokenizer
from keras.utils import to_categorical

start_char = 1
oov_char = 2
index_from = 3
num_words = 1000
classes = ['cocoa', 'grain', 'veg-oil', 'earn', 'acq', 'wheat', 'copper', 'housing', 'money-supply',
           'coffee', 'sugar', 'trade', 'reserves', 'ship', 'cotton', 'carcass', 'crude', 'nat-gas',
           'cpi', 'money-fx', 'interest', 'gnp', 'meal-feed', 'alum', 'oilseed', 'gold', 'tin',
           'strategic-metal', 'livestock', 'retail', 'ipi', 'iron-steel', 'rubber', 'heat', 'jobs',
           'lei', 'bop', 'zinc', 'orange', 'pet-chem', 'dlr', 'gas', 'silver', 'wpi', 'hog', 'lead']
num_classes = len(classes)

# Get data
(x_train, y_train), (x_test, y_test) = reuters.load_data(start_char=start_char,
                                                         oov_char=oov_char, index_from=index_from, num_words=num_words)

tokenizer = Tokenizer(num_words=num_words)
x_train = tokenizer.sequences_to_matrix(x_train, mode='binary')
x_test = tokenizer.sequences_to_matrix(x_test, mode='binary')

y_train = to_categorical(y_train, num_classes=num_classes)
y_test = to_categorical(y_test, num_classes=num_classes)

# Setup NN
model = Sequential()
model.add(Dense(num_classes * 16, activation='relu',
          input_shape=(x_train.shape[1],)))
model.add(Dense(num_classes * 8, activation='relu'))
model.add(GaussianDropout(0.05))
model.add(Dense(num_classes, activation='softmax'))
model.compile(optimizer='sgd', loss='binary_crossentropy',
              metrics=['accuracy'])
model.summary()

model.fit(x=x_train, y=y_train, batch_size=3,
          epochs=15, validation_split=0.2, verbose=1)

# Predict test data
YP = model.predict(x_test)

y_test_n = len(y_test)
correct = 0
for i in range(y_test_n):
    y1 = np.argmax(y_test[i])
    y2 = np.argmax(YP[i])
    if y1 == y2:
        correct += 1
print(f"{correct} {y_test_n}")
print(f"Accuracy: {correct / y_test_n * 100}")

prediction = model.predict(x_train)

prediction_class_number = np.argmax(prediction) % num_classes
print(f"Class number: {prediction_class_number}")
print(f"Class name:{classes[prediction_class_number-1]}")
