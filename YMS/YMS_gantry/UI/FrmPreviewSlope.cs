using System ;
using System.Collections.Generic ;
using System.ComponentModel ;
using System.Data ;
using System.Drawing ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;

namespace YMS_gantry.UI
{
  public partial class FrmPreviewSlope : Form
  {
    private float widthCoef { get ; set ; }

    private float highthCoef { get ; set ; }

    //構台設定
    private float koudaiw = 0 ;
    private float koudaih = 0 ;

    private float defaultX = 90 ;
    private float defaultY = 30 ;
    private float nedaThick = 30 ;

    AllKoudaiFlatFrmData FlatData { get ; set ; }
    SlopeDataForPreview SlopeData { get ; set ; }

    List<DrawSpanSet> DrawData { get ; set ; }

    List<float> spanLeng { get ; set ; }
    List<double> spanHeight { get ; set ; }

    public float topH { get ; set ; }
    public float btmH { get ; set ; }

    public float StartY { get ; set ; }

    public FrmPreviewSlope()
    {
      InitializeComponent() ;
    }

    public FrmPreviewSlope( AllKoudaiFlatFrmData data, List<double> height, SlopeDataForPreview sData )
    {
      InitializeComponent() ;
      this.Text = "スローププレビュー" ;
      this.ShowIcon = false ;
      FlatData = data ;
      SlopeData = sData ;
      spanHeight = height ;
      DrawData = new List<DrawSpanSet>() ;

      spanLeng = new List<float>() ;
      foreach ( double l in FlatData.KyoutyouPillarPitch ) {
        spanLeng.Add( (float) l ) ;
      }

      koudaiw = (float) FlatData.KyoutyouLength ;
      koudaih = GetTotalHeight() ;
    }

    private void FrmPreviewSlope_Load( object sender, EventArgs e )
    {
      DrawPreview() ;
    }

    public void UpdatePreview( AllKoudaiFlatFrmData data, List<double> height, SlopeDataForPreview sData )
    {
      FlatData = data ;
      SlopeData = sData ;
      spanHeight = height ;
      DrawData = new List<DrawSpanSet>() ;

      spanLeng = new List<float>() ;
      foreach ( double l in FlatData.KyoutyouPillarPitch ) {
        spanLeng.Add( (float) l ) ;
      }

      koudaiw = (float) FlatData.KyoutyouLength ;
      koudaih = GetTotalHeight() ;
      DrawPreview() ;
    }

    public void DrawPreview()
    {
      //断面図サイズ確定
      Size pic1Size = this.pictureBox2.Size ;

      widthCoef = ( pic1Size.Width - ( defaultX * 2 ) ) / ( (float) koudaiw ) ;
      highthCoef = ( pic1Size.Height - ( defaultY * 2 ) ) / ( (float) koudaih ) ;
      if ( koudaih <= 0 ) {
        highthCoef = ( pic1Size.Height - ( defaultY * 2 ) ) / ( 700 ) ;
      }

      CalcDrawSlopeDataSet() ;

      //ImageオブジェクトのGraphicsオブジェクトを作成する
      Bitmap canvas = new Bitmap( this.pictureBox2.Width, this.pictureBox2.Height ) ;
      Draw( canvas ) ;
      this.pictureBox2.Image = canvas ;
    }

    private void DrawRect( Bitmap canvas, Point origin, Point[] vertex, float angle, Pen p = null )
    {
      Pen pen = ( p == null ) ? Pens.Black : p ;
      using ( Graphics g = Graphics.FromImage( canvas ) ) {
        if ( vertex.Length > 0 ) {
          //描画原点を移動
          g.TranslateTransform( origin.X, origin.Y ) ;
          //角度がある場合は回転
          g.RotateTransform( angle ) ;
          //多角形を描画する                    
          g.DrawPolygon( pen, vertex ) ;
          g.RotateTransform( -angle ) ;
          g.TranslateTransform( -origin.X, -origin.Y ) ;
        }
      }
    }

    private void DrawLine( Bitmap canvas, Point origin, Point[] vertex, float angle, Pen p = null )
    {
      Pen pen = ( p == null ) ? Pens.Black : p ;
      using ( Graphics g = Graphics.FromImage( canvas ) ) {
        if ( vertex.Length == 2 ) {
          //描画原点を移動
          g.TranslateTransform( origin.X, origin.Y ) ;
          //角度がある場合は回転
          g.RotateTransform( angle ) ;
          //直線を描画する                    
          g.DrawLine( pen, vertex[ 0 ], vertex[ 1 ] ) ;
          g.RotateTransform( -angle ) ;
          g.TranslateTransform( -origin.X, -origin.Y ) ;
        }
      }
    }

