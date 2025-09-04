//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using Autodesk.Revit.UI.Selection ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Windows.Forms ;
using RevitUtil ;
using YMS.Command ;
using YMS.Parts ;

namespace YMS
{
  /// <summary>
  ///   A class with methods to execute requests made by the dialog user.
  /// </summary>
  ///
  public class RequestHandler : IExternalEventHandler
  {
    // A trivial delegate, but handy
    private delegate void DoorOperation( FamilyInstance e ) ;

    // The value of the latest request made by the modeless form
    private Request m_request = new Request() ;

    private XYZ maeP = null ;
    private ModelLine mdLine = null ;
    private XYZ geoDirection = null ;

    /// <summary>
    /// A public property to access the current request value
    /// </summary>
    public Request Request
    {
      get { return m_request ; }
    }

    /// <summary>
    ///   A method to identify this External Event Handler
    /// </summary>
    public String GetName()
    {
      return "R2014 External Event Sample" ;
    }


    /// <summary>
    ///   The top method of the event handler.
    /// </summary>
    /// <remarks>
    ///   This is called by Revit after the corresponding
    ///   external event was raised (by the modeless form)
    ///   and Revit reached the time at which it could call
    ///   the event's handler (i.e. this object)
    /// </remarks>
    ///
    public void Execute( UIApplication uiapp )
    {
      try {
        switch ( Request.Take() ) {
          case RequestId.End :
          {
            EndWarituke() ;
            break ;
          }
          case RequestId.PutHari :
          {
            //梁の割付
            ClsWaritsuke.CreateHari( uiapp ) ;
            break ;
          }
          case RequestId.PutTyouseizai :
          {
            //調整材配置
            CreateTyouseizai( uiapp ) ;
            break ;
          }
          case RequestId.PutKussakuBoidKabe :
          {
            //壁掘削用ボイドのファミリ配置配置
            CreateKussakuBoidKabe( uiapp ) ;
            break ;
          }
          case RequestId.PutKussakuBoidSoil :
          {
            //ソイル掘削用ボイドのファミリ配置配置
            CreateKussakuBoidSoil( uiapp ) ;
            break ;
          }
          case RequestId.PutKussakuBoidSoilLine :
          {
            CreateKussakuBoidSoilLineSelect( uiapp ) ;
            break ;
          }
          case RequestId.PutKussakuBoidKabeLine :
          {
            CreateKussakuBoidKabeLineSelect( uiapp ) ;
            break ;
          }
          case RequestId.UpdateCASE :
          {
            UpdateCASEInfo( uiapp ) ;
            UpdateSelectedCASEInfo( uiapp ) ;
            break ;
          }
          case RequestId.UpdateSelectCASE :
          {
            UpdateSelectedCASEInfo( uiapp ) ;
            break ;
          }
          case RequestId.SanjikuDist :
          {
            UIDocument uidoc = uiapp.ActiveUIDocument ;
            ClsSanjikuPeace.MoveConstSanjikuPeace( uidoc ) ;
            ClsSanjikuPeace.MovedSanjikuPeace( uidoc ) ;
            break ;
          }
          case RequestId.CreateCornerHiuchi :
          {
            UIDocument uidoc = uiapp.ActiveUIDocument ;
            ClsCommandCornerHiuchiBase.CreateCornerHiuchiBase( uidoc ) ;
            break ;
          }
          case RequestId.BaseList :
          {
            UIDocument uidoc = uiapp.ActiveUIDocument ;
            ClsCommandBaseList.CommandRequestBaseList( uidoc ) ;
            break ;
          }
          case RequestId.CASE :
          {
            UIDocument uidoc = uiapp.ActiveUIDocument ;
            clsCommandCASESetting.CommandRequestCASE( uidoc ) ;
            break ;
          }
          default :
          {
            break ;
          }
        }
      }
      finally {
        Application.thisApp.WakeFormUp() ;
      }

      return ;
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    public void EndWarituke()
    {
      //ダイアログの処理終了時に位置情報を初期化
      maeP = null ;
      mdLine = null ;
      geoDirection = null ;
    }

    /// <summary>
    /// 梁配置
    /// </summary>
    /// <param name="uiapp"></param>
    /// <returns></returns>
    public bool CreateHari( UIApplication uiapp )
    {
      UIDocument uidoc = uiapp.ActiveUIDocument ;
      Document doc = uidoc.Document ;

      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        Selection selection = uidoc.Selection ;

        List<ElementId> ids = new List<ElementId>() ;

        bool sel0 = true ;

        //芯を選択済みかの判定
        if ( mdLine == null ) {
          Reference picShin = selection.PickObject( ObjectType.Element, "芯を選択してください" ) ;
          Element selShin = GetSelectLine( doc, picShin ) ;

          if ( selShin == null ) {
            return false ;
          }

          //芯位置作成
          mdLine = selShin as ModelLine ;
        }

        Curve geoCurve = mdLine.GeometryCurve ;
        Line geoLine = geoCurve as Line ;
        geoDirection = geoLine.Direction ; //芯のvector
        double length = geoLine.Length ; //芯の長さ

        XYZ p = new XYZ() ;
        //初期位置の判定
        if ( maeP == null ) {
          p = geoCurve.GetEndPoint( 0 ) ; //芯作成位置（始点）
        }
        else {
          p = maeP ;
        }

        ElementId levelID = GetLevelID( doc, "レベル 1" ) ;
        Autodesk.Revit.DB.View v = getView( doc, "レベル 1", ViewType.FloorPlan ) ;
        Autodesk.Revit.DB.View v3 = getView( doc, "{3D}", ViewType.ThreeD ) ;
        Autodesk.Revit.DB.View vActv = doc.ActiveView ;
        double vSize = 1.0 ;
        if ( vActv.Id == v3.Id ) {
          vSize = 1.0 ;
        }

        if ( vActv.Id == v.Id ) {
          vSize = 0.3 ;
        }

        //単位ベクトルの符号
        int vSignX = Math.Sign( geoDirection.X ) ;
        int vSignY = Math.Sign( geoDirection.Y ) ;
        int vSignZ = Math.Sign( geoDirection.Z ) ;

        for ( ; ; ) {
          try {
            if ( ! sel0 ) {
              p = selection.PickPoint( "位置を選択！" ) ;
              XYZ nearPoint = new XYZ( ( p.X - maeP.X ) * geoDirection.X, ( p.Y - maeP.Y ) * geoDirection.Y, 0 ) ;
              double nearPointLength = nearPoint.GetLength() ;

              int len = (int) ( nearPointLength / ClsRevitUtil.CovertToAPI( 500 ) ) ;
              p = new XYZ( maeP.X + ( ClsRevitUtil.CovertToAPI( len * 500 ) * geoDirection.X ),
                maeP.Y + ( ClsRevitUtil.CovertToAPI( len * 500 ) * geoDirection.Y ), maeP.Z ) ;

              Curve cv0 = Line.CreateBound( maeP, p ) ;

              FamilySymbol sym = GetFamilySymbol( doc, "H形鋼200ｘ200", "切梁" ) ;
              if ( sym == null ) {
                return false ;
              }

              t.Start() ;
              ElementId CreatedID = Create( doc, cv0, levelID, sym ) ;
              t.Commit() ;

              maeP = p ;
            }
            else {
              maeP = p ;
              sel0 = false ;
            }

            t.Start() ;

            foreach ( ElementId id in ids ) {
              doc.Delete( id ) ;
            }

            ids = new List<ElementId>() ;

            for ( int i = 1 ; i < 151 ; i++ ) {
              XYZ newPoint = new XYZ( p.X + ( ClsRevitUtil.CovertToAPI( i * 100 ) * geoDirection.X ),
                p.Y + ( ClsRevitUtil.CovertToAPI( i * 100 ) * geoDirection.Y ),
                p.Z + ( ClsRevitUtil.CovertToAPI( i * 100 ) * geoDirection.Z ) ) ;
              XYZ bubbleEnd = new XYZ( newPoint.X, newPoint.Y, newPoint.Z ) ;
              //これなら動く
              XYZ freeEnd ;
              if ( Math.Abs( geoDirection.Y ) < Math.Abs( geoDirection.X ) ) {
                vSignY = 0 ;
                freeEnd = new XYZ( newPoint.X, 0, newPoint.Z ) ;
              }
              else {
                vSignX = 0 ;
                freeEnd = new XYZ( 0, newPoint.Y, newPoint.Z ) ;
              }

              Curve cv ;
              if ( i % 10 == 0 ) {
                cv = Line.CreateBound(
                  new XYZ( newPoint.X + ( 3 * vSignY ), newPoint.Y + ( 3 * vSignX ), newPoint.Z + ( 3 * vSignZ ) ),
                  new XYZ( newPoint.X - ( 3 * vSignY ), newPoint.Y - ( 3 * vSignX ), newPoint.Z - ( 3 * vSignZ ) ) ) ;
                ElementId defaultTypeId = doc.GetDefaultElementTypeId( ElementTypeGroup.TextNoteType ) ;

                TextNoteOptions opts = new TextNoteOptions( defaultTypeId ) ;
                //opts.HorizontalAlignment = HorizontalTextAlignment.Left;
                //opts.Rotation = Math.PI / 4;
                double maxWidth = TextNote.GetMaximumAllowedWidth( doc, defaultTypeId ) ;
                maxWidth = ClsRevitUtil.CovertToAPI( maxWidth ) ;
                TextNote tx = TextNote.Create( doc, vActv.Id,
                  new XYZ( newPoint.X - ( 0.56 * vSize ), newPoint.Y + ( 0.56 * vSize ), newPoint.Z ), maxWidth,
                  ( i / 10.0 ).ToString() + ".0", opts ) ;
                TextElementType textType = tx.Symbol ;
                BuiltInParameter paraIndex = BuiltInParameter.TEXT_SIZE ;
                Parameter textSize = textType.get_Parameter( paraIndex ) ;

                textSize.Set( maxWidth / ( 10 * vSize ) ) ;

                paraIndex = BuiltInParameter.TEXT_BACKGROUND ;
                Parameter textBack = textType.get_Parameter( paraIndex ) ;
                textBack.Set( 1 ) ; // 0 = Opaque（不透明） :: 1 = Transparent（透過）
                ids.Add( tx.Id ) ;
              }
              else if ( i % 5 == 0 ) {
                cv = Line.CreateBound(
                  new XYZ( newPoint.X + ( 2 * vSignY ), newPoint.Y + ( 2 * vSignX ), newPoint.Z + ( 2 * vSignZ ) ),
                  new XYZ( newPoint.X - ( 2 * vSignY ), newPoint.Y - ( 2 * vSignX ), newPoint.Z - ( 2 * vSignZ ) ) ) ;
              }
              else {
                cv = Line.CreateBound(
                  new XYZ( newPoint.X + ( 1 * vSignY ), newPoint.Y + ( 1 * vSignX ), newPoint.Z + ( 1 * vSignZ ) ),
                  new XYZ( newPoint.X - ( 1 * vSignY ), newPoint.Y - ( 1 * vSignX ), newPoint.Z - ( 1 * vSignZ ) ) ) ;
              }

              if ( vActv == null ) {
                return false ;
              }

              XYZ cutvec = new XYZ( 0, 0, newPoint.Z ) ;
              ReferencePlane plane = doc.Create.NewReferencePlane( bubbleEnd, freeEnd, cutvec, vActv ) ;
              SketchPlane skp = SketchPlane.Create( doc, plane.Id ) ;
              ModelCurve mc = doc.Create.NewModelCurve( cv, skp ) ;

              ids.Add( mc.Id ) ;
            }

            t.Commit() ;
          }
          catch ( OperationCanceledException e ) {
            break ;
          }
          catch ( Exception e ) {
            foreach ( ElementId id in ids ) {
              t.Start() ;
              doc.Delete( id ) ;
              t.Commit() ;
            }

            break ;
          }
        }
      }

      return true ;
    }

