require(PlantMAT);

options(strict = FALSE);

path = "F:\rdata-develop\rdata\tests\data\test_vector.rda";
# path = "D:\biodeep\flavonoid\Rscript\bundle\Flavonoid.rda";
libpos = PlantMAT::read.library(path, libtype = 1);

print(as.data.frame(libpos));