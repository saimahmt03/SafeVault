
namespace SafeVaultAPI.Shared
{
    internal class BaseResult
    {
        internal static class ResultCode
        {
            public const string Success = "Code201";
            public const string Updated = "Code204";
            public const string Retrieved = "Code200";
            public const string Invalid = "Code400";
            public const string Error = "Code500";
            public const string Unauthorized = "Code401";
        }

        internal static class ResultMessage
        {
            public const string Success = "Successfully added.";
            public const string Updated = "Successfully updated.";
            public const string Retrieved = "Successfully loaded.";
            public const string Invalid = "Invalid data format.";
            public const string Error = "Unexpected error occurred.";
            public const string Unauthorized = "Unauthorized access.";  
        }
    }
}