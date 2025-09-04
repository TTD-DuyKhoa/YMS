using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using Autodesk.Revit.DB ;
using Autodesk.Revit.DB.Events ;

namespace YMS_Schedule
{
  class Misc
  {
    public static IEnumerable<FamilyInstance> familyInstancesByName( Document doc, string name )
    {
      return new FilteredElementCollector( doc ).OfClass( typeof( FamilyInstance ) ).Cast<FamilyInstance>()
        .Where( x => x.Name.Equals( name ) ) ;
    }

    public static IEnumerable<FamilyInstance> familyInstancesBySymbolName( Document doc, string name )
    {
      return new FilteredElementCollector( doc ).OfClass( typeof( FamilyInstance ) ).Cast<FamilyInstance>()
        .Where( x => x.Name.Equals( name ) ) ;
    }

    public static void AppEvent_FailuresProcessing_Handler( Object sender, EventArgs args )
    {
      FailuresProcessingEventArgs fpArgs = args as FailuresProcessingEventArgs ;
      FailuresAccessor accessor = fpArgs.GetFailuresAccessor() ;
      //if( !accessor.GetTransactionName().Equals("MyCommand") )
      //{
      //    return;
      //}
      //FailuresAccessor fas = fpArgs.GetFailuresAccessor();
      //fas.GetDocument();
      foreach ( FailureMessageAccessor msgAccessor in accessor.GetFailureMessages() ) {
        FailureDefinitionId id = msgAccessor.GetFailureDefinitionId() ;
        if ( ! FailureDefinitionIdList.Exists( e => e.Guid.ToString() == id.Guid.ToString() ) ) {
          continue ;
        }

        if ( msgAccessor.GetSeverity() == FailureSeverity.Warning ) {
          accessor.DeleteWarning( msgAccessor ) ;
          continue ;
        }

        if ( msgAccessor.GetSeverity() == FailureSeverity.Error ) {
          accessor.DeleteWarning( msgAccessor ) ;
          continue ;
        }

        if ( msgAccessor.GetSeverity() == FailureSeverity.DocumentCorruption ) {
          accessor.DeleteWarning( msgAccessor ) ;
          continue ;
        }
      }

      fpArgs.SetProcessingResult( FailureProcessingResult.Continue ) ;
    }

    public static List<FailureDefinitionId> FailureDefinitionIdList
    {
      get
      {
        List<FailureDefinitionId> list = new List<FailureDefinitionId>() ;
        list.Add( BuiltInFailures.OverlapFailures.WallsOverlap ) ;

        list.Add( BuiltInFailures.OverlapFailures.DivisionSketchOverlap ) ;
        list.Add( BuiltInFailures.OverlapFailures.DuplicateFabricSheet ) ;
        list.Add( BuiltInFailures.OverlapFailures.DuplicateInstances ) ;
        list.Add( BuiltInFailures.OverlapFailures.DuplicatePoints ) ;
        list.Add( BuiltInFailures.OverlapFailures.DuplicateRebar ) ;
        list.Add( BuiltInFailures.OverlapFailures.ELASeparationLinesOverlap ) ;
        list.Add( BuiltInFailures.OverlapFailures.FloorsOverlap ) ;
        list.Add( BuiltInFailures.OverlapFailures.LevelsOverlap ) ;
        list.Add( BuiltInFailures.OverlapFailures.OverlappingElementsTest ) ;
        list.Add( BuiltInFailures.OverlapFailures.RoomSeparationLinesOverlap ) ;
        list.Add( BuiltInFailures.OverlapFailures.SpaceSeparationLinesOverlap ) ;
        list.Add( BuiltInFailures.OverlapFailures.WallAreaBoundaryOverlap ) ;
        #if BUILD_REVIT2022
                list.Add(BuiltInFailures.OverlapFailures.WallELASeperationOverlap);
        #endif
        list.Add( BuiltInFailures.OverlapFailures.WallRoomSeparationOverlap ) ;
        list.Add( BuiltInFailures.OverlapFailures.WallSpaceSeparationOverlap ) ;
        list.Add( BuiltInFailures.OverlapFailures.CurvesOverlap ) ;
        list.Add( BuiltInFailures.OverlapFailures.AreaBoundaryLinesOverlap ) ;
        return list ;
      }
    }

    public static Dictionary<string, string> projectInfo( Document doc, List<string> names = null )
    {
      Dictionary<string, string> res = new Dictionary<string, string>() ;

      ProjectInfo pInfo = doc.ProjectInformation ;
      foreach ( Parameter x in doc.ProjectInformation.Parameters ) {
        string name = x.Definition.Name ;

        if ( names != null ) {
          if ( ! names.Contains( name ) ) {
            continue ;
          }
        }

        if ( ! res.ContainsKey( name ) ) {
          res.Add( name, x.AsString() ) ;
        }
      }

      return res ;
    }

    public static ElementId GetProjectParameterId( Document doc, string name )
    {
      ParameterElement pElem = new FilteredElementCollector( doc ).OfClass( typeof( ParameterElement ) )
        .Cast<ParameterElement>().Where( e => e.Name.Equals( name ) ).FirstOrDefault() ;

      return pElem?.Id ;
    }

    public static List<FamilyInstance> SubComponents( FamilyInstance familyInstance )
    {
      List<FamilyInstance> res = new List<FamilyInstance>() ;

      ICollection<ElementId> subElemSet = familyInstance.GetSubComponentIds() ;
      if ( subElemSet != null ) {
        foreach ( Autodesk.Revit.DB.ElementId ee in subElemSet ) {
          FamilyInstance f = familyInstance.Document.GetElement( ee ) as FamilyInstance ;
          if ( f != null ) {
            res.Add( f ) ;
          }
        }
      }

      return res ;

      // FamilyInstance super = familyInstance.SuperComponent as FamilyInstance;
    }

    public static void ParameterSet( Document doc, FamilyInstance fi, Dictionary<string, string> parameters )
    {
      foreach ( var x in parameters.Keys ) {
        Misc.parameterSet( doc, fi, x, parameters[ x ] ) ;
      }
    }

    public static void parameterSet( Document doc, FamilyInstance fi, string paramName, string value )
    {
      Parameter parameter = fi.LookupParameter( paramName ) ;
      if ( parameter != null && ! parameter.IsReadOnly ) {
        parameter.Set( value ) ;
      }
    }

    public static string levelName( Document doc, ElementId levelId )
    {
      string res = "" ;
      Level lvl = doc.GetElement( levelId ) as Level ;
      if ( res != null ) {
        res = lvl.Name ;
      }

      return res ;
    }

    public static Dictionary<ElementId, string> levelName( Document doc )
    {
      Dictionary<ElementId, string> res = new Dictionary<ElementId, string>() ;

      IList<Element> elms = new FilteredElementCollector( doc ).OfClass( typeof( Level ) ).ToElements() ;
      foreach ( var x in elms ) {
        res.Add( x.Id, levelName( doc, x.Id ) ) ;
      }

      return res ;
    }

    public static Dictionary<string, Level> levelLevel( Document doc )
    {
      Dictionary<string, Level> res = new Dictionary<string, Level>() ;

      IList<Element> elms = new FilteredElementCollector( doc ).OfClass( typeof( Level ) ).ToElements() ;
      foreach ( var x in elms ) {
        res.Add( levelName( doc, x.Id ), x as Level ) ;
      }

      return res ;
    }
  }
}