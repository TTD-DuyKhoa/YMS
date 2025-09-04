//
// (C) Copyright 2003-2019 by Autodesk, Inc. All rights reserved.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.

//
// AUTODESK PROVIDES THIS PROGRAM 'AS IS' AND WITH ALL ITS FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.

using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using System.Collections.Generic ;
using System.Linq ;
using System.Windows.Forms ;
using YMS_gantry.Data ;

namespace YMS_gantry
{
  public class Application : IExternalApplication
  {
    // class instance

    internal static Application thisApp = new Application() ;

    // ModelessForm instance
    public UI.FrmGrouping frmGrouping ;
    public UI.FrmPutFukkouban frmPutFukkouban ;
    public UI.FrmPutOhbikiKetauke frmPutOhbikiKetauke ;
    public UI.FrmPutNedaShugeta frmPutNedaShugeta ;
    public UI.FrmPutFukkougeta frmPutFukkougeta ;
    public UI.FrmPutTaikeikou frmPutTaikeikou ;
    public UI.FrmPutShikigeta frmPutShikigeta ;
    public UI.FrmPutSuchifuna frmPutSuchifuna ;
    public UI.FrmPutTakasaChouseiPlateChouseizai frmPutTakasaChouseiPlateChouseizai ;
    public UI.FrmPutKoubanzai frmPutKoubanzai ;
    public UI.FrmPutJifukuFukkoubanZuredomezai frmPutJifukuFukkoubanZuredomezai ;
    public UI.FrmPutTesuriTesuriShichu frmPutTesuriTesuriShichu ;
    public UI.FrmPutHorizontalBrace frmPutHorizontalBrace ;
    public UI.FrmPutVerticalBrace frmPutVerticalBrace ;
    public UI.FrmPutTeiketsuHojyozai frmPutTeiketsuHojyozai ;
    public UI.FrmPutHorizontalTsunagi frmPutHorizontalTsunagi ;
    public UI.FrmPutHoudue frmPutHoudue ;
    public UI.FrmPutShichu frmPutShichu ;
    public UI.FrmPutKui frmPutKui ;
    public UI.FrmEditSizeList frmEditSizeList ;
    public UI.FrmEditLocationChange frmEditLocationChange ;
    public UI.FrmEditLengthChange frmEditLengthChange ;
    public UI.FrmSonotaKanshouCheck frmSonotaKanshouCheck ;
    public UI.FrmWaritsukeElement frmWaritsuke ;

    public Result OnShutdown( UIControlledApplication application )
    {
      if ( frmGrouping != null && frmGrouping.Visible ) {
        frmGrouping.Close() ;
      }

      if ( frmPutFukkouban != null && frmPutFukkouban.Visible ) {
        frmPutFukkouban.Close() ;
      }

      if ( frmPutOhbikiKetauke != null && frmPutOhbikiKetauke.Visible ) {
        frmPutOhbikiKetauke.Close() ;
      }

      if ( frmPutNedaShugeta != null && frmPutNedaShugeta.Visible ) {
        frmPutNedaShugeta.Close() ;
      }

      if ( frmPutFukkougeta != null && frmPutFukkougeta.Visible ) {
        frmPutFukkougeta.Close() ;
      }

      if ( frmPutTaikeikou != null && frmPutTaikeikou.Visible ) {
        frmPutTaikeikou.Close() ;
      }

      if ( frmPutShikigeta != null && frmPutShikigeta.Visible ) {
        frmPutShikigeta.Close() ;
      }

      if ( frmPutSuchifuna != null && frmPutSuchifuna.Visible ) {
        frmPutSuchifuna.Close() ;
      }

      if ( frmPutTakasaChouseiPlateChouseizai != null && frmPutTakasaChouseiPlateChouseizai.Visible ) {
        frmPutTakasaChouseiPlateChouseizai.Close() ;
      }

      if ( frmPutKoubanzai != null && frmPutKoubanzai.Visible ) {
        frmPutKoubanzai.Close() ;
      }

      if ( frmPutJifukuFukkoubanZuredomezai != null && frmPutJifukuFukkoubanZuredomezai.Visible ) {
        frmPutJifukuFukkoubanZuredomezai.Close() ;
      }

      if ( frmPutTesuriTesuriShichu != null && frmPutTesuriTesuriShichu.Visible ) {
        frmPutTesuriTesuriShichu.Close() ;
      }

      if ( frmPutHorizontalBrace != null && frmPutHorizontalBrace.Visible ) {
        frmPutHorizontalBrace.Close() ;
      }

      if ( frmPutVerticalBrace != null && frmPutVerticalBrace.Visible ) {
        frmPutVerticalBrace.Close() ;
      }

      if ( frmPutTeiketsuHojyozai != null && frmPutTeiketsuHojyozai.Visible ) {
        frmPutTeiketsuHojyozai.Close() ;
      }

      if ( frmPutHorizontalTsunagi != null && frmPutHorizontalTsunagi.Visible ) {
        frmPutHorizontalTsunagi.Close() ;
      }

      if ( frmPutHoudue != null && frmPutHoudue.Visible ) {
        frmPutHoudue.Close() ;
      }

      if ( frmPutShichu != null && frmPutShichu.Visible ) {
        frmPutShichu.Close() ;
      }

      if ( frmPutKui != null && frmPutKui.Visible ) {
        frmPutKui.Close() ;
      }

      if ( frmEditSizeList != null && frmEditSizeList.Visible ) {
        frmEditSizeList.Close() ;
      }

      if ( frmEditLocationChange != null && frmEditLocationChange.Visible ) {
        frmEditLocationChange.Close() ;
      }

      if ( frmEditLengthChange != null && frmEditLengthChange.Visible ) {
        frmEditLengthChange.Close() ;
      }

      if ( frmSonotaKanshouCheck != null && frmSonotaKanshouCheck.Visible ) {
        frmSonotaKanshouCheck.Close() ;
      }

      if ( frmWaritsuke != null && frmWaritsuke.Visible ) {
        frmWaritsuke.Close() ;
      }

      return Result.Succeeded ;
    }

