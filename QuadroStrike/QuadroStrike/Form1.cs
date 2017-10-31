using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

// для работы с библиотекой OpenGL 
using Tao.OpenGl;
// для работы с библиотекой FreeGLUT 
using Tao.FreeGlut;
// для работы с элементом управления SimpleOpenGLControl 
using Tao.Platform.Windows;
using System.Threading;

namespace QuadroStrike
{
    public partial class Form1 : Form
    {
        class Coorditates
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }
            public double Size { get; set; }
            public double R { get; set; }

            public Coorditates()
            {
                X = 1;
                Y = 0;
                Z = -6;
                Size = 0.5;
                R = 0.2;
            }
        }

        class DropSphereState
        {
            public int I { get; set; }
            public bool IsDrop { get; set; }

            public DropSphereState()
            {
                IsDrop = false;
            }
        }

        DropSphereState dropSphereState;
        bool Pouse = false;
        Coorditates sp = new Coorditates();
        List<Coorditates> qp = new List<Coorditates>();

        public Form1()
        {
            InitializeComponent();
            AnT.InitializeContexts();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);

            // отчитка окна 
            Gl.glClearColor(255, 255, 255, 1);

            // установка порта вывода в соответствии с размерами элемента anT 
            Gl.glViewport(0, 0, AnT.Width, AnT.Height);


            // настройка проекции 
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluPerspective(45, (float)AnT.Width / (float)AnT.Height, 0.1, 200);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();

            // настройка параметров OpenGL для визуализации 
            Gl.glEnable(Gl.GL_DEPTH_TEST);

            dropSphereState = new DropSphereState();

            //DrowSphere();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //sp = new Coorditates() { X = -4, Y = -2, Z = -6 };
            Pouse = false;
            dropSphereState.IsDrop = false;
            dropSphereState.I = 0;
            timer1.Start();
            InitializeQuadrats();
            RandomizeQuadratsHeight();
            //DrowAll();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        void DrowSphere(Coorditates sp)
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

            Gl.glLoadIdentity();
            Gl.glColor3f(1.0f, 0, 0);

            Gl.glPushMatrix();
            Gl.glTranslated(sp.X, sp.Y, sp.Z);
            Gl.glRotated(45, 1, 1, 0);

            // рисуем сферу с помощью библиотеки FreeGLUT 
            Glut.glutWireSphere(sp.R, 8, 8);
            Glut.glutWireSphere(0.02, 8, 8);

            Gl.glPopMatrix();
            Gl.glFlush();
            AnT.Invalidate();
        }

        private void AnT_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (!dropSphereState.IsDrop)
                switch (e.KeyData)
                {
                    case Keys.A:
                        {
                            sp.X -= 0.1;
                        }
                        break;
                    case Keys.D:
                        {
                            sp.X += 0.1;
                        }
                        break;
                    case Keys.S:
                        {
                            dropSphereState.IsDrop = true;
                        }
                        break;
                }
        }

        void GrowQuadrats()
        {
            foreach (Coorditates c in qp)
            {
                c.Size += 0.01;
                if (c.Size >= 2)
                {
                    Pouse = true;
                    MessageBox.Show("Колапс ... \nэто максимум что мне может предложить мой словарный запас");
                }
            }
        }

        void DropSphere()
        {
            if (dropSphereState.IsDrop)
            {
                sp.Y -= 0.1;
                dropSphereState.I++;

                if (IfColision() || dropSphereState.I >= 20)
                {
                    dropSphereState.IsDrop = false;
                    dropSphereState.I = 0;
                    sp.Y = 0;
                }
            }
        }

        void DrowQuadrats()
        {
            foreach (Coorditates p in qp)
            {
                Gl.glLoadIdentity();
                Gl.glColor3f(1.0f, 0, 0);


                Gl.glPushMatrix();
                Gl.glTranslated(p.X, p.Y, p.Z);
                Gl.glRotated(-95, 1, 0, 0);
                Gl.glRotated(-47.5, 0, 0, 1);

                Glut.glutWireCylinder(p.R, p.Size, 4, 4);


                Gl.glPopMatrix();
                Gl.glFlush();
                AnT.Invalidate();
            }
        }

        void InitializeQuadrats()
        {
            qp.Clear();

            for (int i = 0; i < 9; i++)
            {
                qp.Add(new Coorditates() { X = -4 + i, Y = -2, Z = -6 });
            }
        }

        void RandomizeQuadratsHeight()
        {
            var r = new Random(4);
            foreach (Coorditates p in qp)
            {
                p.Size = r.Next() % 10 * 0.1;
            }
        }

        void DrowAll()
        {
            DrowSphere(sp);
            DrowQuadrats();
        }

        private void button1_KeyDown(object sender, KeyEventArgs e)
        {
            AnT_KeyDown_1(sender, e);
        }

        bool IfColision()
        {
            foreach (Coorditates c in qp)
            {
                if (sp.X - sp.R <= c.X - c.R * 0.05 && sp.X + sp.R >= c.X + c.R * 0.05 && sp.Y - sp.R <= c.Y + c.Size)
                {
                    c.Size = 0.1;
                    return true;
                }
            }

            return false;
        }

        private void button2_KeyDown(object sender, KeyEventArgs e)
        {
            AnT_KeyDown_1(sender, e);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!Pouse)
            {
                GrowQuadrats();
                DropSphere();
                DrowAll();
                Update();
                Thread.Sleep(100);
            }
        }
    }
}
