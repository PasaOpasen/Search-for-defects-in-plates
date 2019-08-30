library(data.table)

w = getwd()

d = read.table(paste0(w, "/OnePoint.txt"), dec = ",", header = TRUE)
s = readLines(paste0(w, "/OnePoint(info).txt"))

n = d[[1]]
eps1 = d[[2]]
eps2 = d[[3]]
s0 = readLines(paste0(w, "/OnePoint(info).txt"))[1]
print(s0)

tb = cbind(eps1, eps2)
limss = c(min(tb), max(tb))

len = (length(s) - 1) / 3

arr = c("ArrayA", "ArrayB", "ArrayC", "ArrayD")


if (as.logical(readLines("MakeUxtByEvery.txt")[1])) {
    #if (len < 4)
    pdf(file = paste0(s0, ".pdf"), width = 18,height = 22,pointsize = 20)
    layout(mat = matrix(c(1, 2, 3, 4), 4, 1), heights = c(1, 1, 1, 2.5))
    
    for (i in 1:len) {
        print(paste0("subgrafic ",i))
        d = read.table(paste0(w, "/", s[2 * len + 1 + i]), dec = ",", header = TRUE)
        e1 = d[[2]]
        e2=d[[3]]
        tb = cbind(e1, e2)
        lis = c(min(tb), max(tb))

        plot(n, e1, type = "l", lty = 1, lwd = 3, pch = 16, col = "green", ann = FALSE, ylim = lis)
        lines(n, e2, type = "l", lty = 1, lwd = 3, pch = 16, col = "red", ann = FALSE)
        title(main = paste0("Functions ur(x,t), uz(x,t) ",s[1+i]), xlab = "t", ylab = "u")

        legend("topleft", inset = 0.05, title = "Function", c("ur", "uz"), col = c("green", "red"), lty = c(1, 1), pch = c(16, 16))
    }
}else{
pdf(file = paste0(s0, ".pdf"), width = 20)
}

plot(n, eps1, type = "l", lty = 30, lwd = 3, pch = 16, col = "green", ann = FALSE, ylim = limss)
lines(n, eps2, type = "l", lty = 30, lwd = 3, pch = 16, col = "red", ann = FALSE)
title(main = "Functions ur(x,t), uz(x,t) (sum)", xlab = "t", ylab = "u")

legend("topleft", inset = 0.05, title = "Function", c("ur", "uz"), col = c("green", "red"), lty = c(2, 2), pch = c(16, 16))

dev.off()



for (i in 2:(1 + len)) {
    print(i - 1)

    pdf(file = paste0("center = ", s[i], "; ", s0, ".pdf"),paper = "letter", width = 30,height = 25)
    layout(mat = matrix(c(1,2,3,4),2,2,byrow = TRUE),widths = c(1,1.5),heights = c(1,1.5))
    
    df = fread(paste0(arr[i - 1], ".txt"), header = FALSE, dec = ".")[[1]]
    n = seq(from = 101, to = length(df), by = 1)
    df=df[-(1:100)]


    plot(n,df, type = "l", lty = 20, lwd = 0.5, pch = 16, col = "blue", ann = FALSE, ylim = c(min(df), max(df)))
    title(main = arr[i - 1], xlab = "n", ylab = "array")

    d = read.table(paste0(w, "/f(w) from ", s[i], ".txt"), dec = ",", header = TRUE)
    n = d[[1]]

    eps1 = d[[2]]
    eps2 = d[[3]]
    eps3 = sqrt(eps1 ^ 2 + eps2 ^ 2)
    tb = rbind(eps1, eps2, eps3)
    limss = c(min(tb), max(tb))

    plot(n, eps1, type = "l", lty = 6, lwd = 1, pch = 16, col = "green", ann = FALSE, ylim = limss)
    lines(n, eps2, type = "l", lty = 6, lwd = 1, pch = 16, col = "blue", ann = FALSE)
    lines(n, eps3, type = "l", lty = 1, lwd = 1, pch = 16, col = "red", ann = FALSE)
    title(main = "Input data f(w)", xlab = "w", ylab = "f(w)")

    legend("topright", inset = 0.05, title = "Function", c("Re", "Im", "Abs"), col = c("green", "blue", "red"), lty = c(6, 6, 1), pch = c(16, 16, 16))


    d = read.table(paste0(w, "/", s[i + len]), dec = ",", header = TRUE)
    n = d[[1]]
    eps1 = d[[2]]
    eps2 = d[[3]]
    eps3 = d[[4]]
    eps4 = d[[5]]
    eps5 = d[[6]]
    eps6 = d[[7]]

    eps01 = sqrt(eps1^2+eps2^2)
    eps03 = sqrt(eps3 ^ 2 + eps4 ^ 2)
    eps05 = sqrt(eps5 ^ 2 + eps6 ^ 2)

    dt = rbind(eps01, eps03, eps05)
    limss = c(min(dt), max(dt))


    plot(n, eps01, type = "l", lty = 1, lwd = 2, pch = 1, col = "green", ann = FALSE, ylim = limss)
    lines(n, eps03, type = "l", lty = 1, lwd = 2, pch = 1, col = "red", ann = FALSE)
    lines(n, eps05, type = "l", lty = 1, lwd = 2, pch = 1, col = "blue", ann = FALSE)


    title(main = "Functions |u(x,w)|", xlab = "w", ylab = "u")

    legend("topright", inset = 0.05, title = "Function", c("|ux|", "|uy|", "|uz|"), col = c("green", "red", "blue"), lty = c(1, 1,1), pch = c(1, 1,1))


    dt = rbind(eps1, eps2, eps3, eps4, eps5, eps6)
    limss = c(min(dt), max(dt))

    plot(n, eps1, type = "l", lty = 1, lwd = 1, pch = 1, col = "green", ann = FALSE, ylim = limss)
    lines(n, eps2, type = "l", lty = 2, lwd = 0.7, pch = 2, col = "red", ann = FALSE)
    lines(n, eps3, type = "l", lty = 1, lwd = 1, pch = 1, col = "blue", ann = FALSE)
    lines(n, eps4, type = "l", lty = 2, lwd = 0.7, pch = 2, col = "black", ann = FALSE)
    lines(n, eps5, type = "l", lty = 1, lwd = 1, pch = 1, col = "violet", ann = FALSE)
    lines(n, eps6, type = "l", lty = 2, lwd = 0.7, pch = 2, col = "yellow3", ann = FALSE)


    title(main = "Functions u(x,w)", xlab = "w", ylab = "u")

    legend("bottomright", inset = 0.05, title = "Function", c("Re ux", "Im ux", "Re uy", "Im uy", "Re uz", "Im uz"), col = c("green", "red", "blue", "black", "violet", "yellow3"), lty = c(1, 2, 1, 2, 1, 2), pch = c(1, 2, 1, 2, 1, 2))

    dev.off()
}







