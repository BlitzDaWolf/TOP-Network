using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOP_Network.Enum
{
    public enum ActionType
    {
        ACTION_NONE = 0,
        ACTION_MOVE=2,        // ÒÆ¶¯
        ACTION_SKILL=3,       // ¼¼ÄÜ
        ACTION_SKILL_SRC,   // Ê¹ÓÃ¼¼ÄÜ
        ACTION_SKILL_TAR,   // ±»Ê¹ÓÃ¼¼ÄÜ
        ACTION_LOOK,        // ¸üÐÂ½ÇÉ«Íâ¹Û
        ACTION_KITBAG,      // ¸üÐÂ½ÇÉ«µÄµÀ¾ßÀ¸
        ACTION_SKILLBAG,    // ¸üÐÂ¼¼ÄÜÀ¸
        ACTION_ITEM_PICK,   // ¼ñµÀ¾ß
        ACTION_ITEM_THROW,  // ¶ªµÀ¾ß
        ACTION_ITEM_UNFIX,  // µÀ¾ßÐ¶×°
        ACTION_ITEM_USE,    // µÀ¾ßÊ¹ÓÃ
        ACTION_ITEM_POS,    // µÀ¾ß¸Ä±äÎ»ÖÃ
        ACTION_ITEM_DELETE, // µÀ¾ßÉ¾³ý
        ACTION_ITEM_INFO,   // µÀ¾ßÐÅÏ¢
        ACTION_ITEM_FAILED, // µÀ¾ß²Ù×÷Ê§°Ü
        ACTION_LEAN,        // ÒÐ¿¿
        ACTION_CHANGE_CHA,  // ¸ü»»½ÇÉ«
        ACTION_EVENT,       // ´¥·¢ÊÂ¼þ
        ACTION_FACE,        // ¿Í»§¶Ë×ö±íÇé¶¯×÷,Ä¿Ç°·þÎñÆ÷½öÐèÒª×ª·¢¸øÆäËü¿Í»§¶Ë
        ACTION_STOP_STATE,  // Í£Ö¹¼¼ÄÜ×´Ì¬
        ACTION_SKILL_POSE,  // ¼¼ÄÜPose
        ACTION_PK_CTRL,     // PK¿ØÖÆ
        ACTION_LOOK_ENERGY, // ¸üÐÂ½ÇÉ«Íâ¹ÛÄÜÁ¿

        ACTION_TEMP,        // ÁÙÊ±Ð­Òé

        ACTION_SHORTCUT,    // ¿Í»§¶Ë·¢ËÍ¿ì½ÝÀ¸¸ø·þÎñÆ÷´æÅÌ£¬»ò·þÎñÆ÷Í¨Öª¿Í»§¶Ë¿ì½ÝÀ¸ÄÚÈÝ,×¢:±ØÐëÒªµÀ¾ßÀ¸,¼¼ÄÜÀ¸³õÊ¼Íê³Éºó²ÅÄÜ·¢ËÍ
        ACTION_BANK,        // ¸üÐÂÒøÐÐÐÅÏ¢
        ACTION_CLOSE_BANK,  // ¹Ø±ÕÒøÐÐ

        ACTION_KITBAGTMP,       //¸üÐÂÁÙÊ±±³°ü
        ACTION_KITBAGTMP_DRAG,  //ÍÏ·ÅÁÙÊ±±³°üÖÐµÄµÀ¾ß

        ACTION_TOTAL_ITEM_PICK,//¶àµÀ¾ßÊ°È¡

        MAX_ACTION_NUM      // ×î´óÐÐ¶¯¸öÊý
    }
}
