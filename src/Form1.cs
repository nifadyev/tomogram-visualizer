using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace TomogramVisualizer
{
    public partial class Form1 : Form
    {
        Bin bin;
        View view;
        bool loaded;
        bool needReload;
        int currentLayer;
        int frameCount;
        DateTime nextFPSUpdate = DateTime.Now.AddSeconds(1);

        public Form1()
        {
            InitializeComponent();
            bin = new Bin();
            view = new View();
            loaded = false;
            needReload = false;
            currentLayer = 0;
            bin.ReadBin("testbin.bin");
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string str = dialog.FileName;
                bin.ReadBin(str);
                view.SetupView(glControl1.Width, glControl1.Height);
                loaded = true;
                glControl1.Invalidate();
            }
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (loaded)
            {
                //view.DrawQuads(currentLayer);
                if (needReload)
                {
                    view.GenerateTextureImage(currentLayer);
                    view.Load2DTexture();
                    needReload = false;
                }
                view.DrawTexture();
                glControl1.SwapBuffers();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentLayer = trackBar1.Value;
            needReload = true;
        }

        void Application_Idle(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
            {
                displayFPS();
                glControl1.Invalidate();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Application.Idle += Application_Idle;
        }

        void displayFPS()
        {
            if (DateTime.Now >= nextFPSUpdate)
            {
                this.Text = String.Format("CT Visualizer (fps={0})", frameCount);
                nextFPSUpdate = DateTime.Now.AddSeconds(1);
                frameCount = 0;
            }
            frameCount++;
        }
    }
}
