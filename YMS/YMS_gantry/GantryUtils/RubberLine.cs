using Autodesk.Revit.DB ;
using Autodesk.Revit.DB.Events ;
using Autodesk.Revit.UI ;
using System ;
using System.Collections.Generic ;
using System.Diagnostics ;
using System.IO ;
using System.Linq ;
using System.Runtime.InteropServices ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;

namespace YMS_gantry
{
  public class RubberLine
  {
    rubberLineUpdater updater = null ;
    public List<ElementId> elementId_added = new List<ElementId>() ;

    void OnDocumentChanged( object sender, DocumentChangedEventArgs e )
    {
      elementId_added.AddRange( e.GetAddedElementIds() ) ;
    }

    public void setup( AddInId addinid, string familyName )
    {
      updater = new rubberLineUpdater( addinid, familyName ) ;
      UpdaterRegistry.RegisterUpdater( updater ) ;

      ElementClassFilter filter = new ElementClassFilter( typeof( FamilyInstance ) ) ;
      UpdaterRegistry.AddTrigger( updater.GetUpdaterId(), filter, Element.GetChangeTypeElementAddition() ) ;
    }

    private FamilySymbol setupFamily( Document doc, string familyPath )
    {
      FamilySymbol res = null ;

      string familyName = this.updater.familyName ;

      FilteredElementCollector collector = new FilteredElementCollector( doc ) ;
      collector = collector.OfClass( typeof( FamilySymbol ) ) ;
      var query = from element in collector where element.Name == familyName select element ;
      List<Element> famSyms = query.ToList<Element>() ;
      if ( famSyms.Count >= 1 ) {
        res = famSyms[ 0 ] as FamilySymbol ;
      }
      else {
        if ( File.Exists( familyPath ) ) {
          using ( SubTransaction tx = new SubTransaction( doc ) ) {
            tx.Start() ;

            Family family ;
            doc.LoadFamily( familyPath, out family ) ;

            foreach ( ElementId fsids in family.GetFamilySymbolIds() ) {
              ElementType elemType = doc.GetElement( fsids ) as ElementType ;
              if ( elemType.FamilyName == familyName ) {
                res = doc.GetElement( fsids ) as FamilySymbol ;
                if ( ! res.IsActive ) {
                  res.Activate() ;
                }

                break ;
              }
            }

            tx.Commit() ;
          }
        }
      }

      return res ;
    }

    /// <summary>
    /// 2点間指定時にラバーバンドに類似した動作を行う2点間指定
    /// </summary>
    /// <param name="uiapp"></param>
    /// <param name="pt1">始点</param>
    /// <param name="pt2">終点</param>
    /// <param name="removeRubberLine"></param>
    /// <returns></returns>
    public Result Get2Point( UIApplication uiapp, string familyPath, out XYZ pt1, out XYZ pt2, out ElementId idOut,
      bool removeRubberLine = true )
    {
      pt1 = XYZ.Zero ;
      pt2 = XYZ.Zero ;

      UIDocument uidoc = uiapp.ActiveUIDocument ;
      Autodesk.Revit.ApplicationServices.Application app = uiapp.Application ;
      Document doc = uidoc.Document ;

      elementId_added.Clear() ;
      updater.internalESC = false ;

      FamilySymbol symbol = setupFamily( doc, familyPath ) ;

      try {
        app.DocumentChanged += OnDocumentChanged ;

        //作業面に配置になっていないと自由に配置できない
        PromptForFamilyInstancePlacementOptions opt = new PromptForFamilyInstancePlacementOptions() ;
        opt.FaceBasedPlacementType = FaceBasedPlacementType.Default ;
        uidoc.PromptForFamilyInstancePlacement( symbol ) ;

        app.DocumentChanged -= OnDocumentChanged ;
      }
      catch ( Exception ex2 ) {
        if ( ex2.HResult != -2146233088 ) {
          idOut = ElementId.InvalidElementId ;
          return Result.Failed ;
        }
        else {
          if ( ! RubberLine.instance.updater.internalESC ) {
            idOut = ElementId.InvalidElementId ;
            return Result.Cancelled ;
          }
        }
      }

      if ( elementId_added.Count >= 1 ) {
        for ( int i = 0 ; i < elementId_added.Count() ; i++ ) {
          var x = elementId_added[ i ] ;

          Element elm = doc.GetElement( x ) ;
          FamilyInstance fi = doc.GetElement( x ) as FamilyInstance ;
          if ( fi != null ) {
            Options options = new Options() ;
            options.View = doc.ActiveView ;

            List<GeometryInstance> geoms = fi.get_Geometry( options ).Where( o => o is GeometryInstance )
              .Cast<GeometryInstance>().ToList() ;
            foreach ( GeometryInstance gis in geoms ) {
              foreach ( GeometryObject geom in gis.GetInstanceGeometry() ) {
                Line line = geom as Line ;
                if ( line != null ) {
                  pt1 = line.GetEndPoint( 0 ) ;
                  pt2 = line.GetEndPoint( 1 ) ;
                }
              }
            }

            if ( removeRubberLine ) {
              using ( Transaction tr = new Transaction( doc ) ) {
                tr.Start( "removeTempline" ) ;
                doc.Delete( elementId_added ) ;
                tr.Commit() ;
              }
            }
          }
        }
      }

      idOut = elementId_added[ 0 ] ;
      return Result.Succeeded ;
    }