    public Result OnStartup( UIControlledApplication application )
    {
      frmGrouping = null ;
      frmPutFukkouban = null ;
      frmPutOhbikiKetauke = null ;
      frmPutNedaShugeta = null ;
      frmPutFukkougeta = null ;
      frmPutTaikeikou = null ;
      frmPutShikigeta = null ;
      frmPutSuchifuna = null ;
      frmPutTakasaChouseiPlateChouseizai = null ;
      frmPutKoubanzai = null ;
      frmPutJifukuFukkoubanZuredomezai = null ;
      frmPutTesuriTesuriShichu = null ;
      frmPutHorizontalBrace = null ;
      frmPutVerticalBrace = null ;
      frmPutTeiketsuHojyozai = null ;
      frmPutHorizontalTsunagi = null ;
      frmPutHoudue = null ;
      frmPutShichu = null ;
      frmPutKui = null ;
      frmEditSizeList = null ;
      frmEditLocationChange = null ;
      frmEditLengthChange = null ;
      frmSonotaKanshouCheck = null ;
      frmWaritsuke = null ;
      thisApp = this ; // static access to this application instance

      return Result.Succeeded ;
    }
    // ModelessForm instance

    public void ShowForm_FrmGrouping( UIApplication uiapp )
    {
      if ( frmGrouping == null || frmGrouping.IsDisposed ) {
        RequestHandler handler = new RequestHandler() ;

        ExternalEvent exEvent = ExternalEvent.Create( handler ) ;

        frmGrouping = new UI.FrmGrouping( exEvent, handler, uiapp ) ;
        frmGrouping.Show() ;
      }
    }

    public void ShowForm_FrmPutFukkouban( UIApplication uiapp )
    {
      if ( frmPutFukkouban == null || frmPutFukkouban.IsDisposed ) {
        RequestHandler handler = new RequestHandler() ;

        ExternalEvent exEvent = ExternalEvent.Create( handler ) ;

        frmPutFukkouban = new UI.FrmPutFukkouban( exEvent, handler, uiapp ) ;
        frmPutFukkouban.Show() ;
      }
    }

    public void ShowForm_FrmPutOhbikiKetauke( UIApplication uiapp )
    {
      if ( frmPutOhbikiKetauke == null || frmPutOhbikiKetauke.IsDisposed ) {
        RequestHandler handler = new RequestHandler() ;

        ExternalEvent exEvent = ExternalEvent.Create( handler ) ;

        frmPutOhbikiKetauke = new UI.FrmPutOhbikiKetauke( exEvent, handler, uiapp ) ;
        frmPutOhbikiKetauke.Show() ;
      }
    }

    public void ShowForm_FrmPutNedaShugeta( UIApplication uiapp )
    {
      if ( frmPutNedaShugeta == null || frmPutNedaShugeta.IsDisposed ) {
        RequestHandler handler = new RequestHandler() ;

        ExternalEvent exEvent = ExternalEvent.Create( handler ) ;

        frmPutNedaShugeta = new UI.FrmPutNedaShugeta( exEvent, handler, uiapp ) ;
        frmPutNedaShugeta.Show() ;
      }
    }