    private void Draw( Bitmap canvas )
    {
      float defaultY = 30 ;
      float defaultX = 80 ;
      float avg = ( topH - btmH ) / 2 ;
      float mid = topH - avg ;
      float dif = StartY - avg ;
      float firstY = defaultY + avg * highthCoef /*+(dif*highthCoef)*/ ;
      if ( btmH == StartY ) {
        //firstY = defaultY + ((avg+mid) * highthCoef);
      }

      if ( SlopeData.allNoriire ) {
        firstY = 15 + ( ( SlopeData.allNoriireFromOrign ) ? Math.Abs( topH - btmH ) * highthCoef : 0 ) ;
      }

      if ( spanHeight.Sum() == 0 && ! SlopeData.allNoriire ) {
        firstY = (float) ( pictureBox2.Size.Height / 2 ) ;
      }

      float firstX = defaultX ;
      Point pnt = new Point( (int) firstX, (int) firstY ) ;
      float currentW = defaultX ;
      float currentY = firstY ;

      for ( int i = 0 ; i < DrawData.Count ; i++ ) {
        DrawSpanSet data = DrawData[ i ] ;
        int nedaThic = (int) ( -nedaThick * highthCoef ) ;
        if ( Math.Abs( nedaThic ) == 0 ) {
          nedaThic = -11 ;
        }

        //List<Point> ps = new List<Point>() {
        //    new Point(),
        //    new Point(0, nedaThic),
        //    new Point((int)(data.SpnarealLeng), nedaThic),
        //    new Point((int)(data.SpnarealLeng), 0)
        //    };
        List<Point> ps = new List<Point>() { new Point(), new Point( (int) ( data.SpnarealLeng ), 0 ) } ;

        //DrawRect(canvas, pnt, ps.ToArray(), data.Angle, Pens.Orange);
        Pen p = new Pen( Color.Orange, 8 ) ;
        DrawLine( canvas, pnt, ps.ToArray(), data.Angle, p ) ;
        //水平部分には高さを表示
        if ( data.slopeType == SlopeType.none ) {
          using ( Graphics g = Graphics.FromImage( canvas ) ) {
            int baseX = (int) ( pnt.X + ( ( data.SpanLength * widthCoef ) / 2 ) ) ;
            //寸法地
            Font fnt = new Font( "MS UI Gothic", 10 ) ;
            SizeF size = g.MeasureString( data.SpanName.ToString(), fnt ) ;
            float txtWid = size.Width ;
            //文字列を位置(0,0)、青色で表示
            g.DrawString( data.StValHeight, fnt, Brushes.Black, baseX - txtWid / 2, pnt.Y - 20 ) ;
          }
        }

        currentW += data.SpanLength * widthCoef ;
        currentY = currentY - ( data.slopeType == SlopeType.none ? 0 : ( data.SpanHeight ) ) ;
        //スパン表示
        int btmY = (int) ( defaultY + (int) ( Math.Abs( btmH * highthCoef ) + 10 ) ) ;
        Point pntNex = new Point( (int) ( currentW ), (int) ( currentY ) ) ;
        Point b1 = new Point( pnt.X, btmY ) ;
        Point b2 = new Point( pntNex.X, btmY ) ;
        DrawSpan( canvas, pnt, pntNex, b1, b2, data.SpanName ) ;

        pnt = new Point( (int) ( currentW ), (int) ( currentY ) ) ;
        ;
      }
    }

    private void DrawSpan( Bitmap canvas, Point u1, Point u2, Point b1, Point b2, string spanName )
    {
      using ( Graphics g = Graphics.FromImage( canvas ) ) {
        Pen bPen = new Pen( Color.Black ) ;
        bPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot ;
        g.DrawLine( bPen, u1, b1 ) ;
        g.DrawLine( bPen, u2, b2 ) ;
        g.DrawLine( Pens.Black, b1, b2 ) ;
        int bX = Math.Abs( b2.X - b1.X ) / 2 ;
        //寸法地
        Font fnt = new Font( "MS UI Gothic", 10 ) ;
        SizeF size = g.MeasureString( spanName.ToString(), fnt ) ;
        float txtWid = size.Width ;
        //文字列を位置(0,0)、青色で表示
        g.DrawString( spanName, fnt, Brushes.Black, b1.X + bX - txtWid / 2, b2.Y + 6 ) ;
      }
    }

