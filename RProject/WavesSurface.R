library(rgl)
library(plot3D)
library(data.table)

xx = fread("3D ur, uz(x).txt", header = TRUE, dec = ",")
yy = fread("3D ur, uz(y).txt", header = TRUE, dec = ",")
z = fread("3D ur, uz.txt", header = TRUE, dec = ",")
x = xx$x
y = yy$y
z=abs(z)

ur =z$ur
uz=z$uz

st=readLines("3D ur, uz(info).txt")
for (files in 1:(length(st)-1)) {
    s = sub("3D ", "",st[[files]], fixed = TRUE)
    s1 = sub(" .png", " (ur).txt", s, fixed = TRUE)
    s2 = sub(" .png", " (uz).txt", s, fixed = TRUE)

    r =abs(scan(s1))
    z =abs(scan(s2))

    print(paste(files+1, "from", length(st)))

    ur=apply(cbind(ur,r), MARGIN = 1,FUN = max)
    uz = apply(cbind(uz, z), MARGIN = 1, FUN = max)

    #for (i in 1:length(ur)) {
      #  ur[i] = max(c(ur[i], r[i]))
     #   uz[i] = max(c((uz[i], z[i]))
    #}
}

zm = max(ur)
x1 = x / (max(x) - min(x)) * zm
y1 = y / (max(y) - min(y)) * zm
zm = max(uz)
x2 = x / (max(x) - min(x)) * zm
y2 = y / (max(y) - min(y)) * zm

urr = matrix(ur, nrow = length(x), ncol = length(y), byrow = FALSE)
uzz = matrix(uz, nrow = length(x), ncol = length(y), byrow = FALSE)

levels = 30

s = readLines("SurfaceMain.txt")[[1]]
print(s)
ss = sub(", (xmin", " \n(xmin", s,fixed = TRUE)
ss = strsplit(ss, "\n")
s1 = ss[[1]][1]
s2 = ss[[1]][2]

pdf(file = paste(s, ".pdf"), width = 15,height = 12)
par(mfrow = c(1, 2), cex = 1.1, cex.sub = 1.2, col.sub = "blue")
layout(matrix(c(1, 2, 3, 4), 2, 2, byrow = FALSE), widths =  c(2.5, 1))

pp = persp3D(z = urr, x = x, y = y, scale = TRUE, zlab = "ur(x,t)",
       contour = list(nlevels = levels, col = "red"),
#zlim = c(urmin, max(urRe, na.rm = TRUE) * 0.3),
        expand = 0.2,
       image = list(col = grey(seq(0, 1, length.out = 100))), main = "ur-Surface", sub = s1)

persp3D(z = uzz, x = x, y = y, scale = TRUE, zlab = "uz(x,t)",
       contour = list(nlevels = levels, col = "red"),
#zlim = c(-40, max(uzRe, na.rm = TRUE) * 0.7),
        expand = 0.2,
       image = list(col = grey(seq(0, 1, length.out = 100))), main = "uz-Surface", sub=s2)

image(x, y, abs(urr), col = heat.colors(20), main = "|ur|")
image(x, y, abs(uzz), col = topo.colors(20), main = "|uz|")

dev.off()


library(ggplot2)
library(viridis)
library(gridExtra)
library(fields)

len = length(x)
png(filename = paste(s, "(heatmap).png"), height = 600, width = 750)
par( cex = 1.0, cex.sub = 1.3, col.sub = "blue")
urt <- data.frame(ur.abs = c(abs(urr)), x = rep(x, len), y = rep(y, each = len))

 ggplot(urt, aes(x, y, fill = ur.abs)) +
    geom_raster(interpolate = TRUE) +
    coord_fixed(expand = FALSE) +
    scale_fill_viridis() + theme(axis.title.x = element_text(size = 25), axis.title.y = element_text(size = 25), text = element_text(size = 22))
dev.off()

sink(paste(s, "(ur).txt"))
cat(ur)
sink()
sink(paste(s, "(uz).txt"))
cat(uz)
sink()