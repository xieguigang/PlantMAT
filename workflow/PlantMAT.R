imports "package_utils" from "devkit";

require(PlantMAT);

#' title: PlantMAT workflow for flavone annotation
#' description: PlantMAT workflow for flavone annotation

print("Run PlantMAT search, please wait for a while...");

# load from rda workspace file
[@info "Load from rda workspace file, the RData workspace file 
        should contains two symbol for run PlantMAT data 
        analysis: 
		  
		  1. 'args' element for the analysis arguments,
          2. 'library' element for set the reference library data, and
          3. 'peaks' is the ms2 product MS matrix that used for run
             PlantMAT annotation."]
[@type "RData(args, library)"]
const stream as string = ?"--stream" || stop("A configuration data file location string which is encoded in base64 must be provided!");
# load dataset from the RData file
# data contains 3 symbols which are named:
# args
# library
# peaks
#
[args, library, peaks] = (if (file.exists(stream)) {
    stream;
} else {
    base64_decode(stream);
})
|> package_utils::parseRData.raw() 
|> package_utils::unpackRData()
;

args    = deserialize(args);
library = library
|> deserialize()
|> parseLibrary()
;

print("View of the PlantMAT analysis arguments:");
str(args);

print(`we have ${nrow(library)} reference data to run PlantMAT search:`);
print(library, max.print = 10);

outputdir       = args$outputdir;
ionMode         = as.integer(args$ionMode);
saveJSONdetails = as.logical(args$dump_json);
# use default configuration
settings        = config(AglyconeMWRange = [250, 1300], SearchPPM = 5,
	NumofSugarAll  = [0,6],
    NumofAcidAll   = [0,1],
    NumofSugarHex  = [0,6],
    NumofSugarHexA = [0,6],
    NumofSugardHex = [0,6],
    NumofSugarPen  = [0,6],
    NumofAcidMal   = [0,1],
    NumofAcidCou   = [0,1],
    NumofAcidFer   = [0,1],
    NumofAcidSin   = [0,1],
    NumofAcidDDMP  = [0,1]
);

if (ionMode == 0) {
	ionMode = 1;
	warning("PlantMAT script will works in positive mode due to the reason of ion mode is not specificed...");
}

print("view of the configuration values that we used for the analysis:");
print(settings);

data_query = peaks
|> read.mgf
|> ions.unique(eq = 0.85, gt = 0.75, trim = 0.05) 
|> as.query(mol_range = [0, 3000])
;

library
|> parseLibrary
|> MS1TopDown(settings)
|> as.object
|> do.call("MS1CP", query = data_query, ionMode = ionMode)
;