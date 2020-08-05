imports "PlantMAT" from "PlantMAT.Core";

# use default configuration
const settings = config();

print("view of the configuration values that we used for the analysis:");
print(settings);

const test as string = "V:\D042-E035\pos.mzML\save\raw.json";

as.object(MS2ATopDown(settings))$MS2Annotation(read.query_result(test));

# output the annotation result data set
result
:> json(compress = FALSE)
:> writeLines(`${dirname(test)}/${basename(test)}.Ms2TopDown.json`)
;

# output the annotation report html
result
:> html
:> writeLines(`${dirname(test)}/${basename(test)}.Ms2TopDown.html`)
;

result
:> report.table
:> write.csv(file = `${dirname(test)}/${basename(test)}.Ms2TopDown.csv`, row_names = FALSE)
;