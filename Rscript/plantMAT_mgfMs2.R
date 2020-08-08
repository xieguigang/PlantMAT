let config as string = ?"--config";

if (nchar(config) > 0) {
	config = base64_decode(config, asText_encoding = "utf8")
}

print(config);

stop(1);

imports "assembly" from "mzkit";
imports "PlantMAT" from "PlantMAT.Core";

setwd(!script$dir);



let library_csv as string = ?"--lib"  || stop("no library data file was provided!");
let raw_mgf as string     = ?"--ions" || stop("you should provides a valid mgf file data!");
let ionMode as integer    = ?"--ion_mode";
let outputdir as string   = ?"--out"  || `${dirname(raw_mgf)}/${basename(raw_mgf)}`;

if (!file.exists(library_csv)) {
	stop(`the file path of library [${library_csv}] is invalid!`);
}
if (!file.exists(raw_mgf)) {
	stop(`the given raw data file [${raw_mgf}] is not found on your file system!`);
}

if (ionMode == 0) {
	ionMode = 1;
	warning("PlantMAT script will works in positive mode due to the reason of ion mode is not specificed...");
}

# use default configuration
const settings = config(AglyconeMWRange = [400, 1600]);

print("view of the configuration values that we used for the analysis:");
print(settings);

let result = library_csv
:> read.library
:> MS1TopDown(settings)
:> as.object
:> do.call("MS1CP", query = raw_mgf :> read.mgf :> as.query, ionMode = ionMode)
:> as.object(MS2ATopDown(settings))$MS2Annotation
;

# output the annotation result data set
result
:> result.json
:> writeLines(`${outputdir}/PlantMAT.json`)
;

result
:> report.table
:> write.csv(file = `${outputdir}/PlantMAT.csv`, row_names = FALSE)
;