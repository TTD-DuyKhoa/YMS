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
using System.Threading;

namespace YMS
{
    /// <summary>
    ///   A list of requests the dialog has available
    /// </summary>
    /// 
    public enum RequestId : int
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,
        /// <summary>
        /// "Delete" request
        /// </summary>
        End = 1,
        /// <summary>
        /// "梁の割付" request
        /// </summary>
        PutHari = 2,
        /// <summary>
        /// "調整材配置" request
        /// </summary>
        PutTyouseizai = 3,
        /// <summary>
        /// "掘削用ボイドのファミリ配置" request
        /// </summary>
        PutKussakuBoidKabe = 4,
        /// <summary>
        /// "掘削用ボイドのファミリ配置ソイル" request
        /// </summary>
        PutKussakuBoidSoil = 5,
        /// <summary>
        /// "掘削用ボイドのファミリ配置基準線選択ソイル" request
        /// </summary>
        PutKussakuBoidSoilLine = 6,
        /// <summary>
        /// "掘削用ボイドのファミリ配置基準線選択壁" request
        /// </summary>
        PutKussakuBoidKabeLine = 7,
        /// <summary>
        /// "プロジェクト情報に情報を格納" request
        /// </summary>
        SaveProjectInfo = 8,
        /// <summary>
        /// CASEの内容をダイアログに表示更新
        /// </summary>
        UpdateCASE = 9,
        /// <summary>
        /// CASE選択されたもののみ表示
        /// </summary>
        UpdateSelectCASE = 10,
        /// <summary>
        /// 三軸ピースを固定値で動かす
        /// </summary>
        SanjikuDist = 11,
        /// <summary>
        /// コーナー火打ちを作成する
        /// </summary>
        CreateCornerHiuchi = 12,
        /// <summary>
        /// ベース一覧の処理
        /// </summary>
        BaseList = 13,
        /// <summary>
        /// 割付処理
        /// </summary>
        Waritsuke = 14,
        /// <summary>
        /// CASE処理
        /// </summary>
        CASE = 15
    }

    /// <summary>
    ///   A class around a variable holding the current request.
    /// </summary>
    /// <remarks>
    ///   Access to it is made thread-safe, even though we don't necessarily
    ///   need it if we always disable the dialog between individual requests.
    /// </remarks>
    /// 
    public class Request
    {
        // Storing the value as a plain Int makes using the interlocking mechanism simpler
        private int m_request = (int)RequestId.None;

        /// <summary>
        ///   Take - The Idling handler calls this to obtain the latest request. 
        /// </summary>
        /// <remarks>
        ///   This is not a getter! It takes the request and replaces it
        ///   with 'None' to indicate that the request has been "passed on".
        /// </remarks>
        /// 
        public RequestId Take()
        {
            return (RequestId)Interlocked.Exchange(ref m_request, (int)RequestId.None);
        }

        /// <summary>
        ///   Make - The Dialog calls this when the user presses a command button there. 
        /// </summary>
        /// <remarks>
        ///   It replaces any older request previously made.
        /// </remarks>
        /// 
        public void Make(RequestId request)
        {
            Interlocked.Exchange(ref m_request, (int)request);
        }
    }
}
