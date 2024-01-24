// export R# source type define for javascript/typescript language
//
// package_source=PlantMAT

declare namespace PlantMAT {
   module _ {
      /**
      */
      function onLoad(): object;
   }
   /**
     * @param mzPPM default value Is ``30``.
     * @param precursors default value Is ``["[M]+", "[M]-", "[M+H]+", "[M-H]-"]``.
     * @param aglyconeSet default value Is ``null``.
   */
   function configDefault(mzPPM?: any, precursors?: any, aglyconeSet?: any): object;
   /**
     * @param file default value Is ``stdout``.
   */
   function exportJSON(result: any, file?: any): object;
   /**
     * @param file default value Is ``stdout``.
   */
   function exportTable(result: any, file?: any): object;
   /**
     * @param cache default value Is ``./``.
   */
   function KNApSAcKLibrary(query: any, cache?: any): object;
   /**
     * @param ionMode default value Is ``1``.
     * @param settings default value Is ``Call "configDefault"()``.
   */
   function predictIons(ions: any, library: any, ionMode?: any, settings?: any): object;
}
