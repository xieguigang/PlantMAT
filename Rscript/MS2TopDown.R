imports "PlantMAT" from "PlantMAT.Core";

setwd(!script$dir);

const library.csv = "../SampleData/Library.csv";
const demo as string = "../SampleData/A17_Root/metabolite_list.txt";

# use default configuration
const settings = config();

print("view of the configuration values that we used for the analysis:");
print(settings);

let result = library.csv
:> read.library
:> MS1TopDown(settings)
:> as.object
:> do.call("MS1CP", query = readLines(demo) :> query.ms1, ionMode = -1)
:> join.ms2(files = list.files(dirname(demo), pattern = "*.txt"))
:> as.object(MS2ATopDown(settings))$MS2Annotation
;

# output the annotation result data set
result
:> json(compress = FALSE)
:> writeLines("./A17_Root_MS2TopDown.json")
;

# output the annotation report html
result
:> html
:> writeLines("./A17_Root_MS2TopDown.html")
;
