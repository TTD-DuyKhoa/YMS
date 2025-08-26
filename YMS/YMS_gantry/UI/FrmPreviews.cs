using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YMS_gantry.UI;

namespace YMS_gantry
{
    public partial class FrmPreview : Form
    {
        private float widthCoef { get; set; }
        private float highthCoef { get; set; }

        //構台設定
        private float koudaiw = 0;
        private float koudaih = 0;

        private float defaultX = 90;
        private float defaultY = 30;
        private float PlateThick = 10;
        private float nedaHi = 15;
        private float oobikiThick = 15;

        private AllKoudaiFlatFrmData FlatData { get; set; }
        private float _currentThick { get; set; }

        public FrmPreview()
        {
            InitializeComponent();
            widthCoef = 1;
            highthCoef = 1;
        }

        public FrmPreview(AllKoudaiFlatFrmData Data)
        {
            FlatData = Data;
            InitializeComponent();
            widthCoef = 1;
            highthCoef = 1;

            koudaih = (float)FlatData.GetKoudaHeight();
            koudaiw = (float)FlatData.HukuinLength;

        }

        /// <summary>
        /// プレビュー更新
        /// </summary>
        /// <param name="upData"></param>
        public void UpdatePreview(AllKoudaiFlatFrmData upData)
        {
            FlatData = upData;
            widthCoef = 1;
            highthCoef = 1;

            koudaih = (float)FlatData.GetKoudaHeight();
            koudaiw = (float)FlatData.HukuinLength;

            //断面図作図
            DrawDanmenzu();

            //側面図作図
            DrawSokumenzu();
        }

        public FrmPreview(float width, float hight)
        {
            InitializeComponent();
        }

        /// <summary>
        /// 画面ロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            //断面図作図
            DrawDanmenzu();

            //側面図作図
            DrawSokumenzu();
        }

        #region 断面図
        /// <summary>
        /// 断面図作図
        /// </summary>
        private void DrawDanmenzu()
        {
            _currentThick = 0;

            //断面図サイズ確定
            Size pic1Size = this.pictureBox1.Size;
            double FukkoubanSum = FlatData.FukkoubanPitch.Sum();

            widthCoef = (pic1Size.Width - (defaultX * 2)) / (koudaiw);
            if (FukkoubanSum > koudaiw)
            {
                widthCoef = (pic1Size.Width - (defaultX * 2)) / (float)(FukkoubanSum);
            }
            highthCoef = (pic1Size.Height - (defaultY * 2)) / (koudaih);

            //ImageオブジェクトのGraphicsオブジェクトを作成する
            Bitmap canvas = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            _currentThick = defaultY;

            //覆工板
            drawPlateDanmen(canvas);
            //根太描画
            drawNedaDanmen(canvas);
            //大引描画
            drawOhbikiDanmen(canvas);
            //柱描画
            drawPillarDanmen(canvas);

            pictureBox1.Image = canvas;
        }

