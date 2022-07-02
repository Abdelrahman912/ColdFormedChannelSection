namespace ColdFormedChannelSection.Core.Dtos
{
    public class EgyptMomentDto
    {
       
        #region Properties

        public LocalEgyptMomentDto LB { get; }

        public EgyptMomentBaseDto GoverningCase { get;  }

        #endregion

        #region Constructors

        public EgyptMomentDto(LocalEgyptMomentDto lB, EgyptMomentBaseDto governingCase)
        {
            LB = lB;
            GoverningCase = governingCase;
        }

        #endregion
    }
}
