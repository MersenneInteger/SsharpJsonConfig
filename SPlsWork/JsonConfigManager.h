namespace JsonConfigManager;
        // class declarations
         class JsonConfig;
         class Signal;
         class JsonData;
     class JsonConfig 
    {
        // class delegates
        delegate FUNCTION ReturnJsonValuesHandler ( SIMPLSHARPSTRING key , SIMPLSHARPSTRING value , SIMPLSHARPSTRING type );
        delegate FUNCTION FileBusyHandler ( INTEGER fileBusy );

        // class events

        // class functions
        INTEGER_FUNCTION Initialize ( STRING filePath );
        FUNCTION ReadConfig ();
        FUNCTION WriteConfig ();
        FUNCTION SetJsonValues ( STRING key , STRING value , STRING type );
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty ReturnJsonValuesHandler ReturnJsonValues;
        DelegateProperty FileBusyHandler FileBusy;
    };

     class Signal 
    {
        // class delegates

        // class events

        // class functions
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING Key[];
        STRING Type[];
    };

     class JsonData 
    {
        // class delegates

        // class events

        // class functions
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

        // class variables
        INTEGER __class_id__;

        // class properties
    };

