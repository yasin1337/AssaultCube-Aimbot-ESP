using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1
{
    public class Offsets()
    {
        public static int
            ViewMatrix = 0x17DFFC-0x6C+0x4*16,
            LocalPlayer = 0x0017E0A8,
            EntityList = 0x00191FCC,

            //offsets of entity list
            HeadPos = 0x4,
            FeetPos = 0x28,
            Angles = 0x34,
            Health = 0xEC,
            Armor = 0x28,
            Name = 0x205,
            isDead = 0xB4,
            Team = 0x30C,
            Ammo = 0x140;

    }
}