    public void ShowForm_FrmPutFukkougeta( UIApplication uiapp )
    {
      if ( frmPutFukkougeta == null || frmPutFukkougeta.IsDisposed ) {
        RequestHandler handler = new RequestHandler() ;

        ExternalEvent exEvent = ExternalEvent.Create( handler ) ;

        frmPutFukkougeta = new UI.FrmPutFukkougeta( exEvent, handler, uiapp ) ;
        frmPutFukkougeta.Show() ;
      }
    }

    public void ShowForm_FrmPutTaikeikou( UIApplication uiapp )
    {
      if ( frmPutTaikeikou == null || frmPutTaikeikou.IsDisposed ) {
        RequestHandler handler = new RequestHandler() ;

        ExternalEvent exEvent = ExternalEvent.Create( handler ) ;

        frmPutTaikeikou = new UI.FrmPutTaikeikou( exEvent, handler, uiapp ) ;
        frmPutTaikeikou.Show() ;
      }
    }

    public void ShowForm_FrmPutShikigeta( UIApplication uiapp )
    {
      if ( frmPutShikigeta == null || frmPutShikigeta.IsDisposed ) {
        RequestHandler handler = new RequestHandler() ;

        ExternalEvent exEvent = ExternalEvent.Create( handler ) ;

        frmPutShikigeta = new UI.FrmPutShikigeta( exEvent, handler, uiapp ) ;
        frmPutShikigeta.Show() ;
      }
    }

    public void ShowForm_FrmPutSuchifuna( UIApplication uiapp )
    {
      if ( frmPutSuchifuna == null || frmPutSuchifuna.IsDisposed ) {
        RequestHandler handler = new RequestHandler() ;

        ExternalEvent exEvent = ExternalEvent.Create( handler ) ;

        frmPutSuchifuna = new UI.FrmPutSuchifuna( exEvent, handler, uiapp ) ;
        frmPutSuchifuna.Show() ;
      }
    }

    public void ShowForm_FrmPutTakasaChouseiPlateChouseizai( UIApplication uiapp )
    {
      if ( frmPutTakasaChouseiPlateChouseizai == null || frmPutTakasaChouseiPlateChouseizai.IsDisposed ) {
        RequestHandler handler = new RequestHandler() ;

        ExternalEvent exEvent = ExternalEvent.Create( handler ) ;

        frmPutTakasaChouseiPlateChouseizai = new UI.FrmPutTakasaChouseiPlateChouseizai( exEvent, handler, uiapp ) ;
        frmPutTakasaChouseiPlateChouseizai.Show() ;
      }
    }

    public void ShowForm_FrmPutKoubanzai( UIApplication uiapp )
    {
      if ( frmPutKoubanzai == null || frmPutKoubanzai.IsDisposed ) {
        RequestHandler handler = new RequestHandler() ;

        ExternalEvent exEvent = ExternalEvent.Create( handler ) ;

        frmPutKoubanzai = new UI.FrmPutKoubanzai( exEvent, handler, uiapp ) ;
        frmPutKoubanzai.Show() ;
      }
    }

    public void ShowForm_FrmPutJifukuFukkoubanZuredomezai( UIApplication uiapp )
    {
      if ( frmPutJifukuFukkoubanZuredomezai == null || frmPutJifukuFukkoubanZuredomezai.IsDisposed ) {
        RequestHandler handler = new RequestHandler() ;

        ExternalEvent exEvent = ExternalEvent.Create( handler ) ;

        frmPutJifukuFukkoubanZuredomezai = new UI.FrmPutJifukuFukkoubanZuredomezai( exEvent, handler, uiapp ) ;
        frmPutJifukuFukkoubanZuredomezai.Show() ;
      }
    }

    public void ShowForm_FrmPutTesuriTesuriShichu( UIApplication uiapp )
    {
      if ( frmPutTesuriTesuriShichu == null || frmPutTesuriTesuriShichu.IsDisposed ) {
        RequestHandler handler = new RequestHandler() ;

        ExternalEvent exEvent = ExternalEvent.Create( handler ) ;

        frmPutTesuriTesuriShichu = new UI.FrmPutTesuriTesuriShichu( exEvent, handler, uiapp ) ;
        frmPutTesuriTesuriShichu.Show() ;
      }
    }

