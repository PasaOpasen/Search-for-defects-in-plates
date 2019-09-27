library(rgl)
library(plot3D)
library(data.table)

# Read data

filenames = readLines("3D Grafics Data Adress.txt")

arg = fread(filenames[1], header = T, dec = ",")
z = fread(filenames[2], header = T, dec = ",")[[1]]

x = arg[[1]]
y = arg[[2]]

len = length(x)


xlen = max(x) - min(x)
scalecoef = (max(y) - min(y)) / xlen

#x = x * scalecoef

vals = matrix(z, nrow = len, byrow = T)

vals2 = matrix(z, nrow = len, byrow = F)

levels = 15

dt = readLines(filenames[3])
short.name = dt[[1]]
save.name = dt[[2]]
titles = dt[[3]]
xlabs = dt[[4]]
ylabs = dt[[5]]
zlabs = dt[[6]]

#координаты максимума
mx = which.max(z)
xi = mx %/% len + 1
yi = mx - (xi - 1) * len

sink(paste0(save.name, "(MaxCoordinate).txt"))
cat("a b \n")
cat(c(x[xi], y[yi]))
cat("\n")
cat(paste("maximum is", max(z)))
sink()


# Get type
type = readLines("GraficType.txt")[1]

pd = FALSE
pn = FALSE
ht = FALSE
if (type == "all") {
    pd = TRUE
    pn = TRUE
    ht = TRUE
} else if (type == "pdf") {
    pd = TRUE
} else if (type == "png") {
    pn = TRUE
} else {
    ht = TRUE
}

sc = as.logical(readLines("MakeScaleForPdf.txt")[1])

# Create grafics
if (pd) {
    pdf(file = paste0(save.name, ".pdf"), width = 12, height = 12, paper = "letter")
    par(mfrow = c(2, 1), cex = 1.1, cex.sub = 1.2, col.sub = "blue")
    layout(matrix(c(1, 2), 2, 1, byrow = FALSE), heights = c(2.2, 1))

    if (sc) {
        xs = x / xlen
        ys = y / (max(y) - min(x))
    } else {
        tmp = (max(vals) - min(vals)) / (max(x) - min(x))
        xs = x * tmp
        ys = y * tmp
    }

    persp3D(z = vals, x = xs, y = ys, scale = FALSE, zlab = zlabs, xlab = xlabs, ylab = ylabs,
    contour = list(nlevels = levels, col = "red"),
        expand = 0.2,
       image = list(col = grey(seq(0, 1, length.out = 100))), main = titles)

    image(x, y, vals, col = topo.colors(20), main = "|uz|")
    dev.off()
}

if (pn) {
    library(ggplot2)
    library(viridis)
    library(gridExtra)
    library(fields)

    urt = data.frame(vals = z, x = rep(x, len), y = rep(y, each = len))

    png(filename = paste0(save.name, ".png"), 900, 800)
    options(scipen = 4)
    ggplot(urt, aes(x, y, fill = vals)) +
    #scale_x_continuous(breaks = seq(min(x), max(x), length.out = 7)) +
    #scale_y_continuous(breaks = seq(max(y), min(y), length.out = 7))+
    geom_raster(interpolate = TRUE) +
    coord_fixed(expand = FALSE) +
    scale_fill_viridis() + theme(axis.title.x = element_text(size = 25), axis.title.y = element_text(size = 25), text = element_text(size = 22)) +
    labs(fill = zlabs) +
    ggtitle(titles) +
    xlab(xlabs) +
    ylab(ylabs)

    dev.off()
}

if (ht) {
    library(htmlwidgets)
    library(plotly)

    p2 = plot_ly(x = x, y = y, z = vals2, type = "surface", contours = list(
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

    saveWidget(as.widget(p2), paste0(save.name, ".html"), FALSE)
}
