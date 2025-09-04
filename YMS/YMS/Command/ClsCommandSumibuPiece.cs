using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Windows.Forms ;
using YMS.Parts ;

namespace YMS.Command
{
  class ClsCommandSumibuPiece
  {
    public static void CommandSumibuPiece( UIDocument uidoc )
    {
      //ドキュメントを取得
      Document doc = uidoc.Document ;

      //ダイアログを表示
      YMS.DLG.DlgCreateSumibuPiece SP = new DLG.DlgCreateSumibuPiece() ;
      DialogResult result = SP.ShowDialog() ;
      if ( result == DialogResult.Cancel ) {
        return ;
      }

      ClsSumibuPiece clsSP = SP.m_ClsSumibuPiece ;
      ElementId levelID = null ;

      ElementId idHaraokoshiBase1 = null ;
      ElementId idHaraokoshiBase2 = null ;

      XYZ crossPoint = null ;
      double radians = 0.0 ;

      if ( clsSP.m_CreateType == ClsSumibuPiece.CreateType.Manual ) {
        if ( ! ClsRevitUtil.PickObject( uidoc, "1つめの腹起ベース", "腹起ベース", ref idHaraokoshiBase1 ) ) {
          return ;
        }

        Element inst1 = doc.GetElement( idHaraokoshiBase1 ) ;
        FamilyInstance ShinInst1 = doc.GetElement( idHaraokoshiBase1 ) as FamilyInstance ;
        LocationCurve lCurve1 = inst1.Location as LocationCurve ;
        if ( lCurve1 == null ) {
          return ;
        }

        if ( ! ClsRevitUtil.PickObject( uidoc, "2つめの腹起ベース", "腹起ベース", ref idHaraokoshiBase2 ) ) {
          return ;
        }

        Element inst2 = doc.GetElement( idHaraokoshiBase2 ) ;
        FamilyInstance ShinInst2 = doc.GetElement( idHaraokoshiBase2 ) as FamilyInstance ;
        LocationCurve lCurve2 = inst2.Location as LocationCurve ;
        if ( lCurve2 == null ) {
          return ;
        }

        //元データを取得
        levelID = ShinInst1.Host.Id ;
        if ( levelID == null ) {
          return ;
        }

        //入隅か判定
        bool bIrizumi = false ;
        if ( ! ClsHaraokoshiBase.CheckIrizumi( lCurve1.Curve, lCurve2.Curve, ref bIrizumi ) ) {
          return ;
        }

        // 交点と角度を取得
        if ( ! clsSP.GetCrossPointAndAngle( lCurve1, lCurve2, out crossPoint, out radians ) ) {
          return ;
        }

        clsSP.m_BasePoint = crossPoint ;
        clsSP.m_Angle = radians ;

        // 腹起ベースが同段か上下段なのか判定
        ElementId id1 = ShinInst1.Id ;
        string haraDan1 = ClsRevitUtil.GetParameter( doc, id1, "段" ) ;
        clsSP.m_ParentId1 = id1 ;
        string yoko1 = ClsRevitUtil.GetParameter( doc, id1, "横本数" ) ;
        string haraSize1 = ClsRevitUtil.GetParameter( doc, id1, "鋼材サイズ" ) ;

        ElementId id2 = ShinInst2.Id ;
        string haraDan2 = ClsRevitUtil.GetParameter( doc, id2, "段" ) ;
        clsSP.m_ParentId2 = id2 ;
        string yoko2 = ClsRevitUtil.GetParameter( doc, id2, "横本数" ) ;
        string haraSize2 = ClsRevitUtil.GetParameter( doc, id2, "鋼材サイズ" ) ;

        if ( haraDan1 == "同段" && haraDan2 == "同段" || haraDan1 == haraDan2 ) {
          // 間詰め量がある場合
          // 最初に選択された腹起ベースに対してずれる
          if ( clsSP.m_Tsumeryo > 0 ) {
            XYZ midPoint2 = ( lCurve2.Curve.GetEndPoint( 0 ) + lCurve2.Curve.GetEndPoint( 1 ) ) / 2.0 ;
            XYZ vec = midPoint2 - crossPoint ;
            vec = vec.Normalize() ;
            XYZ moveVector = vec * ClsRevitUtil.ConvertDoubleGeo2Revit( clsSP.m_Tsumeryo ) ;
            crossPoint = crossPoint + moveVector ;
          }

          clsSP.m_ToritsukeDan = haraDan1 ; // "同段";
          if ( ! clsSP.CreateSumibuPiece( doc, id1, crossPoint, haraSize1, radians, true ) ) {
            MessageBox.Show( "隅部ピースの配置に失敗しました" ) ;
          }


          ////横ダブル対応
          //if (yoko1 == "ダブル")
          //{
          //    double dOffset1 = ClsYMSUtil.GetKouzaiHabaFromBase(doc, id1) / 2;
          //    XYZ midPoint1 = (lCurve1.Curve.GetEndPoint(0) + lCurve1.Curve.GetEndPoint(1)) / 2.0;
          //    XYZ vecA = crossPoint - midPoint1;
          //    vecA = vecA.Normalize();
          //    XYZ moveVectorA = vecA * ClsRevitUtil.ConvertDoubleGeo2Revit(dOffset1);
          //    clsSP.m_BasePoint = crossPoint + moveVectorA * 3;
          //    if (!clsSP.CreateSumibuPiece(doc, id1, crossPoint + moveVectorA * 3, radians, true))
          //    {
          //        MessageBox.Show("隅部ピースの配置に失敗しました");
          //    }
          //}
        }
        else {
          if ( ( ClsRevitUtil.GetParameter( doc, id1, "縦本数" ) != "シングル" ) ||
               ( ClsRevitUtil.GetParameter( doc, id2, "縦本数" ) != "シングル" ) ) {
            MessageBox.Show( "縦シングル以外には配置できません" ) ;
            return ;
          }

          // 上段腹起の方向と下段腹起の方向を算出して渡す
          XYZ midPoint1 = ( lCurve1.Curve.GetEndPoint( 0 ) + lCurve1.Curve.GetEndPoint( 1 ) ) / 2.0 ;
          XYZ midPoint2 = ( lCurve2.Curve.GetEndPoint( 0 ) + lCurve2.Curve.GetEndPoint( 1 ) ) / 2.0 ;

          // 選択1の単位ベクトル
          XYZ selVec1 = midPoint1 - crossPoint ;
          selVec1 = selVec1.Normalize() ;

          // 選択2の単位ベクトル
          XYZ selVec2 = midPoint2 - crossPoint ;
          selVec2 = selVec2.Normalize() ;

          // 1と2の合成
          XYZ midVec = selVec1 + selVec2 ;

          // 無回転時に想定される腹起ベース2つの単位ベクトルとその合成
          XYZ vec1 = new XYZ( -1, 0, 0 ) ;

          // 主材の中央に配置するためのオフセット量を算出
          double dOffset1 = ClsYMSUtil.GetKouzaiHabaFromBase( doc, id1 ) / 2 ;
          XYZ vecA = crossPoint - midPoint1 ;
          vecA = vecA.Normalize() ;

          double dOffset2 = ClsYMSUtil.GetKouzaiHabaFromBase( doc, id2 ) / 2 ;
          XYZ vecB = crossPoint - midPoint2 ;
          vecB = vecB.Normalize() ;

          XYZ moveVectorA =
            vecA * ClsRevitUtil.ConvertDoubleGeo2Revit( dOffset2 ) ; // ClsRevitUtil.ConvertDoubleGeo2Revit(dOffset1);
          XYZ moveVectorB =
            vecB * ClsRevitUtil.ConvertDoubleGeo2Revit( dOffset1 ) ; // ClsRevitUtil.ConvertDoubleGeo2Revit(dOffset2);

          if ( ! bIrizumi ) {
            var tmp1 = moveVectorA ;
            var tmp2 = moveVectorB ;

            moveVectorA = -tmp1 - ( tmp2 * 2 ) ;
            moveVectorB = -tmp2 - ( tmp1 * 2 ) ;
          }

          if ( haraDan1 == "上段" && haraDan2 == "下段" ) {
            radians = vec1.AngleOnPlaneTo( selVec1, XYZ.BasisZ ) ;
            clsSP.m_BasePoint = crossPoint + moveVectorB ;
            clsSP.m_Angle = radians ;
            clsSP.m_ToritsukeDan = "上段" ;
            if ( ! clsSP.CreateSumibuPiece( doc, id1, crossPoint + moveVectorB, haraSize1, radians, false, true ) ) {
              MessageBox.Show( "隅部ピースの配置に失敗しました" ) ;
            }

            //横ダブル対応
            if ( yoko1 == "ダブル" ) {
              clsSP.m_BasePoint = crossPoint + moveVectorB * 3 ;
              if ( ! clsSP.CreateSumibuPiece( doc, id1, crossPoint + moveVectorB * 3, haraSize1, radians, false,
                    true ) ) {
                MessageBox.Show( "隅部ピースの配置に失敗しました" ) ;
              }
            }

            radians = vec1.AngleOnPlaneTo( selVec2, XYZ.BasisZ ) ;
            clsSP.m_BasePoint = crossPoint + moveVectorA ;
            clsSP.m_Angle = radians ;
            clsSP.m_ToritsukeDan = "下段" ;
            if ( ! clsSP.CreateSumibuPiece( doc, id2, crossPoint + moveVectorA, haraSize2, radians, false, false ) ) {
              MessageBox.Show( "隅部ピースの配置に失敗しました" ) ;
            }

            //横ダブル対応
            if ( yoko2 == "ダブル" ) {
              clsSP.m_BasePoint = crossPoint + moveVectorA * 3 ;
              if ( ! clsSP.CreateSumibuPiece( doc, id2, crossPoint + moveVectorA * 3, haraSize2, radians, false,
                    false ) ) {
                MessageBox.Show( "隅部ピースの配置に失敗しました" ) ;
              }
            }
          }
          else if ( haraDan1 == "下段" && haraDan2 == "上段" ) {
            radians = vec1.AngleOnPlaneTo( selVec1, XYZ.BasisZ ) ;
            clsSP.m_BasePoint = crossPoint + moveVectorB ;
            clsSP.m_Angle = radians ;
            clsSP.m_ToritsukeDan = "下段" ;
            if ( ! clsSP.CreateSumibuPiece( doc, id2, crossPoint + moveVectorB, haraSize1, radians, false, false ) ) {
              MessageBox.Show( "隅部ピースの配置に失敗しました" ) ;
            }

            //横ダブル対応
            if ( yoko2 == "ダブル" ) {
              clsSP.m_BasePoint = crossPoint + moveVectorB * 3 ;
              if ( ! clsSP.CreateSumibuPiece( doc, id2, crossPoint + moveVectorB * 3, haraSize1, radians, false,
                    false ) ) {
                MessageBox.Show( "隅部ピースの配置に失敗しました" ) ;
              }
            }

            radians = vec1.AngleOnPlaneTo( selVec2, XYZ.BasisZ ) ;
            clsSP.m_BasePoint = crossPoint + moveVectorA ;
            clsSP.m_Angle = radians ;
            clsSP.m_ToritsukeDan = "上段" ;
            if ( ! clsSP.CreateSumibuPiece( doc, id1, crossPoint + moveVectorA, haraSize2, radians, false, true ) ) {
              MessageBox.Show( "隅部ピースの配置に失敗しました" ) ;
            }

            //横ダブル対応
            if ( yoko1 == "ダブル" ) {
              clsSP.m_BasePoint = crossPoint + moveVectorA * 3 ;
              if ( ! clsSP.CreateSumibuPiece( doc, id1, crossPoint + moveVectorA * 3, haraSize2, radians, false,
                    true ) ) {
                MessageBox.Show( "隅部ピースの配置に失敗しました" ) ;
              }
            }
          }
          else {
            return ;
          }
        }
      }
      else if ( clsSP.m_CreateType == ClsSumibuPiece.CreateType.Auto ) {
        ElementId id = null ;
        if ( ! ClsHaraokoshiBase.PickBaseObject( uidoc, ref id, "隅部ピースを配置する基準となる腹起ベースを選択" ) ) {
          return ;
        }

        ClsHaraokoshiBase clsHaraBase = new ClsHaraokoshiBase() ;
        clsHaraBase.SetParameter( doc, id ) ;

        List<ClsHaraokoshiBase> lstHaraBase = new List<ClsHaraokoshiBase>() ;
        List<ClsHaraokoshiBase> lstH = ClsHaraokoshiBase.GetAllClsHaraokoshiBaseList( doc ) ;
        List<Tuple<ElementId, ElementId>> myList = new List<Tuple<ElementId, ElementId>>() ;
        foreach ( var item in lstH ) {
          if ( item.m_level == clsHaraBase.m_level ) {
            lstHaraBase.Add( item ) ;
          }
        }

        for ( int i = 0 ; i < lstHaraBase.Count ; i++ ) {
          idHaraokoshiBase1 = lstHaraBase[ i ].m_ElementId ;
          Element inst1 = doc.GetElement( idHaraokoshiBase1 ) ;
          FamilyInstance ShinInst1 = doc.GetElement( idHaraokoshiBase1 ) as FamilyInstance ;
          LocationCurve lCurve1 = inst1.Location as LocationCurve ;

          for ( int k = 0 ; k < lstHaraBase.Count() ; k++ ) {
            idHaraokoshiBase2 = lstHaraBase[ k ].m_ElementId ;
            if ( idHaraokoshiBase1 == idHaraokoshiBase2 ) {
              continue ;
            }

            Element inst2 = doc.GetElement( idHaraokoshiBase2 ) ;
            FamilyInstance ShinInst2 = doc.GetElement( idHaraokoshiBase2 ) as FamilyInstance ;
            LocationCurve lCurve2 = inst2.Location as LocationCurve ;

            // 交点と角度を取得
            if ( ! clsSP.GetCrossPointAndAngle( lCurve1, lCurve2, out crossPoint, out radians ) ) {
              continue ;
            }

            clsSP.m_BasePoint = crossPoint ;
            clsSP.m_Angle = radians ;

            // 既に処理済みか確認
            bool exists = myList.Any( item => item.Item1 == idHaraokoshiBase1 && item.Item2 == idHaraokoshiBase2 ) ||
                          myList.Any( item => item.Item1 == idHaraokoshiBase2 && item.Item2 == idHaraokoshiBase1 ) ;
            if ( exists ) {
              continue ;
            }

            //入隅か判定
            bool bIrizumi = false ;
            if ( ! ClsHaraokoshiBase.CheckIrizumi( lCurve1.Curve, lCurve2.Curve, ref bIrizumi ) ) {
              continue ;
            }

            // 腹起ベースが同段か上下段なのか判定
            ElementId id1 = ShinInst1.Id ;
            string strHaraDan1 = ClsRevitUtil.GetParameter( doc, id1, "段" ) ;
            clsSP.m_ParentId1 = id1 ;
            string yoko1 = ClsRevitUtil.GetParameter( doc, id1, "横本数" ) ;
            string haraSize1 = ClsRevitUtil.GetParameter( doc, id1, "鋼材サイズ" ) ;

            ElementId id2 = ShinInst2.Id ;
            string strHaraDan2 = ClsRevitUtil.GetParameter( doc, id2, "段" ) ;
            clsSP.m_ParentId2 = id2 ;
            string yoko2 = ClsRevitUtil.GetParameter( doc, id2, "横本数" ) ;
            string haraSize2 = ClsRevitUtil.GetParameter( doc, id2, "鋼材サイズ" ) ;

            if ( strHaraDan1 == "同段" && strHaraDan2 == "同段" || strHaraDan1 == strHaraDan2 ) {
              // 間詰め量がある場合
              // 最初に選択された腹起ベースに対してずれる
              if ( clsSP.m_Tsumeryo > 0 ) {
                XYZ midPoint2 = ( lCurve2.Curve.GetEndPoint( 0 ) + lCurve2.Curve.GetEndPoint( 1 ) ) / 2.0 ;
                XYZ vec = midPoint2 - crossPoint ;
                vec = vec.Normalize() ;
                XYZ moveVector = vec * ClsRevitUtil.ConvertDoubleGeo2Revit( clsSP.m_Tsumeryo ) ;
                crossPoint = crossPoint + moveVector ;
              }

              //隅部ピースを作図
              clsSP.m_ToritsukeDan = strHaraDan1 ; // "同段";
              if ( ! clsSP.CreateSumibuPiece( doc, id1, crossPoint, haraSize1, radians, true ) ) {
                MessageBox.Show( "隅部ピースの配置に失敗しました" ) ;
              }
            }
            else {
              var strHaraHousuTate1 = ClsRevitUtil.GetParameter( doc, id1, "縦本数" ) ;
              var strHaraHousuTate2 = ClsRevitUtil.GetParameter( doc, id2, "縦本数" ) ;
              if ( ( strHaraHousuTate1 != "シングル" ) || ( strHaraHousuTate2 != "シングル" ) ) {
                //MessageBox.Show("縦シングル以外には配置できません");
                continue ;
              }

              // 上段腹起の方向と下段腹起の方向を算出して渡す
              XYZ midPoint1 = ( lCurve1.Curve.GetEndPoint( 0 ) + lCurve1.Curve.GetEndPoint( 1 ) ) / 2.0 ;
              XYZ midPoint2 = ( lCurve2.Curve.GetEndPoint( 0 ) + lCurve2.Curve.GetEndPoint( 1 ) ) / 2.0 ;

              // 選択1の単位ベクトル
              XYZ selVec1 = midPoint1 - crossPoint ;
              selVec1 = selVec1.Normalize() ;

              // 選択2の単位ベクトル
              XYZ selVec2 = midPoint2 - crossPoint ;
              selVec2 = selVec2.Normalize() ;

              // 1と2の合成
              XYZ midVec = selVec1 + selVec2 ;

              // 無回転時に想定される腹起ベース2つの単位ベクトルとその合成
              XYZ vec1 = new XYZ( -1, 0, 0 ) ;

              // 主材の中央に配置するためのオフセット量を算出
              double dOffset1 = ClsYMSUtil.GetKouzaiHabaFromBase( doc, id1 ) / 2 ;
              XYZ vecA = crossPoint - midPoint1 ;
              vecA = vecA.Normalize() ;

              double dOffset2 = ClsYMSUtil.GetKouzaiHabaFromBase( doc, id2 ) / 2 ;
              XYZ vecB = crossPoint - midPoint2 ;
              vecB = vecB.Normalize() ;

              XYZ moveVectorA =
                vecA * ClsRevitUtil
                  .ConvertDoubleGeo2Revit( dOffset2 ) ; // ClsRevitUtil.ConvertDoubleGeo2Revit(dOffset1);
              XYZ moveVectorB =
                vecB * ClsRevitUtil
                  .ConvertDoubleGeo2Revit( dOffset1 ) ; // ClsRevitUtil.ConvertDoubleGeo2Revit(dOffset2);

              if ( ! bIrizumi ) {
                var tmp1 = moveVectorA ;
                var tmp2 = moveVectorB ;

                moveVectorA = -tmp1 - ( tmp2 * 2 ) ;
                moveVectorB = -tmp2 - ( tmp1 * 2 ) ;
              }

              if ( strHaraDan1 == "上段" && strHaraDan2 == "下段" ) {
                radians = vec1.AngleOnPlaneTo( selVec1, XYZ.BasisZ ) ;
                clsSP.m_BasePoint = crossPoint + moveVectorB ;
                clsSP.m_Angle = radians ;
                clsSP.m_ToritsukeDan = "上段" ;
                if ( ! clsSP.CreateSumibuPiece( doc, id1, crossPoint + moveVectorB, haraSize1, radians, false,
                      true ) ) {
                  MessageBox.Show( "隅部ピースの配置に失敗しました" ) ;
                }

                //横ダブル対応
                if ( yoko1 == "ダブル" ) {
                  clsSP.m_BasePoint = crossPoint + moveVectorB * 3 ;
                  if ( ! clsSP.CreateSumibuPiece( doc, id1, crossPoint + moveVectorB * 3, haraSize1, radians, false,
                        true ) ) {
                    MessageBox.Show( "隅部ピースの配置に失敗しました" ) ;
                  }
                }

                radians = vec1.AngleOnPlaneTo( selVec2, XYZ.BasisZ ) ;
                clsSP.m_BasePoint = crossPoint + moveVectorA ;
                clsSP.m_Angle = radians ;
                clsSP.m_ToritsukeDan = "下段" ;
                if ( ! clsSP.CreateSumibuPiece( doc, id2, crossPoint + moveVectorA, haraSize2, radians, false,
                      false ) ) {
                  MessageBox.Show( "隅部ピースの配置に失敗しました" ) ;
                }

                //横ダブル対応
                if ( yoko2 == "ダブル" ) {
                  clsSP.m_BasePoint = crossPoint + moveVectorA * 3 ;
                  if ( ! clsSP.CreateSumibuPiece( doc, id2, crossPoint + moveVectorA * 3, haraSize2, radians, false,
                        false ) ) {
                    MessageBox.Show( "隅部ピースの配置に失敗しました" ) ;
                  }
                }
              }
              else if ( strHaraDan1 == "下段" && strHaraDan2 == "上段" ) {
                radians = vec1.AngleOnPlaneTo( selVec1, XYZ.BasisZ ) ;
                clsSP.m_BasePoint = crossPoint + moveVectorB ;
                clsSP.m_Angle = radians ;
                clsSP.m_ToritsukeDan = "下段" ;
                if ( ! clsSP.CreateSumibuPiece( doc, id2, crossPoint + moveVectorB, haraSize1, radians, false,
                      false ) ) {
                  MessageBox.Show( "隅部ピースの配置に失敗しました" ) ;
                }

                //横ダブル対応
                if ( yoko2 == "ダブル" ) {
                  clsSP.m_BasePoint = crossPoint + moveVectorB * 3 ;
                  if ( ! clsSP.CreateSumibuPiece( doc, id2, crossPoint + moveVectorB * 3, haraSize1, radians, false,
                        false ) ) {
                    MessageBox.Show( "隅部ピースの配置に失敗しました" ) ;
                  }
                }

                radians = vec1.AngleOnPlaneTo( selVec2, XYZ.BasisZ ) ;
                clsSP.m_BasePoint = crossPoint + moveVectorA ;
                clsSP.m_Angle = radians ;
                clsSP.m_ToritsukeDan = "上段" ;
                if ( ! clsSP.CreateSumibuPiece( doc, id1, crossPoint + moveVectorA, haraSize2, radians, false,
                      true ) ) {
                  MessageBox.Show( "隅部ピースの配置に失敗しました" ) ;
                }

                //横ダブル対応
                if ( yoko1 == "ダブル" ) {
                  clsSP.m_BasePoint = crossPoint + moveVectorA * 3 ;
                  if ( ! clsSP.CreateSumibuPiece( doc, id1, crossPoint + moveVectorA * 3, haraSize2, radians, false,
                        true ) ) {
                    MessageBox.Show( "隅部ピースの配置に失敗しました" ) ;
                  }
                }
              }
              else {
                continue ;
              }
            }

            myList.Add( Tuple.Create( idHaraokoshiBase1, idHaraokoshiBase2 ) ) ;
          }
        }
      }

      return ;
    }

