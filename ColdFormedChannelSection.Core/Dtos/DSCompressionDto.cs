using ColdFormedChannelSection.Core.Enums;
using System.Collections.Generic;
using System.Linq;
using static ColdFormedChannelSection.Core.Comparers.Comparers;

namespace ColdFormedChannelSection.Core.Dtos
{
    public class DSCompressionDto
    {
        
        #region Properties

        public LocalDSCompressionDto LB { get; }

        public double Pcrd { get; }

        public double Pcre { get; }

        public double Pnl { get; }

        public double Pnd { get; }

        public double Pne { get; }

        public double Ag { get; }

        public double Fy { get; }

        public double Py { get; }

        public NominalStrengthDto GoverningCase { get; }

        #endregion

        #region Contructors

        public DSCompressionDto(LocalDSCompressionDto lb, double pcrd, double pcre, double pnl, double pnd, double pne, double ag, double fy)
        {
            LB = lb;
            Pcrd = pcrd;
            Pcre = pcre;
            Pnl = pnl;
            Pnd = pnd;
            Pne = pne;
            Ag = ag;
            Fy = fy;
            Py = Fy*Ag;

            var nominalLoads = new List<NominalStrengthDto>()
            {
               new NominalStrengthDto(pnl,FailureMode.LOCALBUCKLING),
                new NominalStrengthDto(pne,FailureMode.GLOBALBUCKLING),
               new NominalStrengthDto(pnd,FailureMode.DISTRORTIONALBUCKLING)
            };
            GoverningCase = nominalLoads.Distinct(NominalStrengthEqualComparer).OrderBy(tup => tup.NominalStrength).First();
        }

        #endregion
    }
}
