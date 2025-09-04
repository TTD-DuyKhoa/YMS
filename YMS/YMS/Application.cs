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
using System.Windows.Forms ;
using YMS.Command ;
using YMS.DLG ;
using YMS.Parts ;

namespace YMS
{
  /// <summary>
  /// Implements the Revit add-in interface IExternalApplication
  /// </summary>
  public class Application : IExternalApplication
  {
    // class instance
    internal static Application thisApp = new Application() ;

    // ModelessForm instance
    private DLG.ModelessFormTest m_MyForm ;
    private DLG.DlgCreateVoidFamily dlgCreateVoidFamily ;
    private DLG.DlgCASETest dlgCASETest ;
    private DLG.DlgCreateSanjiku dlgCreateSanjiku ;
    private DLG.DlgCreateCornerHiuchiBase dlgCreateCornerHiuchi ;
    private DLG.DlgBaseList dlgBaseList ;
    private DLG.DlgWaritsuke dlgWaritsuke ;
    private DLG.DlgCASE dlgCASE ;

    #region IExternalApplication Members

    /// <summary>
    /// Implements the OnShutdown event
    /// </summary>
    /// <param name="application"></param>
    /// <returns></returns>
    public Result OnShutdown( UIControlledApplication application )
    {
      if ( m_MyForm != null && m_MyForm.Visible ) {
        m_MyForm.Close() ;
      }

      if ( dlgCreateVoidFamily != null && dlgCreateVoidFamily.Visible ) {
        dlgCreateVoidFamily.Close() ;
      }

      if ( dlgCASETest != null && dlgCASETest.Visible ) {
        dlgCASETest.Close() ;
      }

      if ( dlgCreateSanjiku != null && dlgCreateSanjiku.Visible ) {
        dlgCreateSanjiku.Close() ;
      }

      if ( dlgCreateCornerHiuchi != null && dlgCreateCornerHiuchi.Visible ) {
        dlgCreateCornerHiuchi.Close() ;
      }

      if ( dlgBaseList != null && dlgBaseList.Visible ) {
        dlgBaseList.Close() ;
      }

      if ( dlgWaritsuke != null && dlgWaritsuke.Visible ) {
        dlgWaritsuke.Close() ;
      }

      if ( dlgCASE != null && dlgCASE.Visible ) {
        dlgCASE.Close() ;
      }

      return Result.Succeeded ;
    }

    /// <summary>
    /// Implements the OnStartup event
    /// </summary>
    /// <param name="application"></param>
    /// <returns></returns>
    public Result OnStartup( UIControlledApplication application )
    {
      m_MyForm = null ; // no dialog needed yet; the command will bring it
      dlgCreateVoidFamily = null ;
      dlgCASETest = null ;
      dlgCreateSanjiku = null ;
      dlgCreateCornerHiuchi = null ;
      dlgBaseList = null ;
      dlgWaritsuke = null ;
      dlgCASE = null ;

      thisApp = this ; // static access to this application instance

      return Result.Succeeded ;
    }

    /// <summary>
    ///   This method creates and shows a modeless dialog, unless it already exists.
    /// </summary>
    /// <remarks>
    ///   The external command invokes this on the end-user's request
    /// </remarks>
    ///
    public void ShowForm( UIApplication uiapp )
    {
      // If we do not have a dialog yet, create and show it
      if ( m_MyForm == null || m_MyForm.IsDisposed ) {
        RequestHandler handler = new RequestHandler() ;

        ExternalEvent exEvent = ExternalEvent.Create( handler ) ;

        m_MyForm = new DLG.ModelessFormTest( exEvent, handler ) ;
        m_MyForm.Show() ;
      }
    }

    public void ShowFormdlgCreateVoidFamily( UIApplication uiapp )
    {
      if ( dlgCreateVoidFamily == null || dlgCreateVoidFamily.IsDisposed ) {
        RequestHandler handler = new RequestHandler() ;

        ExternalEvent exEvent = ExternalEvent.Create( handler ) ;

        dlgCreateVoidFamily = new DLG.DlgCreateVoidFamily( exEvent, handler ) ;
        dlgCreateVoidFamily.Show() ;
      }
    }

    public void ShowForm_dlgCASETest( UIApplication uiapp )
    {
      if ( dlgCASETest == null || dlgCASETest.IsDisposed ) {
        RequestHandler handler = new RequestHandler() ;

        ExternalEvent exEvent = ExternalEvent.Create( handler ) ;

        dlgCASETest = new DLG.DlgCASETest( exEvent, handler ) ;
        dlgCASETest.Show() ;
      }
    }

