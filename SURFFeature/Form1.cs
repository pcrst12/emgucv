using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using Emgu.CV.GPU;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using Emgu.CV.VideoSurveillance;
using System.Timers;
namespace SURFFeatureExample
{
    public partial class Form1 : Form
    {
        private Capture cap;
        private IBGFGDetector<Bgr> _forgroundDetector;
        string fileNameText = "lg4.png";
        long matchTime;
        private static System.Timers.Timer aTimer;
        public Form1()
        {
            InitializeComponent();
            aTimer = new System.Timers.Timer(100);//2초
             aTimer.Elapsed += ProcessFrame2;
            aTimer.Enabled = true;
            //--------------------------------------------------------------------캠
            if (cap == null)
            {
                try
                {
                    cap = new Capture();
                }
                catch (NullReferenceException excpt)
                {   //show errors if there is any
                    MessageBox.Show(excpt.Message);
                }
            }
            if (cap != null) //if camera capture has been successfully created
            {
                cap.ImageGrabbed += ProcessFrame;
                //cap.ImageGrabbed += ProcessFrame2;
                cap.Start();

            }
        }

        private void 파일ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void ProcessFrame(object sender, EventArgs e)//캠 함수
        {
            try
            {
                using (Image<Bgr, Byte> img2 = cap.RetrieveBgrFrame())
                using (MemStorage storage = new MemStorage()) //create storage for motion components
                {
                    if (_forgroundDetector == null)
                    {
                        _forgroundDetector = new FGDetector<Bgr>(Emgu.CV.CvEnum.FORGROUND_DETECTOR_TYPE.FGD);
                        _forgroundDetector = new BGStatModel<Bgr>(img2, Emgu.CV.CvEnum.BG_STAT_TYPE.FGD_STAT_MODEL);
                    }
                    _forgroundDetector.Update(img2);
                    imageBox1.Image = img2;
                    storage.Clear();
                }
            }
            catch
            {
            }
        }
        private void ProcessFrame2(object sender, EventArgs e)
        {
            Image<Bgr, Byte> frame = cap.RetrieveBgrFrame();
            using (Image<Gray, Byte> modelImage = new Image<Gray, byte>(fileNameText))
            using (Image<Gray, Byte> observedImage = frame.Convert<Gray, Byte>())
            {
                Image<Bgr, byte> result = DrawMatches.Draw(modelImage, observedImage, out matchTime);
                imageBox3.Image = result;
            }
        }
        private void 이미지열기ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK || result == DialogResult.Yes)
            {
                fileNameText = openFileDialog1.FileName;
                Image<Bgr, Byte> img2 = new Image<Bgr, byte>(fileNameText);
                imageBox2.Image = img2; //최종 이미지 표기
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Image<Bgr, Byte> frame = cap.RetrieveBgrFrame();
            using (Image<Gray, Byte> modelImage = new Image<Gray, byte>(fileNameText))
            using (Image<Gray, Byte> observedImage = frame.Convert<Gray, Byte>())
            {
                Image<Bgr, byte> result = DrawMatches.Draw(modelImage, observedImage, out matchTime);
                imageBox3.Image = result;
            }
        }
    }
}
