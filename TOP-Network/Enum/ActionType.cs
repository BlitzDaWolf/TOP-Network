using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOP_Network.Enum
{
    public enum ActionType
    {
        None = 0,
        Move,        // ÒÆ¶¯
        Skill,       // ¼¼ÄÜ
        SkillSource,   // Ê¹ÓÃ¼¼ÄÜ
        SkillTarget,   // ±»Ê¹ÓÃ¼¼ÄÜ
        Look,        // ¸üÐÂ½ÇÉ«Íâ¹Û
        KitBag,      // ¸üÐÂ½ÇÉ«µÄµÀ¾ßÀ¸
        SkillBag,    // ¸üÐÂ¼¼ÄÜÀ¸
        ItemPick,   // ¼ñµÀ¾ß
        ItemTrow,  // ¶ªµÀ¾ß
        ItemUnfix,  // µÀ¾ßÐ¶×°
        ItemUse,    // µÀ¾ßÊ¹ÓÃ
        ItemPosition,    // µÀ¾ß¸Ä±äÎ»ÖÃ
        ItemDelete, // µÀ¾ßÉ¾³ý
        ItemInfo,   // µÀ¾ßÐÅÏ¢
        ItemFailed, // µÀ¾ß²Ù×÷Ê§°Ü
        Lean,        // ÒÐ¿¿
        ChangeCharacter,  // ¸ü»»½ÇÉ«
        Event,       // ´¥·¢ÊÂ¼þ
        Face,        // ¿Í»§¶Ë×ö±íÇé¶¯×÷,Ä¿Ç°·þÎñÆ÷½öÐèÒª×ª·¢¸øÆäËü¿Í»§¶Ë
        StopState,  // Í£Ö¹¼¼ÄÜ×´Ì¬
        SkillPose,  // ¼¼ÄÜPose
        PkCtrl,     // PK¿ØÖÆ
        LookEnergy, // ¸üÐÂ½ÇÉ«Íâ¹ÛÄÜÁ¿

        Temp,        // ÁÙÊ±Ð­Òé

        Shortcut,    // ¿Í»§¶Ë·¢ËÍ¿ì½ÝÀ¸¸ø·þÎñÆ÷´æÅÌ£¬»ò·þÎñÆ÷Í¨Öª¿Í»§¶Ë¿ì½ÝÀ¸ÄÚÈÝ,×¢:±ØÐëÒªµÀ¾ßÀ¸,¼¼ÄÜÀ¸³õÊ¼Íê³Éºó²ÅÄÜ·¢ËÍ
        Bank,        // ¸üÐÂÒøÐÐÐÅÏ¢
        CloseBank,  // ¹Ø±ÕÒøÐÐ

        Kitbagtmp,       //¸üÐÂÁÙÊ±±³°ü
        KitbagtmpDrag,  //ÍÏ·ÅÁÙÊ±±³°üÖÐµÄµÀ¾ß

        TotalItemPick,//¶àµÀ¾ßÊ°È¡

        MaxActionNum      // ×î´óÐÐ¶¯¸öÊý
    }
}
