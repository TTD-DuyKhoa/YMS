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
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using RevitUtil;
using YMS.gantry;
using YMS_gantry.Command;

namespace YMS_gantry
{
    /// <summary>
    ///   A class with methods to execute requests made by the dialog user.
    /// </summary>
    /// 
    public class RequestHandler : IExternalEventHandler
    {
        // A trivial delegate, but handy
        private delegate void DoorOperation(FamilyInstance e);

        // The value of the latest request made by the modeless form 
        private Request m_request = new Request();

        private XYZ maeP = null;
        private ModelLine mdLine = null;
        private XYZ geoDirection = null;
        /// <summary>
        /// A public property to access the current request value
        /// </summary>
        public Request Request
        {
            get { return m_request; }
        }

        /// <summary>
        ///   A method to identify this External Event Handler
        /// </summary>
        public String GetName()
        {
            return "R2014 External Event Sample";
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
        public void Execute(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            try
            {
                switch (Request.Take())
                {
                    case RequestId.End:
                        {
                            EndWarituke();
                            break;
                        }
                    case RequestId.Grouping:
                        {
                            CmdCreateGrouping cmdGP = new CmdCreateGrouping(uidoc);
                            cmdGP.Excute();
                            break;
                        }
                    case RequestId.Fukkouban:
                        {
                            CmdCreateFukkouban cmdCH = new CmdCreateFukkouban(uidoc);
                            cmdCH.Excute();
                            break;
                        }
                    case RequestId.Ohbiki:
                        {
                            CmdCreateOhbiki cmdCH = new CmdCreateOhbiki(uidoc);
                            cmdCH.Excute();
                            break;
                        }
                    case RequestId.Neda:
                        {
                            CmdCreateNeda cmdCH = new CmdCreateNeda(uidoc);
                            cmdCH.Excute();
                            break;
                        }
                    case RequestId.Fukkougeta:
                        {
                            CmdCreateFukkougeta cmdCH = new CmdCreateFukkougeta(uidoc);
                            cmdCH.Excute();
                            break;
                        }
                    case RequestId.Taikeikou:
                        {
                            CmdCreateTaikeikou cmdCH = new CmdCreateTaikeikou(uidoc);
                            cmdCH.Excute();
                            break;
                        }
                    case RequestId.Shikigeta:
                        {
                            CmdCreateShikigeta cmdCH = new CmdCreateShikigeta(uidoc);
                            cmdCH.Excute();
                            break;
                        }
                    case RequestId.Stiffener:
                        {
                            CmdCreateStiffener cmdCH = new CmdCreateStiffener(uidoc);
                            cmdCH.Excute();
                            break;
                        }
                    case RequestId.TakasaChousei:
                        {
                            CmdCreateTakasaChousei cmdCH = new CmdCreateTakasaChousei(uidoc);
                            cmdCH.Excute();
                            break;
                        }
                    case RequestId.Koubanzai:
                        {
                            CmdCreateKouhan cmdCH = new CmdCreateKouhan(uidoc);
                            cmdCH.Excute();
                            break;
                        }
                    case RequestId.JifukuZuredome:
                        {
                            CmdCreateJifukuZuredome cmdCH = new CmdCreateJifukuZuredome(uidoc);
                            cmdCH.Excute();
                            break;
                        }
                    case RequestId.Tesuri:
                        {
                            CmdCreateTesuri cmdCH = new CmdCreateTesuri(uidoc);
                            cmdCH.Excute();
                            break;
                        }
                    case RequestId.HorizontalBrace:
                        {
                            CmdPutHorizontalBrace cmdCH = new CmdPutHorizontalBrace(uidoc);
                            cmdCH.Excute();
                            break;
                        }
                    case RequestId.VerticalBrace:
                        {
                            CmdPutVerticalBrace cmdCH = new CmdPutVerticalBrace(uidoc);
                            cmdCH.Excute();
                            break;
                        }
                    case RequestId.TeiketsuHojozai:
                        {
                            CmdCreateTeiketsuHojo cmdCH = new CmdCreateTeiketsuHojo(uidoc);
                            cmdCH.Excute();
                            break;
                        }
                    case RequestId.HorizontalTsunagi:
                        {
                            CmdPutHorizontalTsunagi cmdCH = new CmdPutHorizontalTsunagi(uidoc);
                            cmdCH.Excute();
                            break;
                        }
                    case RequestId.Houdue:
                        {
                            CmdCreateHoudue cmdCH = new CmdCreateHoudue(uidoc);
                            cmdCH.Excute();
                            break;
                        }
                    case RequestId.Shichu:
                        {
                            CmdCreatePiller cmdCH = new CmdCreatePiller(uidoc);
                            cmdCH.Excute();
                            break;
                        }
                    case RequestId.Kui:
                        {
                            CmdCreatePile cmdCH = new CmdCreatePile(uidoc);
                            cmdCH.Excute();
                            break;
                        }
                    case RequestId.EditSize:
                        {
                            CmdSizeChange cmd = new CmdSizeChange(uidoc);
                            cmd.ExcuteOfSize();
                            break;
                        }
                    case RequestId.EditSizeCategory:
                        {
                            CmdSizeChange cmd = new CmdSizeChange(uidoc);
                            cmd.ExcuteOfSizeCategory();
                            break;
                        }
                    case RequestId.EditLengthChange:
                        {
                            CmdLengthChange cmd = new CmdLengthChange(uidoc);
                            cmd.Excute();
                            break;
                        }
                    case RequestId.EditLocation:
                        {
                            CmdMoveElement cmd = new CmdMoveElement(uidoc);
                            cmd.Excute();
                            break;
                        }
                    case RequestId.Waritsuke:
                        {
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            finally
            {
                Application.thisApp.WakeFormUp();
            }

            return;
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void EndWarituke()
        {
            //ダイアログの処理終了時に位置情報を初期化
            maeP = null;
            mdLine = null;
            geoDirection = null;
        }

    }  // class

}  // namespace