    private RubberLine()
    {
    }

    private static RubberLine instance = new RubberLine() ;

    public static RubberLine Instance
    {
      get { return instance ; }
    }
  }

  public class rubberLineUpdater : IUpdater
  {
    static AddInId m_appId ;
    static UpdaterId m_updaterId ;
    static IntPtr m_handle ;

    public string familyName { set ; get ; }
    public bool internalESC { set ; get ; }

    public rubberLineUpdater( AddInId id, string rubberFamilyName )
    {
      m_appId = id ;
      m_updaterId = new UpdaterId( m_appId, new Guid( "09C41B35-930F-4EED-9856-964080021FD2" ) ) ;
      m_handle = Process.GetCurrentProcess().MainWindowHandle ;
      familyName = rubberFamilyName ;
      internalESC = false ;
    }

    public void Execute( UpdaterData data )
    {
      var set = data.GetAddedElementIds().Select( x => data.GetDocument().GetElement( x ) as FamilyInstance )
        .Where( x => x != null && x.Name == familyName ).Distinct() ;
      if ( set.Count() >= 1 ) {
        internalESC = true ;
        ClsMiscWin.PostWindowsMessage( (int) m_handle, ClsMiscWin.WM_KEYDOWN, (int) Keys.Escape, 0 ) ;
        ClsMiscWin.PostWindowsMessage( (int) m_handle, ClsMiscWin.WM_KEYDOWN, (int) Keys.Escape, 0 ) ;
      }
    }

    public string GetAdditionalInformation()
    {
      return "updater: newly created rubberLines" ;
    }

    public ChangePriority GetChangePriority()
    {
      return ChangePriority.FloorsRoofsStructuralWalls ;
    }

    public UpdaterId GetUpdaterId()
    {
      return m_updaterId ;
    }

    public string GetUpdaterName()
    {
      return "rubberLine Updater" ;
    }
  }

  class ClsMiscWin
  {
    [DllImport( "User32.dll", EntryPoint = "SendMessage" )]
    public static extern int SendMessage( int hWnd, int Msg, int wParam, int lParam ) ;

    [DllImport( "User32.dll", EntryPoint = "PostMessage" )]
    public static extern int PostMessage( int hWnd, int Msg, int wParam, int lParam ) ;

    public const int WM_KEYDOWN = 0x0100 ;

    public static int SendWindowsMessage( int hWnd, int Msg, int wParam, int lParam )
    {
      int result = 0 ;
      if ( hWnd > 0 ) {
        result = SendMessage( hWnd, Msg, wParam, lParam ) ;
      }

      return result ;
    }

    public static int PostWindowsMessage( int hWnd, int Msg, int wParam, int lParam )
    {
      int result = 0 ;
      if ( hWnd > 0 ) {
        result = PostMessage( hWnd, Msg, wParam, lParam ) ;
      }

      return result ;
    }
  }
}