    public void ShowForm_FrmPutHorizontalBrace( UIApplication uiapp )
    {
      if ( frmPutHorizontalBrace == null || frmPutHorizontalBrace.IsDisposed ) {
        RequestHandler handler = new RequestHandler() ;

        ExternalEvent exEvent = ExternalEvent.Create( handler ) ;

        frmPutHorizontalBrace = new UI.FrmPutHorizontalBrace( exEvent, handler, uiapp ) ;
        frmPutHorizontalBrace.Show() ;
      }
    }

    public void ShowForm_FrmPutVerticalBrace( UIApplication uiapp )
    {
      if ( frmPutVerticalBrace == null || frmPutVerticalBrace.IsDisposed ) {
        RequestHandler handler = new RequestHandler() ;

        ExternalEvent exEvent = ExternalEvent.Create( handler ) ;

        frmPutVerticalBrace = new UI.FrmPutVerticalBrace( exEvent, handler, uiapp ) ;
        frmPutVerticalBrace.Show() ;
      }
    }

    public void ShowForm_FrmPutTeiketsuHojyozai( UIApplication uiapp )
    {
      if ( frmPutTeiketsuHojyozai == null || frmPutTeiketsuHojyozai.IsDisposed ) {
        RequestHandler handler = new RequestHandler() ;

        ExternalEvent exEvent = ExternalEvent.Create( handler ) ;

        frmPutTeiketsuHojyozai = new UI.FrmPutTeiketsuHojyozai( exEvent, handler, uiapp ) ;
        frmPutTeiketsuHojyozai.Show() ;
      }
    }

    public void ShowForm_FrmPutHorizontalTsunagi( UIApplication uiapp )
    {
      if ( frmPutHorizontalTsunagi == null || frmPutHorizontalTsunagi.IsDisposed ) {
        RequestHandler handler = new RequestHandler() ;

        ExternalEvent exEvent = ExternalEvent.Create( handler ) ;

        frmPutHorizontalTsunagi = new UI.FrmPutHorizontalTsunagi( exEvent, handler, uiapp ) ;
        var doc = uiapp.ActiveUIDocument.Document ;
        var modelData = ModelData.GetModelData( doc ) ;
        foreach ( var x in modelData.ListKoudaiData.Select( x => x.AllKoudaiFlatData ) )
          frmPutHorizontalTsunagi.ViewModel.KodaiSet.Add( x ) ;
        frmPutHorizontalTsunagi.ViewModel.RevitDoc = doc ;
        frmPutHorizontalTsunagi.Show() ;
      }
    }

    public void ShowForm_FrmPutHoudue( UIApplication uiapp )
    {
      if ( frmPutHoudue == null || frmPutHoudue.IsDisposed ) {
        RequestHandler handler = new RequestHandler() ;

        ExternalEvent exEvent = ExternalEvent.Create( handler ) ;

        frmPutHoudue = new UI.FrmPutHoudue( exEvent, handler, uiapp ) ;
        frmPutHoudue.Show() ;
      }
    }

    public void ShowForm_FrmPutShichu( UIApplication uiapp )
    {
      if ( frmPutShichu == null || frmPutShichu.IsDisposed ) {
        RequestHandler handler = new RequestHandler() ;

        ExternalEvent exEvent = ExternalEvent.Create( handler ) ;

        frmPutShichu = new UI.FrmPutShichu( exEvent, handler, uiapp ) ;
        frmPutShichu.Show() ;
      }
    }

    public void ShowForm_FrmPutKui( UIApplication uiapp )
    {
      if ( frmPutKui == null || frmPutKui.IsDisposed ) {
        RequestHandler handler = new RequestHandler() ;

        ExternalEvent exEvent = ExternalEvent.Create( handler ) ;

        frmPutKui = new UI.FrmPutKui( exEvent, handler, uiapp ) ;
        frmPutKui.Show() ;
      }
    }

    public void ShowForm_FrmEditSizeList( UIApplication uiapp, List<ElementId> targetElementIdList )
    {
      if ( frmEditSizeList == null || frmEditSizeList.IsDisposed ) {
        RequestHandler handler = new RequestHandler() ;

        ExternalEvent exEvent = ExternalEvent.Create( handler ) ;

        frmEditSizeList = new UI.FrmEditSizeList( exEvent, handler, uiapp, targetElementIdList ) ;
        frmEditSizeList.Show() ;
      }
    }

    public void ShowForm_FrmEditLocationChange( UIApplication uiapp, ElementId elementId )
    {
      if ( frmEditLocationChange == null || frmEditLocationChange.IsDisposed ) {
        RequestHandler handler = new RequestHandler() ;

        ExternalEvent exEvent = ExternalEvent.Create( handler ) ;

        frmEditLocationChange = new UI.FrmEditLocationChange( exEvent, handler, uiapp, elementId ) ;
        frmEditLocationChange.Show() ;
      }
    }

