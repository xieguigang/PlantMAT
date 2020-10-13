imports "PlantMAT" from "PlantMAT.Core";

# use default configuration
const settings = config();

print("view of the configuration values that we used for the analysis:");
print(settings);

const test as string = "F:\PlantMAT.MS1TopDown.json";

let result = test 
:> read.query_result 
:> as.object(MS2ATopDown(settings))$MS2Annotation
;

# output the annotation result data set
# result
# :> json(compress = FALSE)
# :> writeLines(`${dirname(test)}/${basename(test)}.Ms2TopDown.json`)
# ;

# output the annotation report html
# result
# :> html
# :> writeLines(`${dirname(test)}/${basename(test)}.Ms2TopDown.html`)
# ;

result
:> as.stream
:> report.table
:> write.csv(file = `${dirname(test)}/${basename(test)}.Ms2TopDown.csv`, row_names = FALSE)
;

# clear the cache data file
result :> delete;