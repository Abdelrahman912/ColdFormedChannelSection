using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColdFormedChannelSection.App.ViewModels
{
    public class DirectStrengthViewModel : ResistanceBaseViewModel
    {
        public DirectStrengthViewModel(GeneralInfoViewModel generalInfoVM, BracingConditionsViewModel bracingConditionsVM, GeometryViewModel geometryVM , MaterialViewModel materialVM) 
            : base(generalInfoVM, bracingConditionsVM, geometryVM, materialVM)
        {
        }
    }
}
