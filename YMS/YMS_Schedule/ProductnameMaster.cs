using Microsoft.VisualBasic.FileIO ;
using System ;
using System.Collections.Generic ;
using System.IO ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;

namespace YMS_Schedule
{
  internal class ProductnameMaster
  {
    public Dictionary<string, string[]> tbl { get ; set ; }

    public ProductnameMaster()
    {
      tbl = new Dictionary<string, string[]>() ;
      load( Env.productnameMaster() ) ;
    }

    public void load( string fPath )
    {
      TextFieldParser tfp = new TextFieldParser( fPath, Env.encoding ) ;
      tfp.Delimiters = new string[] { Env.separator } ;

      //header
      int index = -1 ;
      string chk = Env.productnameYMS() ;
      while ( ! tfp.EndOfData ) {
        string[] row = tfp.ReadFields() ;
        for ( int i = 0 ; i < row.Length ; i++ ) {
          if ( row[ i ] == chk ) {
            index = i ;
            break ;
          }
        }

        break ;
      }

      if ( index >= 0 ) {
        while ( ! tfp.EndOfData ) {
          string[] row = tfp.ReadFields() ;
          string isYMS = row[ index ] ;
          if ( string.IsNullOrEmpty( isYMS ) ) {
            continue ;
          }

          tbl.Add( isYMS, row ) ;
        }
      }

      tfp.Close() ;
    }

    public string toCSVString( char delimiter = ',' )
    {
      string res = "" ;

      foreach ( var x in tbl.Keys ) {
        for ( int i = 0 ; i < tbl[ x ].Length ; i++ ) {
          res += ( tbl[ x ][ i ] + delimiter ) ;
        }

        res += Environment.NewLine ;
      }

      return res ;
    }
  }
}