    public void ShowForm_dlgCreateSanjiku()
    {
      if ( dlgCreateSanjiku == null || dlgCreateSanjiku.IsDisposed ) {
        RequestHandler handler = new RequestHandler() ;

        ExternalEvent exEvent = ExternalEvent.Create( handler ) ;

        dlgCreateSanjiku = new DLG.DlgCreateSanjiku( exEvent, handler ) ;
        dlgCreateSanjiku.Show() ;
      }
    }

    public void ShowForm_dlgCreateCornerHiuchi( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;
      //ClsCornerHiuchiBase cornerHiuchi = new ClsCornerHiuchiBase();
      if ( dlgCreateCornerHiuchi == null || dlgCreateCornerHiuchi.IsDisposed ) {
        RequestHandler handler = new RequestHandler() ;

        ExternalEvent exEvent = ExternalEvent.Create( handler ) ;

        dlgCreateCornerHiuchi = new DLG.DlgCreateCornerHiuchiBase( exEvent, handler ) ;
        dlgCreateCornerHiuchi.Show() ;
      }
    }

    public void ShowForm_dlgChangeCornerHiuchi( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;
      //�N���X�Ƀf�[�^��ݒ�
      ClsCornerHiuchiBase cornerHiuchi = ClsCommandCornerHiuchiBase.ChangeCornerHiuchiBase( uidoc ) ;

      if ( dlgCreateCornerHiuchi == null || dlgCreateCornerHiuchi.IsDisposed ) {
        RequestHandler handler = new RequestHandler() ;

        ExternalEvent exEvent = ExternalEvent.Create( handler ) ;

        dlgCreateCornerHiuchi = new DLG.DlgCreateCornerHiuchiBase( cornerHiuchi, exEvent, handler ) ;
        dlgCreateCornerHiuchi.Show() ;
      }
    }

    public void ShowForm_dlgBaseList( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;
      //�h�L�������g����A�e�x�[�X���擾
      List<ClsHaraokoshiBase> lstH = ClsHaraokoshiBase.GetAllClsHaraokoshiBaseList( doc ) ;
      List<ClsKiribariBase> lstK = ClsKiribariBase.GetAllClsKiribariBaseList( doc ) ;
      List<ClsCornerHiuchiBase> lstCH = ClsCornerHiuchiBase.GetAllClsCornerHiuchiBaseList( doc ) ;
      List<ClsKiribariHiuchiBase> lstKH = ClsKiribariHiuchiBase.GetAllClsKiribariHiuchiBaseList( doc ) ;
      List<ClsKiribariUkeBase> lstKU = ClsKiribariUkeBase.GetAllClsKiribariUkeBaseList( doc ) ;
      List<ClsKiribariTsunagizaiBase> lstKT = ClsKiribariTsunagizaiBase.GetAllClsKiribariTsunagizaiBaseList( doc ) ;
      List<ClsHiuchiTsunagizaiBase> lstHT = ClsHiuchiTsunagizaiBase.GetAllClsHiuchiTsunagizaiBaseList( doc ) ;
      List<ClsKiribariTsugiBase> lstKTsugi = ClsKiribariTsugiBase.GetAllClsKiribariTsugiBaseList( doc ) ;
      var lstSB = ClsSyabariBase.GetAllClsBaseList( doc ) ;
      var lstST = ClsSyabariTsunagizaiBase.GetAllClsBaseList( doc ) ;
      var lstSU = ClsSyabariUkeBase.GetAllClsBaseList( doc ) ;

      if ( dlgBaseList == null || dlgBaseList.IsDisposed ) {
        RequestHandler handler = new RequestHandler() ;

        ExternalEvent exEvent = ExternalEvent.Create( handler ) ;

        dlgBaseList = new DLG.DlgBaseList( doc, exEvent, handler, lstH, lstK, lstCH, lstKH, lstKU, lstKT, lstHT,
          lstKTsugi, lstSB, lstST, lstSU ) ;
        dlgBaseList.Show() ;
      }
    }

    public void ShowForm_dlgWaritsuke( UIApplication uiapp, FamilyInstance instBase )
    {
      //UIDocument uidoc = uiapp.ActiveUIDocument;
      //Document doc = uidoc.Document;

      //if (dlgWaritsuke == null || dlgWaritsuke.IsDisposed)
      //{
      //    RequestHandler handler = new RequestHandler();

      //    ExternalEvent exEvent = ExternalEvent.Create(handler);

      //    dlgWaritsuke = new DLG.DlgWaritsuke(doc, exEvent, handler, instBase);
      //    dlgWaritsuke.Show();
      //}
      //dlgWaritsuke.InitControl();
    }

