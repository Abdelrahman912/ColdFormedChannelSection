namespace ColdFormedChannelSection.Core.Errors
{
    public class Errors
    {

        public static GenericError CantFindSafeSection =>
            new GenericError($"Cannot find section to carry these loads.");

        public static GenericError LessThanZeroError(string valueName)=>
            new GenericError($"{valueName} must be greater than zero.");
        public static GenericError FileNotFound(string filePath)=>
            new GenericError($"File: \"{filePath}\" is not Found.");

        public static GenericError FileUsedByAnotherProcess(string filePath) =>
           new GenericError($"File: \"{filePath}\" is Opened by another process. Please Close the file first.");
    }
}
