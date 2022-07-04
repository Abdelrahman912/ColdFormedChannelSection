using ColdFormedChannelSection.Core.Enums;
using System.Collections.Generic;

namespace ColdFormedChannelSection.Core
{
    public static class Constants
    {

        public static Dictionary<UnitSystems, Units> Power1Dict = new Dictionary<UnitSystems, Units>()
        {
            [UnitSystems.KIPINCH] = Units.IN,
            [UnitSystems.NMM] = Units.MM,
            [UnitSystems.TONCM] = Units.CM
        };

        public static Dictionary<UnitSystems, Units> Power2Dict = new Dictionary<UnitSystems, Units>()
        {
            [UnitSystems.KIPINCH]=Units.IN_2,
            [UnitSystems.NMM]=Units.MM_2,
            [UnitSystems.TONCM]=Units.CM_2
        };

        public static Dictionary<UnitSystems, Units> Power3Dict = new Dictionary<UnitSystems, Units>()
        {
            [UnitSystems.KIPINCH] = Units.IN_3,
            [UnitSystems.NMM] = Units.MM_3,
            [UnitSystems.TONCM] = Units.CM_3
        };

        public static Dictionary<UnitSystems, Units> Power4Dict = new Dictionary<UnitSystems, Units>()
        {
            [UnitSystems.KIPINCH] = Units.IN_4,
            [UnitSystems.NMM] = Units.MM_4,
            [UnitSystems.TONCM] = Units.CM_4
        };

        public static Dictionary<UnitSystems, Units> Power6Dict = new Dictionary<UnitSystems, Units>()
        {
            [UnitSystems.KIPINCH] = Units.IN_6,
            [UnitSystems.NMM] = Units.MM_6,
            [UnitSystems.TONCM] = Units.CM_6
        };

        public const double PHI_C_EGYPT = 0.8;

        public const double PHI_B_EGYPT = 0.85;

        public const string PHI_C_NAME_EGYPT = "(phi)c";

        public const string COMP_DESIGN_RESIST_EGYPT = "(phi)c * Pn";

        public const string PHI_B_NAME_EGYPT = "(phi)b";

        public const string MOM_DESIGN_RESIST_EGYPT = "(phi)b * Mn";



        public const double PHI_EURO = 1.0;

        public const string PHI_NAME_EURO = "gamma";

        public const string COMP_DESIGN_RESIST_EURO = "Pn/gamma";

        public const string MOM_DESIGN_RESIST_EURO = "Mn/gamma";


        public const double PHI_C_AISI = 0.85;

        public const double PHI_B_AISI = 0.9;

        public const string PHI_C_NAME_AISI = "(phi)c";

        public const string COMP_DESIGN_RESIST_AISI = "(phi)c * Pn";

        public const string PHI_B_NAME_AISI = "(phi)b";

        public const string MOM_DESIGN_RESIST_AISI = "(phi)b * Mn";


        public const double PHI_C_DS = 0.85;

        public const double PHI_B_DS = 0.9;

        public const string PHI_C_NAME_DS = "(phi)c";

        public const string COMP_DESIGN_RESIST_DS = "(phi)c * Pn";

        public const string PHI_B_NAME_DS = "(phi)b";

        public const string MOM_DESIGN_RESIST_DS = "(phi)b * Mn";


        public const double TOL = 0.1;

        public const string EGYPT_UNSTIFF_TABLE_C = "Egypt - Unstiffened - C";
        public const string EGYPT_STIFF_TABL_C = "Egypt - Stiffened - C";
        public const string AISI_STIFF_TABLE_C = "AISI - Stiffened - C";
        public const string AISI_UNSTIFF_TABLE_C = "AISI - Unstiffened - C";

        public const string EGYPT_UNSTIFF_TABLE_Z = "Egypt - Unstiffened - Z";
        public const string EGYPT_STIFF_TABL_Z = "Egypt - Stiffened - Z";
        public const string AISI_STIFF_TABLE_Z = "AISI - Stiffened - Z";
        public const string AISI_UNSTIFF_TABLE_Z = "AISI - Unstiffened - Z";
        public const string DATABASE_FOLDER = "Database";
    }
}
