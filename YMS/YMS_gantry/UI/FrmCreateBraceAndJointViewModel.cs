using System.ComponentModel ;
using YMS_gantry.Master ;
using YMS_gantry ;
using RevitUtil ;
using System.Runtime.Serialization.Formatters ;
using Autodesk.Revit.DB ;

namespace YMS_gantry.UI
{
  public class FrmCreateBraceAndJointViewModel : BindableSuper
  {
    public enum PlaceType
    {
      /// <summary>
      /// 配置方法 - まとめて
      /// </summary>
      Single,

      /// <summary>
      /// 配置方法 - 単体
      /// </summary>
      Multiple
    }

    public enum ConnectingType
    {
      Nil,
      Welding,
      Bolt,
      Metal
    }

    public BindingList<AllKoudaiFlatFrmData> KodaiSet { get ; } = new BindingList<AllKoudaiFlatFrmData>() ;

    private AllKoudaiFlatFrmData m_SelectedKodai { get ; set ; }

    public AllKoudaiFlatFrmData SelectedKodai
    {
      get => m_SelectedKodai ;
      set
      {
        if ( m_SelectedKodai != value ) {
          m_SelectedKodai = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( SelectedKodai ) ) ;
        }
      }
    }

    private bool m_PlaceIsSingle { get ; set ; }

