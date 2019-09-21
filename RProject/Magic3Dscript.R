
library(rgl)
library(plot3D)
library(data.table)

# Read data

filenames = readLines("3D Grafics Data Adress.txt")

arg = fread(filenames[1], header = T, dec = ",")
z = fread(filenames[2], header = T, dec = ",")

x = arg[[1]]
y = arg[[2]]

scalecoef = (max(y) - min(y)) / (max(x) - min(x))
#x = x * scalecoef

vals = matrix(z[[1]], nrow = length(x), ncol = length(y), byrow = TRUE)

levels = 15

dt = readLines(filenames[3])

# Get type
type=readLines("GraficType.txt")[1]

pd = FALSE
pn = FALSE
ht = FALSE
if (type == "all") {
    pd = TRUE
    pn = TRUE
    ht = TRUE
} else if(type=="pdf"){
    pd=TRUE
} else if (type == "png") {
    pn=TRUE
} else {
    ht=TRUE
}

# Create grafics
if (pd) {
    pdf(file = paste0(dt[[1]], ".pdf"), width = 12, height = 12, paper = "letter")
    par(mfrow = c(2, 1), cex = 1.1, cex.sub = 1.2, col.sub = "blue")
    layout(matrix(c(1, 2), 2, 1, byrow = FALSE), heights = c(2.2, 1))

persp3D(z = vals, x = x, y = y, scale = FALSE, zlab = dt[[5]], xlab = dt[[3]], ylab = dt[[4]],
contour = list(nlevels = levels, col = "red"),
        expand = 0.2,
       image = list(col = grey(seq(0, 1, length.out = 100))), main = dt[[2]])

    image(x, y, vals, col = topo.colors(20), main = "|uz|")
dev.off()
}

if (pn) {
    library(ggplot2)
    library(viridis)
    library(gridExtra)
    library(fields)

    len = length(x)
    urt = data.frame(vals = z, x = rep(x, len), y = rep(y, each = len))

    png(filename = paste0(dt[[1]], ".png"))
    options(scipen = 4)
  p= ggplot(urt, aes(x, y, fill = vals)) +
    #scale_x_continuous(breaks = seq(min(x), max(x), length.out = 7)) +
    #scale_y_continuous(breaks = seq(max(y), min(y), length.out = 7))+
    geom_raster(interpolate = TRUE) +
    coord_fixed(expand = FALSE) +
    scale_fill_viridis() + theme(axis.title.x = element_text(size = 25), axis.title.y = element_text(size = 25), text = element_text(size = 22)) +
    labs(fill=dt[[5]])+
    ggtitle(dt[[2]]) +
    xlab(dt[[3]]) +
    ylab(dt[[4]]) 
    p

    dev.off()
}

if (ht) {
    library(htmlwidgets)
    library(plotly)

    p2 = plot_ly(x = x, y = y, z = vals, type = "surface", contours = list(
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

    saveWidget(as.widget(p2), paste0(dt[[1]], ".html"), FALSE)
}

