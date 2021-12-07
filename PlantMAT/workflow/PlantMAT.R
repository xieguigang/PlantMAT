require(PlantMAT);

print("Run PlantMAT search, please wait for a while...");

# load from rda workspace file

[@info "Load from rda workspace file, the RData workspace file 
        should contains two symbol for run PlantMAT data 
        analysis: 1. 'args' element for the analysis arguments,
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
[args, library, peaks] = base::readRData(base64_decode(stream));

print("View of the PlantMAT analysis arguments:");
str(args);

print(`we have ${length(library)} reference data to run PlantMAT search:`);
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