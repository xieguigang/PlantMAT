imports "assembly" from "mzkit";
imports "PlantMAT" from "PlantMAT.Core";

setwd(!script$dir);

const library.csv = "../SampleData/Library.csv";
const raw = "E:\smartnucl_integrative\raw.mgf";

# use default configuration
const settings = config();

print("view of the configuration values that we used for the analysis:");
print(settings);

let result = library.csv
:> read.library
:> MS1TopDown(settings)
:> as.object
:> do.call("MS1CP", query = raw :> read.mgf :> as.query, ionMode = 1)
:> as.object(MS2ATopDown(settings))$MS2Annotation
;

# output the annotation result data set
result
:> json(compress = FALSE)
:> writeLines(`${dirname(raw)}/${basename(raw)}.json`)
;