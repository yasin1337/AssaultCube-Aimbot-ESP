using System.Runtime.InteropServices;
using ezOverLay;
namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        ez ez = new ez();

        methods? APIMethods;
        Entity localPlayer = new Entity();
        List<Entity> entities = new List<Entity>();
        public Form1()
        {
            InitializeComponent();
        }

        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(Keys ArrowKeys);
        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            APIMethods = new methods();
            if (APIMethods != null)
            {
                ez.SetInvi(this);
                ez.DoStuff("AssaultCube", this);
                Thread thread = new Thread(Main) { IsBackground = true };
                thread.Start();

            }

            int i = 0;
        }

        void Main()
        {
            while (true)
            {
                localPlayer = APIMethods.ReadLocalPlayer();
                entities = APIMethods.ReadEntities(localPlayer);

                entities = entities.OrderBy(o => o.mag).ToList();


                if (GetAsyncKeyState(Keys.XButton1) < 0)
                {
                    if (entities.Count > 0)
                    {
                        foreach (var ent in entities)
                        {
                            if (ent.team != localPlayer.team)
                            {
                                var angles = APIMethods.CalculateAngles(localPlayer, ent);
                                APIMethods.Aim(localPlayer, angles.X, angles.Y);
                                break;
                            }
                        }
                    }
                }
                Form1 f = this;
                f.Refresh();
                Thread.Sleep(20);
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen red = new Pen(Color.Red, 3);
            Pen green = new Pen(Color.Green, 3);
            Font font = new Font("Arial", 12); 

            foreach (var ent in entities.ToList())
            {
                var wtsFeet = APIMethods.WorldToScreen(APIMethods.ReadMatrix(), ent.feet, this.Width, this.Height);
                var wtsHead = APIMethods.WorldToScreen(APIMethods.ReadMatrix(), ent.head, this.Width, this.Height);

                if (localPlayer.team == ent.team)
                {
                    g.DrawLine(green, new Point(Width / 2, Height), wtsFeet);
                    g.DrawRectangle(green, APIMethods.CalcRect(wtsFeet, wtsHead));

                    // Draw text below wtsFeet
                    int health = ent.health;
                    string text = ent.playerName + '-' + health;
                    SizeF textSize = g.MeasureString(text, font);
                    PointF textLocation = new PointF(wtsFeet.X - textSize.Width / 2, wtsFeet.Y + 20); 
                    g.DrawString(text, font, Brushes.Green, textLocation);
                }
                else
                {
                    g.DrawLine(red, new Point(Width / 2, Height), wtsFeet);
                    g.DrawRectangle(red, APIMethods.CalcRect(wtsFeet, wtsHead));

                    // Draw text below wtsFeet
                    int health = ent.health;
                    string text = ent.playerName + '-' + health;
                    SizeF textSize = g.MeasureString(text, font);
                    PointF textLocation = new PointF(wtsFeet.X - textSize.Width / 2, wtsFeet.Y + 20); // 20 units below wtsFeet
                    g.DrawString(text, font, Brushes.Red, textLocation);
                }
            }
        }
    }
}
