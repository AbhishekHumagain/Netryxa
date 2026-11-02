using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

internal class GuidToNullConverter(ConverterMappingHints? mappingHints = null)
    : ValueConverter<Guid?, Guid?>(x => x == Guid.Empty ? default : x, x => x, mappingHints);