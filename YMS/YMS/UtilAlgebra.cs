using System.Collections.Generic ;
using System.Linq ;
using System ;
using System.Numerics ;

namespace YMS
{
  /// <summary>
  /// 代数方程式のソルバ
  /// M.Sakuraba
  /// </summary>
  public class UtilAlgebra
  {
    static double Epsilon => 1e-9 ;

    public static bool Eq( double x, double y )
    {
      return Math.Abs( x - y ) < Epsilon ;
    }

    static Complex CubicRoot( Complex z )
    {
      if ( Eq( z.Imaginary, 0.0 ) ) {
        if ( z.Real < 0 ) {
          return -Math.Pow( -z.Real, 1.0 / 3.0 ) ;
        }

        return Math.Pow( z.Real, 1.0 / 3.0 ) ;
      }

      return Complex.Pow( z, 1.0 / 3.0 ) ;
    }

    static Complex SquareRoot( Complex x )
    {
      return Complex.Pow( x, 1.0 / 2.0 ) ;
    }

    public static Complex CalcPolynomial( IEnumerable<Complex> coefficients, Complex z )
    {
      var reversed = coefficients ;
      var result = reversed.FirstOrDefault() ;
      foreach ( var b in reversed.Skip( 1 ) ) {
        result = result * z + b ;
      }

      return result ;
    }

    public static Complex QuadraticDiscriminant( Complex a, Complex b, Complex c )
    {
      return b * b - 4 * a * c ;
    }

    /// <summary>
    /// 2次方程式の解を求める
    /// a x^2 + b x + c = 0
    /// ただし a != 0
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    public static Complex[] SolveQuadraticEquation( Complex a, Complex b, Complex c )
    {
      var discriminant = QuadraticDiscriminant( a, b, c ) ;
      var discriminantSqrt = SquareRoot( discriminant ) ;
      var result = Enumerable.Empty<Complex>().Append( ( -b + discriminantSqrt ) / ( 2 * a ) )
        .Append( ( -b - discriminantSqrt ) / ( 2 * a ) ).ToArray() ;
      #if DEBUG
      var checkResult = result.Select( x => CalcPolynomial( new Complex[] { a, b, c }, x ) ).ToArray() ;
      #endif
      return result ;
    }

    public static Complex[] SolveCubicEquation( double a, double b, double c, double d )
    {
      var results = CardanoMethod( b / a, c / a, d / a ) ;
      #if DEBUG
      var checkResults = results.Select( x => CalcPolynomial( new Complex[] { a, b, c, d }, x ) ).ToArray() ;
      #endif
      return results ;
    }

    private static Complex[] CardanoMethod( double a, double b, double c )
    {
      var p = ( -1.0 / 3 ) * a * a + b ;
      var q = ( 2.0 / 27 ) * a * a * a - ( 1.0 / 3 ) * a * b + c ;
      var r = ( 1.0 / 4 ) * q * q + ( 1.0 / 27 ) * p * p * p ;

      var sqrtR = SquareRoot( r ) ;
      var halfQ = q / 2 ;

      var omega = new Complex( -1.0 / 2, Math.Sqrt( 3 ) / 2 ) ;
      var omega2 = omega * omega ;

      var u = CubicRoot( -halfQ + sqrtR ) ;
      var v = CubicRoot( -halfQ - sqrtR ) ;

      return Enumerable.Empty<Complex>().Append( u + v ).Append( u * omega + v * omega2 )
        .Append( u * omega2 + v * omega ).Select( y => -a / 3 + y ).ToArray() ;
    }

    /// <summary>
    /// 4次方程式の解を求める
    /// ax^4 + bx^3 + cx^2 + dx + e = 0
    /// ただし a != 0 を前提とする
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <param name="d"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public static Complex[] SolveQuarticEquation( double a, double b, double c, double d, double e )
    {
      var results = FerrariMethod( b / a, c / a, d / a, e / a ) ;
      #if DEBUG
      var checkResults = results.Select( x => CalcPolynomial( new Complex[] { a, b, c, d, e }, x ) ).ToArray() ;
      #endif
      return results ;
    }

    private static Complex[] FerrariMethod( double a, double b, double c, double d )
    {
      var p = ( -3.0 / 8 ) * a * a + b ;
      var q = ( 1.0 / 8 ) * a * a * a - ( 1.0 / 2 ) * a * b + c ;
      var r = ( -3.0 / 256 ) * a * a * a * a + ( 1.0 / 16 ) * a * a * b - ( 1.0 / 4 ) * a * c + d ;

      var l3 = -8.0 ;
      var l2 = 4 * p ;
      var l1 = 8 * r ;
      var l0 = q * q - 4 * p * r ;
      var lamdaSet = SolveCubicEquation( l3, l2, l1, l0 ) ;
      var lamda = lamdaSet.FirstOrDefault( x => Eq( x.Imaginary, 0.0 ) ).Real ;

      var alpha = -p + 2 * lamda ;
      var m = SquareRoot( alpha ) ;
      var n = -q / ( 2 * m ) ;

      return Enumerable.Empty<Complex>().Concat( SolveQuadraticEquation( 1, -m, lamda - n ) )
        .Concat( SolveQuadraticEquation( 1, m, lamda + n ) ).Select( y => -a / 4 + y ).ToArray() ;
    }

    public static void test()
    {
      {
        var rand = new Random() ;

        for ( int i = 0 ; i < 1000 ; i++ ) {
          var coefSet = Enumerable.Range( 0, 5 )
            .Select( x => ( rand.Next() % 2 == 0 ? 1.0 : -1.0 ) / rand.NextDouble() )
            //.Select(x => new Complex((rand.Next() % 2 == 0 ? 1.0 : -1.0) / rand.NextDouble(), (rand.Next() % 2 == 0 ? 1.0 : -1.0) / rand.NextDouble()))
            .ToArray() ;
          var k = 0 ;
          var roots = UtilAlgebra.SolveQuarticEquation( coefSet[ k++ ], coefSet[ k++ ], coefSet[ k++ ], coefSet[ k++ ],
            coefSet[ k++ ] ) ;

          var checkResult = roots.Select( x => CalcPolynomial( coefSet.Select( y => y * Complex.One ), x ) ).ToArray() ;

          if ( checkResult.Any( x => x.Magnitude > 1e-2 ) ) {
            Console.WriteLine( "Error solver" ) ;
          }
        }

        //var a = Math.PI; var b = Math.Pow(Math.E, Math.PI); var c = 28.0; var d = 80.0; var e = 48.0;
        //var roots = UtilAlgebra.SolveQuarticEquation(a, b, c, d, e);
      }
      //    {
      //        var a = 1.0; var b = -88.0; var c = 28.0; var d = 88880.0;
      //        var roots = UtilAlgebra.CalcCubicEquation(a, b, c, d);

      //        var results = roots.Select(x => a * x * x * x + b * x * x + c * x + d).ToArray();
      //    }
      //{
      //    var a = 1.0; var b = Math.PI; var c = Math.E; var d = -1.0;
      //    //var a = 1.0 / 3.0; var b = 0.0; var c = -1.0; var d = 5.0;
      //    var roots = UtilAlgebra.SolveCubicEquation(a, b, c, d);
      //}
    }
  }
}