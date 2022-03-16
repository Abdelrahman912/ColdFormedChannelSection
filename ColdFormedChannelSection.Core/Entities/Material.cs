using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColdFormedChannelSection.Core.Entities
{
    public class Material
    {

        #region Properties

        /// <summary>
        /// Yield stress.
        /// </summary>
        public double Fy { get;}

        /// <summary>
        /// Modulus of elsticity.
        /// </summary>
        public double E { get;}

        /// <summary>
        /// Poisson's ratio.
        /// </summary>
        public double V { get; }

        #endregion

        #region Constructors

        public Material(double fy, double e, double v)
        {
            Fy = fy;
            E = e;
            V = v;
        }

        #endregion

    }
}