    /// <summary>
    /// 編集処理
    /// </summary>
    /// <param name="uidoc"></param>
    public static void CommandChangeSumibuPiece( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;

      // ユーザーに隅部ピースを選択させる
      ElementId id = null ;
      if ( ! ClsRevitUtil.PickObjectPartSymbolFilter( uidoc, "隅部ピースを選択してください", "隅部ピース", ref id ) ) {
        return ;
      }

      //隅部ピースの情報を取得
      ClsSumibuPiece clsSP = new ClsSumibuPiece() ;

      // インスタンスから拡張データを取得
      bool sakusei = true ;
      ClsRevitUtil.CustomDataGet<bool>( doc, id, "作成方法", out sakusei ) ;
      clsSP.m_CreateType = ( sakusei == true ) ? ClsSumibuPiece.CreateType.Auto : ClsSumibuPiece.CreateType.Manual ;

      string type = null ;
      ClsRevitUtil.CustomDataGet<string>( doc, id, "タイプ", out type ) ;
      clsSP.m_Type = type ;

      string size = null ;
      ClsRevitUtil.CustomDataGet<string>( doc, id, "サイズ", out size ) ;
      clsSP.m_Size = size ;

      bool sizeSelectFlg = true ;
      ClsRevitUtil.CustomDataGet<bool>( doc, id, "指定フラグ", out sizeSelectFlg ) ;
      clsSP.m_SizeSelectFlg = sizeSelectFlg ;

      string tsumeryo = null ;
      ClsRevitUtil.CustomDataGet<string>( doc, id, "間詰め量", out tsumeryo ) ;
      double.TryParse( tsumeryo, out double res1 ) ;
      clsSP.m_Tsumeryo = res1 ;

      string basepoint = null ;
      ClsRevitUtil.CustomDataGet<string>( doc, id, "基点", out basepoint ) ;
      clsSP.m_BasePoint = ClsRevitUtil.StringToXYZ( basepoint ) ;

      string angle = null ;
      ClsRevitUtil.CustomDataGet<string>( doc, id, "回転角度", out angle ) ;
      double.TryParse( angle, out double res2 ) ;
      clsSP.m_Angle = res2 ;

      ElementId id1 = ElementId.InvalidElementId ;
      ClsRevitUtil.CustomDataGet<ElementId>( doc, id, "腹起ID1", out id1 ) ;
      clsSP.m_ParentId1 = id1 ;

      ElementId id2 = ElementId.InvalidElementId ;
      ClsRevitUtil.CustomDataGet<ElementId>( doc, id, "腹起ID2", out id2 ) ;
      clsSP.m_ParentId2 = id2 ;

      ClsRevitUtil.CustomDataGet<string>( doc, id, "取付段", out string toritsukeDan ) ;
      clsSP.m_ToritsukeDan = toritsukeDan ;

      // 取得した拡張データを用いてダイアログを表示
      DLG.DlgCreateSumibuPiece SP = new DLG.DlgCreateSumibuPiece( clsSP ) ;
      DialogResult result = SP.ShowDialog() ;
      if ( result != DialogResult.OK ) {
        return ;
      }

      clsSP = SP.m_ClsSumibuPiece ;

      Element inst1 = doc.GetElement( id1 ) ;
      FamilyInstance ShinInst1 = doc.GetElement( id1 ) as FamilyInstance ;
      LocationCurve lCurve1 = inst1.Location as LocationCurve ;
      string haraSize1 = ClsRevitUtil.GetParameter( doc, id1, "鋼材サイズ" ) ;

      Element inst2 = doc.GetElement( id2 ) ;
      FamilyInstance ShinInst2 = doc.GetElement( id2 ) as FamilyInstance ;
      LocationCurve lCurve2 = inst2.Location as LocationCurve ;
      string haraSize2 = ClsRevitUtil.GetParameter( doc, id2, "鋼材サイズ" ) ;

      // 間詰め量がある場合
      // 最初に選択された腹起ベースに対してずれる
      XYZ midPoint2 = ( lCurve2.Curve.GetEndPoint( 0 ) + lCurve2.Curve.GetEndPoint( 1 ) ) / 2.0 ;
      XYZ vec = midPoint2 - clsSP.m_BasePoint ;
      vec = vec.Normalize() ;
      XYZ moveVector = vec * ClsRevitUtil.ConvertDoubleGeo2Revit( clsSP.m_Tsumeryo ) ;
      XYZ crossPoint = clsSP.m_BasePoint + moveVector ;

      // 既存のインスタンスを一旦ずらす
      XYZ moveVector1 = new XYZ( 10.0, 0.0, 0.0 ) ;
      using ( Transaction transaction = new Transaction( doc, "Move Instance" ) ) {
        transaction.Start() ;
        ElementTransformUtils.MoveElement( doc, id, moveVector1 ) ;
        transaction.Commit() ;
      }

      // 隅部ピースを再作図
      bool res = false ;
      ClsRevitUtil.CustomDataGet<string>( doc, id, "取付段", out string haraDan ) ;
      if ( haraDan == "同段" ) {
        //res = clsSP.CreateSumibuPiece(doc, id1, clsSP.m_BasePoint, clsSP.m_Angle, true);
        res = clsSP.CreateSumibuPiece( doc, id1, crossPoint, haraSize1, clsSP.m_Angle, true ) ;
      }
      else if ( haraDan == "上段" ) {
        res = clsSP.CreateSumibuPiece( doc, id1, clsSP.m_BasePoint, haraSize1, clsSP.m_Angle, false, true ) ;
      }
      else if ( haraDan == "下段" ) {
        res = clsSP.CreateSumibuPiece( doc, id2, clsSP.m_BasePoint, haraSize2, clsSP.m_Angle, false, false ) ;
      }

      // 古いインスタンスを削除する
      if ( res ) {
        using ( Transaction transaction = new Transaction( doc, "Delete Elements" ) ) {
          transaction.Start() ;
          doc.Delete( id ) ;
          transaction.Commit() ;
        }
      }
      else {
        XYZ moveVector2 = new XYZ( 0.0, -10.0, 0.0 ) ; // Y方向に-10ユニット移動
        using ( Transaction transaction = new Transaction( doc, "Move Instance Back to Original Position" ) ) {
          transaction.Start() ;
          ElementTransformUtils.MoveElement( doc, id, moveVector2 ) ;
          transaction.Commit() ;
        }
      }
    }
  }
}