using System ;
using System.Collections.Generic ;
using System.Linq ;
using Autodesk.Revit.DB ;
using System.IO ;
using Microsoft.VisualBasic.FileIO ;
using System.Collections ;
using System.Reflection ;
using RevitUtil ;

namespace YMS_Schedule
{
  public class Schedule
  {
    public Document doc { get ; set ; }
    public Header h { get ; set ; }
    public List<Item> itms { get ; set ; }
    public Dictionary<string, int> rm { get ; set ; }

    public static ViewSchedule view( Document doc )
    {
      string schduleName = Env.sheduleName() ;
      return new FilteredElementCollector( doc ).OfClass( typeof( ViewSchedule ) )
        .FirstOrDefault<Element>( e => e.Name.Equals( schduleName ) ) as ViewSchedule ;
    }

    public Item find( ElementId elmId )
    {
      return itms.Find( x => x.elmId == elmId ) ;
    }

    public Schedule( ViewSchedule vs )
    {
      itms = new List<Item>() ;

      RevitSchduleHelper.dataGet( vs, itms ) ;

      doc = vs.Document ;
      h = Header.header( vs, Env.sharedParameterId() ) ;
      rm = new Dictionary<string, int>() ;

      if ( itms.Count >= 1 ) {
        //compensate
        int indexDan = h.FindIndex( x => x == Env.sharedParameterDan() ) ;
        int indexLevel = h.FindIndex( x => x == Env.paramLevel ) ;
        Env.ScheduleYMSFilter filter = Env.sheduleFilter( false ) ;
        int indexItem1 = h.FindIndex( x => x == filter[ Env.keyFilter1 ] ) ;
        int indexItem2 = h.FindIndex( x => x == filter[ Env.keyFilter2 ] ) ;
        int indexItem3 = h.FindIndex( x => x == filter[ Env.keyFilter3 ] ) ;
        int indexElementId = h.FindIndex( x => x == "YMS_ElementID" ) ;
        int indexFamilyType = h.FindIndex( x => x == "ファミリとタイプ" ) ;

        foreach ( var x in itms.ToArray() ) {
          //YMS?
          if ( ! Schedule.isYMS( x.items[ indexItem1 ], x.items[ indexItem2 ], x.items[ indexItem3 ] ) ) {
            //ネストしているファミリがYMS有効ならば未だ除去しない
            bool bUse = false ;
            FamilyInstance fi = vs.Document.GetElement( x.elmId ) as FamilyInstance ;
            if ( fi != null ) {
              List<FamilyInstance> nest = Misc.SubComponents( fi ) ;
              foreach ( var y in nest ) {
                ElementId eId = y.Id ;
                string sId = eId.ToString() ;
                int idx = itms.FindIndex( id => id.elmId.ToString() == sId ) ;
                if ( idx < 0 ) {
                  continue ;
                }

                if ( Schedule.isYMS( itms[ idx ].items[ indexItem1 ], itms[ idx ].items[ indexItem2 ],
                      itms[ idx ].items[ indexItem3 ] ) ) {
                  bUse = true ;
                  break ;
                }
              }

              if ( bUse == true ) {
                continue ;
              }
            }

            //log
            string fName = x.items[ indexFamilyType ] ;
            if ( rm.ContainsKey( fName ) ) {
              rm[ fName ]++ ;
            }
            else {
              rm.Add( fName, 1 ) ;
            }

            itms.Remove( x ) ;
            continue ;
          }

          //copy Level to Dan
          x.items[ indexDan ] = x.items[ indexLevel ] ;
        }
      }
    }

    public string toCSVString( char delimiter = ',' )
    {
      string res = "" ;

      res += h.ToCSVString( delimiter ) ;
      res += Environment.NewLine ;

      int replaceIdx = h.IndexOf( "H_YMS_KOUDAI_PARAM" ) ;
      foreach ( Item x in itms ) {
        if ( replaceIdx >= 0 ) {
          x.items[ replaceIdx ] = x.items[ replaceIdx ].Replace( ",", "|" ) ;
        }

        res += x.ToCSVString( delimiter ) ;
        res += Environment.NewLine ;
      }

      return res ;
    }

    public static bool isYMS( string s1, string s2, string s3 )
    {
      //return string.IsNullOrEmpty(s1)
      //    || string.IsNullOrEmpty(s2)
      //    || string.IsNullOrEmpty(s3)
      //    ? false : true;
      return ! string.IsNullOrEmpty( s1 ) || ! string.IsNullOrEmpty( s2 ) || ! string.IsNullOrEmpty( s3 )
        ? true
        : false ;
    }
  }

  public class prjInfo
  {
    string EXT_PRJPATH = "Path" ;
    string EXT_DATE = "Date" ;

    public Dictionary<string, string> infos { set ; get ; }
    public List<string> keys { set ; get ; }

    public prjInfo( Document doc )
    {
      keys = Env.projectInfo().Values.ToList<string>() ;
      infos = Misc.projectInfo( doc, keys ) ;

      keys.Add( EXT_PRJPATH ) ;
      infos.Add( EXT_PRJPATH, "\"" + doc.PathName + "\"" ) ;
      keys.Add( EXT_DATE ) ;
      infos.Add( EXT_DATE, DateTime.Now.ToString( "yyyy年MM月dd日 HH時mm分ss秒" ) ) ;
    }

    public string toCSVString( bool addHeader = true, char delimiter = ',' )
    {
      string res = "" ;
      if ( addHeader ) {
        res += "パラメータ,値" ;
        res += Environment.NewLine ;
      }

      foreach ( var key in keys ) {
        res += ( key + delimiter ) ;
        res += infos.ContainsKey( key ) ? infos[ key ] : "" ;
        res += Environment.NewLine ;
      }

      return res ;
    }
  }
}