using ColdFormedChannelSection.Core.Enums;
using System.Collections.Generic;
using System.Linq;
using static ColdFormedChannelSection.Core.Comparers.Comparers;

namespace ColdFormedChannelSection.Core.Dtos
{
    public class DSMomentDto
    {
       
        #region Properties

        public LocalDSMomentDto LB { get;}

        public double Mcre { get; }

        public double Mcrd { get; }

        public double Mnl { get;}

        public double Mnd { get; }

        public double Mne { get;}

        public double Fy { get; }

        public double Zg { get; }

        public double My { get;  }
        public NominalStrengthDto GoverningCase { get;}

        #endregion

        #region Constructors

        public DSMomentDto(LocalDSMomentDto lb, double mcre, double mcrd, double mnl, double mnd, double mne,double fy,double zg,NominalStrengthDto governingCase)
        {
            LB = lb;
            Mcre = mcre;
            Mcrd = mcrd;
            Mnl = mnl;
            Mnd = mnd;
            Mne = mne;
            var nominalLoads = new List<NominalStrengthDto>()
            {
               new NominalStrengthDto(Mnl,FailureMode.LOCALBUCKLING),
                new NominalStrengthDto(Mne,FailureMode.GLOBALBUCKLING),
               new NominalStrengthDto(Mnd,FailureMode.DISTRORSIONALBUCKLING)
            };
            Fy = fy;
            Zg = zg;
            My = Fy * Zg;
            GoverningCase = governingCase; /*nominalLoads.Distinct(NominalStrengthEqualComparer).OrderBy(tup => tup.NominalStrength).First();*/
        }

        #endregion
    }
}
