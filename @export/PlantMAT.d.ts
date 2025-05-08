// export R# package module type define for javascript/typescript language
//
//    imports "PlantMAT" from "PlantMAT";
//
// ref=PlantMAT.PlantMAT@PlantMAT, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace PlantMAT {
   module as {
      /**
        * @param mol_range default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function query(mgf: any, mol_range?: any, env?: object): object;
      /**
        * @param env default value Is ``null``.
      */
      function stream(query: any, env?: object): any;
   }
   /**
     * @param AglyconeType default value Is ``null``.
     * @param AglyconeSource default value Is ``null``.
     * @param AglyconeMWRange default value Is ``[400,600]``.
     * @param NumofSugarAll default value Is ``[0,6]``.
     * @param NumofAcidAll default value Is ``[0,1]``.
     * @param NumofSugarHex default value Is ``[0,6]``.
     * @param NumofSugarHexA default value Is ``[0,6]``.
     * @param NumofSugardHex default value Is ``[0,6]``.
     * @param NumofSugarPen default value Is ``[0,6]``.
     * @param NumofAcidMal default value Is ``[0,1]``.
     * @param NumofAcidCou default value Is ``[0,1]``.
     * @param NumofAcidFer default value Is ``[0,1]``.
     * @param NumofAcidSin default value Is ``[0,1]``.
     * @param NumofAcidDDMP default value Is ``[0,1]``.
     * @param PrecursorIonType default value Is ``["[M]+","[M]-","[M+H]+","[M-H]-"]``.
     * @param SearchPPM default value Is ``10``.
     * @param NoiseFilter default value Is ``0.05``.
     * @param mzPPM default value Is ``30``.
     * @param aglyconeSet default value Is ``null``.
   */
   function config(AglyconeType?: object, AglyconeSource?: object, AglyconeMWRange?: any, NumofSugarAll?: any, NumofAcidAll?: any, NumofSugarHex?: any, NumofSugarHexA?: any, NumofSugardHex?: any, NumofSugarPen?: any, NumofAcidMal?: any, NumofAcidCou?: any, NumofAcidFer?: any, NumofAcidSin?: any, NumofAcidDDMP?: any, PrecursorIonType?: any, SearchPPM?: number, NoiseFilter?: number, mzPPM?: number, aglyconeSet?: object): object;
   /**
   */
   function delete(cache: object): ;
   /**
     * @param env default value Is ``null``.
   */
   function fromKNApSAcK(KNApSAcK: any, env?: object): object;
   module join {
      /**
        * @param env default value Is ``null``.
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
        * @param precursor_type default value Is ``'[M+H]+'``.
      */
      function err(mz: number, AglyW: number, Attn_w: number, nH2O_w: number, precursor_type?: string): number;
   }
   /**
     * @param ionMode default value Is ``1``.
     * @param sequenceMode default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function MS1CP(query: object, library: object, settings: object, ionMode?: object, sequenceMode?: boolean, env?: object): object;
   /**
   */
   function MS1TopDown(library: object, settings: object): object;
   /**
     * @param ionMode default value Is ``1``.
   */
   function MS2ATopDown(settings: object, ionMode?: object): object;
   /**
     * @param precurosr default value Is ``'[M+H]+'``.
     * @param commonName default value Is ``'natural product'``.
   */
   function neutral_loss(exactMass: number, Hex: object, HexA: object, dHex: object, Pen: object, Mal: object, Cou: object, Fer: object, Sin: object, DDMP: object, precurosr?: string, commonName?: string): object;
   module parse {
      /**
      */
      function config(json: string): object;
   }
   /**
   */
   function parseLibrary(data: object): object;
   /**
   */
   function precursor_types(settings: object, precursor_types: string): object;
   module query {
      /**
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
      */
      function query_result(file: string): object;
   }
   module report {
      /**
        * @param env default value Is ``null``.
      */
      function table(result: any, env?: object): object;
   }
   /**
     * @param env default value Is ``null``.
   */
   function requestKNApSAcK(keywords: any, env?: object): object;
   module result {
      /**
        * @param env default value Is ``null``.
      */
      function json(result: any, env?: object): string;
   }
}
