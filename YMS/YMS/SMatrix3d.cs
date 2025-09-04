using Autodesk.Revit.DB ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.Data ;
using System.IO ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using MatrixMethods = YMS.MatrixCalc ;

namespace YMS
{
  /// <summary>
  /// M.Sakuraba
  /// </summary>
  public class SMatrix3d
  {
    private static int Size => 4 ;

    private double[ , ] m_elements { get ; set ; }

    private SMatrix3d( double[ , ] elements )
    {
      m_elements = elements ;
    }

    public SMatrix3d( double[] src )
    {
      var elements = new double[ Size, Size ] ;
      for ( int i = 0 ; i < Size ; i++ ) {
        for ( int j = 0 ; j < Size ; j++ ) {
          elements[ i, j ] = src.ElementAtOrDefault( Size * i + j ) ;
        }
      }

      m_elements = elements ;
    }

    public SMatrix3d Inverse()
    {
      return new SMatrix3d( MatrixMethods.Inverse( m_elements ) ) ;
    }

    public static SMatrix3d Identity
    {
      get { return new SMatrix3d( new double[] { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 } ) ; }
    }

    public static SMatrix3d ProjX
    {
      get { return new SMatrix3d( new double[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 } ) ; }
    }

    public static SMatrix3d ProjY
    {
      get { return new SMatrix3d( new double[] { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 } ) ; }
    }

    public static SMatrix3d ProjZ
    {
      get { return new SMatrix3d( new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 } ) ; }
    }

    public static SMatrix3d ProjXY
    {
      get { return new SMatrix3d( new double[] { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 } ) ; }
    }

    public static SMatrix3d Displacement( XYZ v )
    {
      return new SMatrix3d( new double[] { 1, 0, 0, v.X, 0, 1, 0, v.Y, 0, 0, 1, v.Z, 0, 0, 0, 1 } ) ;
    }

    public static SMatrix3d BasisTransform( Plane plane )
    {
      var u = plane.XVec ;
      var v = plane.YVec ;
      var w = plane.Normal ;
      var a = new SMatrix3d( new double[] { u.X, u.Y, u.Z, 0, v.X, v.Y, v.Z, 0, w.X, w.Y, w.Z, 0, 0, 0, 0, 1 } ) ;
      var b = Displacement( -plane.Origin ) ;
      return a * b ;
    }

    public static SMatrix3d Rotation( XYZ axis, XYZ centre, double radian )
    {
      var ucs = Plane.CreateByNormalAndOrigin( axis, centre ) ;
      var m = BasisTransform( ucs ) ;
      var minv = m.Inverse() ;
      var cos = Math.Cos( radian ) ;
      var sin = Math.Sin( radian ) ;

      var rotater = new SMatrix3d( new double[] { cos, -sin, 0, 0, sin, cos, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 } ) ;

      return minv * rotater * m ;
    }

    public XYZ TransformVector( XYZ vector )
    {
      var start = this * XYZ.Zero ;
      var end = this * vector ;
      return end - start ;
    }

    public static SMatrix3d operator *( SMatrix3d x, SMatrix3d y )
    {
      return new SMatrix3d( MatrixMethods.Prod( x.m_elements, y.m_elements ) ) ;
    }

    public static SMatrix3d operator *( double alpha, SMatrix3d x )
    {
      return new SMatrix3d( MatrixMethods.Multiply( alpha, x.m_elements ) ) ;
    }

    public static XYZ operator *( SMatrix3d x, XYZ v )
    {
      var srcV = new double[ , ] { { v.X }, { v.Y }, { v.Z }, { 1 } } ;
      var u = MatrixMethods.Prod( x.m_elements, srcV ) ;
      return new XYZ( u[ 0, 0 ], u[ 1, 0 ], u[ 2, 0 ] ) ;
    }

    public static Line operator *( SMatrix3d x, Line line )
    {
      if ( line.IsBound ) {
        return Line.CreateBound( x * line.GetEndPoint( 0 ), x * line.GetEndPoint( 1 ) ) ;
      }
      else {
        return Line.CreateUnbound( x * line.Origin, x.TransformVector( line.Direction ) ) ;
      }
    }

