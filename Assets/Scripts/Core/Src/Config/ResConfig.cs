using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Src.Config
{
    public class ResConfig
    {
        //bundle ID
        public const string DLL = "DLL";
        public const string GUI = "GUI";
        public const string CONFIG = "CONFIG";

        //bundle resource ID
        public const string OT = "OT";

        //GUI
        public const string GUI_DEMO = "GUI_DEMO";
        public const string GUI_MASK = "GUI_MASK";
        
        //GUI - Fight
        public const string GUI_FIGHT = "GUI_FIGHT";
        public const string GUI_FLOATTEXT = "GUI_FLOATTEXT";
        public const string GUI_HERO_UNITHUD = "GUI_HERO_UNITHUD";

        //SCENE
        public const string SCENE_FIGHT = "SCENE_FIGHT";

        //TPL
        public const string TPL_ALL_HERO_DATA = "TPL_ALL_HERO_DATA";
        public const string TPL_HERO_BASE_DATA = "HeroBaseData";
        public const string TPL_HERO_STAR_DATA = "HeroStarData";
        public const string TPL_HERO_GRADE_DATA = "HeroGradeData";
        public const string TPL_HERO_EQUIP_DATA = "HeroEquipData";
        public const string TPL_EQUIP = "EquipTpl";
    }
}
