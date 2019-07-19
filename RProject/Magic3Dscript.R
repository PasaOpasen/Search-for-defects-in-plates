
library(rgl)
library(plot3D)
library(data.table)

xx = fread("args.txt", header = F, dec = ",")
z = fread("vals.txt", header = F, dec = ",")
x = xx[[1]]
y = xx[[2]]

scalecoef = (max(y) - min(y)) / (max(x) - min(x))
x2 = x * scalecoef

z4 = matrix(z[[1]], nrow = length(x), ncol = length(y), byrow = TRUE)

urmin = -20
urmax = 20
levels = 15

dt = readLines("info.txt")

pdf(file = dt[[1]],width =12, height = 12, paper = "letter")


persp3D(z = z4, x = x2, y = y, scale = FALSE, zlab = dt[[5]], xlab = dt[[3]], ylab = dt[[4]],
contour = list(nlevels = levels, col = "red"),
#zlim = c(-40, max(uzIm, na.rm = TRUE) * 0.7),
        expand = 0.2,
       image = list(col = grey(seq(0, 1, length.out = 100))), main = dt[[2]])

dev.off()