namespace ColdFormedChannelSection.Core.Errors
{
    public class Errors
    {
        public static GenericError FileNotFound(string filePath)=>
            new GenericError($"File: \"{filePath}\" is not Found.");

        public static GenericError FileUsedByAnotherProcess(string filePath) =>
           new GenericError($"File: \"{filePath}\" is Opened by another process. Please Close the file first.");
    }
}
