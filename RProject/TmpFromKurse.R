str(AirPassengers) # структура данных

vec = as.vector(AirPassengers)
good_months = c()
for (i in 2:length(vec)) {
    if (vec[i] > vec[i - 1]) {
        good_months = c(good_months, vec[i])
    }
}
good_months



vec = as.vector(AirPassengers)
moving_average <- numeric(135)
for (i in 1:135) {
    moving_average[i]=mean(vec[i:(i+9)])
}

print(moving_average)