    private void CalcDrawSlopeDataSet()
    {
      if ( SlopeData.allNoriire ) {
        //スパンをすべてみる
        //始点側から全乗り入れ
        if ( SlopeData.allNoriireFromOrign ) {
          for ( int i = 0 ; i < SlopeData.Data.Count ; i++ ) {
            DrawSpanSet sSet = new DrawSpanSet() ;
            (SlopeType type, bool isEnd, double percent, double height) cData = SlopeData.Data[ i ] ;
            float length = spanLeng[ i + 1 ] ;
            float angle = -(float) SlopeData.Data.First().percent ;
            float h = (float) ( ( length * widthCoef ) * Math.Tan( RevitUtil.ClsGeo.Deg2Rad( angle ) ) ) * -1 ;

            sSet.slopeType = SlopeType.noriire ;
            sSet.SpanHeight = h ;
            sSet.SpanLength = length ;
            sSet.SpnarealLeng = (float) ( Math.Sqrt( ( length * widthCoef ) * ( length * widthCoef ) + h * h ) ) ;
            sSet.Angle = angle ;
            sSet.SpanName = $"スパン{i + 1}" ;
            DrawData.Add( sSet ) ;
          }
        }
        //終点側から全乗り入れ
        else {
          for ( int i = 0 ; i < SlopeData.Data.Count ; i++ ) {
            DrawSpanSet sSet = new DrawSpanSet() ;
            (SlopeType type, bool isEnd, double percent, double height) cData = SlopeData.Data[ i ] ;
            float length = spanLeng[ i + 1 ] ;
            float angle = (float) SlopeData.Data.Last().percent ;
            float h = (float) ( ( length * widthCoef ) * Math.Tan( RevitUtil.ClsGeo.Deg2Rad( angle ) ) ) * -1 ;

            sSet.slopeType = SlopeType.noriire ;
            sSet.SpanHeight = h ;
            sSet.SpnarealLeng = (float) ( Math.Sqrt( ( length * widthCoef ) * ( length * widthCoef ) + h * h ) ) ;
            sSet.SpanLength = length ;
            sSet.Angle = angle ;
            sSet.SpanName = $"スパン{i + 1}" ;
            DrawData.Add( sSet ) ;
          }
        }
      }
      else {
        //スパンをすべてみる
        float currentFlat = 0 ;
        bool isfEnt = false ;
        for ( int i = 0 ; i < SlopeData.Data.Count ; i++ ) {
          DrawSpanSet sSet = new DrawSpanSet() ;
          (SlopeType type, bool isEnd, double percent, double height) cData = SlopeData.Data[ i ] ;
          float length = spanLeng[ i + 1 ] ;
          float angle = (float) cData.percent ;
          float h = 0 ;
          if ( cData.type == SlopeType.none ) {
            h = (float) ( cData.height * highthCoef ) ;
            currentFlat = /*(currentFlat > 0 && currentFlat != (float)cData.height) ? */
              (float) cData.height /*: currentFlat*/ ;
            isfEnt = false ;
            angle = 0 ;
          }
          else if ( cData.type == SlopeType.slope ) {
            angle = getNextFlatAngle( i, currentFlat ) ;
            isfEnt = false ;
            h = (float) ( ( length * widthCoef ) * Math.Tan( RevitUtil.ClsGeo.Deg2Rad( angle ) ) ) * -1 ;
          }
          else if ( cData.type == SlopeType.noriire ) {
            isfEnt = ( i == 0 || isfEnt ) ? true : false ;
            angle = (float) ( ( isfEnt ) ? -SlopeData.Data.First().percent : SlopeData.Data.Last().percent ) ;
            h = (float) ( ( length * widthCoef ) * Math.Tan( RevitUtil.ClsGeo.Deg2Rad( angle ) ) ) * -1 ;
          }

          sSet.slopeType = cData.type ;
          sSet.SpanHeight = h ;
          sSet.SpanLength = length ;
          sSet.SpnarealLeng =
            (float) ( Math.Sqrt(
              ( length * widthCoef ) * ( length * widthCoef ) /* +(h*highthCoef) * (h*highthCoef)*/ ) ) ;
          if ( sSet.slopeType.Equals( SlopeType.noriire ) ) {
            sSet.SpnarealLeng = (float) ( Math.Sqrt( ( length * widthCoef ) * ( length * widthCoef ) + ( h * h ) ) ) ;
          }

          sSet.Angle = angle ;
          sSet.SpanName = $"スパン{i + 1}" ;
          sSet.StValHeight = $"高さ：{currentFlat}mm" ;
          DrawData.Add( sSet ) ;
        }
      }
    }