        /// <summary>
        /// 断面図覆工板作図
        /// </summary>
        /// <param name="canvas"></param>
        private void drawPlateDanmen(Bitmap canvas)
        {
            if(!FlatData.HasFukkouban) { _currentThick += defaultY + (PlateThick); return; }

            float currentWidth = 0;
            using (Graphics g = Graphics.FromImage(canvas))
            {

                for (int i = 0; i < FlatData.FukkoubanPitch.Count; i++)
                {
                    //覆工板作図
                    float pX = defaultX + currentWidth;
                    float pY = defaultY + _currentThick;

                    g.FillRectangle(Brushes.LightYellow, pX, pY, ((float)FlatData.FukkoubanPitch[i]) * widthCoef, PlateThick);
                    g.DrawRectangle(Pens.Black, pX, pY, ((float)FlatData.FukkoubanPitch[i]) * widthCoef, PlateThick);

                    //寸法線描画
                    drawDim(canvas, new DPoint(pX, pY), new DPoint(pX + (((float)FlatData.FukkoubanPitch[i]) * widthCoef), pY), ((float)FlatData.FukkoubanPitch[i]), 10, 5);
                    currentWidth += ((float)FlatData.FukkoubanPitch[i]) * widthCoef;
                }

                //基準線作図
                if (FlatData.BaseLevel == DefineUtil.eBaseLevel.FukkouTop)
                {
                    float lineY = defaultY + _currentThick + ((float)(FlatData.LevelOffset * highthCoef));
                    g.DrawLine(Pens.DarkRed,defaultX-45, lineY, defaultX+(koudaiw*widthCoef)+45, lineY);
                    Font fnt = new Font("MS UI Gothic", 10);
                    SizeF size = g.MeasureString(FlatData.SelectedLevel, fnt);
                    float txtHei = -size.Height;
                    float txtWid = -size.Width;
                   
                    //文字列を赤色で表示
                    g.DrawString("▽"+FlatData.SelectedLevel, fnt, Brushes.DarkRed, defaultX + (koudaiw * widthCoef) + 15, lineY+txtHei);

                    //差異があれば寸法地
                    //if (FlatData.LevelOffset!=0)
                    //{
                    //    float upY = defaultY + _currentThick+((FlatData.LevelOffset < 0) ? 10 : -10);
                    //    float downY = defaultY + _currentThick + ((FlatData.LevelOffset < 0) ? -10 : 10);
                    //    drawDim(canvas, new DPoint(defaultX, upY), new DPoint(defaultX,downY), FlatData.LevelOffset, 10, 2, false);
                    //}
                }
            }

            //前長寸法作図
            //寸法線描画
            drawDim(canvas, new DPoint(defaultX, defaultY + _currentThick), new DPoint(defaultX + currentWidth, defaultY + _currentThick), FlatData.FukkoubanPitch.Sum(), 22, 5);
            _currentThick += defaultY + (PlateThick);
        }

        /// <summary>
        ///  断面図大引作図
        /// </summary>
        /// <param name="canvas"></param>
        private void drawOhbikiDanmen(Bitmap canvas)
        {
            if(FlatData.ohbikiData.isHkou)
            {
                using (Graphics g = Graphics.FromImage(canvas))
                {
                    float mx = ((float)FlatData.ohbikiData.exOhbikiStartLeng * widthCoef) + ((koudaiw * widthCoef) + ((float)FlatData.ohbikiData.exOhbikiEndLeng * widthCoef));
                    g.FillRectangle(Brushes.DarkGray, defaultX - ((float)FlatData.ohbikiData.exOhbikiStartLeng * widthCoef), _currentThick, mx, oobikiThick);
                    g.DrawRectangle(Pens.Black, defaultX - ((float)FlatData.ohbikiData.exOhbikiStartLeng * widthCoef), _currentThick, mx, oobikiThick);

                    //基準線作図
                    if (FlatData.BaseLevel == DefineUtil.eBaseLevel.OhbikiBtm)
                    {
                        float lineY = _currentThick + oobikiThick + ((float)(FlatData.LevelOffset * highthCoef));
                        g.DrawLine(Pens.DarkRed, defaultX - 45, lineY, defaultX + (koudaiw * widthCoef) + 45, lineY);

                        Font fnt = new Font("MS UI Gothic", 10);
                        SizeF size = g.MeasureString(FlatData.SelectedLevel, fnt);
                        float txtHei = -size.Height;
                        float txtWid = -size.Width;

                        //文字列を青色で表示
                        g.DrawString("▽" + FlatData.SelectedLevel, fnt, Brushes.DarkRed, defaultX + (koudaiw * widthCoef) + 15, lineY + txtHei);
                    }
                }

                //大引突き出し長さ寸法
                drawDim(canvas, new DPoint(defaultX - ((float)FlatData.ohbikiData.exOhbikiStartLeng * widthCoef), _currentThick - oobikiThick)
                              , new DPoint(defaultX, _currentThick - oobikiThick), (float)FlatData.ohbikiData.exOhbikiStartLeng, oobikiThick + 30, 5);
                float Mazleng = ((float)FlatData.ohbikiData.exOhbikiEndLeng * widthCoef) + ((float)(FlatData.HukuinLength * widthCoef));
                drawDim(canvas, new DPoint(Mazleng - ((float)FlatData.ohbikiData.exOhbikiEndLeng * widthCoef) + defaultX, _currentThick - oobikiThick)
                              , new DPoint(Mazleng + defaultX, _currentThick - oobikiThick), (float)FlatData.ohbikiData.exOhbikiEndLeng, oobikiThick + 30, 5);
                //幅員方向長さ
                drawDim(canvas, new DPoint(defaultX, _currentThick - oobikiThick), new DPoint(defaultX + (float)FlatData.HukuinLength * widthCoef, _currentThick - oobikiThick), FlatData.HukuinLength, oobikiThick + 30, 5);

                _currentThick += oobikiThick;

            }
            
        }

