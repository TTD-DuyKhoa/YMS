using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using Autodesk.Revit.UI.Selection ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using YMS_gantry.Material ;
using YMS_gantry.UI ;

namespace YMS_gantry.Command
{
  class CmdCreateTaikeikou
  {
    static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger() ;
    Document _doc { get ; set ; }
    UIDocument _uiDoc { get ; set ; }

    public CmdCreateTaikeikou( UIDocument uiDoc )
    {
      _uiDoc = uiDoc ;
      _doc = _uiDoc.Document ;
    }

    /// <summary>
    /// 実行
    /// </summary>
    /// <returns></returns>
    public Result Excute()
    {
      using ( Transaction tr = new Transaction( _doc ) ) {
        tr.Start( "Taikeikou Placement" ) ;
        ElementId pointId = ElementId.InvalidElementId ;
        try {
          //FrmPutTaikeikou frm = new FrmPutTaikeikou(_uiDoc.Application);
          //if (frm.ShowDialog() != System.Windows.Forms.DialogResult.OK)
          //{
          //    return Result.Cancelled;
          //}
          FrmPutTaikeikou frm = Application.thisApp.frmPutTaikeikou ;

          #region "終始根太選択の場合"

          //対象となる終始点位置の根太を選択
          //SelectionFilterUtil pickFilter = new SelectionFilterUtil(_uiDoc, new List<string> { "根太", "桁受" });
          //FamilyInstance ins1 = null, ins2 = null;
          //if (!pickFilter.Select("1つ目の根太を選択してください", out ins1))
          //{
          //    return Result.Cancelled;
          //}
          //if (!pickFilter.Select("２つ目の根太を選択してください", out ins2))
          //{
          //    return Result.Cancelled;
          //}

          //Neda neda1, neda2;
          //neda1 = (Neda)Neda.ReadFromElement(ins1.Id, _doc);
          //neda2 = (Neda)Neda.ReadFromElement(ins2.Id, _doc);
          //if (neda1.m_KodaiName != neda2.m_KodaiName)
          //{
          //    MessageUtil.Warning("選択された根太は同じ構台のものである必要があります", "対傾構配置");
          //    return Result.Cancelled;
          //}

          //Transform ts1, ts2;
          //ts1 = ins1.GetTransform();
          //ts2 = ins2.GetTransform();
          //if (ts1.BasisZ.Z != ts2.BasisZ.Z)
          //{
          //    MessageUtil.Warning("選択された根太は水平である必要があります", "対傾構配置");
          //    return Result.Cancelled;
          //}

          #endregion

          //終始点選択
          //幅員方向ベクトル
          XYZ pntFirst = _uiDoc.Selection.PickPoint( ObjectSnapTypes.Nearest, "始点を指定" ) ;
          //Curve c1 = GantryUtil.GetCurve(_doc, ins1.Id);
          //XYZ Onc1 = GantryUtil.GetClosestPointOnLine(c1, pntFirst);
          //Face f = GantryUtil.GetTopFaceOfFamilyInstance(ins1);
          AllKoudaiFlatFrmData kData = GantryUtil.GetKoudaiData( _doc, frm.CmbKoudainame.Text ).AllKoudaiFlatData ;
          Level l = RevitUtil.ClsRevitUtil.GetLevel( _doc, kData.SelectedLevel ) as Level ;
          pointId = GantryUtil.InsertPointFamily( _doc, pntFirst, l.GetPlaneReference(), true ) ;
          _doc.Regenerate() ;

          XYZ pntSecond = _uiDoc.Selection.PickPoint( ObjectSnapTypes.Nearest, "２点目を指定" ) ;
          //Curve c2 = GantryUtil.GetCurve(_doc, ins2.Id);
          //XYZ Onc2 = GantryUtil.GetClosestPointOnLine(c2, pntSecond);
          //Curve flatCurve = Line.CreateBound(new XYZ(Onc1.X, Onc1.Y, 0), new XYZ(Onc2.X, Onc2.Y, 0));
          Curve flatCurve = Line.CreateBound( new XYZ( pntFirst.X, pntFirst.Y, 0 ),
            new XYZ( pntSecond.X, pntSecond.Y, 0 ) ) ;

          Taikeikou taikeikou = new Taikeikou() ;
          taikeikou.m_Size = frm.CmbSize.Text ;
          taikeikou.m_KodaiName = frm.CmbKoudainame.Text ;
          taikeikou.m_JointType =
            frm.RbtAttachWelding.Checked ? DefineUtil.eJoinType.Welding : DefineUtil.eJoinType.Bolt ;
          taikeikou.m_BoltType = frm.RbtAttatchBolt.Checked
            ? ( frm.RbtBolt.Checked ? DefineUtil.eBoltType.Normal : DefineUtil.eBoltType.HTB )
            : DefineUtil.eBoltType.None ;
          taikeikou.m_BoltCount = (int) frm.NmcBoltCnt.Value ;

          //交差する根太をすべて集める
          List<ElementId> nedaIds = MaterialSuper.Collect( _doc ).Where( x => x.m_ElementId != null &&
                                                                              x.m_KodaiName == frm.CmbKoudainame.Text &&
                                                                              x.GetType().Equals( typeof( Neda ) ) )
            .Select( x => x.m_ElementId ).ToList() ;
          if ( nedaIds.Count <= 0 ) {
            MessageUtil.Warning( "指定した構台の部材がみつかりませんでした", "対傾構配置" ) ;
            return Result.Cancelled ;
          }

          //全構台内の根太を集めて平面上で交差するか判定
          List<(Neda, XYZ)> crossPnts = new List<(Neda, XYZ)>() ;
          foreach ( ElementId id in nedaIds ) {
            Curve nedaC = GantryUtil.GetCurve( _doc, id ) ;
            Curve flatNedaC = Line.CreateBound( new XYZ( nedaC.GetEndPoint( 0 ).X, nedaC.GetEndPoint( 0 ).Y, 0 ),
              new XYZ( nedaC.GetEndPoint( 1 ).X, nedaC.GetEndPoint( 1 ).Y, 0 ) ) ;
            XYZ intPnts = GantryUtil.FindIntersectionPoint( flatCurve, flatNedaC ) ;
            if ( intPnts != null ) {
              Neda n = Neda.ReadFromElement( id, _doc ) as Neda ;
              crossPnts.Add( ( n, intPnts ) ) ;
            }
          }

          crossPnts = crossPnts.OrderBy( x => x.Item1.m_H ).ToList() ;
          if ( crossPnts.Select( x => x.Item1.m_H == double.MinValue ).Any() ) {
            crossPnts = crossPnts.OrderBy( x => x.Item2.DistanceTo( pntFirst ) ).ToList() ;
          }

          for ( int i = 0 ; i < crossPnts.Count - 1 ; i++ ) {
            Neda target1 = crossPnts[ i ].Item1 ;
            Neda target2 = crossPnts[ i + 1 ].Item1 ;
            if ( target1.m_H >= 0 && target1.m_H == target2.m_H ) {
              continue ;
            }

            if ( target1.SteelSize.Height > target2.SteelSize.Height ) {
              target1 = crossPnts[ i + 1 ].Item1 ;
              target2 = crossPnts[ i ].Item1 ;
            }

            Curve l1 = GantryUtil.GetCurve( _doc, target1.m_ElementId ) ;
            Curve l2 = GantryUtil.GetCurve( _doc, target2.m_ElementId ) ;
            XYZ onP1 = GantryUtil.GetClosestPointOnLine( l1, crossPnts[ i ].Item2 ) ;
            XYZ onP2 = GantryUtil.GetClosestPointOnLine( l2, crossPnts[ i + 1 ].Item2 ) ;

            taikeikou.m_TeiketsuType =
              frm.RbtAttachWelding.Checked ? DefineUtil.eJoinType.Welding : DefineUtil.eJoinType.Bolt ;
            if ( taikeikou.m_TeiketsuType == DefineUtil.eJoinType.Bolt ) {
              taikeikou.m_BoltInfo1 = ( frm.RbtBolt.Checked ) ? "通常" : "ハイテンション" ;
              taikeikou.m_Bolt1Cnt = (int) frm.NmcBoltCnt.Value ;
            }

            string result = Taikeikou.CreateTaikeikou( _doc, target1, target2, taikeikou, onP1, onP2,
              (double) frm.NmcClearlance.Value, frm.ChkNormalSize.Checked, frm.RbtStiffenerY.Checked ) ;
            if ( result != "完了" ) {
              _logger.Error( result ) ;
            }
          }

          //MessageUtil.Information("対傾構配置が完了しました。", "対傾構配置");
        }
        catch ( Autodesk.Revit.Exceptions.OperationCanceledException ) {
          //return Result.Cancelled;
        }
        catch ( Exception ex ) {
          MessageUtil.Error( "対傾構の作成に失敗しました", "対傾構配置" ) ;
          _logger.Error( ex.Message ) ;
          tr.RollBack() ;
          return Result.Failed ;
        }

        if ( pointId != null && pointId != ElementId.InvalidElementId ) {
          FamilyInstance ins = _doc.GetElement( pointId ) as FamilyInstance ;
          if ( ins != null ) {
            _doc.Delete( pointId ) ;
          }
        }

        tr.Commit() ;
      }

      return Result.Succeeded ;
    }
  }
}