    /// <summary>
    /// 調整材配置
    /// </summary>
    /// <param name="uiapp"></param>
    /// <returns></returns>
    public bool CreateTyouseizai( UIApplication uiapp )
    {
      UIDocument uidoc = uiapp.ActiveUIDocument ;
      Document doc = uidoc.Document ;

      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        Selection selection = uidoc.Selection ;

        List<ElementId> ids = new List<ElementId>() ;

        bool sel0 = false ;
        Autodesk.Revit.DB.View v = getView( doc, "レベル 1", ViewType.FloorPlan ) ;
        try {
          while ( ! sel0 ) {
            XYZ p = null ;
            if ( maeP == null ) {
              p = selection.PickPoint( "位置を選択！" ) ;
              sel0 = true ;
            }

            if ( ! sel0 ) {
              p = new XYZ( maeP.X + ( ClsRevitUtil.CovertToAPI( 100 ) * geoDirection.X ),
                maeP.Y + ( ClsRevitUtil.CovertToAPI( 100 ) * geoDirection.Y ),
                maeP.Z + ( ClsRevitUtil.CovertToAPI( 100 ) * geoDirection.Z ) ) ;

              Curve cv0 = Line.CreateBound( maeP, p ) ;

              FamilySymbol sym = GetFamilySymbol( doc, "H形鋼200ｘ200", "切梁" ) ;
              if ( sym == null ) {
                return false ;
              }

              t.Start() ;
              ElementId levelID = GetLevelID( doc, "レベル 1" ) ;
              ElementId CreatedID = Create( doc, cv0, levelID, sym ) ;
              t.Commit() ;

              maeP = p ;
              sel0 = true ;
            }
            else {
              maeP = p ;
              sel0 = false ;
            }
          }
        }
        catch ( OperationCanceledException e ) {
        }
        catch ( Exception e ) {
          foreach ( ElementId id in ids ) {
            t.Start() ;
            doc.Delete( id ) ;
            t.Commit() ;
          }
        }
      }

