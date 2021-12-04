require(PlantMAT);

options(strict = FALSE);

# path = "D:\GCModeller\src\R-sharp\studio\RData\test\test_list.rda";
path = "E:\biodeep\biodeep_pipeline\biodeepNPSearch\data\Flavonoid.rda";
libpos = PlantMAT::read.library(path, libtype = 1);

print(as.data.frame(libpos));