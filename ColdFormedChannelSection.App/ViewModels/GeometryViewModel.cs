using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColdFormedChannelSection.App.ViewModels
{
    internal class GeometryViewModel:ViewModelBase
    {

        #region Private Fields

        private double _totalHeightH;
        private double _totalWidthB;
        private double _internalRadiusR;
        private double _thicknessT;
        private double _totalFoldWidthC;
        private bool _isUnstiffened;
        #endregion

        #region Properties

        public bool IsUnstiffened
        {
            get => _isUnstiffened;
            set => NotifyPropertyChanged(ref _isUnstiffened, value);
        }

        public double TotalFoldWidthC
        {
            get => _totalFoldWidthC;
            set => NotifyPropertyChanged(ref _totalFoldWidthC, value);
        }

        public double ThicknessT
        {
            get => _thicknessT;
            set => NotifyPropertyChanged(ref _thicknessT, value);
        }

        public double InternalRadiusR
        {
            get => _internalRadiusR;
            set => NotifyPropertyChanged(ref _internalRadiusR, value);
        }

        public double TotalWidthB
        {
            get => _totalWidthB;
            set => NotifyPropertyChanged(ref _totalWidthB, value);
        }

        public double TotalHeightH
        {
            get => _totalHeightH;
            set => NotifyPropertyChanged(ref _totalHeightH, value);
        }

        #endregion

        #region Constructors

        public GeometryViewModel()
        {
            TotalFoldWidthC = 0.0;
            TotalHeightH = 0.0;
            TotalWidthB = 0;
            ThicknessT = 0.0;
            InternalRadiusR = 0.0;
            IsUnstiffened = true;
            Mediator.Mediator.Instance.Subscribe<bool>(this, OnStiffChanged, Context.STIFF_UNSTIFF);
        }


        #endregion

        #region Methods

        private void OnStiffChanged(bool isUnstiff)
        {
           IsUnstiffened=isUnstiff;
            if (isUnstiff)
                TotalFoldWidthC = 0;
        }

        #endregion


    }
}
