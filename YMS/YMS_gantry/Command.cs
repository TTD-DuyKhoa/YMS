#region Namespaces

using Autodesk.Revit.ApplicationServices ;
using Autodesk.Revit.Attributes ;
using Autodesk.Revit.DB ;
using Autodesk.Revit.DB.Events ;
using Autodesk.Revit.UI ;
using Autodesk.Revit.UI.Selection ;
using System ;
using System.Collections.Generic ;
using System.Diagnostics ;
using System.Windows.Forms ;
using YMS_gantry.Command ;
using YMS_gantry.UI ;

#endregion

namespace YMS_gantry
{
  /// <summary>
  /// [�ꊇ�z�u] �\��(�t���b�g)�z�u
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class AllPutKoudaiFlat : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      //�v���e�N�g�L�b�g�p����
      if ( ! YMS.ClsProtect.bCheckProtect() ) {
        return Result.Failed ;
      }

      UIApplication uiapp = commandData.Application ;
      UIDocument uidoc = uiapp.ActiveUIDocument ;

      //using (var f = new FrmAllPutKoudaiFlat())
      //{
      //    f.ShowDialog();
      //}
      CmdCreateKoudaiFlat cmd = new CmdCreateKoudaiFlat( uidoc ) ;
      cmd.Excute() ;

