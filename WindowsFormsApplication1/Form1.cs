using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    enum ShapeType { O = I, J, L, O, S, T, Z };
    public partial class Form1 : Form
    {
        int[,] gameMap = new int[10, 24];
        Shape t;  
        Shape Lock;
        Shape Next;
        Shape Ramdom;
        Boolean lockbool;
        Graphics mainGra;
        Graphics subGra;
        Graphics lockGra;
        Boolean change;
        int interval = 230;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            change = true;
            mainGra = panel1.CreateGraphics();
            subGra = panel2.CreateGraphics();
            lockGra = panel3.CreateGraphics();
            lockbool = false;
            t = randomShape();
            do
            {
                Next = randomShape();
            } while (t.shape == Next.shape);
            Next.shapeDrawSub(subGra);
            timer1.Interval = 50;
            timer2.Interval = interval;
            timer1.Start();
            timer2.Start();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            mainGra.Clear(Control.DefaultBackColor);
            FunctionCollection.mapDraw(gameMap, mainGra);
            t.shapeDrawMain(mainGra);

        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            
            gameMain();
        }
        private void gameMain()
        {
            Next.shapeDrawSub(subGra);
            if (!t.moveDown(gameMap))
            {
                FunctionCollection.gameOver(gameMap,timer2);
                t = Next;
                do
                {
                    Next = randomShape();
                } while (t.shape == Next.shape);
                
                subGra.Clear(Control.DefaultBackColor);
                Next.shapeDrawSub(subGra);
                change = true;
                label4.Text = (FunctionCollection.getScore(gameMap)+int.Parse(label4.Text)).ToString("000000");
                timer2.Interval = interval;
            };
        }
        private Shape randomShape()
        {
            Random r = new Random();
            switch (r.Next(0, 8))
            {
                case (1): return new shapeO();
                case (2): return new shapeOther(ShapeType.J);
                case (3): return new shapeOther(ShapeType.L);
                case (4): return new shapeOther(ShapeType.S);
                case (5): return new shapeOther(ShapeType.T);
                case (6): return new shapeOther(ShapeType.Z);
                default: return new shapeI();
            }


        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case (Keys.Up):
                    t.turn(gameMap);
                    break;
                case (Keys.Down):
                    timer2.Interval = 50;
                    break;
                case (Keys.Right):
                    t.moveRight(gameMap);
                    break;
                case (Keys.Left):
                    t.moveLeft(gameMap);
                    break;
                case (Keys.Space):

                case (Keys.L):
                    if (change)
                    {
                        if (lockbool)
                        {
                            Ramdom = randomShape();
                            Ramdom = Lock;
                            Lock = Next;
                            Next = Ramdom;
                        }
                        else
                        {
                            Lock = Next;
                            Next = randomShape();
                            lockbool = true;
                        }
                        subGra.Clear(Control.DefaultBackColor);
                        lockGra.Clear(Control.DefaultBackColor);
                        Lock.shapeDrawSub(lockGra);
                        Next.shapeDrawSub(subGra);
                        change = false;
                    }
                        break; 
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }

    class Block
    {
        int _x, _y;
        ShapeType _shape;
        Color c;
        public Block(int x, int y, ShapeType s)
        {
            this._x = x;
            this._y = y;
            this._shape = s;
            c = FunctionCollection.getColor(s);
        }
        public Boolean canAccept(int[,] map)
        {
            if (this._x > 9 || this._x < 0 || this._y > 23 || map[this._x, this._y] != 0)
            {
                return false;
            }
            return true;
        }
        public void addToMap(int[,] map)
        {
            map[this._x, this._y] = (int)this._shape;
        }
        public void drawBlock(Graphics g)
        {
            g.FillRectangle(new SolidBrush(this.c), new Rectangle(this._x * 20, this._y * 20, 20, 20));
        }
        public void drawBlockSub(Graphics g, int x, int y)
        {
            g.FillRectangle(new SolidBrush(this.c), new Rectangle(this._x * 20 + x, this._y * 20 + y, 20, 20));
        }
    }
    class Shape
    {
        protected int[,] _organizeArray;
        protected ShapeType _shape;
        protected Block[] block;
        protected int _x = 4, _y = 0;
        internal ShapeType shape
        {
            get { return _shape; }
            set { _shape = value; }
        }
        protected Shape(ShapeType st)
        {
            this.shape = st;
            switch (st)
            {
                case (ShapeType.O): this._organizeArray = new int[2, 2];
                    break;
                case (ShapeType.I): this._organizeArray = new int[4, 4];
                    break;
                default: this._organizeArray = new int[3, 3];
                    break;
            }
            block = new Block[4];
        }
        public void initBlock()
        {
            int i = 0;
            for (int j = 0; j < this._organizeArray.GetLength(0); j++)
            {
                for (int k = 0; k < this._organizeArray.GetLength(1); k++)
                {
                    if (this._organizeArray[j, k] != 0)
                    {
                        block[i] = new Block(this._x + j, _y + k, this._shape);
                        i++;
                    }
                }
            }
        }
        public void shapeDrawMain(Graphics g)
        {
            this._y = this._y - 4;
            initBlock();
            this._y = this._y + 4;
            foreach (Block item in block)
            {
                item.drawBlock(g);
            }
        }

        public virtual void shapeDrawSub(Graphics g)
        {

        }

        public Boolean canAct(int[,] map)
        {
            Boolean b = true;
            foreach (Block item in block)
            {
                b = b && item.canAccept(map);
            }
            return b;
        }
      
        public Boolean moveDown(int[,] map)
        {
  
            this._y++;
            initBlock();
            if (canAct(map))
            {
                return true;
            }
            else
            {
                this._y--;

                initBlock();

                addToMap(map);

                return false;
            }
        }
        public void moveLeft(int[,] map)
        {
            this._x--;
            initBlock();
            if (canAct(map))
            {

            }
            else
            {
                this._x++;

                initBlock();
            }
        }
        public void moveRight(int[,] map)
        {
            this._x++;

            initBlock();

            if (canAct(map))
            {

            }
            else
            {
                this._x--;
                initBlock();
            }
        }

        public void turn(int[,] map)
        {
            turnShape();
            initBlock();
            if (canAct(map))
            {

            }
            else
            {
                cancelTurnShape();
                initBlock();
            }
        }
        public virtual void turnShape()
        {

        }
        public virtual void cancelTurnShape()
        {

        }
        public void addToMap(int[,] map)
        {
            foreach (Block item in block)
            {
                item.addToMap(map);
            }
        }
    }
    class shapeO : Shape
    {
        public shapeO()
            : base(ShapeType.O)
        {
           
            for (int j = 0; j < this._organizeArray.GetLength(0); j++)
            {
                for (int k = 0; k < this._organizeArray.GetLength(1); k++)
                {
                    this._organizeArray[j, k] = (int)this._shape;
                }
            }
            initBlock();
        }
        public override void shapeDrawSub(Graphics g)
        {
            foreach (Block item in block)
            {
                item.drawBlockSub(g, -50, 25);
            }
        }
    }
    class shapeI : Shape
    {
        public shapeI()
            : base(ShapeType.I)
        {
           
            this._organizeArray[1, 0] = (int)this._shape;
            this._organizeArray[1, 1] = (int)this._shape;
            this._organizeArray[1, 2] = (int)this._shape;
            this._organizeArray[1, 3] = (int)this._shape;
            initBlock();
        }
        public override void turnShape()
        {
           
            if (this._organizeArray[1, 0] == 0)
            {
               
                Array.Clear(this._organizeArray, 0, this._organizeArray.Length);
                this._organizeArray[1, 0] = (int)this._shape;
                this._organizeArray[1, 1] = (int)this._shape;
                this._organizeArray[1, 2] = (int)this._shape;
                this._organizeArray[1, 3] = (int)this._shape;
            }
            else
            {
               
                Array.Clear(this._organizeArray, 0, this._organizeArray.Length);
                this._organizeArray[0, 1] = (int)this._shape;
                this._organizeArray[1, 1] = (int)this._shape;
                this._organizeArray[2, 1] = (int)this._shape;
                this._organizeArray[3, 1] = (int)this._shape;
            }
        }
        public override void cancelTurnShape()
        {
            
            turnShape();
        }
        public override void shapeDrawSub(Graphics g)
        {
            foreach (Block item in block)
            {
                
                item.drawBlockSub(g, -60, 10);
            }
        }
    }
    class shapeOther : Shape
    {
        public shapeOther(ShapeType s)
            : base(s)
        {

            switch (s)
            {

                case (ShapeType.J):
                    this._organizeArray[2, 0] = (int)this._shape;
                    this._organizeArray[2, 1] = (int)this._shape;
                    this._organizeArray[2, 2] = (int)this._shape;
                    this._organizeArray[1, 2] = (int)this._shape;
                    break;
                case (ShapeType.L):
                    this._organizeArray[1, 0] = (int)this._shape;
                    this._organizeArray[1, 1] = (int)this._shape;
                    this._organizeArray[2, 1] = (int)this._shape;
                    this._organizeArray[2, 2] = (int)this._shape;
                    break;
                case (ShapeType.S):
                    this._organizeArray[0, 2] = (int)this._shape;
                    this._organizeArray[0, 1] = (int)this._shape;
                    this._organizeArray[1, 0] = (int)this._shape;
                    this._organizeArray[1, 1] = (int)this._shape;
                    break;
                case (ShapeType.T):
                    this._organizeArray[0, 1] = (int)this._shape;
                    this._organizeArray[1, 1] = (int)this._shape;
                    this._organizeArray[2, 1] = (int)this._shape;
                    this._organizeArray[1, 0] = (int)this._shape;
                    break;
                case ShapeType.Z:
                    this._organizeArray[0, 0] = (int)this._shape;
                    this._organizeArray[0, 1] = (int)this._shape;
                    this._organizeArray[1, 1] = (int)this._shape;
                    this._organizeArray[1, 2] = (int)this._shape;
                    break;
            }
            initBlock();
        }
        public override void cancelTurnShape()
        {
           
            int[,] arrayRam = new int[3, 3];
            Array.Copy(this._organizeArray, arrayRam, 9);
            this._organizeArray[0, 0] = arrayRam[2, 0];
            this._organizeArray[0, 1] = arrayRam[1, 0];
            this._organizeArray[0, 2] = arrayRam[0, 0];
            this._organizeArray[1, 2] = arrayRam[0, 1];
            this._organizeArray[2, 2] = arrayRam[0, 2];
            this._organizeArray[2, 1] = arrayRam[1, 2];
            this._organizeArray[2, 0] = arrayRam[2, 2];
            this._organizeArray[1, 0] = arrayRam[2, 1];
        }
        public override void turnShape()
        {
      
            int[,] arrayRam = new int[3, 3];
            Array.Copy(this._organizeArray, arrayRam, 9);
            this._organizeArray[2, 0] = arrayRam[0, 0];
            this._organizeArray[1, 0] = arrayRam[0, 1];
            this._organizeArray[0, 0] = arrayRam[0, 2];
            this._organizeArray[0, 1] = arrayRam[1, 2];
            this._organizeArray[0, 2] = arrayRam[2, 2];
            this._organizeArray[1, 2] = arrayRam[2, 1];
            this._organizeArray[2, 2] = arrayRam[2, 0];
            this._organizeArray[2, 1] = arrayRam[1, 0];
        }
        public override void shapeDrawSub(Graphics g)
        {
            foreach (Block item in block)
            {
            
                item.drawBlockSub(g, -63, 10);
            }
        }
    }

    static class FunctionCollection
    {
 
        public static Color getColor(ShapeType s)
        {
            switch ((int)s)
            {
                case (1): return Color.Blue ;
                case (2): return Color.Red ;
                case (3): return Color.Pink ;
                case (4): return Color.Purple ;
                case (5): return Color.Orange ;
                case (6): return Color.SeaGreen ;
                default: return Color.SkyBlue;
            }
        }

        public static void mapDraw(int[,] map, Graphics g)
        {
            for (int j = 0; j < map.GetLength(0); j++)
            {
                for (int k = 0; k < map.GetLength(1); k++)
                {
                    if (map[j, k] != 0)
                    {
                        g.FillRectangle(new SolidBrush(getColor((ShapeType)map[j, k])), new Rectangle(j * 20, (k - 4) * 20, 20, 20));
                    }
                }
            }
        }

        public static int getScore(int[,] map)
        {
            int Score = 0;
            int i = 0;
            for (int j = 0; j < 24; j++)
            {
                for (int k = 0; k < 10; k++)
                {
                    if (map[k, j] != 0)
                    {
                        i++;
                    }
                }
                if (i == 10)
                {
                    Score++;
                   
                    for (int w = j; w > 1; w--)
                    {
                        for (int l = 0; l < 10; l++)
                        {
                            map[l, w] = map[l, w - 1];
                        }
                    }
                }
                i = 0;
            }
            switch (Score)
            {
                case (1): return 1000;
                case (2): return 2250;
                case (3): return 3000;
                case (4): return 5000;
                default: return 0;
            }
        }
        public static void gameOver(int[,] map,Timer timer) {
            for (int i = 0; i < 10; i++)
			{
                if (map[i, 3] != 0) {
                    timer.Stop();
                    MessageBox.Show("GAME OVER", "WARNING");
                    Environment.Exit(0);
                }
			}
        }
    }
}
