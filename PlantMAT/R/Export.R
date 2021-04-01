imports "assembly" from "mzkit";
imports "math" from "mzkit";
imports "PlantMAT" from "PlantMAT.Core";

#' output the annotation result data set
#'
let exportJSON as function(result, file = "stdout") {
	result
	:> as.stream
	:> as.vector(mode = "query")
	:> result.json
	:> writeLines(file)
	;
}

#' export result as excel table
let exportTable as function(result, file = "stdout") {
	result
	:> as.stream
	:> report.table
	:> write.csv(file = file, row_names = FALSE)
	;
}