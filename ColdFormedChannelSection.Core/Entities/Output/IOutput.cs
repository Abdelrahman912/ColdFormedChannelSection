using ColdFormedChannelSection.Core.Entities;
using System;

namespace ColdFormedChannelSection.Core.Entities
{
    public interface IOutput
    {

        public IReport GenerateReport(Func<IReport> func) =>
            func();

    }

}