    public void ShowForm_FrmEditLengthChange( UIApplication uiapp, List<ElementId> targetElementIdList )
    {
      if ( frmEditLengthChange == null || frmEditLengthChange.IsDisposed ) {
        RequestHandler handler = new RequestHandler() ;

        ExternalEvent exEvent = ExternalEvent.Create( handler ) ;

        frmEditLengthChange = new UI.FrmEditLengthChange( exEvent, handler, uiapp, targetElementIdList ) ;
        frmEditLengthChange.Show() ;
      }
    }

    public void ShowForm_Waritsuke( UIApplication uiapp )
    {
      if ( frmWaritsuke == null || frmWaritsuke.IsDisposed ) {
        RequestHandler handler = new RequestHandler() ;

        ExternalEvent exEvent = ExternalEvent.Create( handler ) ;

        frmWaritsuke = new UI.FrmWaritsukeElement( exEvent, handler, uiapp ) ;
        frmWaritsuke.Show() ;
      }
    }

    public void ShowForm_FrmSonotaKanshouCheck( UIApplication uiapp )
    {
      if ( frmSonotaKanshouCheck == null || frmSonotaKanshouCheck.IsDisposed ) {
        RequestHandler handler = new RequestHandler() ;

        ExternalEvent exEvent = ExternalEvent.Create( handler ) ;

        frmSonotaKanshouCheck = new UI.FrmSonotaKanshouCheck( exEvent, handler, uiapp ) ;
        frmSonotaKanshouCheck.Show() ;
      }
    }

    public void WakeFormUp()
    {
      if ( frmGrouping != null ) {
        frmGrouping.WakeUp() ;
      }

      if ( frmPutFukkouban != null ) {
        frmPutFukkouban.WakeUp() ;
      }

      if ( frmPutOhbikiKetauke != null ) {
        frmPutOhbikiKetauke.WakeUp() ;
      }

      if ( frmPutNedaShugeta != null ) {
        frmPutNedaShugeta.WakeUp() ;
      }

      if ( frmPutFukkougeta != null ) {
        frmPutFukkougeta.WakeUp() ;
      }

      if ( frmPutTaikeikou != null ) {
        frmPutTaikeikou.WakeUp() ;
      }

      if ( frmPutShikigeta != null ) {
        frmPutShikigeta.WakeUp() ;
      }

      if ( frmPutSuchifuna != null ) {
        frmPutSuchifuna.WakeUp() ;
      }

      if ( frmPutTakasaChouseiPlateChouseizai != null ) {
        frmPutTakasaChouseiPlateChouseizai.WakeUp() ;
      }

      if ( frmPutKoubanzai != null ) {
        frmPutKoubanzai.WakeUp() ;
      }

      if ( frmPutJifukuFukkoubanZuredomezai != null ) {
        frmPutJifukuFukkoubanZuredomezai.WakeUp() ;
      }

      if ( frmPutTesuriTesuriShichu != null ) {
        frmPutTesuriTesuriShichu.WakeUp() ;
      }

      if ( frmPutHorizontalBrace != null ) {
        frmPutHorizontalBrace.WakeUp() ;
      }

      if ( frmPutVerticalBrace != null ) {
        frmPutVerticalBrace.WakeUp() ;
      }

      if ( frmPutTeiketsuHojyozai != null ) {
        frmPutTeiketsuHojyozai.WakeUp() ;
      }

      if ( frmPutHorizontalTsunagi != null ) {
        frmPutHorizontalTsunagi.WakeUp() ;
      }

      if ( frmPutHoudue != null ) {
        frmPutHoudue.WakeUp() ;
      }

      if ( frmPutShichu != null ) {
        frmPutShichu.WakeUp() ;
      }

      if ( frmPutKui != null ) {
        frmPutKui.WakeUp() ;
      }

      if ( frmEditSizeList != null ) {
        frmEditSizeList.WakeUp() ;
      }

      if ( frmEditLocationChange != null ) {
        frmEditLocationChange.WakeUp() ;
      }

      if ( frmEditLengthChange != null ) {
        frmEditLengthChange.WakeUp() ;
      }

      if ( frmSonotaKanshouCheck != null ) {
        frmSonotaKanshouCheck.WakeUp() ;
      }

      if ( frmWaritsuke != null ) {
        frmWaritsuke.WakeUp() ;
      }
    }
  }
}