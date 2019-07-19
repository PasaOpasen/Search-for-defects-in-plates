x <- 1:100
y <- 1:100
z <- x %o% y
z <- z*sin(z) + - .1 * z



urt <- data.frame(ur.abs = c(abs(z$ur)), x = rep(x, len), y = rep(y, each = len))
p1 = ggplot(urt, aes(x, y, fill = ur.abs)) +
    geom_raster(interpolate = TRUE) +
    coord_fixed(expand = FALSE) +
    scale_fill_viridis()

urz <- data.frame(uz.abs = c(abs(z$uz)), x = rep(x, len), y = rep(y, each = len))
p2 = ggplot(urz, aes(x, y, fill = uz.abs)) +
    geom_raster(interpolate = TRUE) +
    coord_fixed(expand = FALSE) +
    scale_fill_viridis()

grid.arrange(p1, p2, newpage = FALSE)


image.plot(x, y, z = abs(ur),
           ylab = "y", xlab = "x", maintainer = "|ur|", zlim = c(0, max(abs(rlim))))