imports "PlantMAT" from "PlantMAT.Core";

setwd(!script$dir);

const library.csv = "../Library.csv";
const demo as string = "../SampleData/A17_Root/metabolite_list.txt";

library.csv
:> read.library
:> MS1TopDown(settings = config())
:> as.object
:> do.call(MS1CP, query = readLines(demo) :> query.ms1)
:> json
:> writeLines("./A17_Root_MS1TopDown.json")
;
