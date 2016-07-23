using System;
using Xunit;

namespace GMem.Test
{
    // <summary>
    /// Tested game name: Torchlight 2
    /// Version: v.1.25.5.2
    /// Character: Ranger
    /// To execute the test, you can either change everything down below for a different game 
    /// or make a character on torchlight 2 with the same stats
    /// Torchlight 2 v.1.25.5.2 cheat table can be found below
    /// http://www.cheatengine.org/tables/downloadtable.php?tid=1393
    /// </summary>
    public class GMemWriteTests
    {
        private const int playerGoldToSet               = 1338;
        private const float playerManaToSet             = 32.2f;
        private const byte playerSkillPointsToSet       = 85;
        private const double playerStatsToSet           = 1.06099789589765E-312; // 83 stat points as int.
        private const string UIACLabel                  = "Bctive Quests";
        // "Active Quests" as string
        private readonly byte[] rawBytes                = new byte[] { 65, 99, 116, 105, 118, 101, 32, 81, 117, 101, 115, 116, 115 };
        // "Active Questt" as string
        private readonly string[] hexAOB                = new string[] { "41", "63", "74", "69", "76", "65", "20", "51", "75", "65", "73", "74", "74" };
        private GMemProcess torchlightProcess           = new GMemProcess(TLMemoryInfo.gameName, TLMemoryInfo.moduleName);
        private ptrObject mainPTR;
        // Attention: You need to pause the game in order to make this test result viable because of mana regeneration.
        [Fact]
        public void write_function_writes_float()
        {
            mainPTR = torchlightProcess.create_ptr_object(TLMemoryInfo.torchlightPlayerAddress, TLMemoryInfo.torchlightPlayerManaOffsets);
            bool iswritten = torchlightProcess.write<float>(mainPTR, playerManaToSet);
            Assert.Equal(iswritten, true);
            float memoryManaValue = torchlightProcess.read<float>(mainPTR);
            Assert.Equal(memoryManaValue, playerManaToSet);
        }

        [Fact]
        public void write_function_writes_byte()
        {
            mainPTR = torchlightProcess.create_ptr_object(TLMemoryInfo.torchlightPlayerAddress, TLMemoryInfo.torchlightPlayerSkillOffsets);
            bool iswritten = torchlightProcess.write<byte>(mainPTR, playerSkillPointsToSet);
            Assert.Equal(iswritten, true);
            byte memorySKPValue = torchlightProcess.read<byte>(mainPTR);
            Assert.Equal(memorySKPValue, playerSkillPointsToSet);
        }

        [Fact]
        public void write_function_writes_double()
        {
            mainPTR = torchlightProcess.create_ptr_object(TLMemoryInfo.torchlightPlayerAddress, TLMemoryInfo.torchlightPlayerStatsOffsets);
            bool iswritten = torchlightProcess.write<double>(mainPTR, playerStatsToSet);
            Assert.Equal(iswritten, true);
            double memorySTValue = torchlightProcess.read<double>(mainPTR);
            Assert.Equal(memorySTValue, playerStatsToSet);
        }

        [Fact]
        public void write_function_writes_int()
        {
            mainPTR = torchlightProcess.create_ptr_object(TLMemoryInfo.torchlightPlayerAddress, TLMemoryInfo.torchlightPlayerGoldOffsets);
            bool iswritten = torchlightProcess.write<int>(mainPTR, playerGoldToSet);
            Assert.Equal(iswritten, true);
            double memoryGoldValue = torchlightProcess.read<int>(mainPTR);
            Assert.Equal(memoryGoldValue, playerGoldToSet);
        }

        [Fact]
        public void write_function_writes_string()
        {
            mainPTR = torchlightProcess.create_ptr_object(TLMemoryInfo.torchlightUIAddress, TLMemoryInfo.torchlightUIACLabelOffsets);
            bool iswritten = torchlightProcess.write<string>(mainPTR, UIACLabel);
            Assert.Equal(iswritten, true);
            string memoryStringValue = torchlightProcess.read<string>(mainPTR, UIACLabel.Length);
            Assert.Equal(memoryStringValue, UIACLabel);
        }

        [Fact]
        public void write_function_writes_rawAOB()
        {
            mainPTR = torchlightProcess.create_ptr_object(TLMemoryInfo.torchlightUIAddress, TLMemoryInfo.torchlightUIACLabelOffsets);
            bool iswritten = torchlightProcess.write<byte[]>(mainPTR, rawBytes);
            Assert.Equal(iswritten, true);
            byte[] memoryBytes = torchlightProcess.read<byte[]>(mainPTR, rawBytes.Length);
            for (int i = 0; i < rawBytes.Length; i++) Assert.Equal(memoryBytes[i], rawBytes[i]);
        }

        [Fact]
        public void write_function_writes_stringAOB()
        {
            mainPTR = torchlightProcess.create_ptr_object(TLMemoryInfo.torchlightUIAddress, TLMemoryInfo.torchlightUIACLabelOffsets);
            bool iswritten = torchlightProcess.write<string[]>(mainPTR, hexAOB);
            Assert.Equal(iswritten, true);
            string[] memoryBytes = torchlightProcess.read<string[]>(mainPTR, hexAOB.Length);
            for (int i = 0; i < rawBytes.Length; i++) Assert.Equal(memoryBytes[i], hexAOB[i]);
        }

        [Fact]
        public void write_function_throws_invcastexception()
        {
            mainPTR = torchlightProcess.create_ptr_object(TLMemoryInfo.torchlightPlayerAddress, TLMemoryInfo.torchlightPlayerGoldOffsets);
            Exception ex = Assert.Throws<InvalidCastException>(()=> torchlightProcess.write<decimal>(mainPTR, playerGoldToSet));
            Assert.Equal(TLMemoryInfo.exceptionMessage, ex.Message);
        }
    }
}