    public static Plane operator *( SMatrix3d x, Plane plane )
    {
      var origin = x * plane.Origin ;
      var basisX = x.TransformVector( plane.XVec ) ;
      var basisY = x.TransformVector( plane.YVec ) ;
      //var basisZ = x.TransformVector(plane.Normal);
      return Plane.CreateByOriginAndBasis( origin, basisX, basisY ) ;
    }

    public static SMatrix3d operator +( SMatrix3d x, SMatrix3d y )
    {
      return new SMatrix3d( MatrixMethods.Add( x.m_elements, y.m_elements ) ) ;
    }

    public static SMatrix3d operator -( SMatrix3d x )
    {
      return ( -1.0 ) * x ;
    }

    public static SMatrix3d operator -( SMatrix3d x, SMatrix3d y )
    {
      return x + ( -y ) ;
    }
  }

  /// <summary>
  /// 行列演算関連 M.Sakuraba
  /// </summary>
  public static class MatrixCalc
  {
    public static double[ , ] Add( double[ , ] x, double[ , ] y )
    {
      if ( x.GetLength( 0 ) != y.GetLength( 0 ) ) throw new ArgumentException() ;
      if ( x.GetLength( 1 ) != y.GetLength( 1 ) ) throw new ArgumentException() ;

      var m = x.GetLength( 0 ) ;
      var n = x.GetLength( 1 ) ;

      var result = new double[ m, n ] ;
      for ( int i = 0 ; i < m ; i++ ) {
        for ( int j = 0 ; j < n ; j++ ) {
          result[ i, j ] = x[ i, j ] + y[ i, j ] ;
        }
      }

      return result ;
    }

    public static double[ , ] Multiply( double alpha, double[ , ] x )
    {
      var m = x.GetLength( 0 ) ;
      var n = x.GetLength( 1 ) ;

      var result = new double[ m, n ] ;
      for ( int i = 0 ; i < m ; i++ ) {
        for ( int j = 0 ; j < n ; j++ ) {
          result[ i, j ] = alpha * x[ i, j ] ;
        }
      }

      return result ;
    }

    public static double[ , ] Prod( double[ , ] x, double[ , ] y )
    {
      if ( x.GetLength( 1 ) != y.GetLength( 0 ) ) throw new ArgumentException() ;

      var m = x.GetLength( 0 ) ;
      var n = y.GetLength( 0 ) ;
      var l = y.GetLength( 1 ) ;

      var result = new double[ m, l ] ;
      for ( int i = 0 ; i < m ; i++ ) {
        for ( int j = 0 ; j < l ; j++ ) {
          var value = 0.0 ;
          for ( int k = 0 ; k < n ; k++ ) {
            value += x[ i, k ] * y[ k, j ] ;
          }

          result[ i, j ] = value ;
        }
      }

      return result ;
    }

    /// <summary>
    /// 逆行列
    /// 正方でない行列が渡されたときは例外エラー
    /// 非正則行列が渡されたときは null を返す
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static double[ , ] Inverse( double[ , ] x )
    {
      if ( ! IsSquare( x ) ) throw new ArgumentException() ;
      var det = Det( x ) ;
      if ( ClsGeo.GEO_EQ( det, 0 ) ) return null ;
      var cofactorMatrix = CofactorMatrix( x ) ;
      return Multiply( 1.0 / det, cofactorMatrix ) ;
    }

    /// <summary>
    /// 正方行列 x の行列式 (余因子展開による)
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static double Det( double[ , ] x )
    {
      if ( ! IsSquare( x ) ) throw new ArgumentException() ;
      var n = x.GetLength( 0 ) ;
      if ( n == 0 ) return 1 ;
      var result = 0.0 ;
      for ( int i = 0 ; i < n ; i++ ) {
        result += x[ 0, i ] * Cofactor( x, 0, i ) ;
      }

      return result ;
    }

    /// <summary>
    /// 正方行列 x の i,j-余因子
    /// </summary>
    /// <param name="x">正方行列</param>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static double Cofactor( double[ , ] x, int i, int j )
    {
      if ( ! IsSquare( x ) ) throw new ArgumentException() ;

      var sign = ( ( i + j ) % 2 == 0 ) ? 1 : -1 ;
      var minorMatrix = MinorMatrix( x, i, j ) ;
      var det = Det( minorMatrix ) ;

      return sign * det ;
    }

    /// <summary>
    /// 余因子行列
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static double[ , ] CofactorMatrix( double[ , ] x )
    {
      if ( ! IsSquare( x ) ) throw new ArgumentException() ;

      var n = x.GetLength( 0 ) ;
      var result = new double[ n, n ] ;
      for ( int i = 0 ; i < n ; i++ ) {
        for ( int j = 0 ; j < n ; j++ ) {
          result[ i, j ] = Cofactor( x, j, i ) ;
        }
      }

      return result ;
    }

    /// <summary>
    /// 行列 x の i,j-小行列
    /// </summary>
    /// <param name="x">行列</param>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    public static double[ , ] MinorMatrix( double[ , ] x, int i, int j )
    {
      var m = x.GetLength( 0 ) ;
      var n = x.GetLength( 1 ) ;

      var result = new double[ m - 1, n - 1 ] ;
      for ( int p = 0 ; p < m ; p++ ) {
        if ( i == p ) {
          continue ;
        }

        var ip = i < p ? p - 1 : p ;
        for ( int q = 0 ; q < n ; q++ ) {
          if ( j == q ) continue ;
          var jq = j < q ? q - 1 : q ;
          result[ ip, jq ] = x[ p, q ] ;
        }
      }

      return result ;
    }

    public static bool IsSquare( double[ , ] x )
    {
      return x.GetLength( 0 ) == x.GetLength( 1 ) ;
    }

    public static double[ , ] Transposed( double[ , ] x )
    {
      var m = x.GetLength( 0 ) ;
      var n = x.GetLength( 1 ) ;
      var result = new double[ n, m ] ;
      for ( int i = 0 ; i < m ; i++ ) {
        for ( int j = 0 ; j < n ; j++ ) {
          result[ j, i ] = x[ i, j ] ;
        }
      }

      return result ;
    }

    public static double[ , ] FromXYZ( XYZ a )
    {
      var n = 3 ;
      var result = new double[ n, 1 ] ;
      for ( int i = 0 ; i < n ; i++ ) {
        result[ i, 0 ] = a[ i ] ;
      }

      return result ;
    }
  }

  /// <summary>
  /// M.Sakuraba
  /// </summary>
  public static class SLibRevitReo
  {
    /// <summary>
    /// M.Sakuraba
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static XYZ MidPoint( XYZ a, XYZ b )
    {
      return 0.5 * ( a + b ) ;
    }

    public static XYZ MidPoint( Line line )
    {
      if ( line.IsBound ) {
        return MidPoint( line.GetEndPoint( 0 ), line.GetEndPoint( 1 ) ) ;
      }

      return null ;
    }

    /// <summary>
    /// 平面と直線の交点
    /// 求められない時は null を返す
    /// M.Sakuraba
    /// </summary>
    /// <param name="plane"></param>
    /// <param name="line"></param>
    /// <returns></returns>
    public static XYZ IntersectPlaneAndLine( Plane plane, Line line )
    {
      var px = plane.XVec ;
      var py = plane.YVec ;
      var p0 = plane.Origin ;
      var l0 = line.Origin ;
      var ld = line.Direction ;

      var a = new double[ 3, 3 ] ;
      for ( int i = 0 ; i < 3 ; i++ ) {
        a[ i, 0 ] = px[ i ] ;
        a[ i, 1 ] = py[ i ] ;
        a[ i, 2 ] = -ld[ i ] ;
      }

      var b = MatrixCalc.FromXYZ( l0 - p0 ) ;
      var detA = MatrixCalc.Det( a ) ;
      var invA = MatrixCalc.Inverse( a ) ;
      if ( invA == null ) {
        return null ;
      }

      var ans = MatrixCalc.Prod( invA, b ) ;
      var result = l0 + ans[ 2, 0 ] * ld ;
      return result ;
    }

    /// <summary>
    /// M.Sakuraba
    /// </summary>
    /// <param name="line"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    public static XYZ ClosestLineAndPoint( Line line, XYZ point )
    {
      var plane = Plane.CreateByNormalAndOrigin( line.Direction, point ) ;
      return IntersectPlaneAndLine( plane, line ) ;
    }

    /// <summary>
    /// 2直線の近接点を求める
    /// M.Sakuraba
    /// </summary>
    /// <param name="line0"></param>
    /// <param name="line1"></param>
    /// <param name="point0">line0 上にある line1 の近接点</param>
    /// <param name="point1">line1 上にある line0 の近接点</param>
    /// <returns>line0 と line1 が平行なときは false</returns>
    public static bool ClosestLineAndLine( Line line0, Line line1, out XYZ point0, out XYZ point1 )
    {
      var normA = line0.Direction.CrossProduct( line1.Direction ) ;
      var alpha = Plane.CreateByNormalAndOrigin( normA, line0.Origin ) ;
      var mA = SMatrix3d.BasisTransform( alpha ) ;
      var mAvec = SMatrix3d.BasisTransform( Plane.CreateByOriginAndBasis( XYZ.Zero, alpha.XVec, alpha.YVec ) ) ;
      var projZ = SMatrix3d.ProjZ ;

      var p0Prime = mA * line0.Origin ;
      var d0Prime = mAvec * line0.Direction ;
      var p1Prime = mA * line1.Origin ;
      var d1Prime = mAvec * line1.Direction ;

      var a = new double[ , ] { { d0Prime.X, -d1Prime.X }, { d0Prime.Y, -d1Prime.Y }, } ;
      var b = new double[ , ] { { p1Prime.X - p0Prime.X }, { p1Prime.Y - p0Prime.Y }, } ;
      if ( ClsGeo.GEO_EQ( MatrixCalc.Det( a ), 0.0 ) ) {
        point0 = point1 = null ;
        return false ;
      }

      var x = MatrixCalc.Prod( MatrixCalc.Inverse( a ), b ) ;

      //var q00 = p1Prime + x[1, 0] * d1Prime;

      var lamda0 = x[ 0, 0 ] ;
      var lamda1 = x[ 1, 0 ] ;

      var q0Prime = p0Prime + lamda0 * d0Prime ;
      //var toLine1 = line1.Origin - p1Prime;
      var q1Prime = p1Prime + lamda1 * d1Prime ;

      var mInv = mA.Inverse() ;

      point0 = mInv * q0Prime ;
      point1 = mInv * q1Prime ;
      return true ;
    }

    /// <summary>
    /// 角度を -pi から pi の間へ変換する
    /// M.Sakuraba
    /// </summary>
    /// <param name="radian"></param>
    /// <returns></returns>
    public static double PrincipalRadian( double radian )
    {
      var start = -Math.PI ;
      var cycle = 2.0 * Math.PI ;
      var src = radian - start ;
      var result = src ;
      if ( src > 0 ) {
        var n = Math.Floor( src / cycle ) ;
        result = src - n * cycle ;
      }
      else {
        var n = Math.Floor( src / cycle ) ;
        result = src - ( n + 1 ) * cycle ;
      }

      return result + start ;
    }

    /// <summary>
    /// 平面と平面の交線
    /// M.Sakuraba
    /// </summary>
    /// <param name="alpha"></param>
    /// <param name="beta"></param>
    /// <returns>unbound line</returns>
    public static Line IntersectPlaneAndPlane( Plane alpha, Plane beta )
    {
      //2025/3/25
      var t = SMatrix3d.BasisTransform( alpha ) ;
      var beta1 = t * beta ;
      var a = beta1.Normal.X ;
      var b = beta1.Normal.Y ;
      //var c = beta1.Normal.Z;
      var d = -beta1.Normal.DotProduct( beta1.Origin ) ;

      var delta = a * a + b * b ;
      if ( ClsGeo.GEO_EQ( delta, 0.0 ) ) {
        return null ;
      }

      var p = ( 1.0 / delta ) * new XYZ( -a * d, -b * d, 0.0 ) ;
      var dir = new XYZ( b, -a, 0.0 ) ;

      var line = Line.CreateUnbound( p, dir ) ;
      var inverseT = t.Inverse() ;
      return inverseT * line ;
    }

    [Flags]
    public enum DivisionStatement
    {
      //Nil = 0,
      /// <summary>
      /// 内分点
      /// </summary>
      Internal = 1,

      /// <summary>
      /// 外分点
      /// </summary>
      External = 2,

      /// <summary>
      /// 端点
      /// </summary>
      Terminal = 4,

      /// <summary>
      /// 直線上ではない
      /// </summary>
      Other = 8,
    }

    /// <summary>
    /// pt が有限線分 boundLine の内分,外分,端点,直線上外を判定
    /// </summary>
    /// <param name="boundLine"></param>
    /// <param name="pt"></param>
    /// <returns></returns>
    public static DivisionStatement CalcDivisionStatement( Line boundLine, XYZ pt )
    {
      var a = boundLine.GetEndPoint( 0 ) ;
      var b = boundLine.GetEndPoint( 1 ) ;

      var toA = a - pt ;
      var toB = b - pt ;

      if ( ClsGeo.GEO_EQ0( toA.CrossProduct( toB ).GetLength() ) ) {
        var dotProd = toA.DotProduct( toB ) ;
        if ( ClsGeo.GEO_LT( dotProd, 0.0 ) ) {
          return DivisionStatement.Internal ;
        }
        else if ( ClsGeo.GEO_LT( 0.0, dotProd ) ) {
          return DivisionStatement.External ;
        }

        return DivisionStatement.Terminal ;
      }

      return DivisionStatement.Other ;
    }
  }

  /// <summary>
  /// CSV 読み込みライブラリ
  /// M.Sakuraba
  /// </summary>
  public static class SLibCsvSerializer
  {
    public static DataTable Deserialize( string csvPath, Encoding encoding )
    {
      var array = ReadFromCsv( csvPath, encoding ) ;
      return ConvertFrom( array ) ;
    }

    private static string[][] ReadFromCsv( in string filePath, in Encoding encoding )
    {
      var separater = ',' ;
      var quoter = '\"' ;
      var result = new List<string[]>() ;
      using ( var reader = new StreamReader( filePath, encoding ) ) {
        while ( ! reader.EndOfStream ) {
          var lineText = reader.ReadLine() ;
          var textArray = Separate( lineText, separater, quoter ).ToArray() ;
          result.Add( textArray ) ;
        }
      }

      return result.ToArray() ;
    }

    private static DataTable ConvertFrom( string[][] rawData )
    {
      var result = new DataTable() ;

      //ヘッダ
      var headerLine = rawData.FirstOrDefault() ;
      var columnCount = headerLine.Length ;
      for ( int i = 0 ; i < headerLine.Length ; i++ ) {
        var text = headerLine[ i ] ;
        result.Columns.Add( text ) ;
      }

      for ( int i = 1 ; i < rawData.Length ; i++ ) {
        var contentLine = rawData.ElementAtOrDefault( i ) ;
        if ( contentLine.Length == columnCount ) {
          result.Rows.Add( contentLine ) ;
        }
      }

      return result ;
    }

    static IEnumerable<string> Separate( string lineText, char separater, char quoter )
    {
      var charArray = lineText.ToArray() ;
      if ( ! charArray.Any() ) {
        yield break ;
      }

      var startIndex = 0 ;
      var inQuote = false ;

      for ( int i = 0 ; i < charArray.Length ; i++ ) {
        var grif = charArray[ i ] ;
        if ( grif == quoter ) {
          inQuote = ! inQuote ;
        }
        else if ( ! inQuote && grif == separater ) {
          yield return new string( charArray, startIndex, i - startIndex ).Trim( quoter ) ;
          startIndex = i + 1 ;
        }
      }

      yield return new string( charArray, startIndex, charArray.Length - startIndex ).Trim( quoter ) ;
    }

    public static bool ParseBool( string src )
    {
      if ( bool.TryParse( src, out bool result ) ) {
        return result ;
      }

      return false ;
    }
  }
}