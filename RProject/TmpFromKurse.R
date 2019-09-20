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


descriptions_stat = aggregate(cbind(hp, disp) ~ am, mtcars, sd)
print(descriptions_stat)



qv = subset(airquality,Month %in% c(7, 8, 9))

result = aggregate(Ozone ~ Month, qv, FUN = length)
print(result)


library(psych)
tmp = describeBy(airquality[, c(1, 2, 3, 4)], group = airquality$Month)
print(tmp)

str(iris)
print(describeBy(iris,group = iris$Species)$virginica)


my_vector <- rnorm(17)
my_vector[sample(1:17, 10)] <- NA # на десять случайных позиций поместим NA

m = mean(my_vector, na.rm = T)
fixed_vector = my_vector
fixed_vector[is.na(fixed_vector)] = m
print(my_vector)
print(fixed_vector)


dimnames(HairEyeColor)
tmp = HairEyeColor[,, 1]
red_men <- prop.table(tmp,2)[3,2]

print(red_men)

ch = sum(HairEyeColor[, "Green", 2])
print(ch)




library("ggplot2")
mydata <- as.data.frame(HairEyeColor[,,"Female"])
print(mydata)

obj <- ggplot(data = mydata, aes(x = Hair, y = Freq, fill = Eye)) +
    geom_bar(stat = "identity", position = position_dodge()) +
    scale_fill_manual(values = c("Brown", "Blue", "Darkgrey", "Darkgreen"))
obj













