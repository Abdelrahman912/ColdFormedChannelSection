using ColdFormedChannelSection.Core.Enums;

namespace ColdFormedChannelSection.Core.Entities
{
    public class ReportItem
    {

        #region Properties

        public string Name { get; }

        public string Value { get; }

        public Units Unit { get; }

        #endregion

        #region Constructors

        public ReportItem(string name, string value, Units unit)
        {
            Name = name;
            Value = value;
            Unit = unit;
        }

        #endregion
    }
}
