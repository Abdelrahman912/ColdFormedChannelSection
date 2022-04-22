﻿using ColdFormedChannelSection.App.ViewModels;
using ColdFormedChannelSection.Core.Entities;

namespace ColdFormedChannelSection.App.Extensions
{
    internal static class ViewModelsExtension
    {
        internal static LengthBracingConditions AsEntity(this BracingConditionsViewModel vm)
        {
            return new LengthBracingConditions(
                lx: vm.Lx,
                ly: vm.Ly,
                lz: vm.Lz,
                kx: vm.Kx,
                ky: vm.Ky,
                kz: vm.Kz,
                lu: vm.Lu,
                cb: vm.Cb,
                c1: vm.C1);

        }
    }
}
