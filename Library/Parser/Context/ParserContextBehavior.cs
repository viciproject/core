using System;
using System.Linq;

namespace Vici.Core.Parser
{
    [Flags]
    public enum ParserContextBehavior
    {
        Default = 0,

        NullIsFalse = 0x0001,
        NotNullIsTrue = 0x0002,
        NotZeroIsTrue = 0x0004,
        ZeroIsFalse = 0x0004,
        EmptyStringIsFalse = 0x0010,
        NonEmptyStringIsTrue = 0x0020,
        EmptyCollectionIsFalse = 0x0040,

        Falsy = NullIsFalse|NotNullIsTrue|NotZeroIsTrue|ZeroIsFalse|EmptyStringIsFalse|NonEmptyStringIsTrue|EmptyCollectionIsFalse,

        ReturnNullWhenNullReference = 0x0100,

        Easy = Falsy|ReturnNullWhenNullReference,
        
        CaseInsensitiveVariables = 0x8000
    }
}