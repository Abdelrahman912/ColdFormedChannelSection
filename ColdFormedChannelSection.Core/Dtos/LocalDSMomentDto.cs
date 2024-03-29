﻿using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;

namespace ColdFormedChannelSection.Core.Dtos
{
    public abstract class LocalDSMomentDto
    {

        #region Properties

        public double Mcrl { get; }

        #endregion

        #region Constructors

        protected LocalDSMomentDto( double mcrl)
        {
            Mcrl = mcrl;
        }

        #endregion

        #region Methods

        public abstract ListReportSection AsReportSection(TypeOfSection sectionType);

        #endregion
    }
}
