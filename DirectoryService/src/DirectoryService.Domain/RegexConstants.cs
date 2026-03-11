using System;

namespace DirectoryService.Domain;

public sealed class RegexConstants
{
    public const string IanaTimeZoneRegex = @"^[A-Za-z]+\/[A-Za-z_]+$";
    public const string LATIN_REGEX = @"^[a-zA-Z]+$";
}
