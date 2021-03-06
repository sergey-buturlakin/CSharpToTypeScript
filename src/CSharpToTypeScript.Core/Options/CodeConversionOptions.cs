namespace CSharpToTypeScript.Core.Options
{
    public class CodeConversionOptions : ModuleNameConversionOptions
    {
        public CodeConversionOptions(bool export, bool useTabs, int? tabSize = null,
            DateOutputType convertDatesTo = DateOutputType.String, NullableOutputType convertNullablesTo = NullableOutputType.Null,
            bool toCamelCase = true, bool removeInterfacePrefix = true, ImportGenerationMode importGenerationMode = ImportGenerationMode.None,
            bool useKebabCase = false, bool appendModelSuffix = false, QuotationMark quotationMark = QuotationMark.Double,
            bool optionalReferenceTypes = false, bool noSemicolon = false, bool enumAsString = false)
        : base(useKebabCase, appendModelSuffix, removeInterfacePrefix)
        {
            Export = export;
            UseTabs = useTabs;
            TabSize = tabSize;
            ConvertDatesTo = convertDatesTo;
            ConvertNullablesTo = convertNullablesTo;
            ToCamelCase = toCamelCase;
            ImportGenerationMode = importGenerationMode;
            QuotationMark = quotationMark;
            OptionalReferenceTypes = optionalReferenceTypes;
            NoSemicolon = noSemicolon;
            EnumAsString = enumAsString;
        }

        public bool Export { get; set; }
        public bool UseTabs { get; set; }
        public int? TabSize { get; set; }
        public DateOutputType ConvertDatesTo { get; set; }
        public NullableOutputType ConvertNullablesTo { get; set; }
        public bool ToCamelCase { get; set; }
        public ImportGenerationMode ImportGenerationMode { get; set; }
        public QuotationMark QuotationMark { get; set; }
        public bool OptionalReferenceTypes { get; set; }
        public bool NoSemicolon { get; set; }
        public bool EnumAsString { get; set; }
    }
}