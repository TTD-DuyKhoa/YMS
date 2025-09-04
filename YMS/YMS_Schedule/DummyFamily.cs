using Autodesk.Revit.DB.Events ;
using Autodesk.Revit.DB.Structure ;
using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using System ;
using System.Collections.Generic ;
using System.IO ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using RevitUtil ;

namespace YMS_Schedule
{
  internal class DummyFamily
  {
    public FamilySymbol symbol { set ; get ; }
    public Document doc { set ; get ; }
    public string path { set ; get ; }

    /// <summary>
    /// 使わないこと　重い
    /// </summary>
    /// <param name="doc_"></param>
    public DummyFamily( Document doc_ )
    {
      doc = doc_ ;
      path = familyPath() ;
      symbol = load( doc, path ) ;
    }

    public FamilyInstance install( XYZ location = null )
    {
      FamilyInstance res = null ;

      UIControlledApplication uicapp = App.uicapp ;

      using ( Transaction trans = new Transaction( doc ) ) {
        EventHandler<FailuresProcessingEventArgs> h = null ;
        try {
          h = new EventHandler<FailuresProcessingEventArgs>( Misc.AppEvent_FailuresProcessing_Handler ) ;
          uicapp.ControlledApplication.FailuresProcessing += h ;

          trans.Start( "AddFamilyInstance" ) ;
          FailureHandlingOptions failOpt = trans.GetFailureHandlingOptions() ;
          failOpt.SetFailuresPreprocessor( new YMS.WarningSwallower() ) ;
          trans.SetFailureHandlingOptions( failOpt ) ;
          FamilyInstance fi = doc.Create.NewFamilyInstance( location == null ? XYZ.Zero : location, symbol,
            StructuralType.NonStructural ) ;
          trans.Commit() ;

          res = fi ;
        }
        catch {
          return res ;
        }
        finally {
          uicapp.ControlledApplication.FailuresProcessing -= h ;
        }
      }

      return res ;
    }

    public static string familyPath()
    {
      return Env.dummyFamilyPath() ;
    }

    static public FamilySymbol load( Document doc, string fPath )
    {
      FamilySymbol res = null ;

      string fName = Path.GetFileNameWithoutExtension( familyPath() ) ;

      Family family = new FilteredElementCollector( doc ).OfClass( typeof( Family ) )
        .FirstOrDefault<Element>( e => e.Name.Equals( fName ) ) as Family ;

      using ( Transaction trans = new Transaction( doc ) ) {
        try {
          trans.Start( "LoadFamily" ) ;
          if ( family == null ) {
            doc.LoadFamily( fPath, out family ) ;
          }

          foreach ( ElementId eId in family.GetFamilySymbolIds() ) {
            res = doc.GetElement( eId ) as FamilySymbol ;
            if ( ! res.IsActive ) {
              res.Activate() ;
              doc.Regenerate() ;
            }

            break ;
          }

          trans.Commit() ;
        }
        catch ( Exception e ) {
          return res ;
        }
      }

      return res ;
    }

    public static Dictionary<string, string> parameters( ViewSchedule vs )
    {
      return parameters( Header.header( vs, Env.sharedParameterId() ) ) ;
    }

    public static Dictionary<string, string> parameters( Header h )
    {
      Dictionary<string, string> res = new Dictionary<string, string>() ;
      foreach ( var x in h ) {
        if ( ! res.ContainsKey( x ) ) {
          res.Add( x, "" ) ;
        }
      }

      return res ;
    }
  }
}