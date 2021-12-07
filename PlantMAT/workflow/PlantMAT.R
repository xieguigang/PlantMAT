require(PlantMAT);

print("Run PlantMAT search, please wait for a while...");

# load from rda workspace file

[@info "Load from rda workspace file, the RData workspace file 
        should contains two symbol for run PlantMAT data 
        analysis: 1. 'args' element for the analysis arguments and,
        2. 'library' element for set the reference library data."]
[@type "RData(args, library)"]
const stream as string = ?"--stream" || stop("A configuration data file location string which is encoded in base64 must be provided!");
const [args, library]  = base::readRData(base64_decode(stream));

print("View of the PlantMAT analysis arguments:");
str(args);

print(`we have ${length(library)} reference data to run PlantMAT search:`);
print(library, max.print = 10);