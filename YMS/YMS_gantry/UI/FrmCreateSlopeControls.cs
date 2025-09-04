using Autodesk.Revit.DB ;
using System ;
using System.Collections.Generic ;
using System.Collections.ObjectModel ;
using System.ComponentModel ;
using System.Drawing ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;
using System.Xml.Serialization ;
using YMS_gantry.Master ;
using static YMS_gantry.UI.FrmCreateSlopeViewModel ;

//using static YMS_gantry.UI.FrnCreateSlopeControls.YMSGridColumnEnum<T>;

namespace YMS_gantry.UI.FrnCreateSlopeControls
{
  /// <summary>
  /// スロープ種類設定の GridView
  /// </summary>
  public class YMSGridViewSlopeStyle : YMSDataGridViewSuper
  {
    protected override void OnCellValueChanged( DataGridViewCellEventArgs e )
    {
      base.OnCellValueChanged( e ) ;
      if ( this.Columns[ e.ColumnIndex ] is YMSGridColumnSlopeType slopeTypeColumn ) {
        if ( DataSource is IEnumerable<StyleRow> dataSource ) {
          SettingCellEnabled( dataSource, e.RowIndex ) ;
        }
      }
    }

    protected override void OnBindingContextChanged( EventArgs e )
    {
      base.OnBindingContextChanged( e ) ;
    }

    protected override void OnDataBindingComplete( DataGridViewBindingCompleteEventArgs e )
    {
      base.OnDataBindingComplete( e ) ;
      if ( e.ListChangedType == ListChangedType.ItemAdded ) {
        if ( DataSource is Collection<StyleRow> dataSource ) {
          SettingCellEnabled( dataSource, dataSource.Count - 1 ) ;
        }
      }
    }

    private void SettingCellEnabled( IEnumerable<StyleRow> dataSource, int rowIndex )
    {
      var row = dataSource.ElementAtOrDefault( rowIndex ) ;
      if ( row == null ) {
        return ;
      }

      var radianCell = this[ 2, rowIndex ] ;
      var levelCell = this[ 3, rowIndex ] ;
      TurnCellEnable( radianCell, row.SlopeType == SlopeStyle.Entrance ) ;
      TurnCellEnable( levelCell, row.SlopeType == SlopeStyle.Nil ) ;
    }

    private void TurnCellEnable( DataGridViewCell cell, bool enabled )
    {
      if ( enabled ) {
        cell.Style.BackColor = SystemColors.Window ;
        cell.Style.ForeColor = SystemColors.ControlText ;
        cell.ReadOnly = false ;
      }
      else {
        cell.Style.BackColor = SystemColors.ControlDark ;
        cell.Style.ForeColor = SystemColors.GrayText ;
        cell.ReadOnly = true ;
      }
    }

    protected override void OnParentBindingContextChanged( EventArgs e )
    {
      base.OnParentBindingContextChanged( e ) ;
    }
  }

  public class YMSGridViewSupportParts : YMSDataGridViewSuper
  {
  }

  public class YMSDataGridViewSuper : DataGridView
  {
    public YMSDataGridViewSuper()
    {
    }

    public bool InDesignMode => DesignMode ;

    protected override void OnCellValidating( DataGridViewCellValidatingEventArgs e )
    {
      var gridView = this ;
      base.OnCellValidating( e ) ;
      //新しい行のセルで無い、かつセルの内容が変更されている時のみ、数値が入力されているか確認
      if ( e.RowIndex == gridView.NewRowIndex || ! gridView.IsCurrentCellDirty ) {
        return ;
      }

      if ( gridView.Columns[ e.ColumnIndex ] is YMSGridColumnRealNum column ) {
        var inputText = e.FormattedValue.ToString() ;
        if ( ! string.IsNullOrEmpty( inputText ) ) {
          if ( ! double.TryParse( inputText, out var number ) ) {
            gridView.CancelEdit() ;
          }
        }
      }
    }

    private bool CellChanging { get ; set ; } = false ;

    protected override void OnCellValueChanged( DataGridViewCellEventArgs e )
    {
      base.OnCellValueChanged( e ) ;
      if ( CellChanging ) {
        return ;
      }

      if ( SelectedCells.Count <= 1 ) {
        return ;
      }

      using ( var defer = new Defer( () => CellChanging = true, () => CellChanging = false ) ) {
        // 複数セルの同時入力処理
        foreach ( var cell in SelectedCells.Cast<DataGridViewCell>() ) {
          var dataCell = this[ e.ColumnIndex, e.RowIndex ] ;
          if ( dataCell == null ) {
            continue ;
          }

          if ( cell.ValueType == dataCell.ValueType ) {
            cell.Value = dataCell.Value ;
          }
        }
      }
    }
  }

