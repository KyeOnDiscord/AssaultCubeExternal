using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssaultCubeExternal
{
    internal static class Offsets
    {
        internal const int NoRecoil = 0x63786;


        //Local Player
        internal const int LocalPlayer = 0x10F4F4;

        internal const int m_XPos = 0x0038;
        internal const int m_YPos = 0x003C;
        internal const int m_ZPos = 0x0040;
        internal const int m_isPosMoving = 0x0070;

        internal const int m_Health = 0x00F8;
        internal const int m_Vest = 0x00FC;
        internal const int m_Team = 0x0329;

    }
}