    public bool PlaceIsSingle
    {
      get => m_PlaceIsSingle ;
      set
      {
        if ( m_PlaceIsSingle != value ) {
          m_PlaceIsSingle = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( PlaceIsSingle ) ) ;
        }
      }
    }

    private bool m_PlaceIsMultiple { get ; set ; } = true ;

    public bool PlaceIsMultiple
    {
      get => m_PlaceIsMultiple ;
      set
      {
        if ( m_PlaceIsMultiple != value ) {
          m_PlaceIsMultiple = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( PlaceIsMultiple ) ) ;
        }
      }
    }

    private bool m_VrtBraceHasK { get ; set ; } = true ;

    public bool VrtBraceHasK
    {
      get => m_VrtBraceHasK ;
      set
      {
        if ( m_VrtBraceHasK != value ) {
          m_VrtBraceHasK = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( VrtBraceHasK ) ) ;
        }
      }
    }

    private bool m_VrtBraceHasH { get ; set ; } = true ;

    public bool VrtBraceHasH
    {
      get => m_VrtBraceHasH ;
      set
      {
        if ( m_VrtBraceHasH != value ) {
          m_VrtBraceHasH = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( VrtBraceHasH ) ) ;
        }
      }
    }

    public BindingList<ClsMasterCsv> VrtBraceSizeList { get ; } = new BindingList<ClsMasterCsv>() ;

    private ClsMasterCsv m_VrtBraceSize { get ; set ; }

    public ClsMasterCsv VrtBraceSize
    {
      get => m_VrtBraceSize ;
      set
      {
        if ( m_VrtBraceSize != value ) {
          m_VrtBraceSize = value ;

          int index = VrtBraceSizeList.IndexOf( m_VrtBraceSize ) ;
          m_VrtBraceSizeIndex = index ;

          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( VrtBraceSize ) ) ;
        }
      }
    }

    private int m_VrtBraceSizeIndex { get ; set ; }

    public int VrtBraceSizeIndex
    {
      get => m_VrtBraceSizeIndex ;
      set
      {
        if ( m_VrtBraceSizeIndex != value ) {
          m_VrtBraceSizeIndex = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( VrtBraceSizeIndex ) ) ;
        }
      }
    }

    private bool m_VrtBracePriorH { get ; set ; } = true ;

    public bool VrtBracePriorH
    {
      get => m_VrtBracePriorH ;
      set
      {
        if ( m_VrtBracePriorH != value ) {
          m_VrtBracePriorH = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( VrtBracePriorH ) ) ;
        }
      }
    }

    private bool m_VrtBracePriorK { get ; set ; } = false ;

    public bool VrtBracePriorK
    {
      get => m_VrtBracePriorK ;
      set
      {
        if ( m_VrtBracePriorK != value ) {
          m_VrtBracePriorK = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( VrtBracePriorK ) ) ;
        }
      }
    }

    private int m_VrtBrace1UpperXMm { get ; set ; }

    public int VrtBrace1UpperXMm
    {
      get => m_VrtBrace1UpperXMm ;
      set
      {
        if ( m_VrtBrace1UpperXMm != value ) {
          m_VrtBrace1UpperXMm = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( VrtBrace1UpperXMm ) ) ;
        }
      }
    }

    public double VrtBrace1UpperXFt => ClsRevitUtil.CovertToAPI( VrtBrace1UpperXMm ) ;

    private int m_VrtBrace1UpperYMm { get ; set ; }

    public int VrtBrace1UpperYMm
    {
      get => m_VrtBrace1UpperYMm ;
      set
      {
        if ( m_VrtBrace1UpperYMm != value ) {
          m_VrtBrace1UpperYMm = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( VrtBrace1UpperYMm ) ) ;
        }
      }
    }

    public double VrtBrace1UpperYFt => ClsRevitUtil.CovertToAPI( VrtBrace1UpperYMm ) ;

    private int m_VrtBrace1LowerXMm { get ; set ; }

    public int VrtBrace1LowerXMm
    {
      get => m_VrtBrace1LowerXMm ;
      set
      {
        if ( m_VrtBrace1LowerXMm != value ) {
          m_VrtBrace1LowerXMm = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( VrtBrace1LowerXMm ) ) ;
        }
      }
    }

    public double VrtBrace1LowerXFt => ClsRevitUtil.CovertToAPI( VrtBrace1LowerXMm ) ;

    private int m_VrtBrace1LowerYMm { get ; set ; }

    public int VrtBrace1LowerYMm
    {
      get => m_VrtBrace1LowerYMm ;
      set
      {
        if ( m_VrtBrace1LowerYMm != value ) {
          m_VrtBrace1LowerYMm = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( VrtBrace1LowerYMm ) ) ;
        }
      }
    }

    public double VrtBrace1LowerYFt => ClsRevitUtil.CovertToAPI( VrtBrace1LowerYMm ) ;

    private int m_VrtBrace2UpperXMm { get ; set ; }

    public int VrtBrace2UpperXMm
    {
      get => m_VrtBrace2UpperXMm ;
      set
      {
        if ( m_VrtBrace2UpperXMm != value ) {
          m_VrtBrace2UpperXMm = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( VrtBrace2UpperXMm ) ) ;
        }
      }
    }

    public double VrtBrace2UpperXFt => ClsRevitUtil.CovertToAPI( VrtBrace2UpperXMm ) ;

    private int m_VrtBrace2UpperYMm { get ; set ; }

    public int VrtBrace2UpperYMm
    {
      get => m_VrtBrace2UpperYMm ;
      set
      {
        if ( m_VrtBrace2UpperYMm != value ) {
          m_VrtBrace2UpperYMm = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( VrtBrace2UpperYMm ) ) ;
        }
      }
    }

    public double VrtBrace2UpperYFt => ClsRevitUtil.CovertToAPI( VrtBrace2UpperYMm ) ;

    private int m_VrtBrace2LowerXMm { get ; set ; }

    public int VrtBrace2LowerXMm
    {
      get => m_VrtBrace2LowerXMm ;
      set
      {
        if ( m_VrtBrace2LowerXMm != value ) {
          m_VrtBrace2LowerXMm = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( VrtBrace2LowerXMm ) ) ;
        }
      }
    }

    public double VrtBrace2LowerXFt => ClsRevitUtil.CovertToAPI( VrtBrace2LowerXMm ) ;

    private int m_VrtBrace2LowerYMm { get ; set ; }

    public int VrtBrace2LowerYMm
    {
      get => m_VrtBrace2LowerYMm ;
      set
      {
        if ( m_VrtBrace2LowerYMm != value ) {
          m_VrtBrace2LowerYMm = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( VrtBrace2LowerYMm ) ) ;
        }
      }
    }

    public double VrtBrace2LowerYFt => ClsRevitUtil.CovertToAPI( VrtBrace2LowerYMm ) ;

    //private bool m_VrtBraceHasLowest { get; set; }
    //public bool VrtBraceHasLowest
    //{
    //    get => m_VrtBraceHasLowest;
    //    set
    //    {
    //        if (m_VrtBraceHasLowest != value)
    //        {
    //            m_VrtBraceHasLowest = value;
    //            this.OnPropertyChanged();
    //            this.OnPropertyChanged(nameof(VrtBraceHasLowest));
    //        }
    //    }
    //}

    private bool m_VrtBraceHasRound { get ; set ; } = false ;

    public bool VrtBraceHasRound
    {
      get => m_VrtBraceHasRound ;
      set
      {
        if ( m_VrtBraceHasRound != value ) {
          m_VrtBraceHasRound = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( VrtBraceHasRound ) ) ;
        }
      }
    }

    private int m_VrtBraceRoundMm { get ; set ; }

    public int VrtBraceRoundMm
    {
      get => m_VrtBraceRoundMm ;
      set
      {
        if ( m_VrtBraceRoundMm != value ) {
          m_VrtBraceRoundMm = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( VrtBraceRoundMm ) ) ;
        }
      }
    }

    public double VrtBraceRoundFt => ClsRevitUtil.CovertToAPI( VrtBraceRoundMm ) ;

    public ConnectingType VrtBraceConnectingType
    {
      get
      {
        if ( m_VrtBraceIsBolt ) return ConnectingType.Bolt ;
        if ( VrtBraceIsMetal ) return ConnectingType.Metal ;
        return ConnectingType.Welding ;
      }
    }

    private bool m_VrtBraceIsWelding { get ; set ; } = true ;

    public bool VrtBraceIsWelding
    {
      get => m_VrtBraceIsWelding ;
      set
      {
        if ( m_VrtBraceIsWelding != value ) {
          m_VrtBraceIsWelding = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( VrtBraceIsWelding ) ) ;
        }
      }
    }

    private bool m_VrtBraceIsBolt { get ; set ; } = false ;

    public bool VrtBraceIsBolt
    {
      get => m_VrtBraceIsBolt ;
      set
      {
        if ( m_VrtBraceIsBolt != value ) {
          m_VrtBraceIsBolt = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( VrtBraceIsBolt ) ) ;
        }
      }
    }

    private bool m_VrtBraceIsMetal { get ; set ; } = false ;

    public bool VrtBraceIsMetal
    {
      get => m_VrtBraceIsMetal ;
      set
      {
        if ( m_VrtBraceIsMetal != value ) {
          m_VrtBraceIsMetal = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( VrtBraceIsMetal ) ) ;
        }
      }
    }

    private bool m_HrzBraceHasPlacing { get ; set ; } = true ;

    public bool HrzBraceHasPlacing
    {
      get => m_HrzBraceHasPlacing ;
      set
      {
        if ( m_HrzBraceHasPlacing != value ) {
          m_HrzBraceHasPlacing = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( HrzBraceHasPlacing ) ) ;
        }
      }
    }

    private bool m_HrzBraceHasNotPlacing { get ; set ; } = false ;

    public bool HrzBraceHasNotPlacing
    {
      get => m_HrzBraceHasNotPlacing ;
      set
      {
        if ( m_HrzBraceHasNotPlacing != value ) {
          m_HrzBraceHasNotPlacing = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( HrzBraceHasNotPlacing ) ) ;
        }
      }
    }

    public BindingList<ClsMasterCsv> HrzBraceSizeList { get ; } = new BindingList<ClsMasterCsv>() ;

    private ClsMasterCsv m_HrzBraceSize { get ; set ; }

    public ClsMasterCsv HrzBraceSize
    {
      get => m_HrzBraceSize ;
      set
      {
        if ( m_HrzBraceSize != value ) {
          m_HrzBraceSize = value ;

          int index = HrzBraceSizeList.IndexOf( m_HrzBraceSize ) ;
          m_HrzBraceSizeIndex = index ;

          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( HrzBraceSize ) ) ;
        }
      }
    }

    private int m_HrzBraceSizeIndex { get ; set ; }

    public int HrzBraceSizeIndex
    {
      get => m_HrzBraceSizeIndex ;
      set
      {
        if ( m_HrzBraceSizeIndex != value ) {
          m_HrzBraceSizeIndex = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( HrzBraceSizeIndex ) ) ;
        }
      }
    }

    private bool m_HrzBraceIsOhbikiInner { get ; set ; } = true ;

    public bool HrzBraceIsOhbikiInner
    {
      get => m_HrzBraceIsOhbikiInner ;
      set
      {
        if ( m_HrzBraceIsOhbikiInner != value ) {
          m_HrzBraceIsOhbikiInner = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( HrzBraceIsOhbikiInner ) ) ;
        }
      }
    }

    private bool m_HrzBraceIsLowerOhbiki { get ; set ; } = false ;

    public bool HrzBraceIsLowerOhbiki
    {
      get => m_HrzBraceIsLowerOhbiki ;
      set
      {
        if ( m_HrzBraceIsLowerOhbiki != value ) {
          m_HrzBraceIsLowerOhbiki = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( HrzBraceIsLowerOhbiki ) ) ;
        }
      }
    }

    private bool m_HrzBraceHasTopPlate { get ; set ; } = false ;

    public bool HrzBraceHasTopPlate
    {
      get => m_HrzBraceHasTopPlate ;
      set
      {
        if ( m_HrzBraceHasTopPlate != value ) {
          m_HrzBraceHasTopPlate = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( HrzBraceHasTopPlate ) ) ;
        }
      }
    }

    private int m_HrzBraceXMm { get ; set ; }

    public int HrzBraceXMm
    {
      get => m_HrzBraceXMm ;
      set
      {
        if ( m_HrzBraceXMm != value ) {
          m_HrzBraceXMm = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( HrzBraceXMm ) ) ;
        }
      }
    }

    public double HrzBraceXFt => ClsRevitUtil.CovertToAPI( HrzBraceXMm ) ;

    private int m_HrzBraceYMm { get ; set ; }

    public int HrzBraceYMm
    {
      get => m_HrzBraceYMm ;
      set
      {
        if ( m_HrzBraceYMm != value ) {
          m_HrzBraceYMm = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( HrzBraceYMm ) ) ;
        }
      }
    }

    public double HrzBraceYFt => ClsRevitUtil.CovertToAPI( HrzBraceYMm ) ;

    public ConnectingType HrzBraceConnectingType
    {
      get
      {
        if ( HrzBraceIsBolt ) return ConnectingType.Bolt ;
        if ( HrzBraceIsMetal ) return ConnectingType.Metal ;
        return ConnectingType.Welding ;
      }
    }

    private bool m_HrzBraceIsWelding { get ; set ; } = true ;

    public bool HrzBraceIsWelding
    {
      get => m_HrzBraceIsWelding ;
      set
      {
        if ( m_HrzBraceIsWelding != value ) {
          m_HrzBraceIsWelding = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( HrzBraceIsWelding ) ) ;
        }
      }
    }

    private bool m_HrzBraceIsBolt { get ; set ; } = false ;

    public bool HrzBraceIsBolt
    {
      get => m_HrzBraceIsBolt ;
      set
      {
        if ( m_HrzBraceIsBolt != value ) {
          m_HrzBraceIsBolt = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( HrzBraceIsBolt ) ) ;
        }
      }
    }

    private bool m_HrzBraceIsMetal { get ; set ; } = false ;

    public bool HrzBraceIsMetal
    {
      get => m_HrzBraceIsMetal ;
      set
      {
        if ( m_HrzBraceIsMetal != value ) {
          m_HrzBraceIsMetal = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( HrzBraceIsMetal ) ) ;
        }
      }
    }

    private bool m_HrzJointHasK { get ; set ; } = true ;

    public bool HrzJointHasK
    {
      get => m_HrzJointHasK ;
      set
      {
        if ( m_HrzJointHasK != value ) {
          m_HrzJointHasK = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( HrzJointHasK ) ) ;
        }
      }
    }

    private bool m_HrzJointHasH { get ; set ; } = true ;

    public bool HrzJointHasH
    {
      get => m_HrzJointHasH ;
      set
      {
        if ( m_HrzJointHasH != value ) {
          m_HrzJointHasH = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( HrzJointHasH ) ) ;
        }
      }
    }

    public BindingList<ClsMasterCsv> HrzJointSizeList { get ; } = new BindingList<ClsMasterCsv>() ;

    private ClsMasterCsv m_HrzJointSize { get ; set ; }

    public ClsMasterCsv HrzJointSize
    {
      get => m_HrzJointSize ;
      set
      {
        if ( m_HrzJointSize != value ) {
          m_HrzJointSize = value ;

          int index = HrzJointSizeList.IndexOf( m_HrzJointSize ) ;
          m_HrzJointSizeIndex = index ;

          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( HrzJointSize ) ) ;
        }
      }
    }

    private int m_HrzJointSizeIndex { get ; set ; }

    public int HrzJointSizeIndex
    {
      get => m_HrzJointSizeIndex ;
      set
      {
        if ( m_HrzJointSizeIndex != value ) {
          m_HrzJointSizeIndex = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( HrzJointSizeIndex ) ) ;
        }
      }
    }

    private bool m_HrzJointUpperIsH { get ; set ; } = true ;

    public bool HrzJointUpperIsH
    {
      get => m_HrzJointUpperIsH ;
      set
      {
        if ( m_HrzJointUpperIsH != value ) {
          m_HrzJointUpperIsH = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( HrzJointUpperIsH ) ) ;
        }
      }
    }

    private bool m_HrzJointLowerIsH { get ; set ; } = false ;

    public bool HrzJointLowerIsH
    {
      get => m_HrzJointLowerIsH ;
      set
      {
        if ( m_HrzJointLowerIsH != value ) {
          m_HrzJointLowerIsH = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( HrzJointLowerIsH ) ) ;
        }
      }
    }

    private int m_HrzJointStartMm { get ; set ; }

    public int HrzJointStartMm
    {
      get => m_HrzJointStartMm ;
      set
      {
        if ( m_HrzJointStartMm != value ) {
          m_HrzJointStartMm = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( HrzJointStartMm ) ) ;
        }
      }
    }

    public double HrzJointStartFt => ClsRevitUtil.CovertToAPI( HrzJointStartMm ) ;

    private int m_HrzJointEndMm { get ; set ; }

    public int HrzJointEndMm
    {
      get => m_HrzJointEndMm ;
      set
      {
        if ( m_HrzJointEndMm != value ) {
          m_HrzJointEndMm = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( HrzJointEndMm ) ) ;
        }
      }
    }

    public double HrzJointEndFt => ClsRevitUtil.CovertToAPI( HrzJointEndMm ) ;

    public ConnectingType HrzJointConnectingType
    {
      get
      {
        if ( HrzJointIsBolt ) return ConnectingType.Bolt ;
        if ( HrzJointIsMetal ) return ConnectingType.Metal ;
        return ConnectingType.Welding ;
      }
    }

    private bool m_HrzJointIsWelding { get ; set ; } = true ;

    public bool HrzJointIsWelding
    {
      get => m_HrzJointIsWelding ;
      set
      {
        if ( m_HrzJointIsWelding != value ) {
          m_HrzJointIsWelding = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( HrzJointIsWelding ) ) ;
        }
      }
    }

    private bool m_HrzJointIsBolt { get ; set ; } = false ;

    public bool HrzJointIsBolt
    {
      get => m_HrzJointIsBolt ;
      set
      {
        if ( m_HrzJointIsBolt != value ) {
          m_HrzJointIsBolt = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( HrzJointIsBolt ) ) ;
        }
      }
    }

    private bool m_HrzJointIsMetal { get ; set ; } = false ;

    public bool HrzJointIsMetal
    {
      get => m_HrzJointIsMetal ;
      set
      {
        if ( m_HrzJointIsMetal != value ) {
          m_HrzJointIsMetal = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( HrzJointIsMetal ) ) ;
        }
      }
    }

    private int m_DanCount { get ; set ; }

    public int DanCount
    {
      get => m_DanCount ;
      set
      {
        if ( m_DanCount != value ) {
          m_DanCount = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( DanCount ) ) ;
        }
      }
    }

    private int m_DanBaseInterval { get ; set ; }

    public int DanBaseInterval
    {
      get => m_DanBaseInterval ;
      set
      {
        if ( m_DanBaseInterval != value ) {
          m_DanBaseInterval = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( DanBaseInterval ) ) ;
        }
      }
    }

    public BindingList<DansuRow> DanRowSet { get ; } = new BindingList<DansuRow>() ;

    public class DansuRow
    {
      public int Index { get ; set ; }
      public int IntervalMm { get ; set ; }
      public double IntervalFt => ClsRevitUtil.CovertToAPI( IntervalMm ) ;
      public bool HasJoint { get ; set ; }
      public bool HasBrace { get ; set ; }
    }

    public Document RevitDoc { get ; set ; } = null ;
  }
}