imports "assembly" from "mzkit";
imports "math" from "mzkit";
imports "PlantMAT" from "PlantMAT.Core";

#' Run ion predicts
#'
#' @param the peakMs2 ions
#' @param library the plant metabolite library, should contains the 
#'     \code{SMILES} structure data!
#'
let predictIons as function(ions, library, ionMode = 1, settings = configDefault()) {
	# run MS1TopDown
	ions 
	:> as.query 
	:> MS1CP(library, settings, ionMode)
	
	# run Ms2 analysis for molecular structure prediction
	:> as.object(MS2ATopDown(settings))$MS2Annotation
	;
}

#' apply the default configuration
#'
let configDefault as function(mzPPM = 30) {
	config(AglyconeMWRange = [200, 1200], SearchPPM = 5, mzPPM = mzPPM, NoiseFilter = 0.01,
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
} 