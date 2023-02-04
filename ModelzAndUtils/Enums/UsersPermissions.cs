using System.Runtime.Serialization;

// ReSharper disable UnusedMember.Global

namespace ModelzAndUtils.Enums;

[Serializable]
public enum UsersPermissions
{
    [EnumMember]
    None,
    [EnumMember]
    Read,
    [EnumMember]
    Write,
    [EnumMember]
    System
}