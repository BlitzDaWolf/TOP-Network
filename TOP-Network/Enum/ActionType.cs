using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOP_Network.Enum
{
    public enum ActionType
    {
        enumACTION_NONE = 0,
        enumACTION_MOVE,        // ÒÆ¶¯
        enumACTION_SKILL,       // ¼¼ÄÜ
        enumACTION_SKILL_SRC,   // Ê¹ÓÃ¼¼ÄÜ
        enumACTION_SKILL_TAR,   // ±»Ê¹ÓÃ¼¼ÄÜ
        enumACTION_LOOK,        // ¸üÐÂ½ÇÉ«Íâ¹Û
        enumACTION_KITBAG,      // ¸üÐÂ½ÇÉ«µÄµÀ¾ßÀ¸
        enumACTION_SKILLBAG,    // ¸üÐÂ¼¼ÄÜÀ¸
        enumACTION_ITEM_PICK,   // ¼ñµÀ¾ß
        enumACTION_ITEM_THROW,  // ¶ªµÀ¾ß
        enumACTION_ITEM_UNFIX,  // µÀ¾ßÐ¶×°
        enumACTION_ITEM_USE,    // µÀ¾ßÊ¹ÓÃ
        enumACTION_ITEM_POS,    // µÀ¾ß¸Ä±äÎ»ÖÃ
        enumACTION_ITEM_DELETE, // µÀ¾ßÉ¾³ý
        enumACTION_ITEM_INFO,   // µÀ¾ßÐÅÏ¢
        enumACTION_ITEM_FAILED, // µÀ¾ß²Ù×÷Ê§°Ü
        enumACTION_LEAN,        // ÒÐ¿¿
        enumACTION_CHANGE_CHA,  // ¸ü»»½ÇÉ«
        enumACTION_EVENT,       // ´¥·¢ÊÂ¼þ
        enumACTION_FACE,        // ¿Í»§¶Ë×ö±íÇé¶¯×÷,Ä¿Ç°·þÎñÆ÷½öÐèÒª×ª·¢¸øÆäËü¿Í»§¶Ë
        enumACTION_STOP_STATE,  // Í£Ö¹¼¼ÄÜ×´Ì¬
        enumACTION_SKILL_POSE,  // ¼¼ÄÜPose
        enumACTION_PK_CTRL,     // PK¿ØÖÆ
        enumACTION_LOOK_ENERGY, // ¸üÐÂ½ÇÉ«Íâ¹ÛÄÜÁ¿

        enumACTION_TEMP,        // ÁÙÊ±Ð­Òé

        enumACTION_SHORTCUT,    // ¿Í»§¶Ë·¢ËÍ¿ì½ÝÀ¸¸ø·þÎñÆ÷´æÅÌ£¬»ò·þÎñÆ÷Í¨Öª¿Í»§¶Ë¿ì½ÝÀ¸ÄÚÈÝ,×¢:±ØÐëÒªµÀ¾ßÀ¸,¼¼ÄÜÀ¸³õÊ¼Íê³Éºó²ÅÄÜ·¢ËÍ
        enumACTION_BANK,        // ¸üÐÂÒøÐÐÐÅÏ¢
        enumACTION_CLOSE_BANK,  // ¹Ø±ÕÒøÐÐ

        enumACTION_KITBAGTMP,       //¸üÐÂÁÙÊ±±³°ü
        enumACTION_KITBAGTMP_DRAG,  //ÍÏ·ÅÁÙÊ±±³°üÖÐµÄµÀ¾ß

        enumACTION_TOTAL_ITEM_PICK,//¶àµÀ¾ßÊ°È¡

        enumMAX_ACTION_NUM      // ×î´óÐÐ¶¯¸öÊý
    }
}