        /// <summary>
        /// 断面図根太作図
        /// </summary>
        /// <param name="canvas"></param>
        private void drawNedaDanmen(Bitmap canvas)
        {
            using (Graphics g = Graphics.FromImage(canvas))
            {
                float currentWidth = 0;
                float pY = defaultY + _currentThick;
                for (int i = 0; i < FlatData.NedaPitch.Count; i++)
                {
                    float pX = defaultX + currentWidth;
                    currentWidth += ((float)FlatData.NedaPitch[i]) * widthCoef;

                    //根太描画
                    drawHkou(canvas, new Point((int)pX, (int)(pY + (PlateThick * highthCoef))), 10, (int)nedaHi, 2, Brushes.LightSalmon);
                }
                drawHkou(canvas, new Point((int)(defaultX + currentWidth), (int)(defaultY + _currentThick + PlateThick * highthCoef)), 10, (int)nedaHi, 2, Brushes.LightSalmon);
            }
            _currentThick += nedaHi;
        }

        /// <summary>
        /// 断面図柱作図
        /// </summary>
        /// <param name="canvas"></param>
        private void drawPillarDanmen(Bitmap canvas)
        {
            using (Graphics g = Graphics.FromImage(canvas))
            {
                float currentWidth = (float)FlatData.beginNedaDiff * widthCoef;
                for (int i = 0; i < FlatData.HukuinPillarPitch.Count; i++)
                {
                    if(i==0)
                    {
                        currentWidth += (float)FlatData.HukuinPillarPitch[0] * widthCoef;
                    }
                    float pX = defaultX + currentWidth-5;

                    //柱描画
                    g.FillRectangle(Brushes.LightBlue, pX, _currentThick, 10, 200);
                    g.DrawRectangle(Pens.Black, pX, _currentThick, 10, 200);
                    if (i == 0)
                    {
                        drawDim(canvas, new DPoint(pX, _currentThick), new DPoint(pX, _currentThick + 200), FlatData.pilePillerData.PillarLength, 15, 3, false);
                    }
                    
                    if(i!=FlatData.HukuinPillarPitch.Count-1)
                    {
                        float nextP = pX + (float)FlatData.HukuinPillarPitch[i + 1] * widthCoef;
                        drawDim(canvas, new DPoint(pX + 5, _currentThick + 200), new DPoint(nextP + 5, _currentThick + 200), (float)FlatData.HukuinPillarPitch[i+1], 10, 5, true, true);
                        currentWidth += (float)FlatData.HukuinPillarPitch[i + 1] * widthCoef;
                    }
                }

                if(!FlatData.ohbikiData.isHkou)
                {
                    float thick = _currentThick;
                    currentWidth = (float)(defaultX +((float)FlatData.beginNedaDiff * widthCoef));
                    for(int i=0;i < FlatData.ohbikiData.OhbikiDan; i++)
                    {
                        float mx = ((float)FlatData.ohbikiData.exOhbikiStartLeng * widthCoef) + ((koudaiw * widthCoef) + ((float)FlatData.ohbikiData.exOhbikiEndLeng * widthCoef));
                        g.FillRectangle(Brushes.DarkGray, currentWidth - ((float)FlatData.ohbikiData.exOhbikiStartLeng * widthCoef), thick+(i*15), mx,15);
                        g.DrawRectangle(Pens.Black, currentWidth - ((float)FlatData.ohbikiData.exOhbikiStartLeng * widthCoef), thick + (i * 15), mx, 15);
                        //thick += oobikiThick / 2;
                    }
                }
            }
            _currentThick += 15;
        }

      
        #endregion]

