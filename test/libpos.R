require(PlantMAT);

options(strict = FALSE);

libpos = PlantMAT::read.library("D:\biodeep\flavonoid\Rscript\bundle\Flavonoid.rda", libtype = 1);

print(as.data.frame(libpos));