      return Result.Succeeded ;
    }
  }

  ///// <summary>
  ///// [�ꊇ�z�u] �\��(����`��)�z�u
  ///// </summary>
  //[Transaction(TransactionMode.Manual)]
  //public class AllPutKoudaiUnique : IExternalCommand
  //{
  //    public Result Execute(ExternalCommandData commandData,
  //      ref string message, ElementSet elements)
  //    {
  //        UIApplication uiapp = commandData.Application;
  //        UIDocument uidoc = uiapp.ActiveUIDocument;

  //        var result = MessageBox.Show("�\��̐V�K�ꊇ�z�u�������Ȃ��܂����H", "�m�F", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
  //        if (result == DialogResult.Yes)
  //        {
  //            IList<Element> elementList = uidoc.Selection.PickElementsByRectangle("�z�u�ΏۂƂȂ�O�g����͈͎w�肵�Ă��������B");
  //            if (elementList.Count == 0) return Result.Succeeded;

  //            var basePoint = uidoc.Selection.PickObject(ObjectType.PointOnElement, "�z�u��_���w�肵�Ă��������B");
  //            var lengthVector = uidoc.Selection.PickObject(ObjectType.PointOnElement, "���������̃x�N�g�����w�肵�Ă��������B");
  //            var widthVector = uidoc.Selection.PickObject(ObjectType.PointOnElement, "���������̃x�N�g�����w�肵�Ă��������B");
  //        }

  //        using (var f = new FrmAllPutKoudaiUnique())
  //        {
  //            f.ShowDialog();
  //        }

  //        return Result.Succeeded;
  //    }
  //}

  /// <summary>
  /// [�ꊇ�z�u] �X���[�v�쐬
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class CreateSlope : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      //�v���e�N�g�L�b�g�p����
      if ( ! YMS.ClsProtect.bCheckProtect() ) {
        return Result.Failed ;
      }

      UIApplication uiapp = commandData.Application ;
      UIDocument uidoc = uiapp.ActiveUIDocument ;

      CmdCreateSlope cmd = new CmdCreateSlope( uidoc.Document ) ;
      cmd.Excute( uidoc ) ;

      return Result.Succeeded ;
    }
  }

  /// <summary>
  /// [�ꊇ�z�u] �u���[�X�E�c�i�M�z�u
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class AllPutBraceTsunagi : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      //�v���e�N�g�L�b�g�p����
      if ( ! YMS.ClsProtect.bCheckProtect() ) {
        return Result.Failed ;
      }

      UIApplication uiapp = commandData.Application ;
      UIDocument uidoc = uiapp.ActiveUIDocument ;

      var cmd = new CmdAllPutBraceTsunagi() ;
      cmd.Execute( uidoc ) ;

      return Result.Succeeded ;
    }
  }

  /// <summary>
  /// [�ꊇ�z�u] �O���[�s���O
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class Grouping : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      //�v���e�N�g�L�b�g�p����
      if ( ! YMS.ClsProtect.bCheckProtect() ) {
        return Result.Failed ;
      }

      UIApplication uiapp = commandData.Application ;
      UIDocument uidoc = uiapp.ActiveUIDocument ;

      //CmdCreateGrouping cmd = new CmdCreateGrouping(uidoc);
      //cmd.Excute();

      Application.thisApp.ShowForm_FrmGrouping( uiapp ) ;

      //using (var f = new FrmGrouping(uidoc))
      //{
      //    f.ShowDialog();
      //}

      return Result.Succeeded ;
    }
  }

  ///// <summary>
  ///// [�ʔz�u] �e�X�g�p �������e�X�g�p�̃R�}���h
  ///// </summary>
  //[Transaction(TransactionMode.Manual)]
  //public class PutTest : IExternalCommand
  //{
  //    //public Result Execute(ExternalCommandData commandData,
  //    //  ref string message, ElementSet elements)
  //    //{
  //    //    UIApplication uiapp = commandData.Application;
  //    //    UIDocument uidoc = uiapp.ActiveUIDocument;
  //    //    MessageBox.Show("�e�X�g�z�u");
  //    //    return Result.Succeeded;
  //    //}
  //    public Result Execute(
  //     ExternalCommandData commandData,
  //     ref string message,
  //     ElementSet elements)
  //    {
  //        UIApplication uiapp = commandData.Application;
  //        UIDocument uidoc = uiapp.ActiveUIDocument;
  //        Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
  //        Document doc = uidoc.Document;

  //        return GantryUtil.CreateRubberLine(uidoc);
  //    }
  //}

  /// <summary>
  /// [�ʔz�u] ���H�z�u
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class PutFukkouban : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      ////�v���e�N�g�L�b�g�p����
      //if (!YMS.ClsProtect.bCheckProtect())
      //{
      //    return Result.Failed;
      //}
      UIApplication uiapp = commandData.Application ;
      UIDocument uidoc = uiapp.ActiveUIDocument ;

      ////CmdCreateFukkouban cmd = new CmdCreateFukkouban(uidoc.Document);
      ////cmd.Excute(uidoc);
      //////using (var f = new FrmPutFukkouban(uiapp))
      //////{
      //////    if(f.ShowDialog()==DialogResult.OK)
      //////    {
      //////        Fukkouban.CreateFukkoudan(uiapp, uiapp.ActiveUIDocument.Document,"SS400", DefineUtil.eFukkoubanType.TwoM, true);
      //////    }
      //////}
      /////

      Application.thisApp.ShowForm_FrmPutFukkouban( uiapp ) ;

      return Result.Succeeded ;
    }
  }

  /// <summary>
  /// [�ʔz�u] ���(����)�z�u
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class PutOobikiKetauke : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      //�v���e�N�g�L�b�g�p����
      if ( ! YMS.ClsProtect.bCheckProtect() ) {
        return Result.Failed ;
      }

      UIApplication uiapp = commandData.Application ;
      UIDocument uidoc = uiapp.ActiveUIDocument ;
      //CmdCreateOhbiki cmd = new CmdCreateOhbiki(uidoc);
      //cmd.Excute();
      Application.thisApp.ShowForm_FrmPutOhbikiKetauke( uiapp ) ;

      return Result.Succeeded ;
    }
  }

  /// <summary>
  /// [�ʔz�u] ����(�包)�z�u
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class PutNedaShugeta : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      //�v���e�N�g�L�b�g�p����
      if ( ! YMS.ClsProtect.bCheckProtect() ) {
        return Result.Failed ;
      }

      UIApplication uiapp = commandData.Application ;
      UIDocument uidoc = uiapp.ActiveUIDocument ;

      //CmdCreateNeda cmd = new CmdCreateNeda(uidoc);
      //cmd.Excute();
      Application.thisApp.ShowForm_FrmPutNedaShugeta( uiapp ) ;

      return Result.Succeeded ;
    }
  }

  /// <summary>
  /// [�ʔz�u] ���H���z�u
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class PutFukkougeta : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      //�v���e�N�g�L�b�g�p����
      if ( ! YMS.ClsProtect.bCheckProtect() ) {
        return Result.Failed ;
      }

      UIApplication uiapp = commandData.Application ;
      UIDocument uidoc = uiapp.ActiveUIDocument ;

      //CmdCreateFukkougeta cmd = new CmdCreateFukkougeta(uidoc);
      //if (!cmd.Excute())
      //{
      //    return Result.Failed;
      //}
      Application.thisApp.ShowForm_FrmPutFukkougeta( uiapp ) ;

      return Result.Succeeded ;
    }
  }

  /// <summary>
  /// [�ʔz�u] �ΌX�\�E�ΌX�\��t�v���[�g�z�u
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class PutTaikeikou : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      //�v���e�N�g�L�b�g�p����
      if ( ! YMS.ClsProtect.bCheckProtect() ) {
        return Result.Failed ;
      }

      UIApplication uiapp = commandData.Application ;
      UIDocument uidoc = uiapp.ActiveUIDocument ;

      //CmdCreateTaikeikou cmd = new CmdCreateTaikeikou(uidoc);
      //return cmd.Excute();
      Application.thisApp.ShowForm_FrmPutTaikeikou( uiapp ) ;

      return Result.Succeeded ;
    }
  }

  /// <summary>
  /// [�ʔz�u] �~���z�u
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class PutShikigeta : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      //�v���e�N�g�L�b�g�p����
      if ( ! YMS.ClsProtect.bCheckProtect() ) {
        return Result.Failed ;
      }

      UIApplication uiapp = commandData.Application ;
      UIDocument uidoc = uiapp.ActiveUIDocument ;
      //CmdCreateShikigeta cmd = new CmdCreateShikigeta(uidoc);
      //cmd.Excute();
      Application.thisApp.ShowForm_FrmPutShikigeta( uiapp ) ;

      return Result.Succeeded ;
    }
  }

  /// <summary>
  /// [�ʔz�u] �X�`�t�i�[�v���[�g�E�X�`�t�i�[�W���b�L�z�u
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class PutSuchifuna : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      //�v���e�N�g�L�b�g�p����
      if ( ! YMS.ClsProtect.bCheckProtect() ) {
        return Result.Failed ;
      }

      UIApplication uiapp = commandData.Application ;
      UIDocument uidoc = uiapp.ActiveUIDocument ;

      //CmdCreateStiffener cmd = new CmdCreateStiffener(uidoc);
      //if (cmd.Excute() != Result.Succeeded)
      //{
      //    return Result.Failed;
      //}
      Application.thisApp.ShowForm_FrmPutSuchifuna( uiapp ) ;

      return Result.Succeeded ;
    }
  }

  /// <summary>
  /// [�ʔz�u] ���������v���[�g�E�����ޔz�u
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class PutTakasaChouseiPlate : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      //�v���e�N�g�L�b�g�p����
      if ( ! YMS.ClsProtect.bCheckProtect() ) {
        return Result.Failed ;
      }

      UIApplication uiapp = commandData.Application ;
      UIDocument uidoc = uiapp.ActiveUIDocument ;

      //CmdCreateTakasaChousei cmd = new CmdCreateTakasaChousei(uidoc);
      //if (cmd.Excute())
      //{
      //    return Result.Succeeded;
      //}
      //else
      //{
      //    return Result.Failed;
      //}
      Application.thisApp.ShowForm_FrmPutTakasaChouseiPlateChouseizai( uiapp ) ;
      return Result.Succeeded ;
    }
  }

  /// <summary>
  /// [�ʔz�u] �|�ޔz�u
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class PutKoubanzai : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      //�v���e�N�g�L�b�g�p����
      if ( ! YMS.ClsProtect.bCheckProtect() ) {
        return Result.Failed ;
      }

      UIApplication uiapp = commandData.Application ;
      UIDocument uidoc = uiapp.ActiveUIDocument ;

      //CmdCreateKouhan cmd = new CmdCreateKouhan(uidoc);
      //cmd.Excute();

      Application.thisApp.ShowForm_FrmPutKoubanzai( uiapp ) ;

      return Result.Succeeded ;
    }
  }

  /// <summary>
  /// [�ʔz�u] �n���E���H�Y���~�ߍޔz�u
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class PutJifukuFukkoubanZuredome : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      //�v���e�N�g�L�b�g�p����
      if ( ! YMS.ClsProtect.bCheckProtect() ) {
        return Result.Failed ;
      }

      UIApplication uiapp = commandData.Application ;
      UIDocument uidoc = uiapp.ActiveUIDocument ;
      //CmdCreateJifukuZuredome cmd = new CmdCreateJifukuZuredome(uidoc);
      //cmd.Excute();
      Application.thisApp.ShowForm_FrmPutJifukuFukkoubanZuredomezai( uiapp ) ;

      return Result.Succeeded ;
    }
  }

  /// <summary>
  /// [�ʔz�u] �萠�E�萠�x���z�u
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class PutTesuriTesurishichu : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      //�v���e�N�g�L�b�g�p����
      if ( ! YMS.ClsProtect.bCheckProtect() ) {
        return Result.Failed ;
      }

      UIApplication uiapp = commandData.Application ;
      UIDocument uidoc = uiapp.ActiveUIDocument ;
      //CmdCreateTesuri cmd = new CmdCreateTesuri(uidoc);
      //cmd.Excute();
      Application.thisApp.ShowForm_FrmPutTesuriTesuriShichu( uiapp ) ;

      return Result.Succeeded ;
    }
  }

  /// <summary>
  /// [�ʔz�u] �����u���[�X�z�u
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class PutHorizontalBrace : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      //�v���e�N�g�L�b�g�p����
      if ( ! YMS.ClsProtect.bCheckProtect() ) {
        return Result.Failed ;
      }

      UIApplication uiapp = commandData.Application ;
      UIDocument uidoc = uiapp.ActiveUIDocument ;

      //using (var f = new FrmPutHorizontalBrace())
      //{
      //    f.ShowDialog();
      //}
      Application.thisApp.ShowForm_FrmPutHorizontalBrace( uiapp ) ;

      return Result.Succeeded ;
    }
  }


  /// <summary>
  /// [�ʔz�u] �����u���[�X�z�u
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class PutVerticalBrace : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      //�v���e�N�g�L�b�g�p����
      if ( ! YMS.ClsProtect.bCheckProtect() ) {
        return Result.Failed ;
      }

      UIApplication uiapp = commandData.Application ;
      UIDocument uidoc = uiapp.ActiveUIDocument ;

      //using (var f = new FrmPutVerticalBrace())
      //{
      //    f.ShowDialog();
      //}
      Application.thisApp.ShowForm_FrmPutVerticalBrace( uiapp ) ;

      //Reference ref= uidoc.Selection.PickObject(ObjectType.Face, "�ʂ�I��");

      //GantoryUtil.CreateReferencePlaneFromFace();
      return Result.Succeeded ;
    }
  }

  /// <summary>
  /// [�ʔz�u] �����⏕�ޔz�u
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class PutTeiketsuHojyozai : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      //�v���e�N�g�L�b�g�p����
      if ( ! YMS.ClsProtect.bCheckProtect() ) {
        return Result.Failed ;
      }

      UIApplication uiapp = commandData.Application ;
      UIDocument uidoc = uiapp.ActiveUIDocument ;

      //CmdCreateTeiketsuHojo cmd = new CmdCreateTeiketsuHojo(uidoc);
      //cmd.Excute();
      Application.thisApp.ShowForm_FrmPutTeiketsuHojyozai( uiapp ) ;

      return Result.Succeeded ;
    }
  }

  /// <summary>
  /// [�ʔz�u] �����Ȃ��z�u
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class PutHorizontalTsunagi : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      //�v���e�N�g�L�b�g�p����
      if ( ! YMS.ClsProtect.bCheckProtect() ) {
        return Result.Failed ;
      }

      UIApplication uiapp = commandData.Application ;
      Application.thisApp.ShowForm_FrmPutHorizontalTsunagi( uiapp ) ;

      return Result.Succeeded ;
    }
  }

  /// <summary>
  /// [�ʔz�u] ����z�u
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class PutHoudue : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      //�v���e�N�g�L�b�g�p����
      if ( ! YMS.ClsProtect.bCheckProtect() ) {
        return Result.Failed ;
      }

      UIApplication uiapp = commandData.Application ;
      UIDocument uidoc = uiapp.ActiveUIDocument ;

      //CmdCreateHoudue cmd = new CmdCreateHoudue(uidoc);
      //cmd.Excute();
      Application.thisApp.ShowForm_FrmPutHoudue( uiapp ) ;

      return Result.Succeeded ;
    }
  }

  /// <summary>
  /// [�ʔz�u] �x���z�u
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class PutShichu : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      //�v���e�N�g�L�b�g�p����
      if ( ! YMS.ClsProtect.bCheckProtect() ) {
        return Result.Failed ;
      }

      UIApplication uiapp = commandData.Application ;
      UIDocument uidoc = uiapp.ActiveUIDocument ;

      //CmdCreatePiller cmd = new CmdCreatePiller(uidoc);
      //cmd.Excute();
      Application.thisApp.ShowForm_FrmPutShichu( uiapp ) ;

      return Result.Succeeded ;
    }
  }

  /// <summary>
  /// [�ʔz�u] �\��Y(�x���Y)�z�u
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class PutKui : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      //�v���e�N�g�L�b�g�p����
      if ( ! YMS.ClsProtect.bCheckProtect() ) {
        return Result.Failed ;
      }

      UIApplication uiapp = commandData.Application ;
      UIDocument uidoc = uiapp.ActiveUIDocument ;

      //CmdCreatePile cmd = new CmdCreatePile(uidoc);
      //cmd.Excute();
      Application.thisApp.ShowForm_FrmPutKui( uiapp ) ;

      return Result.Succeeded ;
    }
  }

  /// <summary>
  /// [���ޏC��] �T�C�Y�ꗗ
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class EditSizeList : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      //�v���e�N�g�L�b�g�p����
      if ( ! YMS.ClsProtect.bCheckProtect() ) {
        return Result.Failed ;
      }

      UIApplication uiapp = commandData.Application ;
      UIDocument uidoc = uiapp.ActiveUIDocument ;

      try {
        IList<Element> getElements = uidoc.Selection.PickElementsByRectangle( "�Ώە��ނ��w�肵�Ă��������B" ) ;
        List<ElementId> selectedElementIdList = new List<ElementId>() ;
        foreach ( var element in getElements ) {
          selectedElementIdList.Add( element.Id ) ;
        }

        Application.thisApp.ShowForm_FrmEditSizeList( uiapp, selectedElementIdList ) ;
      }
      catch ( Autodesk.Revit.Exceptions.OperationCanceledException ex ) {
        // ESC�L�[���ɂ��L�����Z���������������ꍇ�͉��������ɏ������I����B
      }

      return Result.Succeeded ;
    }
  }

  /// <summary>
  /// [���ޏC��] �ʒu�ύX
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class EditLocationChange : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      //�v���e�N�g�L�b�g�p����
      if ( ! YMS.ClsProtect.bCheckProtect() ) {
        return Result.Failed ;
      }

      UIApplication uiapp = commandData.Application ;
      UIDocument uidoc = uiapp.ActiveUIDocument ;

      try {
        var selectedElement = uidoc.Selection.PickObject( ObjectType.Element, "�Ώە��ނ��w�肵�Ă��������B" ) ;
        var element = uidoc.Document.GetElement( selectedElement.ElementId ) ;
        Application.thisApp.ShowForm_FrmEditLocationChange( uiapp, element.Id ) ;
      }
      catch ( Autodesk.Revit.Exceptions.OperationCanceledException ex ) {
        // ESC�L�[���ɂ��L�����Z���������������ꍇ�͉��������ɏ������I����B
      }

      return Result.Succeeded ;
    }
  }

  /// <summary>
  /// [���ޏC��] �����ύX
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class EditLengthChange : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      //�v���e�N�g�L�b�g�p����
      if ( ! YMS.ClsProtect.bCheckProtect() ) {
        return Result.Failed ;
      }

      UIApplication uiapp = commandData.Application ;
      UIDocument uidoc = uiapp.ActiveUIDocument ;

      try {
        IList<Element> getElements = uidoc.Selection.PickElementsByRectangle( "�Ώە��ނ��w�肵�Ă��������B" ) ;
        List<ElementId> selectedElementIdList = new List<ElementId>() ;
        foreach ( var element in getElements ) {
          selectedElementIdList.Add( element.Id ) ;
        }

        Application.thisApp.ShowForm_FrmEditLengthChange( uiapp, selectedElementIdList ) ;
      }
      catch ( Autodesk.Revit.Exceptions.OperationCanceledException ex ) {
        // ESC�L�[���ɂ��L�����Z���������������ꍇ�͉��������ɏ������I����B
      }

      return Result.Succeeded ;
    }
  }

  /// <summary>
  /// [���t] ���ފ��t
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class WaritsukeElement : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      //�v���e�N�g�L�b�g�p����
      if ( ! YMS.ClsProtect.bCheckProtect() ) {
        return Result.Failed ;
      }

      UIApplication uiapp = commandData.Application ;
      UIDocument uidoc = uiapp.ActiveUIDocument ;

      CmdWaritsuke cmd = new CmdWaritsuke( uidoc ) ;
      cmd.Excute() ;

      return Result.Succeeded ;
    }
  }

  /// <summary>
  /// [���̑�] ���`�F�b�N
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class SonotaKanshouCheck : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      //�v���e�N�g�L�b�g�p����
      if ( ! YMS.ClsProtect.bCheckProtect() ) {
        return Result.Failed ;
      }

      UIApplication uiapp = commandData.Application ;
      Application.thisApp.ShowForm_FrmSonotaKanshouCheck( uiapp ) ;

      return Result.Succeeded ;
    }
  }

  /// <summary>
  /// [���̑�] �ʔz�u
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class KobetsuHaichi : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      UIApplication uiapp = commandData.Application ;
      UIDocument uidoc = uiapp.ActiveUIDocument ;

      if ( ! YMS.ClsProtect.bCheckProtect() ) {
        return Result.Failed ;
      }

      YMS.Command.ClsCommandKobetsuHaichi.CommandKobetsuHaichi( uiapp, false ) ;


      return Result.Succeeded ;
    }
  }

  [Transaction( TransactionMode.Manual )]
  [Regeneration( RegenerationOption.Manual )]
  class CmdPlaceFamilyInstance : IExternalCommand
  {
    List<ElementId> _added_element_ids = new List<ElementId>() ;

    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      UIApplication uiapp = commandData.Application ;
      UIDocument uidoc = uiapp.ActiveUIDocument ;
      var app = uiapp.Application ;
      Document doc = uidoc.Document ;
      FilteredElementCollector collector = new FilteredElementCollector( doc ) ;
      collector.OfCategory( BuiltInCategory.OST_Doors ) ;
      collector.OfClass( typeof( FamilySymbol ) ) ;
      FamilySymbol symbol = collector.FirstElement() as FamilySymbol ;
      _added_element_ids.Clear() ;
      app.DocumentChanged += new EventHandler<DocumentChangedEventArgs>( OnDocumentChanged ) ;
      uidoc.PromptForFamilyInstancePlacement( symbol ) ;
      app.DocumentChanged -= new EventHandler<DocumentChangedEventArgs>( OnDocumentChanged ) ;
      int n = _added_element_ids.Count ;
      TaskDialog.Show( "Place Family Instance",
        string.Format( "{0} element{1} added.", n, ( ( 1 == n ) ? "" : "s" ) ) ) ;
      return Result.Succeeded ;
    }

    void OnDocumentChanged( object sender, DocumentChangedEventArgs e )
    {
      _added_element_ids.AddRange( e.GetAddedElementIds() ) ;
    }
  }
}