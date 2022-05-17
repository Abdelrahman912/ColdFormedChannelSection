using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColdFormedChannelSection.App.ViewModels
{
    public class EffectiveWidthViewModel : ResistanceBaseViewModel
    {
        public EffectiveWidthViewModel(GeneralInfoViewModel generalInfoVM, BracingConditionsViewModel bracingConditionsVM, GeometryViewModel geometryVM,MaterialViewModel materialVM,InputLoadViewModel inputLoadVM) 
            : base(generalInfoVM, bracingConditionsVM, geometryVM,materialVM,inputLoadVM)
        {

        }
    }
}
