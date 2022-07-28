using CSharp.Functional.Errors;

namespace ColdFormedChannelSection.Core.Errors
{
    public class Errors
    {

        public static GenericError EuroInternalRadiusError =>
         new GenericError($"Unsafe, Resistance of the cross section should be determined by tests (R > 0.04 t E/Fy).");

        public static GenericError CantCalculateNominalStrength =>
          new GenericError($"Cannot calculate section nominal strength due to section dimensions ratio violation.");

        public static GenericError CantFindSafeSection =>
            new GenericError($"Cannot find section to carry these loads.");

        public static GenericError GreaterThanZeroError(string valueName)=>
            new GenericError($"{valueName} must be greater than zero.");


        public static GenericError GreaterThanOrEqualZeroError(string valueName) =>
            new GenericError($"{valueName} must be greater than or equal zero.");

        public static GenericError FileNotFound(string filePath)=>
            new GenericError($"File: \"{filePath}\" is not Found.");

        public static GenericError FileUsedByAnotherProcess(string filePath) =>
           new GenericError($"File: \"{filePath}\" is Opened by another process. Please Close the file first.");
    }
}