    public void ShowForm_dlgCASE( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;

      if ( dlgCASE == null || dlgCASE.IsDisposed ) {
        RequestHandler handler = new RequestHandler() ;

        ExternalEvent exEvent = ExternalEvent.Create( handler ) ;

        DlgCaseSelect selDlg = new DlgCaseSelect() ;
        if ( selDlg.ShowDialog() == DialogResult.No ) {
          //�������[�h
          //���ύX���[�h
          List<ElementId> Ids = new List<ElementId>() ;
          if ( ! ClsCASE.PickObjectsKabe( uidoc, ref Ids ) )
            return ;

          ClsCASE cCase = new ClsCASE( ClsCASE.enKabeshu.all ) ;
          dlgCASE = new DlgCASE( DlgCASE.enMode.wariateMode, doc, exEvent, handler ) ;

          dlgCASE.m_koyaitaDataList = new List<ClsCASE.stCASEKoyaita>() ;
          dlgCASE.m_koyaitaDataList.AddRange( cCase.GetKoyaitaCaseList( doc, Ids ) ) ;
          dlgCASE.m_koyaitaDataList.AddRange( cCase.GetKoyaitaCaseList( doc ) ) ;

          dlgCASE.m_oyaguiDataList = new List<ClsCASE.stCASEOyagui>() ;
          dlgCASE.m_oyaguiDataList.AddRange( cCase.GetOyaguiCaseList( doc, Ids ) ) ;
          dlgCASE.m_oyaguiDataList.AddRange( cCase.GetOyaguiCaseList( doc ) ) ;

          dlgCASE.m_smwDataList = new List<ClsCASE.stCASESMW>() ;
          dlgCASE.m_smwDataList.AddRange( cCase.GetSMWCaseList( doc, Ids ) ) ;
          dlgCASE.m_smwDataList.AddRange( cCase.GetSMWCaseList( doc ) ) ;

          dlgCASE.m_kabeDataList = new List<ClsCASE.stCASERenzokuKabe>() ;
          dlgCASE.m_kabeDataList.AddRange( cCase.GetRenzokukabeCaseList( doc, Ids ) ) ;
          dlgCASE.m_kabeDataList.AddRange( cCase.GetRenzokukabeCaseList( doc ) ) ;

          dlgCASE.Show() ;
        }
        else {
          //���ύX���[�h
          ClsCASE cCase = new ClsCASE( ClsCASE.enKabeshu.all ) ;
          dlgCASE = new DlgCASE( DlgCASE.enMode.editMode, doc, exEvent, handler ) ;

          dlgCASE.m_koyaitaDataList = cCase.GetKoyaitaCaseList( doc ) ;
          dlgCASE.m_oyaguiDataList = cCase.GetOyaguiCaseList( doc ) ;
          dlgCASE.m_smwDataList = cCase.GetSMWCaseList( doc ) ;
          dlgCASE.m_kabeDataList = cCase.GetRenzokukabeCaseList( doc ) ;

          dlgCASE.Show() ;
        }
      }
    }

    /// <summary>
    ///   Waking up the dialog from its waiting state.
    /// </summary>
    ///
    public void WakeFormUp()
    {
      if ( m_MyForm != null ) {
        m_MyForm.WakeUp() ;
      }

      if ( dlgCreateVoidFamily != null ) {
        dlgCreateVoidFamily.WakeUp() ;
      }

      if ( dlgCASETest != null ) {
        dlgCASETest.WakeUp() ;
      }

      if ( dlgCreateSanjiku != null ) {
        dlgCreateSanjiku.WakeUp() ;
      }

      if ( dlgCreateCornerHiuchi != null ) {
        dlgCreateCornerHiuchi.WakeUp() ;
      }

      if ( dlgBaseList != null ) {
        dlgBaseList.WakeUp() ;
      }

      //if (dlgWaritsuke != null)
      //{
      //    dlgWaritsuke.WakeUp();
      //}
      if ( dlgCASE != null ) {
        dlgCASE.WakeUp() ;
      }
    }

    #endregion

    public DLG.DlgCreateVoidFamily GetForm()
    {
      return dlgCreateVoidFamily ;
    }

    public DLG.DlgCASETest GetForm_dlgCASETest()
    {
      return dlgCASETest ;
    }

    public DLG.DlgCreateSanjiku GetForm_dlgCreateSanjiku()
    {
      return dlgCreateSanjiku ;
    }

    public DLG.DlgCreateCornerHiuchiBase GetForm_dlgCreateCornerHiuchi()
    {
      return dlgCreateCornerHiuchi ;
    }

    public DLG.DlgBaseList GetForm_dlgBaseList()
    {
      return dlgBaseList ;
    }

    public DLG.DlgWaritsuke GetForm_dlgWaritsuke()
    {
      return dlgWaritsuke ;
    }

    public DLG.DlgCASE GetForm_dlgCASE()
    {
      return dlgCASE ;
    }
  }
}