using System;
using Xunit;

namespace GMem.Test
{   /// <summary>
    /// Tested game name: Torchlight 2
    /// Version: v.1.25.5.2
    /// Character: Ranger
    /// To execute the test, you can either change everything down below for a different game 
    /// or make a character on torchlight 2 with the same stats
    /// Torchlight 2 v.1.25.5.2 cheat table can be found below
    /// http://www.cheatengine.org/tables/downloadtable.php?tid=1393
    /// </summary>
    public class GMemReadTests
    {
        private const string exceptionMessage           = "The data type you have entered is not valid.";
        private const string gameName                   = "Torchlight2";
        private const string moduleName                 = "Torchlight2";
        private const int torchlightPlayerAddress       = 0x031CFAA4;
        private const int playerGold                    = 1337;
        private const float playerFullMana              = 82;
        private const byte playerSkillPoints            = 50;
        private const double playerStats                = 1.06099789572967E-312; // 50 stat points as int.
        private int[] torchlightPlayerManaOffsets       = new int[] { 0x14, 0x584 };
        private int[] torchlightPlayerSkillOffsets      = new int[] { 0x14, 0x5ac };
        private int[] torchlightPlayerGoldOffsets       = new int[] { 0x14, 0x590 };
        private int[] torchlightPlayerStatsOffsets      = new int[] { 0x14, 0x5a8 };
        private GMemProcess torchlightProcess           = new GMemProcess(gameName, moduleName);
        private ptrObject mainPTR;
        [Fact]
        public void read_function_reads_float()
        {
            mainPTR = torchlightProcess.create_ptr_object(torchlightPlayerAddress, torchlightPlayerManaOffsets);
            Assert.Equal(playerFullMana, torchlightProcess.read<float>(mainPTR));
        }

        [Fact]
        public void read_function_reads_byte()
        {
            mainPTR = torchlightProcess.create_ptr_object(torchlightPlayerAddress, torchlightPlayerSkillOffsets);
            Assert.Equal(playerSkillPoints, torchlightProcess.read<byte>(mainPTR));
        }

        [Fact]
        public void read_function_reads_double()
        {
            mainPTR = torchlightProcess.create_ptr_object(torchlightPlayerAddress, torchlightPlayerStatsOffsets);
            Assert.Equal(playerStats, torchlightProcess.read<double>(mainPTR));
        }

        [Fact]
        public void read_function_reads_int()
        {
            mainPTR = torchlightProcess.create_ptr_object(torchlightPlayerAddress, torchlightPlayerGoldOffsets);
            Assert.Equal(playerGold, torchlightProcess.read<int>(mainPTR));
        }

        [Fact]
        public void read_function_throws_invcastexception()
        {
            mainPTR = torchlightProcess.create_ptr_object(torchlightPlayerAddress, torchlightPlayerGoldOffsets);
            Exception ex = Assert.Throws<InvalidCastException>(()=> torchlightProcess.read<decimal>(mainPTR));
            Assert.Equal(exceptionMessage, ex.Message);
        }

    }
}
