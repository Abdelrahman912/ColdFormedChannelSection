﻿using ColdFormedChannelSection.Core.Enums;
using System.Collections.Generic;
using System.Linq;

namespace ColdFormedChannelSection.Core.Entities
{
    public  class Report:IReport
    {
       
        #region Properties

        public UnitSystems UnitSystem { get; }

        public string Name { get;  }

        public List<IReportSection> Sections { get;}

        #endregion

        #region Constructors

        public Report(UnitSystems unitSystem, string name,List<IReportSection> sections)
        {
            UnitSystem = unitSystem;
            Name = name;
            Sections = sections;
        }

        #endregion


        #region Methods

        public  IReport Convert( UnitSystems target)
        {
            var newSections = Sections.Select(sec => sec.Convert(UnitSystem, target)).ToList();
            return new Report(target, Name, newSections);
        }

        #endregion


    }
}
