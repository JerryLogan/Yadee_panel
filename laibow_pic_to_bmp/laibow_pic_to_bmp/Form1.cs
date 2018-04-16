using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
namespace laibow_pic_to_bmp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Graphics g;     
        Bitmap bmp;
        string totxt ="";
        string hexvalue_str;
        int[,] greyarrayOrigin;//= new int[128, 128];
        int[,] greyarrayHalfOrigin;// = new int[128, 64];
        int temp_width;
        string fn = "";

        private void writetotxt(string address, string strHex)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(address, true))
            {
                
                file.WriteLine("const uint8_t "+fn+" [" + pictureBox2.Height + "][" + temp_width + "] = ");
                file.Write("{   ");
                file.WriteLine("// " + "size: (" + pictureBox2.Width + "*" + pictureBox2.Height + ")" );
                file.WriteLine();
                file.Write(strHex);
                file.WriteLine();
                file.WriteLine("};");
                file.WriteLine();
                file.WriteLine("//====");
                file.WriteLine();
            }
        }
        private void button1_Click(object sender, EventArgs e)//load image
        {
            label2.Text = "";
            totxt = null;
            label1.Text = "size";
            textBox2.Text = null;
            openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "open image";

            openFileDialog1.Filter = "Image Files(*.BMP;*.PNG;*.JPG;*.GIF)|*.BMP;*.PNG;*.JPG;*.GIF|All files (*.*)|*.*";
            try
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    //fn = openFileDialog1.FileName;
                    fn = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);
                    
                    pictureBox1.Image = new Bitmap(openFileDialog1.FileName);
                }
                bmp = (Bitmap)pictureBox1.Image;
                pictureBox1.Width = bmp.Width; //8
                pictureBox1.Height = bmp.Height; //16
                temp_width = (int)Math.Round(bmp.Width / 2.0);
                greyarrayOrigin = new int[bmp.Height, temp_width*2];

                greyarrayHalfOrigin = new int[bmp.Height, temp_width];
                label1.Text = "( " + pictureBox1.Width + " * " + pictureBox1.Height + " )";
                //MessageBox.Show(temp_width.ToString() + "," + (temp_width*2).ToString());
                for (int i = 0; i < pictureBox1.Height; i++)
                {
                    for (int j = 0; j < temp_width; j++)
                    {
                        greyarrayOrigin[i, j] = 0;
                    }
                }

            }
            catch (Exception ex) { }

        }
        int g8(int x)
        {
            double input;
            input = Math.Round(x / 17.0)*17;
            
            int result=(int)input;

            return (int)input;
        }
        string tobinary(int x)
        {
            string result ="";
            result = Convert.ToString(x, 2);
            return result;
        }
        private void button2_Click(object sender, EventArgs e) //grey16
        {
            try
            {
                Bitmap bm1 = (Bitmap)pictureBox1.Image;
               
                int avg1 = 0;

                //Console.WriteLine("(g8(avg1)/17).ToString(X) ");
                for (int y = 0; y < pictureBox1.Image.Height ; y++)
                {
                    for (int x = 0; x < pictureBox1.Image.Width ; x++)
                    {
                        Color c1 = bm1.GetPixel(x, y);

                       /*
                       /    avg1 rgb的數值
                       /    g8(avg1) 灰階度 0~F
                       /    tobinary(g8(avg1)) 0~1111;
                       */
                         avg1 = (c1.R + c1.G + c1.B) / 3;
                        bm1.SetPixel(x, y, Color.FromArgb(g8(avg1), g8(avg1), g8(avg1)));
                        greyarrayOrigin[y, x] = g8(avg1)/17;
                        Console.Write((g8(avg1)/17).ToString("X") + ",");
                    }
                    Console.WriteLine();
                }
                pictureBox2.Width = pictureBox1.Width;
                pictureBox2.Height = pictureBox1.Height;
                pictureBox2.Image = bm1;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            /////////////////////////////////////////////////////////////////////////////////////////////
           //兩格合併
            for (int i = 0; i < pictureBox1.Height; i++)//37
            {
                for (int j = 0; j < pictureBox1.Width; j+=2)//14
                {
                    //原本
                    // greyarrayHalfOrigin[i, j/2] = greyarrayOrigin[i, j] << 4 | greyarrayOrigin[i, j + 1];

                    //Console.Write(j+",");
                    
                    //左右交換
                    greyarrayHalfOrigin[i, j / 2] = greyarrayOrigin[i, j]  | greyarrayOrigin[i, j + 1]<<4;
                }
            }

            //Console.WriteLine(" 0x+ greyarray128x64[i, j].ToString(X)");
            for (int i = 0; i < pictureBox1.Height; i++)
            {
                for (int j = 0; j < temp_width ; j ++)
                {
                    //Console.Write("0x"+ greyarrayHalfOrigin[i, j].ToString("X")+",");
                    hexvalue_str = greyarrayHalfOrigin[i, j].ToString("X");
                    //textBox2.Text += "0x" + hexvalue_str + ",";
                    totxt += "0x" + hexvalue_str + ",";
                }
                //Console.WriteLine();
                //textBox2.Text += Environment.NewLine;
                totxt += Environment.NewLine; 
            }
            //write to txt
            writetotxt(@"C:\Users\gan\Desktop\LiaBow_1111.txt", totxt);
            label2.Text = "done";
            //open txt
            //System.Diagnostics.Process.Start(@"C:\Users\gan\Desktop\LiaBow_tmep.txt");
        }
    }
}