      return true ;
    }

    /// <summary>
    /// 壁掘削作成
    /// </summary>
    /// <param name="uiapp"></param>
    /// <returns></returns>
    public bool CreateKussakuBoidKabe( UIApplication uiapp )
    {
      UIDocument uidoc = uiapp.ActiveUIDocument ;
      Document doc = uidoc.Document ;

      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        ISelectionFilter selFilter = new WallSelectionFilter() ;
        Selection selection = uidoc.Selection ;

        Autodesk.Revit.DB.View v = getView( doc, "レベル 1", ViewType.FloorPlan ) ;
        try {
          Reference picWall = selection.PickObject( ObjectType.Element, selFilter, "壁を選択してください" ) ;
          if ( picWall == null ) {
            return false ;
          }

          //選択した部材ID
          ElementId selId = picWall.ElementId ;
          Wall wall = null ;
          try {
            wall = doc.GetElement( selId ) as Wall ;
          }
          catch ( Exception ) {
            throw ;
          }

          if ( wall == null ) {
            //ClsMessage.ShowMessageBox("壁を選択してください。", System.Windows.Forms.MessageBoxIcon.Error);
            return false ;
          }

          //壁位置作成
          Curve cv = ( wall.Location as LocationCurve ).Curve ;
          XYZ wallOrigin = cv.GetEndPoint( 0 ) ;
          XYZ wallEndPoint = cv.GetEndPoint( 1 ) ;
          XYZ wallDirection = wallEndPoint - wallOrigin ;
          double wallLength = wallDirection.GetLength() ;
          XYZ p = new XYZ( ( wallOrigin.X + wallEndPoint.X ) / 2.0, ( wallOrigin.Y + wallEndPoint.Y ) / 2.0,
            ( wallOrigin.Z + wallEndPoint.Z ) / 2.0 ) ;

          //掘削シンボル作成
          FamilySymbol sym = GetFamilySymbol( doc, "掘削用ボイド", "掘削用ボイド" ) ;
          if ( sym == null ) {
            return false ;
          }

          t.Start() ;
          ElementId levelID = GetLevelID( doc, "レベル 1" ) ;
          ElementId CreatedID = Create( doc, p, levelID, sym ) ;

          //パラメーターの設定処理
          SetParameter( doc, CreatedID, "長さ", wallLength ) ;


          DLG.DlgCreateVoidFamily m_MyForm = Application.thisApp.GetForm() ;

          if ( m_MyForm != null && m_MyForm.Visible ) {
            SetParameter( doc, CreatedID, "掘削深さ1", ClsRevitUtil.CovertToAPI( m_MyForm.GetKussaku( 1 ) ) ) ;
            SetParameter( doc, CreatedID, "掘削深さ2", ClsRevitUtil.CovertToAPI( m_MyForm.GetKussaku( 2 ) ) ) ;
          }

          //切り取り処理
          Element hostElem = doc.GetElement( selId ) ;
          Element cutElem = doc.GetElement( CreatedID ) ;

          InstanceVoidCutUtils.AddInstanceVoidCut( doc, hostElem, cutElem ) ;
          t.Commit() ;
        }
        catch ( OperationCanceledException e ) {
        }
        catch ( Exception e ) {
        }
      }

