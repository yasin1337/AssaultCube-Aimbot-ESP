using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1
{
    public class Entity
    {
        public IntPtr baseAdd;
        public Vector3 feet, head;
        public Vector2 viewAngles;
        public float mag, viewOffset;
        public int health, team, ammo, dead;
        public string playerName;

    }

    public class ViewMatrix
    {
        public float x, y, z, m;
        public float x1, y1, z1, m1;
        public float x2, y2, z2, m2;
        public float x3, y3, z3, m3;
    }
}
