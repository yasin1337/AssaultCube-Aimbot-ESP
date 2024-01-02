using Swed32;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public class methods
    {
        public Swed mem;
        public IntPtr moduleBase;


        public Entity ReadLocalPlayer()
        {
            var localPlayer = ReadEntity(mem.ReadPointer(moduleBase, Offsets.LocalPlayer));
            localPlayer.viewAngles.X = mem.ReadFloat(localPlayer.baseAdd, Offsets.Angles);
            localPlayer.viewAngles.Y = mem.ReadFloat(localPlayer.baseAdd, Offsets.Angles + 0x4);
            return localPlayer;
        }
        public Entity ReadEntity(IntPtr entBase)
        {
            var ent = new Entity();
            ent.baseAdd = entBase;
            ent.ammo = mem.ReadInt(ent.baseAdd, Offsets.Ammo);
            ent.health = mem.ReadInt(ent.baseAdd, Offsets.Health);
            ent.team = mem.ReadInt(ent.baseAdd, Offsets.Team);
            ent.feet = mem.ReadVec(ent.baseAdd, Offsets.FeetPos);
            ent.head = mem.ReadVec(ent.baseAdd, Offsets.HeadPos);
            ent.playerName = Encoding.UTF8.GetString(mem.ReadBytes(ent.baseAdd, Offsets.Name, 11));
            return ent;
        }

        public List<Entity> ReadEntities(Entity localPlayer)
        {
            var list = new List<Entity>();
            var entityList = mem.ReadPointer(moduleBase, Offsets.EntityList);
            for (int i = 0; i < 4; i++)
            {
                var currentEntityBase = mem.ReadPointer(entityList, i * 0x4);
                var ent = ReadEntity(currentEntityBase);
                ent.mag = CalcMag(localPlayer, ent);
                if(ent.health > 0)
                {
                    list.Add(ent);
                }
               
            }
            return list;
        }

        public Vector2 CalculateAngles(Entity localPlayer, Entity destination)
        {
        float x, y;

            var deltaX = destination.head.X - localPlayer.head.X;
            var deltaY = destination.head.Y - localPlayer.head.Y;

            x = (float)(Math.Atan2(deltaY, deltaX) * 180 / Math.PI) + 90;
            float deltaZ = destination.head.Z - localPlayer.head.Z;
            float dist = CalcDistance(localPlayer, destination);

            y = (float)(Math.Atan2(deltaZ, dist) * 180 / Math.PI);

            return new Vector2(x, y);
        }

        public void Aim(Entity ent, float x, float y)
        {
            mem.WriteFloat(ent.baseAdd, Offsets.Angles, x);
            mem.WriteFloat(ent.baseAdd, Offsets.Angles + 0x4, y);
        }

        public static float CalcDistance(Entity localPlayer, Entity destination)
        {
            return (float)
                Math.Sqrt(Math.Pow(destination.feet.X - localPlayer.feet.X, 2)
                + Math.Pow(destination.feet.Y - localPlayer.feet.Y, 2));

        }
        public static float CalcMag(Entity localPlayer, Entity destination)
        {
            return (float)
                Math.Sqrt(Math.Pow(destination.feet.X - localPlayer.feet.X, 2 ) 
                + Math.Pow(destination.feet.X -  localPlayer.feet.Y,2) + 
                Math.Pow(destination.feet.Z -  localPlayer.feet.Z, 2));
        }

        public Point WorldToScreen(ViewMatrix mtx, Vector3 pos, int width, int height)
        {
            var twoD = new Point();
            float screenW = (mtx.m * pos.X) + (mtx.m1 * pos.Y) + (mtx.m2 * pos.Z) + mtx.m3;
            
            if(screenW > 0.001f)
            {
                float screenX = (mtx.x * pos.X) + (mtx.x1 * pos.Y) + (mtx.x2 * pos.Z) + mtx.x3;
                float screenY = (mtx.y * pos.X) + (mtx.y1 * pos.Y) + (mtx.y2 * pos.Z) + mtx.y3;

                float camX = width / 2f;
                float camY = height / 2f;

                float X = camX + (camX * screenX / screenW);
                float y = camY - (camY * screenY / screenW);
                twoD.X = (int)X;
                twoD.Y = (int)y;
                return twoD;
            } else
            {
                return new Point(-99, -99);
            }

        }

        public Rectangle CalcRect(Point feet, Point head)
        {
            var rect = new Rectangle();
            rect.X = head.X - (feet.Y - head.Y) / 4;
            rect.Y = head.Y;

            rect.Width = (feet.Y - head.Y) / 2;
            rect.Height = (feet.Y - head.Y);
            return rect;
        }
        public ViewMatrix ReadMatrix()
        {
            var viewMatrix = new ViewMatrix();
            var mtx = mem.ReadMatrix(moduleBase + Offsets.ViewMatrix);

            viewMatrix.x = mtx[0];
            viewMatrix.y = mtx[1];
            viewMatrix.z = mtx[2];
            viewMatrix.m = mtx[3];

            viewMatrix.x1 = mtx[4];
            viewMatrix.y1 = mtx[5];
            viewMatrix.z1 = mtx[6];
            viewMatrix.m1 = mtx[7];

            viewMatrix.x2 = mtx[8];
            viewMatrix.y2 = mtx[9];
            viewMatrix.z2 = mtx[10];
            viewMatrix.m2 = mtx[11];
            
            viewMatrix.x3 = mtx[12];
            viewMatrix.y3 = mtx[13];
            viewMatrix.z3 = mtx[14];
            viewMatrix.m3 = mtx[15];


            return viewMatrix;
        }


        public methods()
        {
            mem = new Swed("ac_client");
            moduleBase = mem.GetModuleBase(".exe");
        }

    }
}
