using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.IO;

namespace Digital_Sinthesizer_1
{
    public partial class Form1 : Form
    {
        private const int SAMPLE_RATE = 44100;
        private const short BITS_PER_SAMPLE = 16;
        public static Dictionary<int, float> Notes = new Dictionary<int, float>();
        public static float frequency;
        public Form1()
        {
            InitializeComponent();
            Notes.Add(65, 130.81f);
            Notes.Add(87, 138.59f);
            Notes.Add(83, 146.83f);
            Notes.Add(69, 155.56f);
            Notes.Add(68, 164.81f);
            Notes.Add(70, 174.61f);
            Notes.Add(84, 185.00f);
            Notes.Add(71, 196.00f);
            Notes.Add(72, 220.00f);
            Notes.Add(89, 207.65f);
            Notes.Add(85, 233.08f);
            Notes.Add(74, 246.94f);
            Notes.Add(75, 261.63f);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            short[] wave = new short[SAMPLE_RATE];
            byte[] binaryWave = new byte[SAMPLE_RATE * sizeof(short)];

            if (Notes.ContainsKey(e.KeyValue)==true)
            {
                frequency = (float)Notes[e.KeyValue];
                for (int i = 0; i < SAMPLE_RATE; i++)
                {
                    wave[i] = Convert.ToInt16(short.MaxValue * Math.Sin(((Math.PI * 2 * frequency) / SAMPLE_RATE) * i));
                }
                Buffer.BlockCopy(wave, 0, binaryWave, 0, wave.Length * sizeof(short));
                using (MemoryStream memoryStream = new MemoryStream())
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
                {
                    short blockAlign = BITS_PER_SAMPLE / 8;
                    int subChunckTwoSize = SAMPLE_RATE * blockAlign;
                    binaryWriter.Write(new[] { 'R', 'I', 'F', 'F' });
                    binaryWriter.Write(36 + subChunckTwoSize);
                    binaryWriter.Write(new[] { 'W', 'A', 'V', 'E', 'f', 'm', 't', ' ' });
                    binaryWriter.Write(16);
                    binaryWriter.Write((short)1);
                    binaryWriter.Write((short)1);
                    binaryWriter.Write(SAMPLE_RATE);
                    binaryWriter.Write(SAMPLE_RATE * blockAlign);
                    binaryWriter.Write(blockAlign);
                    binaryWriter.Write(BITS_PER_SAMPLE);
                    binaryWriter.Write(new[] { 'd', 'a', 't', 'a' });
                    binaryWriter.Write(subChunckTwoSize);
                    binaryWriter.Write(binaryWave);
                    memoryStream.Position = 0;
                    new SoundPlayer(memoryStream).Play();
                }
            }
        }
    }

    public enum WaveForm
    {
        Sine, Square, Saw, Triangle, Noise
    }

}