    private float GetTotalHeight()
    {
      float cunnretH = 0 ;
      float currentFlat = 0 ;
      bool isfEnt = false ;

      if ( SlopeData.allNoriire ) {
        if ( SlopeData.allNoriireFromOrign ) {
          double h = koudaiw * Math.Tan( SlopeData.Data.First().percent ) ;
          topH = 0 ;
          btmH = (float) -h ;
          StartY = btmH ;
        }
        else {
          double h = koudaiw * Math.Tan( SlopeData.Data.First().percent ) ;
          topH = 0 ;
          btmH = (float) -h ;
          StartY = 0 ;
        }
      }
      else {
        for ( int i = 0 ; i < SlopeData.Data.Count ; i++ ) {
          DrawSpanSet sSet = new DrawSpanSet() ;
          (SlopeType type, bool isEnd, double percent, double height) cData = SlopeData.Data[ i ] ;
          float length = spanLeng[ i + 1 ] ;
          float angle = (float) cData.percent ;
          if ( cData.type == SlopeType.none ) {
            currentFlat = (float) cData.height ;
            isfEnt = false ;
            angle = 0 ;
            cunnretH = ( ! RevitUtil.ClsGeo.GEO_EQ0( cunnretH ) ) ? cunnretH : 0 ;
          }
          else if ( cData.type == SlopeType.slope ) {
            angle = getNextFlatAngle( i, currentFlat ) ;
            isfEnt = false ;
            double height = Math.Abs( length * Math.Tan( angle ) ) ;
            if ( angle < 0 ) {
              cunnretH = (float) ( cunnretH - height ) ;
            }
            else {
              cunnretH = (float) ( cunnretH + height ) ;
            }
          }
          else if ( cData.type == SlopeType.noriire ) {
            isfEnt = ( i == 0 || isfEnt ) ? true : false ;
            angle = (float) ( ( isfEnt ) ? -SlopeData.Data.First().percent : SlopeData.Data.Last().percent ) ;
            double height = Math.Abs( length * Math.Tan( angle ) ) ;
            if ( i == 0 ) {
              btmH = (float) -height ;
              cunnretH = btmH ;
            }
            else {
              if ( angle < 0 ) {
                cunnretH = (float) ( cunnretH + height ) ;
              }
              else {
                cunnretH = (float) ( cunnretH - height ) ;
              }
            }
          }

          if ( i == 0 ) {
            StartY = btmH ;
          }

          if ( topH < cunnretH ) {
            topH = cunnretH ;
          }

          if ( btmH > cunnretH ) {
            btmH = cunnretH ;
          }
        }
      }

      return Math.Abs( topH - btmH ) ;
    }

    /// <summary>
    /// スロープ部の次の高さを求める
    /// </summary>
    /// <param name="CurrentIndex"></param>
    /// <returns></returns>
    private float getNextFlatAngle( int CurrentIndex, float cHeight )
    {
      float l = 0, h = 0, a = 0, nextH = 0 ;
      for ( int i = CurrentIndex ; i < SlopeData.Data.Count ; i++ ) {
        (SlopeType type, bool isEnd, double percent, double height) cData = SlopeData.Data[ i ] ;
        float length = spanLeng[ i + 1 ] ;
        if ( cData.type == SlopeType.slope ) {
          l += length ;
        }
        else if ( cData.type == SlopeType.none ) {
          h = (float) cData.height ;
          nextH = h ;
          break ;
        }
      }

      if ( l != 0 && h != cHeight ) {
        h = ( ( h > cHeight ) ? h - cHeight : cHeight - h ) ;
        a = (float) ( ( Math.Atan( h / l ) ) * ( 180 / Math.PI ) ) ;
        a = ( nextH < cHeight ) ? a : a * -1 ;
      }

      return a ;
    }

    private void btnOK_Click( object sender, EventArgs e )
    {
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btnUpdate_Click( object sender, EventArgs e )
    {
      if ( this.Owner == null ) {
        return ;
      }

      FrmCreateSlope parent = (FrmCreateSlope) this.Owner ;
      parent.PrevireUpdata() ;
    }
  }

  public class DrawSpanSet
  {
    /// <summary>
    /// スパン種類
    /// </summary>
    public SlopeType slopeType { get ; set ; }

    /// <summary>
    /// スパンの高度
    /// </summary>
    public float SpanHeight { get ; set ; }

    public float SpanLength { get ; set ; }

    public float SpnarealLeng { get ; set ; }
    public float Angle { get ; set ; }

    public string SpanName { get ; set ; }

    public string StValHeight { get ; set ; }


    public DrawSpanSet()
    {
      slopeType = SlopeType.none ;
      SpanHeight = 0 ;
      SpanLength = 0 ;
      SpnarealLeng = 0 ;
      Angle = 0 ;
      SpanName = "" ;
      StValHeight = "" ;
    }
  }

  public class DPoint
  {
    public float x { get ; set ; }
    public float y { get ; set ; }

    public DPoint( float X = 0, float Y = 0 )
    {
      x = X ;
      y = Y ;
    }
  }
}