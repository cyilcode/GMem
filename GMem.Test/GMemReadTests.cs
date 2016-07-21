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
        private const int playerGold                    = 1337;
        private const float playerFullMana              = 82;
        private const byte playerSkillPoints            = 50;
        private const double playerStats                = 1.06099789572967E-312; // 50 stat points as int.
        private const long playerGoldLong               = 41047002449209; // 1337 as int.
        private const string UIACLabel                  = "Active Quests";
        private readonly byte[] rawBytes                = new byte[] { 65, 99, 116, 105, 118, 101, 32, 81, 117, 101, 115, 116, 115 };
        private readonly string[] AOB                   = new string[] { "41", "63", "74", "69", "76", "65", "20", "51", "75", "65", "73", "74", "73" };
        private GMemProcess torchlightProcess           = new GMemProcess(TLMemoryInfo.gameName, TLMemoryInfo.moduleName);
        private ptrObject mainPTR;
        [Fact]
        public void read_function_reads_float()
        {
            mainPTR = torchlightProcess.create_ptr_object(TLMemoryInfo.torchlightPlayerAddress, TLMemoryInfo.torchlightPlayerManaOffsets);
            Assert.Equal(playerFullMana, torchlightProcess.read<float>(mainPTR));
        }

        [Fact]
        public void read_function_reads_byte()
        {
            mainPTR = torchlightProcess.create_ptr_object(TLMemoryInfo.torchlightPlayerAddress, TLMemoryInfo.torchlightPlayerSkillOffsets);
            Assert.Equal(playerSkillPoints, torchlightProcess.read<byte>(mainPTR));
        }

        [Fact]
        public void read_function_reads_double()
        {
            mainPTR = torchlightProcess.create_ptr_object(TLMemoryInfo.torchlightPlayerAddress, TLMemoryInfo.torchlightPlayerStatsOffsets);
            Assert.Equal(playerStats, torchlightProcess.read<double>(mainPTR));
        }

        [Fact]
        public void read_function_reads_int()
        {
            mainPTR = torchlightProcess.create_ptr_object(TLMemoryInfo.torchlightPlayerAddress, TLMemoryInfo.torchlightPlayerGoldOffsets);
            Assert.Equal(playerGold, torchlightProcess.read<int>(mainPTR));
        }

        [Fact]
        public void read_function_reads_short()
        {
            mainPTR = torchlightProcess.create_ptr_object(TLMemoryInfo.torchlightPlayerAddress, TLMemoryInfo.torchlightPlayerGoldOffsets);
            Assert.Equal(playerGold, torchlightProcess.read<short>(mainPTR));
        }

        [Fact]
        public void read_function_reads_long()
        {
            mainPTR = torchlightProcess.create_ptr_object(TLMemoryInfo.torchlightPlayerAddress, TLMemoryInfo.torchlightPlayerGoldOffsets);
            Assert.Equal(playerGoldLong, torchlightProcess.read<long>(mainPTR));
        }

        [Fact]
        public void read_function_reads_string()
        {
            mainPTR = torchlightProcess.create_ptr_object(TLMemoryInfo.torchlightUIAddress, TLMemoryInfo.torchlightUIACLabelOffsets);
            Assert.Equal(UIACLabel, torchlightProcess.read<string>(mainPTR, UIACLabel.Length));
        }


        [Fact]
        public void read_function_reads_rawbytes()
        {
            mainPTR = torchlightProcess.create_ptr_object(TLMemoryInfo.torchlightUIAddress, TLMemoryInfo.torchlightUIACLabelOffsets);
            byte[] dataBuffer = torchlightProcess.read<byte[]>(mainPTR, rawBytes.Length);
            for (int i = 0; i < rawBytes.Length; i++)
            {
                Assert.Equal(dataBuffer[i], rawBytes[i]);
            }
        }

        [Fact]
        public void read_functions_reads_AOBHexString()
        {
            mainPTR = torchlightProcess.create_ptr_object(TLMemoryInfo.torchlightUIAddress, TLMemoryInfo.torchlightUIACLabelOffsets);
            string[] dataBuffer = torchlightProcess.read<string[]>(mainPTR, AOB.Length);
            for (int i = 0; i < AOB.Length; i++)
            {
                Assert.Equal(dataBuffer[i], AOB[i]);
            }
        }

        [Fact]
        public void read_function_throws_invcastexception()
        {
            mainPTR = torchlightProcess.create_ptr_object(TLMemoryInfo.torchlightPlayerAddress, TLMemoryInfo.torchlightPlayerGoldOffsets);
            Exception ex = Assert.Throws<InvalidCastException>(()=> torchlightProcess.read<decimal>(mainPTR));
            Assert.Equal(TLMemoryInfo.exceptionMessage, ex.Message);
        }

    }
}
