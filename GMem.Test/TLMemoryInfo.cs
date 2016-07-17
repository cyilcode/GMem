using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMem.Test
{
    public class TLMemoryInfo
    {
        public static string exceptionMessage               = "The data type you have entered is not valid.";
        public static string gameName                       = "Torchlight2";
        public static string moduleName                     = "Torchlight2";
        public static int torchlightPlayerAddress           = 0x031CFAA4;
        public static int[] torchlightPlayerManaOffsets     = new int[] { 0x14, 0x584 };
        public static int[] torchlightPlayerSkillOffsets    = new int[] { 0x14, 0x5ac };
        public static int[] torchlightPlayerGoldOffsets     = new int[] { 0x14, 0x590 };
        public static int[] torchlightPlayerStatsOffsets    = new int[] { 0x14, 0x5a8 };
    }
}
