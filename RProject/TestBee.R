
#bee
s = read.table("bee.txt", dec = ",")
x = s[[1]]
y = s[[2]]
n = 1:length(x)
df = data.frame(pair = n, error = (x - y) / y * 100)

png(filename = "bee.png",width = 800, height = 850)
library(ggplot2)
ggplot(df, aes(x = pair, y = error)) +
    geom_point(col = "green", size = 3) +
    geom_line(y = 0, col = "red", size = 1.3)

dev.off()