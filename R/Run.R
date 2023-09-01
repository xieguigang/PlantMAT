imports "assembly" from "mzkit";
imports "math" from "mzkit";
imports "PlantMAT" from "PlantMAT.Core";

#' Run ion predicts
#'
#' @param the peakMs2 ions
#' @param library the plant metabolite library, should contains the 
#'     \code{SMILES} structure data!
#'
const predictIons as function(ions, library, ionMode = 1, settings = configDefault()) {
	# run MS1TopDown
	ions 
	|> as.query 
	|> MS1CP(library, settings, ionMode)
	
	# run Ms2 analysis for molecular structure prediction
	|> as.object(MS2ATopDown(settings))$MS2Annotation
	;
}

#' apply the default configuration
#'
#' @param mzPPM ppm value for MS2 annotation
#' @param aglyconeSet a list of user defined aglycone candidate set.
#' 
const configDefault as function(mzPPM = 30, precursors = ["[M]+", "[M]-", "[M+H]+", "[M-H]-"], aglyconeSet = NULL) {
	config(AglyconeMWRange = [250, 1200], SearchPPM = 5, mzPPM = mzPPM, NoiseFilter = 0.01,
		NumofSugarAll    = [0,9999],
		NumofAcidAll     = [0,9999],
		NumofSugarHex    = [0,12],
		NumofSugarHexA   = [0,12],
		NumofSugardHex   = [0,12],
		NumofSugarPen    = [0,12],
		NumofAcidMal     = [0,12],
		NumofAcidCou     = [0,12],
		NumofAcidFer     = [0,12],
		NumofAcidSin     = [0,12],
		NumofAcidDDMP    = [0,12],
		PrecursorIonType = precursors,
		aglyconeSet      = aglyconeSet 
	);
} 