using System.Runtime.Serialization;

namespace Rhymond.OpenAI.Models;

public enum Approach 
{
    [EnumMember(Value = "rtr")]
    RetrieveThenRead,
    [EnumMember(Value = "rrr")]    
    ReadRetrieveRead,
    [EnumMember(Value = "rda")]    
    ReadDecomposeAsk
};
