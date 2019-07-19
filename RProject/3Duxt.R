library(rgl)
library(plot3D)
library(data.table)

xx = fread("3D ur, uz(x).txt", header = TRUE, dec = ",")
yy = fread("3D ur, uz(y).txt", header = TRUE, dec = ",")
z = fread("3D ur, uz.txt", header = TRUE, dec = ",")
x = xx$x #;xx
y = yy$y

len=length(x)

zl = fread("zlims.txt", header = TRUE, dec = ",")
coef = 1.0
rlim = c(zl[[1]][1], zl[[1]][2]) * 4
zlim = c(zl[[2]][1], zl[[2]][2]) * 2

#cc = 5
#z$ur[z$ur < rlim[1] * cc | z$ur > rlim[2] * cc] = NA
#z$uz[z$uz < zlim[1] * cc | z$uz > zlim[2] * cc] = NA

#zm = max(abs(z), na.rm = TRUE)
zm = max(abs(rlim))
x1 = x / (max(x) - min(x)) * zm*2
y1 = y / (max(y) - min(y)) * zm*2
rrlim =rlim*1.2 #c(-zm, zm) * 0.8

zm = max(abs(zlim))
x2 = x / (max(x) - min(x)) * zm*2
y2 = y / (max(y) - min(y)) * zm*2
zzlim = zlim*1.2#c(-zm, zm) * 0.8

ur = matrix(z$ur, nrow = length(x), ncol = length(y), byrow = TRUE)
uz = matrix(z$uz, nrow = length(x), ncol = length(y), byrow = TRUE)

levels = 30

st = readLines("3D ur, uz(title).txt")
st
s=st[[1]]
ss =sub(", t =",", \n t =", st[[1]])
ss = strsplit(ss,"\n")
s1 = ss[[1]][1]
s2 = ss[[1]][2]

pdf(file = paste("3D ur, uz(title ,", s, ").pdf"), width = 24)
par(mfrow = c(1, 2), cex = 1.0, cex.sub = 0.9, col.sub = "blue")

pp = persp3D(z = ur, x = x1, y = y1, scale = FALSE, zlab = "ur(x,t)",
       contour = list(nlevels = levels, col = "red"),
#zlim = c(urmin, max(urRe, na.rm = TRUE) * 0.3),
        expand = 0.2,
       image = list(col = grey(seq(0, 1, length.out = 100))), main = "ur(x,t)", sub = s1, zlim = rlim)

pp2=persp3D(z = uz, x = x2, y = y2, scale = FALSE, zlab = "uz(x,t)",
       contour = list(nlevels = levels, col = "red"),
#zlim = c(-40, max(uzRe, na.rm = TRUE) * 0.7),
        expand = 0.2,
       image = list(col = grey(seq(0, 1, length.out = 100))), main = "uz(x,t)", sub = s2, zlim = zlim)

dev.off()

sink(paste(s, "(ur).txt"))
cat(ur)
sink()
sink(paste(s, "(uz).txt"))
cat(uz)
sink()


if (FALSE) {
    library(plotly)
    p1 = plot_ly(x = x, y = y, z = ~ur, type = "surface", contours = list(
    z = list(
      show = TRUE,
      usecolormap = TRUE,
      highlightcolor = "#ff0000",
      project = list(z = TRUE)
      )
    )
  ) %>%
    layout(
    scene = list(
      camera = list(
        eye = list(x = 1.87, y = 0.88, z = -0.64)
        )
      ))
    p2 = plot_ly(x = x, y = y, z = ~uz, type = "surface", contours = list(
    z = list(
      show = TRUE,
      usecolormap = TRUE,
      highlightcolor = "#ff0000",
      project = list(z = TRUE)
      )
    )
  ) %>%
    layout(
    scene = list(
      camera = list(
        eye = list(x = 1.87, y = 0.88, z = -0.64)
        )
      ))

    library(htmlwidgets)

    saveWidget(as.widget(p1), "ur.html", FALSE)
    saveWidget(as.widget(p2), "uz.html", FALSE)

}