        #region 側面図
        /// <summary>
        /// 断面図作図
        /// </summary>
        private void DrawSokumenzu()
        {
            _currentThick = 0;
            koudaiw = (float)FlatData.KyoutyouLength;
            //断面図サイズ確定
            Size pic1Size = this.pictureBox2.Size;

            widthCoef = (pic1Size.Width - (defaultX * 2)) / ((float)koudaiw);
            highthCoef = (pic1Size.Height - (defaultY * 2)) / ((float)koudaih);

            //ImageオブジェクトのGraphicsオブジェクトを作成する
            Bitmap canvas = new Bitmap(this.pictureBox2.Width, this.pictureBox2.Height);
            _currentThick = defaultY;

            //覆工板
            drawPlateSokumen(canvas);
            //根太描画
            drawNedaSokumen(canvas);
            //大引描画
            drawOhbikiSokumen(canvas);
            //柱描画
            drawPillarSokumen(canvas);

            this.pictureBox2.Image = canvas;
        }

        /// <summary>
        /// 断面図覆工板作図
        /// </summary>
        /// <param name="canvas"></param>
        private void drawPlateSokumen(Bitmap canvas)
        {
            

            float currentWidth = 0;
            int hCount = (int)Math.Ceiling(FlatData.KyoutyouLength / 1000);
            using (Graphics g = Graphics.FromImage(canvas))
            {
                for (int i = 0; i < hCount; i++)
                {
                    if (FlatData.HasFukkouban)
                    {
                        //覆工板作図
                        float pX = defaultX + currentWidth;
                        float pY = defaultY + _currentThick;

                        g.FillRectangle(Brushes.LightYellow, pX, pY, (float)1000 * widthCoef, PlateThick);
                        g.DrawRectangle(Pens.Black, pX, pY, (float)1000 * widthCoef, PlateThick);

                    }

                    //寸法線描画
                    //drawDim(canvas, new DPoint(pX, pY), new DPoint(pX + ((float)pSize[i] * widthCoef), pY), pSize[i], 10, 5);
                    currentWidth += 1000 * widthCoef;
                }

                //基準線作図
                if (FlatData.BaseLevel == DefineUtil.eBaseLevel.FukkouTop)
                {
                    float lineY = defaultY + _currentThick + ((float)(FlatData.LevelOffset * highthCoef));
                    g.DrawLine(Pens.DarkRed, defaultX - 45, lineY, defaultX + (koudaiw * widthCoef) + 45, lineY);
                    Font fnt = new Font("MS UI Gothic", 10);
                    SizeF size = g.MeasureString(FlatData.SelectedLevel, fnt);
                    float txtHei = -size.Height;
                    float txtWid = -size.Width;

                    //文字列を青色で表示
                    g.DrawString("▽"+FlatData.SelectedLevel, fnt, Brushes.DarkRed, defaultX + (koudaiw * widthCoef) + 17, lineY + txtHei);

                    //差異があれば寸法地
                    //if (FlatData.LevelOffset != 0)
                    //{
                    //    float upY = (FlatData.LevelOffset < 0) ? defaultX : defaultX - 5;
                    //    float downY = (FlatData.LevelOffset < 0) ? defaultX - 5 : defaultX;
                    //    drawDim(canvas, new DPoint(defaultX, upY), new DPoint(defaultX, downY), FlatData.LevelOffset, 10, 2, false);
                    //}
                }
            }

            //根太突き出し長さ寸法
            drawDim(canvas, new DPoint(defaultX - ((float)FlatData.nedaData.exNedaStartLeng * widthCoef), defaultY + _currentThick)
                          , new DPoint(defaultX, defaultY + _currentThick), (float)FlatData.nedaData.exNedaStartLeng, 10, 5);
            drawDim(canvas, new DPoint(currentWidth + defaultX, defaultY + _currentThick)
                          , new DPoint(currentWidth + defaultX + ((float)FlatData.nedaData.exNedaEndLeng * widthCoef), defaultY + _currentThick), (float)FlatData.nedaData.exNedaEndLeng, 10, 5);


            //前長寸法作図
            //寸法線描画
            drawDim(canvas, new DPoint(defaultX, defaultY + _currentThick), new DPoint(defaultX + currentWidth, defaultY + _currentThick), FlatData.KyoutyouLength, 22, 5);
            _currentThick += defaultY + (PlateThick);
        }

