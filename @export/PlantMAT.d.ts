// export R# package module type define for javascript/typescript language
//
//    imports "PlantMAT" from "PlantMAT.Core";
//
// ref=PlantMAT.Core.PlantMAT@PlantMAT.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * PlantMAT: A Metabolomics Tool for Predicting the Specialized 
 *  Metabolic Potential of a System and for Large-Scale Metabolite 
 *  Identifications
 * 
*/
declare namespace PlantMAT {
   module as {
      /**
       * create plantMAT query from the given mgf ion stream
       * 
       * 
        * @param mgf the mgf ion collection or peak ms2 data model collection.
        * @param mol_range 
        * + default value Is ``null``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function query(mgf: any, mol_range?: any, env?: object): object;
      /**
        * @param env default value Is ``null``.
      */
      function stream(query: any, env?: object): any;
   }
   /**
    * create plantMAT configuration
    *  
    *  if all of the parameter is omit, then you can create a settings 
    *  model with all configuration set to default values.
    * 
    * 
     * @param AglyconeType -
     * 
     * + default value Is ``null``.
     * @param AglyconeSource -
     * 
     * + default value Is ``null``.
     * @param AglyconeMWRange -
     * 
     * + default value Is ``[400,600]``.
     * @param NumofSugarAll -
     * 
     * + default value Is ``[0,6]``.
     * @param NumofAcidAll -
     * 
     * + default value Is ``[0,1]``.
     * @param NumofSugarHex -
     * 
     * + default value Is ``[0,6]``.
     * @param NumofSugarHexA -
     * 
     * + default value Is ``[0,6]``.
     * @param NumofSugardHex -
     * 
     * + default value Is ``[0,6]``.
     * @param NumofSugarPen -
     * 
     * + default value Is ``[0,6]``.
     * @param NumofAcidMal -
     * 
     * + default value Is ``[0,1]``.
     * @param NumofAcidCou -
     * 
     * + default value Is ``[0,1]``.
     * @param NumofAcidFer -
     * 
     * + default value Is ``[0,1]``.
     * @param NumofAcidSin -
     * 
     * + default value Is ``[0,1]``.
     * @param NumofAcidDDMP -
     * 
     * + default value Is ``[0,1]``.
     * @param PrecursorIonType a character vector of list all precursor types that could be apply 
     *  for search the ms1 annotation in plantMAT.
     * 
     * + default value Is ``["[M]+","[M]-","[M+H]+","[M-H]-"]``.
     * @param SearchPPM -
     * 
     * + default value Is ``10``.
     * @param NoiseFilter -
     * 
     * + default value Is ``0.05``.
     * @param mzPPM -
     * 
     * + default value Is ``30``.
     * @param aglyconeSet 
     * + default value Is ``null``.
   */
   function config(AglyconeType?: object, AglyconeSource?: object, AglyconeMWRange?: any, NumofSugarAll?: any, NumofAcidAll?: any, NumofSugarHex?: any, NumofSugarHexA?: any, NumofSugardHex?: any, NumofSugarPen?: any, NumofAcidMal?: any, NumofAcidCou?: any, NumofAcidFer?: any, NumofAcidSin?: any, NumofAcidDDMP?: any, PrecursorIonType?: any, SearchPPM?: number, NoiseFilter?: number, mzPPM?: number, aglyconeSet?: object): object;
   /**
   */
   function delete(cache: object): ;
   /**
    * Create PlantMAT reference meta library from KNApSAcK database
    * 
    * 
     * @param KNApSAcK -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function fromKNApSAcK(KNApSAcK: any, env?: object): object;
   module join {
      /**
       * join ms2 spectra data with the corresponding ms1 query values
       * 
       * 
        * @param ms1 the ms1 peak features
        * @param files a file path vector of the ms2 spectra matrix list for each ms1 peaks
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function ms2(ms1: any, files: string, env?: object): object;
   }
   module KNApSAcK {
      module as {
         /**
           * @param solver default value Is ``null``.
           * @param env default value Is ``null``.
         */
         function table(KNApSAcK: any, solver?: object, env?: object): object;
      }
   }
   module ms1 {
      /**
       * debug test tools
       * 
       * 
        * @param mz ms1 ``m/z`` value
        * @param AglyW exact mass
        * @param Attn_w -
        * @param nH2O_w -
        * @param precursor_type -
        * 
        * + default value Is ``'[M+H]+'``.
      */
      function err(mz: number, AglyW: number, Attn_w: number, nH2O_w: number, precursor_type?: string): number;
   }
   /**
    * Run ms1 search
    * 
    * > ``options(verbose = TRUE)`` for display debug information.
    * 
     * @param query -
     * @param library -
     * @param settings -
     * @param ionMode -
     * 
     * + default value Is ``1``.
     * @param sequenceMode run in algorithm debug mode, do not set this option to TRUE in production mode.
     * 
     * + default value Is ``false``.
     * @param env 
     * + default value Is ``null``.
   */
   function MS1CP(query: object, library: object, settings: object, ionMode?: object, sequenceMode?: boolean, env?: object): object;
   /**
    * performs combinatorial enumeration, and show the calculation progress (MS1CP)
    * 
    * 
   */
   function MS1TopDown(library: object, settings: object): object;
   /**
    * performs MS2 annotation
    * 
    * 
     * @param settings -
     * @param ionMode 
     * + default value Is ``1``.
   */
   function MS2ATopDown(settings: object, ionMode?: object): object;
   /**
     * @param precurosr default value Is ``'[M+H]+'``.
     * @param commonName default value Is ``'natural product'``.
   */
   function neutral_loss(exactMass: number, Hex: object, HexA: object, dHex: object, Pen: object, Mal: object, Cou: object, Fer: object, Sin: object, DDMP: object, precurosr?: string, commonName?: string): object;
   module parse {
      /**
       * parse the settings value from a given json string
       * 
       * 
        * @param json settings value in json text format.
      */
      function config(json: string): object;
   }
   /**
    * read ms1 library file
    * 
    * 
   */
   function parseLibrary(data: object): object;
   /**
   */
   function precursor_types(settings: object, precursor_types: string): object;
   module query {
      /**
       * parse ms1 query data
       * 
       * 
        * @param metabolite_list the input query file content.
      */
      function ms1(metabolite_list: string): object;
   }
   module read {
      /**
      */
      function neutrals(file: string): object;
      module PlantMAT {
         /**
         */
         function report_table(file: string): object;
      }
      /**
       * read the query result json file
       * 
       * 
        * @param file the file path of the json file or the json string text
      */
      function query_result(file: string): object;
   }
   module report {
      /**
       * run report table output
       * 
       * 
        * @param result -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function table(result: any, env?: object): object;
   }
   /**
    * query KNApSAcK database
    * 
    * 
     * @param keywords -
     * @param env 
     * + default value Is ``null``.
   */
   function requestKNApSAcK(keywords: any, env?: object): object;
   module result {
      /**
        * @param env default value Is ``null``.
      */
      function json(result: any, env?: object): string;
   }
}