      return true ;
    }

    /// <summary>
    /// ソイル掘削動作
    /// </summary>
    /// <param name="uiapp"></param>
    /// <returns></returns>
    public bool CreateKussakuBoidSoil( UIApplication uiapp )
    {
      UIDocument uidoc = uiapp.ActiveUIDocument ;
      Document doc = uidoc.Document ;

      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        //ISelectionFilter selFilter = new WallSelectionFilter();
        Selection selection = uidoc.Selection ;

        Autodesk.Revit.DB.View v = getView( doc, "レベル 1", ViewType.FloorPlan ) ;
        try {
          IList<Reference> picSoil = selection.PickObjects( ObjectType.Element, "ソイルを選択してください" ) ;
          if ( picSoil == null ) {
            return false ;
          }

          //選択した部材ID
          List<ElementId> selId = new List<ElementId>() ;
          foreach ( Reference rf in picSoil ) {
            selId.Add( rf.ElementId ) ;
          }

          if ( selId.Count < 1 ) {
            return false ;
          }

          FamilyInstance soil1 = null ;
          FamilyInstance soilmax = null ;

          //長さ追加用
          FamilyInstance soila = null ;
          FamilyInstance soilb = null ;
          try {
            soil1 = doc.GetElement( selId[ 0 ] ) as FamilyInstance ;
            soilmax = doc.GetElement( selId[ selId.Count - 1 ] ) as FamilyInstance ;

            if ( 1 < selId.Count ) {
              soila = doc.GetElement( selId[ 0 ] ) as FamilyInstance ;
              soilb = doc.GetElement( selId[ 1 ] ) as FamilyInstance ;
            }
          }
          catch ( Exception ) {
            throw ;
          }

          if ( soil1 == null && soilmax == null ) {
            //ClsMessage.ShowMessageBox("壁を選択してください。", System.Windows.Forms.MessageBoxIcon.Error);
            return false ;
          }

          //ソイル位置作成
          XYZ p1 = ( soil1.Location as LocationPoint ).Point ;
          XYZ pmax = ( soilmax.Location as LocationPoint ).Point ;
          XYZ soilOrigin = p1 ;
          XYZ soilEndPoint = pmax ;

          XYZ soilDirection = soilEndPoint - soilOrigin ;
          double soilLength = soilDirection.GetLength() ;

          XYZ p = new XYZ( ( soilOrigin.X + soilEndPoint.X ) / 2.0, ( soilOrigin.Y + soilEndPoint.Y ) / 2.0,
            ( soilOrigin.Z + soilEndPoint.Z ) / 2.0 ) ;

          //ソイルの中心から両端の長さを追加
          if ( soilb != null ) {
            XYZ pa = ( soila.Location as LocationPoint ).Point ;
            XYZ pb = ( soilb.Location as LocationPoint ).Point ;
            XYZ abDirection = pa - pb ;
            double abLength = abDirection.GetLength() ;
            soilLength += abLength * 2 ;
          }

          //掘削シンボル作成
          FamilySymbol sym = GetFamilySymbol( doc, "掘削用ボイド", "掘削用ボイド" ) ;
          if ( sym == null ) {
            return false ;
          }

          t.Start() ;
          ElementId levelID = GetLevelID( doc, "レベル 1" ) ;
          ElementId CreatedID = Create( doc, p, levelID, sym ) ;

          //パラメーターの設定処理
          SetParameter( doc, CreatedID, "長さ", soilLength ) ;
          SetParameter( doc, CreatedID, "幅", ClsRevitUtil.CovertToAPI( 300 ) ) ;


          DLG.DlgCreateVoidFamily m_MyForm = Application.thisApp.GetForm() ;

          if ( m_MyForm != null && m_MyForm.Visible ) {
            SetParameter( doc, CreatedID, "掘削深さ1", ClsRevitUtil.CovertToAPI( m_MyForm.GetKussaku( 1 ) ) ) ;
            SetParameter( doc, CreatedID, "掘削深さ2", ClsRevitUtil.CovertToAPI( m_MyForm.GetKussaku( 2 ) ) ) ;
          }

          //切り取り処理
          for ( int i = 0 ; i < selId.Count ; i++ ) {
            Element hostElem = doc.GetElement( selId[ i ] ) ;
            Element cutElem = doc.GetElement( CreatedID ) ;

            InstanceVoidCutUtils.AddInstanceVoidCut( doc, hostElem, cutElem ) ;
          }

          t.Commit() ;
        }
        catch ( OperationCanceledException e ) {
        }
        catch ( Exception e ) {
        }
      }

      return true ;
    }

    /// <summary>
    /// ソイル掘削線分選択動作
    /// </summary>
    /// <param name="uiapp"></param>
    /// <returns></returns>
    public bool CreateKussakuBoidSoilLineSelect( UIApplication uiapp )
    {
      UIDocument uidoc = uiapp.ActiveUIDocument ;
      Document doc = uidoc.Document ;

      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        //ISelectionFilter selFilter = new WallSelectionFilter();
        Selection selection = uidoc.Selection ;

        Autodesk.Revit.DB.View v = getView( doc, "レベル 1", ViewType.FloorPlan ) ;
        try {
          IList<Reference> picSoil = selection.PickObjects( ObjectType.Element, "ソイルを選択してください" ) ;
          if ( picSoil == null ) {
            return false ;
          }

          //選択した部材ID
          List<ElementId> selId = new List<ElementId>() ;
          foreach ( Reference rf in picSoil ) {
            selId.Add( rf.ElementId ) ;
          }

          if ( selId.Count < 1 ) {
            return false ;
          }

          //線分選択
          Reference picLine = selection.PickObject( ObjectType.Element, "線分を選択してください" ) ;
          ElementId selLine = picLine.ElementId ;
          Element line = null ;
          try {
            line = doc.GetElement( selLine ) ;
          }
          catch ( Exception ) {
            throw ;
          }

          if ( line == null ) {
            //ClsMessage.ShowMessageBox("壁を選択してください。", System.Windows.Forms.MessageBoxIcon.Error);
            return false ;
          }

          //ソイル位置作成
          Curve cv = ( line.Location as LocationCurve ).Curve ;
          XYZ lineOrigin = cv.GetEndPoint( 0 ) ;
          XYZ lineEndPoint = cv.GetEndPoint( 1 ) ;
          XYZ soilDirection = lineEndPoint - lineOrigin ;
          double wallLength = soilDirection.GetLength() ;
          XYZ p = new XYZ( ( lineOrigin.X + lineEndPoint.X ) / 2.0, ( lineOrigin.Y + lineEndPoint.Y ) / 2.0,
            ( lineOrigin.Z + lineEndPoint.Z ) / 2.0 ) ;

          //掘削シンボル作成
          FamilySymbol sym = GetFamilySymbol( doc, "掘削用ボイド", "掘削用ボイド" ) ;
          if ( sym == null ) {
            return false ;
          }

          t.Start() ;
          ElementId levelID = GetLevelID( doc, "レベル 1" ) ;
          ElementId CreatedID = Create( doc, p, levelID, sym ) ;

          //パラメーターの設定処理
          SetParameter( doc, CreatedID, "長さ", wallLength ) ;
          //SetParameter(doc, CreatedID, "幅", ClsRevitUtil.CovertToAPI(300));


          DLG.DlgCreateVoidFamily m_MyForm = Application.thisApp.GetForm() ;

          if ( m_MyForm != null && m_MyForm.Visible ) {
            SetParameter( doc, CreatedID, "掘削深さ1", ClsRevitUtil.CovertToAPI( m_MyForm.GetKussaku( 1 ) ) ) ;
            SetParameter( doc, CreatedID, "掘削深さ2", ClsRevitUtil.CovertToAPI( m_MyForm.GetKussaku( 2 ) ) ) ;
          }

          //切り取り処理
          for ( int i = 0 ; i < selId.Count ; i++ ) {
            Element hostElem = doc.GetElement( selId[ i ] ) ;
            Element cutElem = doc.GetElement( CreatedID ) ;

            InstanceVoidCutUtils.AddInstanceVoidCut( doc, hostElem, cutElem ) ;
          }

          doc.Delete( selLine ) ;
          t.Commit() ;
        }
        catch ( OperationCanceledException e ) {
        }
        catch ( Exception e ) {
        }
      }

      return true ;
    }

    /// <summary>
    /// 壁掘削線分選択動作
    /// </summary>
    /// <param name="uiapp"></param>
    /// <returns></returns>
    public bool CreateKussakuBoidKabeLineSelect( UIApplication uiapp )
    {
      UIDocument uidoc = uiapp.ActiveUIDocument ;
      Document doc = uidoc.Document ;

      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        ISelectionFilter selFilter = new WallSelectionFilter() ;
        Selection selection = uidoc.Selection ;

        Autodesk.Revit.DB.View v = getView( doc, "レベル 1", ViewType.FloorPlan ) ;
        try {
          Reference picWall = selection.PickObject( ObjectType.Element, selFilter, "壁を選択してください" ) ;
          if ( picWall == null ) {
            return false ;
          }

          //選択した部材ID
          ElementId selId = picWall.ElementId ;

          //線分選択
          Reference picLine = selection.PickObject( ObjectType.Element, "線分を選択してください" ) ;
          ElementId selLine = picLine.ElementId ;
          Element line = null ;
          try {
            line = doc.GetElement( selLine ) ;
          }
          catch ( Exception ) {
            throw ;
          }

          if ( line == null ) {
            //ClsMessage.ShowMessageBox("壁を選択してください。", System.Windows.Forms.MessageBoxIcon.Error);
            return false ;
          }

          //ソイル位置作成
          Curve cv = ( line.Location as LocationCurve ).Curve ;
          XYZ lineOrigin = cv.GetEndPoint( 0 ) ;
          XYZ lineEndPoint = cv.GetEndPoint( 1 ) ;
          XYZ soilDirection = lineEndPoint - lineOrigin ;
          double wallLength = soilDirection.GetLength() ;
          XYZ p = new XYZ( ( lineOrigin.X + lineEndPoint.X ) / 2.0, ( lineOrigin.Y + lineEndPoint.Y ) / 2.0,
            ( lineOrigin.Z + lineEndPoint.Z ) / 2.0 ) ;
          //掘削シンボル作成
          FamilySymbol sym = GetFamilySymbol( doc, "掘削用ボイド", "掘削用ボイド" ) ;
          if ( sym == null ) {
            return false ;
          }

          t.Start() ;
          ElementId levelID = GetLevelID( doc, "レベル 1" ) ;
          ElementId CreatedID = Create( doc, p, levelID, sym ) ;

          //パラメーターの設定処理
          SetParameter( doc, CreatedID, "長さ", wallLength ) ;


          DLG.DlgCreateVoidFamily m_MyForm = Application.thisApp.GetForm() ;

          if ( m_MyForm != null && m_MyForm.Visible ) {
            SetParameter( doc, CreatedID, "掘削深さ1", ClsRevitUtil.CovertToAPI( m_MyForm.GetKussaku( 1 ) ) ) ;
            SetParameter( doc, CreatedID, "掘削深さ2", ClsRevitUtil.CovertToAPI( m_MyForm.GetKussaku( 2 ) ) ) ;
          }

          //切り取り処理
          Element hostElem = doc.GetElement( selId ) ;
          Element cutElem = doc.GetElement( CreatedID ) ;

          InstanceVoidCutUtils.AddInstanceVoidCut( doc, hostElem, cutElem ) ;
          doc.Delete( selLine ) ;
          t.Commit() ;
        }
        catch ( OperationCanceledException e ) {
        }
        catch ( Exception e ) {
        }
      }

      return true ;
    }


    /// <summary>
    /// 図面上のCASE情報全てをダイアログに表示
    /// </summary>
    /// <param name="uiapp"></param>
    /// <returns></returns>
    public bool UpdateCASEInfo( UIApplication uiapp )
    {
      UIDocument uidoc = uiapp.ActiveUIDocument ;
      Document doc = uidoc.Document ;

      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        ISelectionFilter selFilter = new WallSelectionFilter() ;
        Selection selection = uidoc.Selection ;
        List<ElementId> caseElementIds = new List<ElementId>() ;
        Autodesk.Revit.DB.View v = getView( doc, "レベル 1", ViewType.FloorPlan ) ;
        try {
          //すべてのCASEファミリを取得
          caseElementIds = GetFamilyInstanceList( doc, "ファミリCASE作成TEST", "ファミリCASE作成TEST" ) ;
          //caseElementIds = GetAllCreatedFamilyInstanceList(doc);
          if ( caseElementIds.Count < 1 ) {
            return false ;
          }

          DLG.DlgCASETest m_FormCASE = Application.thisApp.GetForm_dlgCASETest() ;

          if ( m_FormCASE != null && m_FormCASE.Visible ) {
            List<string> allCASEList = new List<string>() ;

            for ( int i = 0 ; i < caseElementIds.Count ; i++ ) {
              allCASEList.Add( GetParameter( doc, caseElementIds[ i ], "CASE文字" ) ) ;
            }

            m_FormCASE.SetDataGridViewUp( allCASEList ) ;
          }

          t.Commit() ;
        }
        catch ( OperationCanceledException e ) {
        }
        catch ( Exception e ) {
        }
      }

      return true ;
    }

    public bool UpdateSelectedCASEInfo( UIApplication uiapp )
    {
      UIDocument uidoc = uiapp.ActiveUIDocument ;
      Document doc = uidoc.Document ;

      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        ISelectionFilter selFilter = new WallSelectionFilter() ;
        Selection selection = uidoc.Selection ;
        List<ElementId> caseElementIds = new List<ElementId>() ;
        Autodesk.Revit.DB.View v = getView( doc, "レベル 1", ViewType.FloorPlan ) ;
        try {
          //すべてのCASEファミリを取得
          ICollection<ElementId> selectionIds = selection.GetElementIds() ;
          foreach ( ElementId id in selectionIds ) {
            caseElementIds.Add( id ) ;
          }

          //caseElementIds = GetFamilyInstanceList(doc, "ファミリCASE作成TEST", "ファミリCASE作成TEST");
          //caseElementIds = GetAllCreatedFamilyInstanceList(doc);
          if ( caseElementIds.Count < 1 ) {
            return false ;
          }

          DLG.DlgCASETest m_FormCASE = Application.thisApp.GetForm_dlgCASETest() ;

          if ( m_FormCASE != null && m_FormCASE.Visible ) {
            List<string> allCASEList = new List<string>() ;

            for ( int i = 0 ; i < caseElementIds.Count ; i++ ) {
              allCASEList.Add( GetParameter( doc, caseElementIds[ i ], "CASE文字" ) ) ;
            }

            m_FormCASE.SetDataGridViewLo( allCASEList ) ;
          }

          t.Commit() ;
        }
        catch ( OperationCanceledException e ) {
        }
        catch ( Exception e ) {
        }
      }

      return true ;
    }


    /// <summary>
    /// 指定ビューを取得
    /// </summary>
    /// <param name="doc">Document</param>
    /// <param name="name">ビュー名</param>
    public static Autodesk.Revit.DB.View getView( Document doc, string viewName, ViewType type )
    {
      Autodesk.Revit.DB.View getV = null ;

      IList<Element> views = new FilteredElementCollector( doc ).OfClass( typeof( Autodesk.Revit.DB.View ) )
        .ToElements() ;
      foreach ( Autodesk.Revit.DB.View v in views ) {
        if ( v.Name == viewName && v.ViewType == type ) {
          getV = v ;
          break ;
        }
      }

      return getV ;
    }

    /// <summary>
    /// FamilySymbol取得
    /// </summary>
    /// <param name="doc">Document</param>
    /// <param name="familyName">ファミリ名</param>
    /// <param name="typeName">タイプ名</param>
    /// <param name="categoryName">カテゴリ名</param>
    /// <returns></returns>
    public static FamilySymbol GetFamilySymbol( Document doc, string familyName, string typeName,
      string categoryName = "" )
    {
      if ( doc == null ) {
        return null ;
      }

      FamilySymbol symbol = null ;
      IEnumerable<Element> elems = null ;
      if ( string.IsNullOrEmpty( categoryName ) ) {
        elems = from elem in ( new FilteredElementCollector( doc ).OfClass( typeof( FamilySymbol ) ).ToElements() )
          let sym = elem as FamilySymbol
          where sym.Name == typeName && sym.Family.Name == familyName
          select sym ;
      }
      else {
        elems = from elem in ( new FilteredElementCollector( doc ).OfClass( typeof( FamilySymbol ) ).ToElements() )
          let sym = elem as FamilySymbol
          where sym.Name == typeName && sym.Family.Name == familyName && sym.Family.FamilyCategory.Name == categoryName
          select sym ;
      }

      foreach ( Element elemTmp in elems ) {
        symbol = elemTmp as FamilySymbol ;
        break ;
      }

      return symbol ;
    }

    public static ElementId GetLevelID( Document doc, string levelName )
    {
      if ( doc == null ) {
        return null ;
      }

      ElementId elemId = null ;
      IEnumerable<Element> elems = null ;
      elems = from elem in ( new FilteredElementCollector( doc ).OfClass( typeof( Level ) ).ToElements() )
        let level = elem as Level
        where level.Name == levelName
        select level ;

      foreach ( Level lv in elems ) {
        elemId = lv.Id ;
      }

      return elemId ;
    }

    /// <summary>
    /// 1点指示のファミリ作図
    /// </summary>
    /// <param name="doc">ドキュメント</param>
    /// <param name="point">座標</param>
    /// <param name="levelId">レベル</param>
    /// <param name="symbol">ファミリシンボル</param>
    /// <returns></returns>
    public ElementId Create( Document doc, XYZ point, ElementId levelId, FamilySymbol symbol )
    {
      if ( symbol == null ) {
        return null ;
      }

      ElementId id = null ;

      if ( ! symbol.IsActive ) {
        symbol.Activate() ;
      }

      try {
        Level lv = doc.GetElement( levelId ) as Level ;
        FamilyInstance newInstance = doc.Create.NewFamilyInstance( point, symbol, lv,
          Autodesk.Revit.DB.Structure.StructuralType.NonStructural ) ;

        //配置後に改めてデフォルト値設定
        ModifyLevel( doc, newInstance.Id, levelId ) ;
        ModifyOffset( doc, newInstance.Id, 0 ) ;
        id = newInstance.Id ;
      }
      catch {
        return null ;
      }

      return id ;
    }

    /// <summary>
    /// Doubleパラメーターの変更
    /// </summary>
    /// <param name="doc">ドキュメント</param>
    /// <param name="CreatedID">シンボルID</param>
    /// <param name="paramName">パラメーター名</param>
    /// <param name="val">Double値</param>
    /// <returns></returns>
    public static bool SetParameter( Document doc, ElementId CreatedID, string paramName, Double val )
    {
      Parameter parm = doc.GetElement( CreatedID ).LookupParameter( paramName ) ;
      if ( parm == null ) {
        return false ;
      }

      parm.Set( val ) ;
      return true ;
    }

    /// <summary>
    /// Stringパラメーターの変更
    /// </summary>
    /// <param name="doc">ドキュメント</param>
    /// <param name="CreatedID">シンボルID</param>
    /// <param name="paramName">パラメーター名</param>
    /// <returns></returns>
    public static string GetParameter( Document doc, ElementId CreatedID, string paramName )
    {
      try {
        Parameter parm = doc.GetElement( CreatedID ).LookupParameter( paramName ) ;
        if ( parm == null ) {
          return string.Empty ;
        }

        return parm.AsValueString() ;
      }
      catch {
        return string.Empty ;
      }
    }

    public static ElementId Create( Document doc, Curve cv, ElementId levelId, FamilySymbol symbol )
    {
      if ( symbol == null ) {
        return null ;
      }

      ElementId id = null ;

      if ( ! symbol.IsActive ) {
        symbol.Activate() ;
      }

      try {
        Level lv = doc.GetElement( levelId ) as Level ;
        FamilyInstance newInstance = doc.Create.NewFamilyInstance( cv, symbol, lv,
          Autodesk.Revit.DB.Structure.StructuralType.NonStructural ) ;

        //配置後に改めてデフォルト値設定
        ModifyLevel( doc, newInstance.Id, levelId ) ;
        ModifyOffset( doc, newInstance.Id, 0 ) ;
        id = newInstance.Id ;
      }
      catch {
        return null ;
      }

      return id ;
    }

    /// <summary>
    /// レベル変更
    /// </summary>
    /// <param name="doc">ドキュメント</param>
    /// <param name="id">ID</param>
    /// <param name="levelId"></param>
    /// <returns></returns>
    public static bool ModifyLevel( Document doc, ElementId id, ElementId levelId )
    {
      if ( id == null || doc == null ) {
        return false ;
      }

      Parameter param = doc.GetElement( id ).get_Parameter( BuiltInParameter.FAMILY_LEVEL_PARAM ) ;
      if ( param == null ) {
        return false ;
      }

      if ( param.IsReadOnly ) {
        return false ;
      }

      param.Set( levelId ) ;

      return true ;
    }

    public static bool ModifyOffset( Document doc, ElementId id, double offsetValue )
    {
      if ( id == null || doc == null ) {
        return false ;
      }

      Parameter param = doc.GetElement( id ).get_Parameter( BuiltInParameter.INSTANCE_FREE_HOST_OFFSET_PARAM ) ;
      if ( param == null ) {
        return false ;
      }

      if ( param.IsReadOnly ) {
        return false ;
      }

      param.Set( offsetValue ) ;
      return true ;
    }

    /// <summary>
    /// LiNE選択オブジェクト取得
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="picLine"></param>
    /// <returns>選択したLINEのElement</returns>
    public static Element GetSelectLine( Document doc, Reference picLine )
    {
      Element selLine = null ;
      if ( picLine == null ) {
        return selLine ;
      }

      //選択した部材ID
      ElementId selId = picLine.ElementId ;
      try {
        selLine = doc.GetElement( selId ) ;
      }
      catch ( Exception ) {
        throw ;
      }

      return selLine ;
    }

    /// <summary>
    /// 指定条件に一致したFamilyInstance群取得
    /// </summary>
    /// <param name="doc">Document</param>
    /// <param name="familyName">ファミリ名</param>
    /// <param name="typeName">タイプ名</param>
    /// <param name="categoryName">カテゴリ名</param>
    /// <param name="levelName">レベル名</param>
    /// <returns></returns>
    public static List<ElementId> GetFamilyInstanceList( Document doc, string familyName, string typeName,
      string categoryName = "", string levelName = "" )
    {
      List<ElementId> idList = new List<ElementId>() ;
      IEnumerable<Element> idIe = GetFamilyInstance( doc, familyName, typeName, categoryName, levelName ) ;
      foreach ( Element elem in idIe ) {
        idList.Add( elem.Id ) ;
      }

      return idList ;
    }

    public static IEnumerable<Element> GetFamilyInstance( Document doc, string familyName, string typeName,
      string categoryName = "", string levelName = "" )
    {
      if ( doc == null ) {
        return null ;
      }

      IEnumerable<Element> elems = null ;
      if ( string.IsNullOrEmpty( categoryName ) ) {
        elems = from elem in ( new FilteredElementCollector( doc ).OfClass( typeof( FamilyInstance ) ).ToElements() )
          let sym = elem as FamilyInstance
          where sym.Name == typeName && sym.Symbol.FamilyName == familyName
          select sym ;
      }
      else {
        elems = from elem in ( new FilteredElementCollector( doc ).OfClass( typeof( FamilyInstance ) ).ToElements() )
          let sym = elem as FamilyInstance
          where sym.Name == typeName && sym.Symbol.FamilyName == familyName && sym.Category.Name == categoryName
          select sym ;
      }

      //(doc.GetElement(sym.LevelId) as Level).Name == levelName ×
      //levelnameが空でなければレベル名でフィルタをかける
      if ( levelName != "" ) {
        elems = FilterFamilyInstanceLevelName( doc, elems, levelName ) ;
      }

      return elems ;
    }

    /// <summary>
    /// レベル名でフィルターをかける
    /// </summary>
    /// <param name="doc">docment</param>
    /// <param name="elems">Element群</param>
    /// <param name="levelName">レベル名</param>
    /// <returns></returns>
    private static IEnumerable<Element> FilterFamilyInstanceLevelName( Document doc, IEnumerable<Element> elems,
      string levelName )
    {
      if ( doc == null ) {
        return null ;
      }

      IEnumerable<Element> ret = null ;

      ret = from elem in elems
        let sym = elem as FamilyInstance
        where true == CheckEQLevelName( doc, sym, levelName )
        select sym ;

      return ret ;
    }

    /// <summary>
    /// FamilyInstaceのレベル名はlevelNameと同一か
    /// </summary>
    /// <param name="doc">document</param>
    /// <param name="sym">FamilyInstace</param>
    /// <param name="levelName">比較対象</param>
    /// <returns></returns>
    private static bool CheckEQLevelName( Document doc, FamilyInstance Ins, string levelName )
    {
      if ( doc == null ) {
        return false ;
      }

      bool ret = false ;

      //※要素によってそれぞれチェックするレベルのパラメータが異なる
      Parameter beamParam = doc.GetElement( Ins.Id ).get_Parameter( BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM ) ;
      Parameter clmParam = doc.GetElement( Ins.Id ).get_Parameter( BuiltInParameter.FAMILY_BASE_LEVEL_PARAM ) ;

      if ( beamParam == null && clmParam == null ) {
        return false ;
      }

      string paramLvl = string.Empty ;
      if ( beamParam != null ) {
        paramLvl = beamParam.AsValueString() ;
      }
      else if ( clmParam != null ) {
        paramLvl = clmParam.AsValueString() ;
      }


      if ( paramLvl == levelName ) {
        ret = true ;
      }

      return ret ;
    }

    /// <summary>
    /// 作図済みの全FamilyInstanceを取得
    /// </summary>
    /// <returns></returns>
    public static List<ElementId> GetAllCreatedFamilyInstanceList( Document doc )
    {
      List<ElementId> idList = new List<ElementId>() ;
      IEnumerable<Element> idIe = GetAllCreatedFamilyInstance( doc ) ;
      foreach ( Element elem in idIe ) {
        idList.Add( elem.Id ) ;
      }

      return idList ;
    }

    public static IEnumerable<Element> GetAllCreatedFamilyInstance( Document doc )
    {
      IEnumerable<Element> elems = null ;

      elems = from elem in ( new FilteredElementCollector( doc ).OfClass( typeof( FamilyInstance ) ).ToElements() )
        let sym = elem as FamilyInstance
        select sym ;

      return elems ;
    }
  } // class
} // namespace