        /// <summary>
        ///  断面図大引作図
        /// </summary>
        /// <param name="canvas"></param>
        private void drawOhbikiSokumen(Bitmap canvas)
        {
            if(FlatData.ohbikiData.isHkou)
            {
                using (Graphics g = Graphics.FromImage(canvas))
                {
                    float currentWidth = 0;
                    for (int i = 0; i < FlatData.KyoutyouPillarPitch.Count; i++)
                    {
                        currentWidth += (float)FlatData.KyoutyouPillarPitch[i] * widthCoef;
                        float pX = defaultX + currentWidth;
                        float pY = defaultY + _currentThick;

                        //根太描画
                        drawHkou(canvas, new Point((int)pX, (int)(pY + (PlateThick * highthCoef))), 10, (int)nedaHi, 2, Brushes.LightGray);
                    }
                    drawHkou(canvas, new Point((int)(defaultX + currentWidth), (int)(defaultY + _currentThick + PlateThick * highthCoef)), 10, (int)nedaHi, 2, Brushes.LightGray);

                    //基準線作図
                    if (FlatData.BaseLevel == DefineUtil.eBaseLevel.OhbikiBtm)
                    {
                        float lineY = _currentThick + oobikiThick + ((float)(FlatData.LevelOffset * highthCoef));
                        g.DrawLine(Pens.DarkRed, defaultX - 45, lineY, defaultX + (koudaiw * widthCoef) + 45, lineY);
                        Font fnt = new Font("MS UI Gothic", 10);
                        SizeF size = g.MeasureString(FlatData.SelectedLevel, fnt);
                        float txtHei = -size.Height;
                        float txtWid = -size.Width;

                        //文字列を青色で表示
                        g.DrawString("▽" + FlatData.SelectedLevel, fnt, Brushes.DarkRed, defaultX + (koudaiw * widthCoef) + 17, lineY + txtHei);

                        //差異があれば寸法地
                        //if (FlatData.LevelOffset != 0)
                        //{
                        //    float upY = (FlatData.LevelOffset < 0) ? defaultX : defaultX - 5;
                        //    float downY = (FlatData.LevelOffset < 0) ? defaultX - 5 : defaultX;
                        //    drawDim(canvas, new DPoint(defaultX, upY), new DPoint(defaultX, downY), FlatData.LevelOffset, 10, 2, false);
                        //}
                    }
                }
                _currentThick += nedaHi;
            }
        }

