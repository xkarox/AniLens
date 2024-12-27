using System.ComponentModel;

namespace AniLens.Shared;


public enum Error
{
    [Description("Default error")]
    Default = 0,
    [Description("Not found")]
    NotFound = 1,
    [Description("Internal error")]
    Internal = 2,
    [Description("Parameter error")]
    Parameter = 3,
    [Description("UNathorized action")]
    Unauthorized = 4,
    [Description("Invalid credentials provided")]
    InvalidCredentials = 5
}

public static class ErrorExtensions
{
    public static string ToDescriptionString(this Error val)
    {
        var attributes = (DescriptionAttribute[])val
            .GetType()
            .GetField(val.ToString())!
            .GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : val.ToString();
    }
}