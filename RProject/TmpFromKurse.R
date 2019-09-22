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



dat = HairEyeColor["Brown",, "Female"]
print(dat)

chisq.test(dat)


library("ggplot2")

main_stat =chisq.test(x = diamonds$cut, y = diamonds$color)[[1]]

print(main_stat)


library("ggplot2")
d = diamonds
price.mean = mean(d$price)
carat.mean = mean(d$carat)
d$price = factor(ifelse(d$price >= price.mean, 1, 0))
d$carat = factor(ifelse(d$carat >= carat.mean, 1, 0))
main_stat = chisq.test(x = d$price, y = d$carat)[[1]]
print(main_stat)



fisher_test = fisher.test(mtcars$am, mtcars$vs)[[1]]
print(fisher_test)

s = ToothGrowth
t_stat=t.test(s$len[s$supp == "OJ" & s$dose == 0.5], s$len[s$supp == "VC" & s$dose == 2])$statistic

str(t.test(s$len[s$supp == "OJ" & s$dose == 0.5], s$len[s$supp == "VC" & s$dose == 2]))



df <- read.csv(url('https://stepic.org/media/attachments/lesson/11504/lekarstva.csv'))
t.test(df$Pressure_before, df$Pressure_after, paired = T)



df = read.table("dataset_11504_15 (1).txt", dec = ".")
df[[2]] = factor(df[[2]])
df=data.frame(df)
print(df)

bartlett.test(df[[1]] ~ df[[2]])
wilcox.test(df[[1]] ~ df[[2]])



df = read.table("dataset_11504_16 (1).txt", dec = ".")
df = data.frame(df)
print(df)

t.test(df[[1]], df[[2]], var.equal = F)