        /// <summary>
        /// 断面図根太作図
        /// </summary>
        /// <param name="canvas"></param>
        private void drawNedaSokumen(Bitmap canvas)
        {
            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.FillRectangle(Brushes.LightSalmon, defaultX - ((float)FlatData.nedaData.exNedaStartLeng * widthCoef), _currentThick, (koudaiw + (float)FlatData.nedaData.exNedaStartLeng+ (float)FlatData.nedaData.exNedaEndLeng) * widthCoef, oobikiThick);
                g.DrawRectangle(Pens.Black, defaultX - ((float)FlatData.nedaData.exNedaStartLeng * widthCoef), _currentThick, (koudaiw + (float)FlatData.nedaData.exNedaStartLeng+ (float)FlatData.nedaData.exNedaEndLeng) * widthCoef, oobikiThick);
            }
            _currentThick += oobikiThick;

        }

        /// <summary>
        /// 断面図柱作図
        /// </summary>
        /// <param name="canvas"></param>
        private void drawPillarSokumen(Bitmap canvas)
        {
            using (Graphics g = Graphics.FromImage(canvas))
            {
                float currentWidth = 0;
                MaterialSize ms = GantryUtil.GetKouzaiSize(FlatData.ohbikiData.OhbikiSize);

                for (int i = 0; i < FlatData.KyoutyouPillarPitch.Count; i++)
                {
                    if (i == 0)
                    {
                        currentWidth += (float)FlatData.KyoutyouPillarPitch[0] * widthCoef;
                    }
                    float pX = defaultX + currentWidth - 5; 

                    if((i==0&&FlatData.IsFirstShikigeta)||(i==FlatData.KyoutyouPillarPitch.Count-1&&FlatData.IsLastShikigeta))
                    {
                        //敷桁位置に柱不要
                    }
                    else
                    {
                        //柱描画
                        g.FillRectangle(Brushes.LightBlue, pX, _currentThick, 10, 200);
                        g.DrawRectangle(Pens.Black, pX, _currentThick, 10, 200);
                        if (!FlatData.ohbikiData.isHkou)
                        {
                            float thick = _currentThick;
                            for (int j = 0; j < FlatData.ohbikiData.OhbikiDan; j++)
                            {
                                thick = _currentThick + (15 * j);
                                if (ms.Shape == MaterialShape.L)
                                {
                                    drawAngle(canvas, new Point((int)pX, (int)thick), 15, 15, 3, Brushes.LightGray, false);
                                    drawAngle(canvas, new Point((int)pX + 10, (int)thick), 15, 15, 3, Brushes.LightGray, true);
                                }
                                else
                                {
                                    drawCannel(canvas, new Point((int)pX, (int)thick), 9, 15, 3, Brushes.LightGray, false);
                                    drawCannel(canvas, new Point((int)pX + 10, (int)thick), 9, 15, 3, Brushes.LightGray, true);
                                }
                            }
                        }

                    }


                    if (i == 0)
                    {
                        drawDim(canvas, new DPoint(pX, _currentThick), new DPoint(pX, _currentThick + 200), FlatData.pilePillerData.PillarLength, 15, 3, false);
                    }
                    if(i!= FlatData.KyoutyouPillarPitch.Count-1)
                    {
                        float nextP = pX + (float)FlatData.KyoutyouPillarPitch[i + 1] * widthCoef;
                        drawDim(canvas, new DPoint(pX + 5, _currentThick + 200), new DPoint(nextP + 5, _currentThick + 200), (float)FlatData.KyoutyouPillarPitch[i+1], 10, 5, true, true);
                        currentWidth += (float)FlatData.KyoutyouPillarPitch[i + 1] * widthCoef;
                    }
                }
            }
            _currentThick += 15;
        }
        #endregion

        /// <summary>
        /// H鋼作図 (描画ポイントはフランジ上部中央)
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="drawPoint"></param>
        /// <param name="width"></param>
        /// <param name="hight"></param>
        /// <param name="thick"></param>
        private void drawHkou(Bitmap canvas, Point drawPoint, int width, int hight, int thick, Brush brush)
        {
            using (Graphics g = Graphics.FromImage(canvas))
            {
                PointF[] ps = {
                 new Point(drawPoint.X-(width/2), (int)_currentThick),                //1
                 new Point(drawPoint.X+(width/2), (int)_currentThick),                //2
                 new Point(drawPoint.X+(width/2), (int)_currentThick+thick),          //3
                 new Point(drawPoint.X+(thick/2), (int)_currentThick+thick),          //4
                 new Point(drawPoint.X+(thick/2), (int)_currentThick+(hight)-thick),  //5
                 new Point(drawPoint.X+(width/2), (int)_currentThick+(hight)-thick),  //6
                 new Point(drawPoint.X+(width/2), (int)_currentThick+(hight)),        //7
                 new Point(drawPoint.X-(width/2), (int)_currentThick+(hight)),        //8
                 new Point(drawPoint.X-(width/2), (int)_currentThick+(hight)-thick),  //9
                 new Point(drawPoint.X-(thick/2), (int)_currentThick+(hight)-thick),  //10
                 new Point(drawPoint.X-(thick/2), (int)_currentThick+thick),          //11
                 new Point(drawPoint.X - (width / 2), (int)_currentThick + thick)     //12
                };

                g.FillPolygon(brush, ps);
                g.DrawPolygon(Pens.Black, ps);
            }
        }

        private void drawCannel(Bitmap canvas, Point drawPoint, int width, int hight, int thick, Brush brush,bool isRight)
        {
            using (Graphics g = Graphics.FromImage(canvas))
            {
              
                if(!isRight)
                {
                    PointF[] ps ={
                 drawPoint,                                                                   //1
                 new Point(drawPoint.X - (width),        drawPoint.Y),                        //2
                 new Point(drawPoint.X - (width),         (int)drawPoint.Y + thick),                //3
                 new Point(drawPoint.X - (thick),   (int)drawPoint.Y + thick),                //4
                 new Point(drawPoint.X - (thick),   (int)drawPoint.Y + (hight)-(thick)),    //5
                 new Point(drawPoint.X - (width),   (int)drawPoint.Y + (hight)-(thick)),    //6
                 new Point(drawPoint.X - (width),   (int)drawPoint.Y + (hight)),      //7
                 new Point(drawPoint.X, (int)drawPoint.Y + (hight))                           //8
                };
                    g.FillPolygon(brush, ps);
                    g.DrawPolygon(Pens.Black, ps);
                }
                else
                {
                    PointF[] ps ={
                 drawPoint,                                                                   //1
                 new Point(drawPoint.X + (width),        drawPoint.Y),                        //2
                 new Point(drawPoint.X + (width),         (int)drawPoint.Y + thick),                //3
                 new Point(drawPoint.X + (thick),   (int)drawPoint.Y + thick),                //4
                 new Point(drawPoint.X + (thick),   (int)drawPoint.Y + (hight)-(thick)),    //5
                 new Point(drawPoint.X + (width),   (int)drawPoint.Y + (hight)-(thick)),    //6
                 new Point(drawPoint.X + (width),   (int)drawPoint.Y + (hight)),      //7
                 new Point(drawPoint.X, (int)drawPoint.Y + (hight))                           //8
                };
                    g.FillPolygon(brush, ps);
                    g.DrawPolygon(Pens.Black, ps);

                }
            }
        }

        private void drawAngle (Bitmap canvas, Point drawPoint, int width, int hight, int thick, Brush brush,bool isRight)
        {
            using (Graphics g = Graphics.FromImage(canvas))
            {

                if (!isRight)
                {
                    PointF[] ps ={
                 drawPoint,                                                                   //1
                 new Point(drawPoint.X - (width),              drawPoint.Y),                  //2
                 new Point(drawPoint.X - (width),         (int)drawPoint.Y + thick),          //3
                 new Point(drawPoint.X - (thick),   (int)drawPoint.Y + (thick)),          //4
                 new Point(drawPoint.X - (thick),   (int)drawPoint.Y + (hight)),        //5
                 new Point(drawPoint.X ,   (int)drawPoint.Y + (hight))                        //6
                };
                    g.FillPolygon(brush, ps);
                    g.DrawPolygon(Pens.Black, ps);
                }
                else
                {
                    PointF[] ps ={
                 drawPoint,                                                                   //1
                 new Point(drawPoint.X + (width),              drawPoint.Y),                  //2
                 new Point(drawPoint.X + (width),         (int)drawPoint.Y + thick),          //3
                 new Point(drawPoint.X + (thick),   (int)drawPoint.Y + (thick)),          //4
                 new Point(drawPoint.X + (thick),   (int)drawPoint.Y + (hight)),        //5
                 new Point(drawPoint.X ,   (int)drawPoint.Y + (hight))                        //6
                };
                    g.FillPolygon(brush, ps);
                    g.DrawPolygon(Pens.Black, ps);
                }
            }
        }

        /// <summary>
        /// 寸法線描画
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <param name="dist"></param>
        private void drawDim(Bitmap canvas, DPoint start, DPoint end, double value, float dist, float margin, bool ishol = true, bool reverse = false,DPoint adjustText=null)
        {
            if(start.x==end.x&&start.y==end.y)
            {
                return;
            }

            using (Graphics g = Graphics.FromImage(canvas))
            {
                float m = (reverse) ? margin : -margin;
                float d = (reverse) ? dist : -dist;
                float dimDif = (reverse) ? -3 : 3;
                float stDif = 0;

                if (ishol)
                {
                    float baseY = (!reverse) ? (start.y < end.y) ? start.y : end.y : (start.y > end.y) ? start.y : end.y;

                    //補助線
                    g.DrawLine(Pens.Black, start.x, baseY + d + m, start.x, baseY + m);
                    g.DrawLine(Pens.Black, end.x, baseY + d + m, end.x, baseY + m);

                    //寸法線
                    g.DrawLine(Pens.Black, start.x, baseY + d + m + dimDif, end.x, baseY + d + m + dimDif);

                    //寸法値
                    Font fnt = new Font("MS UI Gothic", 10);
                    SizeF size = g.MeasureString(value.ToString(), fnt);
                    float txtHei = (reverse) ? size.Height : -size.Height;
                    float txtWid = -size.Width;
                    if (adjustText != null)
                    {
                        txtHei += adjustText.y;
                        txtWid += adjustText.x;
                    }
                    stDif = (reverse) ? d + m + (txtHei / 2) - 1 : ((d + m + (txtHei / 2)) - 1);
                    //文字列を青色で表示
                    g.DrawString(value.ToString(), fnt, Brushes.Blue, Math.Abs((start.x + end.x) / 2) + txtWid / 2, baseY + stDif);
                }
                else
                {
                    float baseX = (!reverse) ? (start.x < end.x) ? start.x : end.x : (start.x > end.x) ? start.x : end.x;

                    //補助線
                    g.DrawLine(Pens.Black, baseX + d + m, start.y, baseX + m, start.y);
                    g.DrawLine(Pens.Black, baseX + d + m, end.y, baseX + m, end.y);

                    //寸法線
                    g.DrawLine(Pens.Black, baseX + d + m + dimDif, start.y, baseX + d + m + dimDif, end.y);

                    //寸法地
                    Font fnt = new Font("MS UI Gothic", 10);
                    SizeF size = g.MeasureString(value.ToString(), fnt);
                    float txtWid = (reverse) ? size.Width : -size.Width;
                    if (adjustText != null)
                    {
                        txtWid += adjustText.x;
                    }
                    //文字列を位置(0,0)、青色で表示
                    g.DrawString(value.ToString(), fnt, Brushes.Blue, baseX + d + m + txtWid - stDif, Math.Abs((start.y + end.y) / 2) + stDif);
                }

            }
        }

        /// <summary>
        /// OKボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.Owner == null) { return; }

            FrmAllPutKoudaiFlat parent = (FrmAllPutKoudaiFlat)this.Owner;
            parent.PrevireUpdata();
        }
    }

    public class DPoint
    {
        public float x { get; set; }
        public float y { get; set; }

        public DPoint(float X = 0, float Y = 0)
        {
            x = X;
            y = Y;
        }
    }


}
