imports "assembly" from "mzkit";
imports "PlantMAT" from "PlantMAT.Core";
imports "stringr" from "R.base";

print("Run PlantMAT search, please wait for a while...");
  
# json_arguments = jsonlite::toJSON(list(
#	 lib      = PlantMAT.lib,
#	 ions     = mgfTxt,
#	 ion_mode = libtype,
#	 out      = outputdir
# ));

# run PlantMAT in docker
# run = function(container, commandline, workdir = "/", name = NULL, volume = NULL)
# docker run mzkit:v1.11_install_plantmat plantMAT_mgfMs2
# docker::run(
#    container   = PlantMAT, 
#    commandline = sprintf('plantMAT_mgfMs2 --config %s', base64enc::base64encode(charToRaw(json_arguments))), 
#    workdir     = outputdir, 
#    volume      = list(env = list(host = outputdir, virtual = outputdir))
# );

# fix docker run command line bugs
let cli_config as string = ?"--config";

if (nchar(cli_config) > 0) {
	cli_config = fromJSON(base64_decode(cli_config, asText_encoding = "utf8"));
	
	# run utf8 decode
	for(key in names(cli_config)) {
		cli_config[[key]] = decode.R_rawstring(cli_config[[key]], encoding = "utf8");
	}
	
	cli_config :> str;
} else {
	cli_config = list();
}

setwd(!script$dir);

let library_csv as string = (?"--lib"      || cli_config$lib)  || stop("no library data file was provided!");
let raw_mgf as string     = (?"--ions"     || cli_config$ions) || stop("you should provides a valid mgf file data!");
let outputdir as string   = (?"--out"      || cli_config$out)  || `${dirname(raw_mgf)}/${basename(raw_mgf)}`;
let ionMode as integer    = (?"--ion_mode" || cli_config$ion_mode);

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