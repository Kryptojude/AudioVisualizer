using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Media;

namespace AudioVisualizer
{
    public partial class Form1 : Form
    {
        Rectangle rec = new Rectangle(0, 0, 50, 15);
        FileStream fStream;
        SoundPlayer player = new SoundPlayer("audio.wav");
        int bytesPerSecond = 176822;
        int bytesPerTick;
        int tick = 100;
        Action refresh;

        public Form1()
        {
            InitializeComponent();
            refresh = new Action(() => { Refresh(); });

            fStream = new FileStream("audio.wav", FileMode.Open);
            fStream.Seek(44, SeekOrigin.Begin);

            bytesPerTick = bytesPerSecond / (1000 / tick);

            Thread thread = new Thread(Loop);
            thread.Start();
        }

        private void Loop()
        { 
            player.Play();
            Stopwatch watch = new Stopwatch();
            watch.Start();

            while (true)
            {
                if(watch.ElapsedMilliseconds >= tick)
                {
                    watch.Restart();
                    byte[] bytes = new byte[bytesPerTick];
                    fStream.Read(bytes, 0, bytesPerTick);
                    fStream.Seek(bytesPerTick, SeekOrigin.Current);
                    //Make average of bytes
                    int average = 0;
                    foreach (byte Byte in bytes)
                        average += Byte;
                    average /= bytes.Length;
                    Debug.WriteLine(average);
                    rec.Y = 250 - average;
                    rec.Height = 250 - rec.Y;

                    Invoke(refresh);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Black, rec);
        }
    }
}
