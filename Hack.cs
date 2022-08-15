using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssaultCubeExternal
{
    internal static class Hack
    {

        internal static async Task HackThreadAsync()
        {
            int gameBase = (int)Memory.GetModuleBaseAddress("ac_client.exe");
            int LocalPlayer = Memory.ReadMemory<int>(gameBase + Offsets.LocalPlayer);
            while (true)
            {
                List<string> EnabledHacks = Program.EnabledHacks;


                //No Recoil
                   if (EnabledHacks.IndexOf("No Recoil") == -1)//If No Recoil is off
                   {
                        //\x50\x8d\x4c\x24\x1c\x51\x8b\xce\xff\xd2
                        Memory.PatchEx(gameBase + Offsets.NoRecoil, new byte[] { 0x50, 0x8D, 0x4C, 0x24, 0x1C, 0x51, 0x8B, 0xCE, 0xFF, 0xD2 });
                   }
                   else
                   {
                    Memory.NopEx(gameBase + Offsets.NoRecoil, 10);
                   }

                   if (EnabledHacks.IndexOf("Unlimited Health") != -1)
                   {
                        Memory.WriteMemory<int>(LocalPlayer + Offsets.m_Health, 9999);
                   }

                   if (EnabledHacks.IndexOf("Unlimited Armor") != -1)
                   {
                   Memory.WriteMemory<int>(LocalPlayer + Offsets.m_Vest, 9999);
                   }


                await Task.Delay(TimeSpan.FromSeconds(1));//Update once per second
            }
            
        
        
        }

    }
}
