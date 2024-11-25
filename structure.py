import numpy as np
import keras
import matplotlib.pyplot as plt

sigma = (3 / 100) ** (0.5)
(x_train, _), (x_test, _) = keras.datasets.mnist.load_data()
x_train = x_train.astype('float32') / 255.
x_test = x_test.astype('float32') / 255.
x_train = np.reshape(x_train, (len(x_train), 28, 28, 1))
x_test = np.reshape(x_test, (len(x_test), 28, 28, 1))

x_train_noisy = x_train + np.random.normal(loc=0.0, scale=sigma, size=x_train.shape)
x_test_noisy = x_test + np.random.normal(loc=0.0, scale=sigma, size=x_test.shape)

model = keras.models.load_model('filter_model.keras')
keras.utils.plot_model(model, "model.png", show_shapes=True)

reconstructed_images = model.predict(x_test_noisy)

# 8. Визуализация результатов
plt.figure(figsize=(10, 5))
for i in range(4):
    plt.subplot(3, 4, i + 1)
    plt.imshow(x_test[i].reshape(28, 28), cmap='gray')
    plt.axis('off')

    plt.subplot(3, 4, i + 5)
    plt.imshow(x_test_noisy[i].reshape(28, 28), cmap='gray')
    plt.axis('off')

    plt.subplot(3, 4, i + 9)
    plt.imshow(reconstructed_images[i].reshape(28, 28), cmap='gray')
    plt.axis('off')

plt.show()