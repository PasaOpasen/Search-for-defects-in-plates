
library(data.table)

w=getwd()

d = read.table(paste0(w,"\\OnePoint.txt"), dec = ",",header = TRUE)
n = d[[1]]

eps1 = d[[2]]
eps2 = d[[3]]
s = readLines(paste0(w, "\\OnePoint(info).txt"))[1]
print(s)
limss = c(min(min(eps1), min(eps2)), max(max(eps1), max(eps2)))

pdf(file = paste0(s, ".pdf"), width = 20)

plot(n, eps1, type = "l", lty = 30, lwd = 3, pch = 16, col = "green", ann = FALSE,ylim=limss)
lines(n, eps2, type = "l", lty = 30, lwd = 3, pch = 16, col = "red", ann = FALSE)
title(main = "Functions ur(x,t), uz(x,t)", xlab = "t", ylab = "u")

legend("topleft", inset = 0.05, title = "Function", c("ur", "uz"), col = c("green", "red"), lty = c(2, 2), pch = c(16, 16))

dev.off()


library(data.table)

w = getwd()

d = read.table(paste0(w, "\\OnePoint(w).txt"), dec = ",", header = TRUE)
n = d[[1]]

eps1 = d[[2]]
eps2 = d[[3]]
eps3 = d[[4]]
eps4 = d[[5]]
eps5 = d[[6]]
eps6 = d[[7]]

s = readLines(paste0(w, "\\OnePoint(info).txt"))[2]
#limss = c(min(min(eps1), min(eps2)), max(max(eps1), max(eps2)))

pdf(file = paste0(s, ".pdf"), width = 20)

plot(n, eps1, type = "l", lty = 30, lwd = 3, pch = 16, col = "green", ann = FALSE)
lines(n, eps2, type = "l", lty = 30, lwd = 3, pch = 16, col = "red", ann = FALSE)
lines(n, eps3, type = "l", lty = 30, lwd = 3, pch = 16, col = "blue", ann = FALSE)
lines(n, eps4, type = "l", lty = 30, lwd = 3, pch = 16, col = "black", ann = FALSE)
lines(n, eps5, type = "l", lty = 30, lwd = 3, pch = 16, col = "yellow", ann = FALSE)
lines(n, eps6, type = "l", lty = 30, lwd = 3, pch = 16, col = "pink", ann = FALSE)


title(main = "Functions u(x,w)", xlab = "t", ylab = "u")

legend("topleft", inset = 0.05, title = "Function", c("Re ux", "Im ux","Re uy","Im uy","Re uz","Im uz"))

dev.off()




w = getwd()

d = read.table(paste0(w, "\\f(w) from (0 , 0).txt"), dec = ",", header = TRUE)
n = d[[1]]

eps1 = d[[2]]
eps2 = d[[3]]
eps3=sqrt(eps1^2+eps2^2)
print(s)
limss = c(min(min(eps1), min(eps2)), max(max(eps1), max(eps2)))

pdf(file = paste0("f(w) from(0, 0)", ".pdf"), width = 20)

plot(n, eps3, type = "l", lty = 30, lwd = 3, pch = 16, col = "green", ann = FALSE, ylim = c(min(eps3),max(eps3)))
#plot(n, eps3, type = "l", lty = 30, lwd = 3, pch = 16, col = "green", ann = FALSE, ylim = limss)
#lines(n, eps2, type = "l", lty = 30, lwd = 3, pch = 16, col = "red", ann = FALSE)
#title(main = "Functions Re(f(w)), Im(f(w))", xlab = "t", ylab = "u")

#legend("topleft", inset = 0.05, title = "Function", c("Re", "Im"), col = c("green", "red"), lty = c(2, 2), pch = c(16, 16))

dev.off()