  public class YMSGridColumnSlopeType : YMSGridColumnEnum<FrmCreateSlopeViewModel.SlopeStyle>
  {
  }

  public class YMSGridColumnTensetsubanType : YMSGridColumnMaster
  {
    static Lazy<Record[]> _RecordSet { get ; } = new Lazy<Record[]>( () =>
    {
      var plateSet = ClsMasterCsv.Shared.Select( x => x as ClsStiffenerCsv ).Where( x => x != null )
        .Where( x => x.Type == ClsStiffenerCsv.Tensetsuban )
        .Select( x => new Record { DisplayName = x.Size, MasterRecord = x } ) ;

      return Enumerable.Empty<Record>().Append( new Record { DisplayName = YMSGridNames.Nil } )
        //.Append(new Record { DisplayName = YMSGridNames.Plate })
        .Concat( plateSet ).ToArray() ;
    } ) ;

    static Record[] RecordSet => _RecordSet.Value ;
    //static YMSGridColumnTensetsubanType()
    //{
    //    //var plateSet = ClsMasterCsv.Shared
    //    //    .Select(x => x as ClsStiffenerCsv)
    //    //    .Where(x => x != null)
    //    //    .Where(x => x.Type == ClsStiffenerCsv.Tensetsuban)
    //    //    .Select(x => new Record { DisplayName = x.Size, MasterRecord = x });

    //    //RecordSet = Enumerable.Empty<Record>()
    //    //    .Append(new Record { DisplayName = YMSGridNames.Nil })
    //    //    //.Append(new Record { DisplayName = YMSGridNames.Plate })
    //    //    .Concat(plateSet)
    //    //    .ToArray();
    //}
    protected override void OnDataGridViewChanged()
    {
      base.OnDataGridViewChanged() ;
      if ( this.DataGridView is YMSDataGridViewSuper gridView ) {
        if ( ! gridView.InDesignMode ) {
          DataSource = RecordSet ;
        }
      }
    }
  }

  //public class YMSGridColumnMadumezaiType : YMSGridColumnEnum<FrmCreateSlopeViewModel.MadumezaiType> { }
  public class YMSGridColumnMadumezaiType : YMSGridColumnMaster
  {
    static Lazy<Record[]> _RecordSet { get ; } = new Lazy<Record[]>( () =>
    {
      var angleSet = ClsMasterCsv.Shared.Where( x => ! string.IsNullOrEmpty( x.MadumeSymbol ) )
        .Select( x => new Record { DisplayName = x.Size, MasterRecord = x } ) ;

      return Enumerable.Empty<Record>().Append( new Record { DisplayName = YMSGridNames.Nil } )
        //.Append(new Record { DisplayName = YMSGridNames.Plate })
        .Concat( angleSet ).ToArray() ;
    } ) ;

    static Record[] RecordSet => _RecordSet.Value ;
    //static YMSGridColumnMadumezaiType()
    //{
    //    //var angleSet = ClsMasterCsv.Shared
    //    //    .Where(x => !string.IsNullOrEmpty(x.MadumeSymbol))
    //    //    .Select(x => new Record { DisplayName = x.Size, MasterRecord = x });

    //    //RecordSet = Enumerable.Empty<Record>()
    //    //    .Append(new Record { DisplayName = YMSGridNames.Nil })
    //    //    //.Append(new Record { DisplayName = YMSGridNames.Plate })
    //    //    .Concat(angleSet)
    //    //    .ToArray();
    //}
    protected override void OnDataGridViewChanged()
    {
      base.OnDataGridViewChanged() ;
      if ( this.DataGridView is YMSDataGridViewSuper gridView ) {
        if ( ! gridView.InDesignMode ) {
          DataSource = RecordSet ;
        }
      }
    }
  }

