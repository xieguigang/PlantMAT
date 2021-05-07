
let KNApSAcKLibrary as function(query, cache = "./") {
	options(KNApSAcK.cache = cache);
	
	query 
	:> PlantMAT::requestKNApSAcK 
	:> PlantMAT::fromKNApSAcK
	;
}