  //public class YMSGridColumnStiffenerType : YMSGridColumnEnum<FrmCreateSlopeViewModel.StiffenerType> { }
  public class YMSGridColumnStiffenerType : YMSGridColumnMaster
  {
    static Lazy<Record[]> _RecordSet { get ; } = new Lazy<Record[]>( () =>
    {
      var jackSet = ClsMasterCsv.Shared.Where( x => x is ClsStiffenerCsv )
        .Where( x => x.Type == ClsStiffenerCsv.SHJack || x.Type == ClsStiffenerCsv.DWJJack )
        .Select( x => new Record { DisplayName = x.Size, MasterRecord = x } ) ;

      return Enumerable.Empty<Record>().Append( new Record { DisplayName = YMSGridNames.Nil } )
        .Append( new Record { DisplayName = YMSGridNames.Plate } ).Concat( jackSet ).ToArray() ;
    } ) ;

    static Record[] RecordSet => _RecordSet.Value ;
    //static YMSGridColumnStiffenerType()
    //{
    //    //var jackSet = ClsMasterCsv.Shared
    //    //    .Where(x => x is ClsStiffenerCsv)
    //    //    .Where(x => x.Type == ClsStiffenerCsv.Jack)
    //    //    .Select(x => new Record { DisplayName = x.Size, MasterRecord = x });

    //    //RecordSet = Enumerable.Empty<Record>()
    //    //    .Append(new Record { DisplayName = YMSGridNames.Nil })
    //    //    .Append(new Record { DisplayName = YMSGridNames.Plate })
    //    //    .Concat(jackSet)
    //    //    .ToArray();
    //}
    protected override void OnDataGridViewChanged()
    {
      base.OnDataGridViewChanged() ;
      if ( this.DataGridView is YMSDataGridViewSuper gridView ) {
        if ( ! gridView.InDesignMode ) {
          DataSource = RecordSet ;
        }
      }
    }
  }

  /// <summary>
  /// Enum を列挙して表示する DataGridViewCombo
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class YMSGridColumnEnum<T> : DataGridViewComboBoxColumn where T : Enum
  {
    public class YMSGridComboRecord
    {
      public T Value { get ; set ; }
      public string DisplayName { get ; set ; }
      public ClsMasterCsv MasterCsv { get ; set ; }
    }

    public YMSGridColumnEnum()
    {
    }

    protected override void OnDataGridViewChanged()
    {
      base.OnDataGridViewChanged() ;
      if ( this.DataGridView is YMSDataGridViewSuper gridView ) {
        if ( ! gridView.InDesignMode ) {
          DisplayMember = nameof( YMSGridComboRecord.DisplayName ) ;
          ValueMember = nameof( YMSGridComboRecord.Value ) ;

          DataSource = Enum.GetValues( typeof( T ) ).Cast<T>()
            .Where( x => x.GetAttrubute<ComboEnumAttribute>() != null ).Select( x =>
              new YMSGridComboRecord { Value = x, DisplayName = x.GetAttrubute<ComboEnumAttribute>()?.DisplayName } )
            .ToArray() ;
        }
      }
    }
  }

  public class YMSGridNames
  {
    public static string Nil => "無し" ;
    public static string Plate => "プレート" ;
  }

  public class YMSGridColumnMaster : DataGridViewComboBoxColumn
  {
    public class Record
    {
      public string DisplayName { get ; set ; }
      public ClsMasterCsv MasterRecord { get ; set ; }
    }

    public YMSGridColumnMaster()
    {
    }

    protected override void OnDataGridViewChanged()
    {
      base.OnDataGridViewChanged() ;
      if ( this.DataGridView is YMSDataGridViewSuper gridView ) {
        if ( ! gridView.InDesignMode ) {
          DisplayMember = nameof( Record.DisplayName ) ;
          ValueMember = nameof( Record.DisplayName ) ;

          //DataSource = Enum.GetValues(typeof(T))
          //    .Cast<T>()
          //    .Where(x => x.GetAttrubute<ComboEnumAttribute>() != null)
          //    .Select(x => new Record
          //    {
          //        Value = x,
          //        DisplayName = x.GetAttrubute<ComboEnumAttribute>()?.DisplayName
          //    })
          //    .ToArray();
        }
      }
    }
  }

  /// <summary>
  /// 実数値入力用の GridColumn
  /// </summary>
  public class YMSGridColumnRealNum : DataGridViewTextBoxColumn
  {
    public YMSGridColumnRealNum()
    {
      DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight ;
    }
  }

  public class Defer : IDisposable
  {
    Action EndProcess { get ; }

    public Defer( Action begin, Action end )
    {
      begin?.Invoke() ;
      EndProcess = end ;
    }

    public void Dispose()
    {
      EndProcess?.Invoke() ;
    }
  }
}