using Autodesk.Revit.DB;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace YMS.Parts
{
    public class ClsJosn
    {
        public static void ReadJOSN(Document doc)
        {
            string path = string.Empty;
            if(ClsYMSUtil.SelectJSONFile(ref path))
            {
                var data = DeserializeFromFile<JosnData>(path);
                if(data is JosnData jData && data != null)
                {
                    foreach(var rootLayer in jData.RootLayers)
                    {
                        if (rootLayer.Object.children == null)
                            continue;

                        //rootLayer.Object.children[0]=支保工のもの？
                        //rootLayer.Object.children[0].children[0]//腹起などグループ分けされているかも切梁など増えたら分ける必要がある
                        var chilchil = rootLayer.Object.children[0].children[0];
                        var name = chilchil.name;
                        if (!name.Contains("腹起"))//腹起でTESTを行っている
                            continue;
                        foreach (var chilchilchil in chilchil.children)
                        {
                            var customData = chilchilchil.UserData.CustomData;

                            //位置のみでクラスデータを持っているため対象のPointを探す
                            var array = new List<double>();
                            foreach(var geometrie in rootLayer.Geometries)
                            {
                                if(geometrie.uuid == chilchilchil.geometry)
                                {
                                    array = geometrie.data.Attributes.Position.array.ToList();
                                    break;
                                }
                            }
                            if (array.Count != 6)//始終点のみのはず場合によっては一点も
                                continue;

                            //腹起作成TEST
                            var tmpStPoint = new XYZ(array[0], array[1], array[2]);
                            var tmpEdPoint = new XYZ(array[3], array[4], array[5]);

                            var clsHaraokoshiBase = new ClsHaraokoshiBase();
                            /*ここは読み込んだ値*/
                            clsHaraokoshiBase.m_kouzaiType = customData.steelType;
                            clsHaraokoshiBase.m_kouzaiSize = customData.steelSize;
                            clsHaraokoshiBase.m_level = rootLayer.Object.name;// "支保工1段目";//Levelのプロパティが現在ない不明
                            clsHaraokoshiBase.m_dan = customData.stage;
                            clsHaraokoshiBase.m_yoko = customData.horizontalBraceCount == "シングル" ? ClsHaraokoshiBase.SideNum.Single : ClsHaraokoshiBase.SideNum.Double;
                            clsHaraokoshiBase.m_tate = customData.verticalBraceCount == "シングル" ? ClsHaraokoshiBase.VerticalNum.Single : ClsHaraokoshiBase.VerticalNum.Double;
                            /*ここは読み込んだ値*/

                            //Lightで腹起が中心で作成されている場合オフセット
                            double syuzaiSize = 0;// Master.ClsYamadomeCsv.GetWidth("40HA");
                            clsHaraokoshiBase.CreateHaraokoshiBase(doc, tmpStPoint, tmpEdPoint, syuzaiSize);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// JSONファイルからの読み込み
        /// </summary>
        /// <typeparam name="T">データ型</typeparam>
        /// <param name="path">ファイルパス</param>
        /// <returns></returns>
        public static T DeserializeFromFile<T>(string path)
        {
            if (File.Exists(path) is false) return default;
            T data;
            try
            {
                // JSONファイルを開く
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    // JSONファイルを読み出す
                    using (var sr = new StreamReader(stream))
                    {
                        // デシリアライズオブジェクト関数に読み込んだデータを渡して、
                        // 指定されたデータ用のクラス型で値を返す。
                        //string test = "{\"metaData\":{\"version\":\"0.0.1\", \"created_at\":\"past\", \"updated_at\":\"2025_01_30_12_10_18\", \"created_by\":\"whoami\", \"updated_by\":\"su\"}}";//TEST用
                        var readEnd = sr.ReadToEnd();
                        data = JsonConvert.DeserializeObject<T>(readEnd);//直接sr.ReadToEnd()を引数に設定すると読込が出来ない
                        
                        //return JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
                    }
                }
            }
            catch (Exception ex)
            {
                string mess = ex.Message;
                return default;
            }
            return data;
        }
        public static void CreateBase(Document doc)
        {
            XYZ tmpStPoint = new XYZ();
            XYZ tmpEdPoint = new XYZ();

            List<List<XYZ>> sePointList = new List<List<XYZ>>();
            List<XYZ> pointList = new List<XYZ>();
            ////本目
            //pointList.Add(new XYZ(0, 0, 0));
            //pointList.Add(new XYZ(0, 0, 0));
            //sePointList.Add(pointList.ToList());
            //pointList.Clear();

            ////////////////////腹起ベース/////////////////////
            //1本目
            pointList.Add(new XYZ(-27.2144134912447, -7.883328489805398, 0));
            pointList.Add(new XYZ(0.954177183384225, -7.88332848905407, 0));
            sePointList.Add(pointList.ToList());
            pointList.Clear();
            //2本目
            pointList.Add(new XYZ(-0.358158774621028, -9.19566444705932, 0));
            pointList.Add(new XYZ(-0.358158774620964, 10.4893749230194, 0));
            sePointList.Add(pointList.ToList());
            pointList.Clear();
            //3本目
            pointList.Add(new XYZ(0.954177183384281, 9.17703896501417, 0));
            pointList.Add(new XYZ(-27.2610459137286, 9.17703896501426, 0));
            sePointList.Add(pointList.ToList());
            pointList.Clear();
            //4本目
            pointList.Add(new XYZ(-25.9020775332394, 11.145542902021, 0));
            pointList.Add(new XYZ(-25.9020775332395, -9.19566444705923, 0));
            sePointList.Add(pointList.ToList());
            pointList.Clear();

            foreach (var point in sePointList)
            {
                tmpStPoint = point[0];
                tmpEdPoint = point[1];

                var clsHaraokoshiBase = new ClsHaraokoshiBase();
                /*ここは読み込んだ値*/
                clsHaraokoshiBase.m_kouzaiType = "主材";
                clsHaraokoshiBase.m_kouzaiSize = "40HA";
                clsHaraokoshiBase.m_level = "支保工1段目";
                clsHaraokoshiBase.m_dan = "同段";
                clsHaraokoshiBase.m_yoko = "シングル" == "シングル" ? ClsHaraokoshiBase.SideNum.Single : ClsHaraokoshiBase.SideNum.Double;
                clsHaraokoshiBase.m_tate = "シングル" == "シングル" ? ClsHaraokoshiBase.VerticalNum.Single : ClsHaraokoshiBase.VerticalNum.Double;
                /*ここは読み込んだ値*/

                //Lightで腹起が中心で作成されている場合オフセット
                double syuzaiSize = 0;// Master.ClsYamadomeCsv.GetWidth("40HA");
                clsHaraokoshiBase.CreateHaraokoshiBase(doc, tmpStPoint, tmpEdPoint, syuzaiSize);
            }
            sePointList.Clear();
            ////////////////////腹起ベース/////////////////////
            ////////////////////切梁ベース/////////////////////
            //1本目
            pointList.Add(new XYZ(-13.1301181539302, -7.88332848905403, 0));
            pointList.Add(new XYZ(-13.1301181539302, 9.17703896501421, 0));
            sePointList.Add(pointList.ToList());
            pointList.Clear();
            //2本目
            pointList.Add(new XYZ(-25.9020775332395, 0.974939227481457, 0));
            pointList.Add(new XYZ(-0.358158774620996, 0.974939227481374, 0));
            sePointList.Add(pointList.ToList());
            pointList.Clear();
            //3本目
            pointList.Add(new XYZ(-25.9020775332394, 3.62925721568506, 0));
            pointList.Add(new XYZ(-0.358158774620986, 3.62925721568498, 0));
            sePointList.Add(pointList.ToList());
            pointList.Clear();

            foreach (var point in sePointList)
            {
                tmpStPoint = point[0];
                tmpEdPoint = point[1];

                var clsKiribariBase = new ClsKiribariBase();
                /*ここは読み込んだ値*/
                clsKiribariBase.m_kouzaiType = "主材";
                clsKiribariBase.m_kouzaiSize = "40HA";
                clsKiribariBase.m_Floor = "支保工1段目";
                clsKiribariBase.m_tanbuStart = "なし";
                clsKiribariBase.m_tanbuEnd = "なし";
                clsKiribariBase.m_jack1 = "ｷﾘﾝｼﾞｬｯｷ(KJ)";
                clsKiribariBase.m_jack2 = "油圧ｼﾞｬｯｷ(KOP)";
                /*ここは読み込んだ値*/

                clsKiribariBase.m_Dan = "同段";//まだ不明。腹起との接触判定で段を取得するものと思われる

                clsKiribariBase.CreateKiribariBase(doc, tmpStPoint, tmpEdPoint);
            }
            sePointList.Clear();
            ////////////////////切梁ベース///////////////////
            ////////////////////ジャッキ/////////////////////
            //1本目
            pointList.Add(new XYZ(-21.820998517, 0.974939227, -4.921259843));
            sePointList.Add(pointList.ToList());
            pointList.Clear();
            //2本目
            //pointList.Add(new XYZ(-25.9020775332395, 0.974939227481457, 0));
            //sePointList.Add(pointList.ToList());
            //pointList.Clear();

            foreach (var point in sePointList)
            {
                var insertPoint = point[0];
                insertPoint = new XYZ(insertPoint.X, insertPoint.Y, 0);

                var clsJack = new ClsJack();
                /*ここは読み込んだ値*/
                clsJack.m_JackType = "ｷﾘﾝｼﾞｬｯｷ(KJ)";
                clsJack.m_SyuzaiSize = "40HA";
                ElementId levalId = ClsRevitUtil.GetLevelID(doc, "支保工1段目");
                ElementId baseId = null;
                var baseIdList = ClsKiribariBase.GetAllKiribariBaseList(doc, levalId);
                foreach(var id in baseIdList)
                {
                    var inst = doc.GetElement(id) as FamilyInstance;
                    var cv = (inst.Location as LocationCurve).Curve;
                    tmpStPoint = cv.GetEndPoint(0);
                    tmpEdPoint = cv.GetEndPoint(1);
                    cv = Line.CreateBound(new XYZ(tmpStPoint.X, tmpStPoint.Y, 0), new XYZ(tmpEdPoint.X, tmpEdPoint.Y, 0));
                    if (ClsRevitUtil.IsPointOnCurve(cv, insertPoint))
                    {
                        baseId = id;
                        //tmpStPoint = cv.GetEndPoint(0);
                        //tmpEdPoint = cv.GetEndPoint(1);
                        break;
                    }
                }
                /*ここは読み込んだ値*/

                if(baseId != null)
                    clsJack.CreateJack(doc, baseId, tmpStPoint, tmpEdPoint, insertPoint);
            }
            sePointList.Clear();
            ////////////////////ジャッキ/////////////////////
            ////////////////////隅火打ベース/////////////////
            //1本目
            pointList.Add(new XYZ(-25.9020775332395, -4.40347228242728, 0));
            pointList.Add(new XYZ(-22.4222213266128, -7.883328489054, 0));
            sePointList.Add(pointList.ToList());
            pointList.Clear();
            //2本目
            pointList.Add(new XYZ(-3.83801498124773, -7.88332848905406, 0));
            pointList.Add(new XYZ(-0.358158774621012, -4.40347228242736, 0));
            sePointList.Add(pointList.ToList());
            pointList.Clear();
            //3本目
            pointList.Add(new XYZ(-0.358158774620979, 5.69718275838746, 0));
            pointList.Add(new XYZ(-3.83801498124768, 9.17703896501418, 0));
            sePointList.Add(pointList.ToList());
            pointList.Clear();
            //4本目
            pointList.Add(new XYZ(-22.4222213266127, 9.17703896501424, 0));
            pointList.Add(new XYZ(-25.9020775332394, 5.69718275838755, 0));
            sePointList.Add(pointList.ToList());
            pointList.Clear();
            //5本目
            pointList.Add(new XYZ(-20.1023171888616, -7.883328489054, 0));
            pointList.Add(new XYZ(-25.9020775332395, -2.08356814467613, 0));
            sePointList.Add(pointList.ToList());
            pointList.Clear();

            foreach (var point in sePointList)
            {
                tmpStPoint = point[0];
                tmpEdPoint = point[1];
                double length = Line.CreateBound(tmpStPoint, tmpEdPoint).Length;

                var clsCornerHiuchiBase = new ClsCornerHiuchiBase();
                /*ここは読み込んだ値*/
                clsCornerHiuchiBase.m_SteelType = "主材";
                clsCornerHiuchiBase.m_SteelSize = "40HA";
                clsCornerHiuchiBase.m_Floor = "支保工1段目";
                clsCornerHiuchiBase.m_Kousei = "ダブル" == "シングル" ? ClsCornerHiuchiBase.Kousei.Single : ClsCornerHiuchiBase.Kousei.Double;
                /*ここは読み込んだ値*/

                clsCornerHiuchiBase.m_angle = 45.00;//角度の求め方は不明。接触する腹起とのなす角を求めるものと思われる※方法等分と同じ
                clsCornerHiuchiBase.m_Dan = "同段";//まだ不明。腹起との接触判定で段を取得するものと思われる

                //とりあえずなしで設定。Lightで端部部品が設定できないのに何か設定してしまうと長さが合わないため
                clsCornerHiuchiBase.m_TanbuParts1 = "なし";
                //clsCornerHiuchiBase.m_HiuchiUkePieceSize1 = "なし";
                clsCornerHiuchiBase.m_TanbuParts2 = "なし";
                //clsCornerHiuchiBase.m_HiuchiUkePieceSize2 = "なし";

                /*位置から算出した値*/
                clsCornerHiuchiBase.m_HiuchiZureryo = 0.0;
                clsCornerHiuchiBase.m_HiuchiLengthDoubleUpperL1 = ClsRevitUtil.CovertFromAPI(length);
                clsCornerHiuchiBase.m_HiuchiLengthDoubleUnderL2 = ClsRevitUtil.CovertFromAPI(length);
                clsCornerHiuchiBase.m_HiuchiLengthSingleL = ClsRevitUtil.CovertFromAPI(length);
                /*位置から算出した値*/

                clsCornerHiuchiBase.CreateCornerHiuchi(doc, tmpStPoint, tmpEdPoint);
            }
            sePointList.Clear();
            ///////////////////隅火打ベース/////////////////////
            ////////////////////切梁火打ベース//////////////////
            //1本目
            pointList.Add(new XYZ(-13.1301181539302, -4.38259314518755, 0));
            pointList.Add(new XYZ(-16.6308534977967, -7.88332848905402, 0));
            sePointList.Add(pointList.ToList());
            pointList.Clear();
            //2本目
            pointList.Add(new XYZ(-13.1301181539302, -4.38259314518756, 0));
            pointList.Add(new XYZ(-9.62938281006377, -7.88332848905404, 0));
            sePointList.Add(pointList.ToList());
            pointList.Clear();
            //3本目
            pointList.Add(new XYZ(-13.1301181539302, 5.67630362114775, 0));
            pointList.Add(new XYZ(-9.62938281006375, 9.1770389650142, 0));
            sePointList.Add(pointList.ToList());
            pointList.Clear();
            //4本目
            pointList.Add(new XYZ(-13.1301181539302, 5.67630362114774, 0));
            pointList.Add(new XYZ(-16.6308534977967, 9.17703896501422, 0));
            sePointList.Add(pointList.ToList());
            pointList.Clear();

            foreach (var point in sePointList)
            {
                tmpStPoint = point[0];
                tmpEdPoint = point[1];
                double length = Line.CreateBound(tmpStPoint, tmpEdPoint).Length;

                var clsKiribariHiuchiBase = new ClsKiribariHiuchiBase();
                /*ここは読み込んだ値*/
                clsKiribariHiuchiBase.m_SteelType = "主材";
                clsKiribariHiuchiBase.m_Floor = "支保工1段目";
                string syoriType = "切梁-腹起";
                if(syoriType == "切梁－切梁")
                {
                    clsKiribariHiuchiBase.m_ShoriType = ClsKiribariHiuchiBase.ShoriType.KiriKiri;
                }
                else if(syoriType == "切梁-腹起")
                {
                    clsKiribariHiuchiBase.m_ShoriType = ClsKiribariHiuchiBase.ShoriType.KiriHara;
                }
                else
                {
                    clsKiribariHiuchiBase.m_ShoriType = ClsKiribariHiuchiBase.ShoriType.SanjikuHara;
                }
                clsKiribariHiuchiBase.m_CreateHoho = "シングル" == "シングル" ? ClsKiribariHiuchiBase.CreateHoho.Single : ClsKiribariHiuchiBase.CreateHoho.Double;
                if(clsKiribariHiuchiBase.m_CreateHoho == ClsKiribariHiuchiBase.CreateHoho.Single)
                {
                    clsKiribariHiuchiBase.m_SteelSizeSingle = "40HA";
                }
                else
                {
                    clsKiribariHiuchiBase.m_SteelSizeDoubleUpper = "40HA";
                    clsKiribariHiuchiBase.m_SteelSizeDoubleUnder = "40HA";
                }
                clsKiribariHiuchiBase.m_Angle = 45.00;
                clsKiribariHiuchiBase.m_Dan = "同段";
                /*ここは読み込んだ値*/

                //とりあえずなしで設定。Lightで端部部品が設定できないのに何か設定してしまうと長さが合わないため
                clsKiribariHiuchiBase.m_PartsTypeKiriSide = "なし";
                //clsKiribariHiuchiBase.m_PartsSizeKiriSide = "なし";
                clsKiribariHiuchiBase.m_PartsTypeHaraSide = "なし";
                //clsKiribariHiuchiBase.m_PartsSizeHaraSide = "なし";

                /*位置から算出した値*/
                clsKiribariHiuchiBase.m_HiuchiZureRyo = 0;
                clsKiribariHiuchiBase.m_HiuchiLengthDoubleUpperL1 = (int)ClsRevitUtil.CovertFromAPI(length);
                clsKiribariHiuchiBase.m_HiuchiLengthDoubleUnderL2 = (int)ClsRevitUtil.CovertFromAPI(length);
                clsKiribariHiuchiBase.m_HiuchiLengthSingleL = (int)ClsRevitUtil.CovertFromAPI(length);
                /*位置から算出した値*/

                clsKiribariHiuchiBase.CreateKiribariHiuchiBase(doc, tmpStPoint, tmpEdPoint);
            }
            sePointList.Clear();
            ///////////////////切梁火打ベース/////////////////////
            ///////////////////切梁受けベース/////////////////////
            //1本目
            pointList.Add(new XYZ(-18.0716553876434, -1.62008661633595, 0));
            pointList.Add(new XYZ(-7.90105171310275, -1.62008661633599, 0));
            sePointList.Add(pointList.ToList());
            pointList.Clear();

            foreach (var point in sePointList)
            {
                tmpStPoint = point[0];
                tmpEdPoint = point[1];

                var clsKiribariUkeBase = new ClsKiribariUkeBase();
                /*ここは読み込んだ値*/
                clsKiribariUkeBase.m_SteelType = "主材";
                clsKiribariUkeBase.m_SteelSize = "40HA";
                clsKiribariUkeBase.m_Floor = "支保工1段目";
                /*ここは読み込んだ値*/

                clsKiribariUkeBase.m_Dan = "下段";//固定
                clsKiribariUkeBase.m_TsukidashiRyoS = 0;//突き出し量のデフォルトは不明
                clsKiribariUkeBase.m_TsukidashiRyoE = 0;

                clsKiribariUkeBase.ChangeKiribariUkeBase(doc, tmpStPoint, tmpEdPoint);
            }
            sePointList.Clear();
            ////////////////////切梁受けベース/////////////////////
            ////////////////////切梁繋ぎ材ベース/////////////////////
            //1本目
            pointList.Add(new XYZ(-4.65044674951582, 4.76272814284305, 0));
            pointList.Add(new XYZ(-4.65044674951583, -0.158531699676638, 0));
            sePointList.Add(pointList.ToList());
            pointList.Clear();

            foreach (var point in sePointList)
            {
                tmpStPoint = point[0];
                tmpEdPoint = point[1];

                var clsKiribariTsunagizaiBase = new ClsKiribariTsunagizaiBase();
                /*ここは読み込んだ値*/
                clsKiribariTsunagizaiBase.m_SteelType = "主材";
                clsKiribariTsunagizaiBase.m_SteelSize = "40HA";
                clsKiribariTsunagizaiBase.m_Floor = "支保工1段目";
                string torituke = "ボルト";
                if(torituke == "ボルト")
                {
                    clsKiribariTsunagizaiBase.m_ToritsukeHoho = ClsKiribariTsunagizaiBase.ToritsukeHoho.Bolt;
                    clsKiribariTsunagizaiBase.m_BoltType1 = "普通ボルト";//orハイテンションボルトorトルシア型ボルト
                    List<string> sizeList = Master.ClsBoltCsv.GetSizeList(clsKiribariTsunagizaiBase.m_BoltType1);
                    if(sizeList.Count != 0)
                        clsKiribariTsunagizaiBase.m_BoltType2 = sizeList[0];//ボルト種類ごとの初期定義をデフォルトに
                    clsKiribariTsunagizaiBase.m_BoltNum = 1;//ボルトデフォルト本数は不明
                }
                else if (torituke == "ブルマン")
                {
                    clsKiribariTsunagizaiBase.m_ToritsukeHoho = ClsKiribariTsunagizaiBase.ToritsukeHoho.Buruman;
                }
                else
                {
                    clsKiribariTsunagizaiBase.m_ToritsukeHoho = ClsKiribariTsunagizaiBase.ToritsukeHoho.Rikiman;
                }
                /*ここは読み込んだ値*/

                clsKiribariTsunagizaiBase.m_Dan = "上段";//固定
                
                clsKiribariTsunagizaiBase.ChangeKiribariTsunagizaiBase(doc, tmpStPoint, tmpEdPoint);
            }
            sePointList.Clear();
            ////////////////////切梁繋ぎ材ベース/////////////////////
            ////////////////////火打繋ぎ材ベース/////////////////////
            //1本目
            pointList.Add(new XYZ(-21.7804336730623, -5.0452599359778, 0));
            pointList.Add(new XYZ(-24.1003378108134, -7.36516407372893, 0));
            sePointList.Add(pointList.ToList());
            pointList.Clear();

            foreach (var point in sePointList)
            {
                tmpStPoint = point[0];
                tmpEdPoint = point[1];

                var clsHiuchiTsunagizaiBase = new ClsHiuchiTsunagizaiBase();
                /*ここは読み込んだ値*/
                clsHiuchiTsunagizaiBase.m_SteelType = "主材";
                clsHiuchiTsunagizaiBase.m_SteelSize = "40HA";
                clsHiuchiTsunagizaiBase.m_Floor = "支保工1段目";
                string torituke = "ボルト";
                if (torituke == "ボルト")
                {
                    clsHiuchiTsunagizaiBase.m_ToritsukeHoho = ClsHiuchiTsunagizaiBase.ToritsukeHoho.Bolt;
                    clsHiuchiTsunagizaiBase.m_BoltType1 = "普通ボルト";//orハイテンションボルトorトルシア型ボルト
                    List<string> sizeList = Master.ClsBoltCsv.GetSizeList(clsHiuchiTsunagizaiBase.m_BoltType1);
                    if (sizeList.Count != 0)
                        clsHiuchiTsunagizaiBase.m_BoltType2 = sizeList[0];//ボルト種類ごとの初期定義をデフォルトに
                    clsHiuchiTsunagizaiBase.m_BoltNum = 1;//ボルトデフォルト本数は不明
                }
                else if (torituke == "ブルマン")
                {
                    clsHiuchiTsunagizaiBase.m_ToritsukeHoho = ClsHiuchiTsunagizaiBase.ToritsukeHoho.Buruman;
                }
                else
                {
                    clsHiuchiTsunagizaiBase.m_ToritsukeHoho = ClsHiuchiTsunagizaiBase.ToritsukeHoho.Rikiman;
                }
                /*ここは読み込んだ値*/

                clsHiuchiTsunagizaiBase.m_Dan = "上段";//固定

                clsHiuchiTsunagizaiBase.ChangeHiuchiTsunagizaiBase(doc, tmpStPoint, tmpEdPoint);
            }
            sePointList.Clear();
            ////////////////////火打繋ぎ材ベース/////////////////////
            ////////////////////切梁継ぎベース/////////////////////
            //1本目
            pointList.Add(new XYZ(-19.0123137537119, 3.62925721568504, 0));
            pointList.Add(new XYZ(-19.0123137537119, 9.17703896501423, 0));
            sePointList.Add(pointList.ToList());
            pointList.Clear();

            foreach (var point in sePointList)
            {
                tmpStPoint = point[0];
                tmpEdPoint = point[1];
                double length = Line.CreateBound(tmpStPoint, tmpEdPoint).Length;

                var clsKiribariTsugiBase = new ClsKiribariTsugiBase();
                /*ここは読み込んだ値*/
                clsKiribariTsugiBase.m_SteelType = "主材";
                clsKiribariTsugiBase.m_Floor = "支保工1段目";
                clsKiribariTsugiBase.m_Kousei = "ダブル" == "シングル" ? ClsKiribariTsugiBase.Kousei.Single : ClsKiribariTsugiBase.Kousei.Double;
                if (clsKiribariTsugiBase.m_Kousei == ClsKiribariTsugiBase.Kousei.Single)
                {
                    clsKiribariTsugiBase.m_SteelSizeSingle = "40HA";
                }
                else
                {
                    clsKiribariTsugiBase.m_KiriSideSteelSizeDouble = "40HA";
                    clsKiribariTsugiBase.m_HaraSideSteelSizeDouble = "40HA";
                }
                clsKiribariTsugiBase.m_Dan = "上段";
                /*ここは読み込んだ値*/

                //とりあえずなしで設定。Lightで端部部品が設定できないのに何か設定してしまうと長さが合わないため
                clsKiribariTsugiBase.m_KiriSideParts = "なし";
                clsKiribariTsugiBase.m_HaraSideParts = "なし";

                /*位置から算出した値*/
                clsKiribariTsugiBase.m_KiriSideTsunagiLength = (int)ClsRevitUtil.CovertFromAPI(length);
                clsKiribariTsugiBase.m_HaraSideTsunagiLength = (int)ClsRevitUtil.CovertFromAPI(length);
                /*位置から算出した値*/

                clsKiribariTsugiBase.CreateKiribariTsugiBase(doc, tmpStPoint, tmpEdPoint);
            }
            sePointList.Clear();
            ///////////////////切梁継ぎベース/////////////////////
            //斜梁は未実装
        }
        public static void CreateKabe(Document doc)
        {
            XYZ tmpStPoint = new XYZ();
            XYZ tmpEdPoint = new XYZ();

            List<List<XYZ>> sePointList = new List<List<XYZ>>();
            List<XYZ> pointList = new List<XYZ>();
            ////本目
            //pointList.Add(new XYZ(0, 0, 0));
            //pointList.Add(new XYZ(0, 0, 0));
            //sePointList.Add(pointList.ToList());
            //pointList.Clear();
            
            ////////////////////壁芯/////////////////////
            //作成した壁芯のIDは保持しておく必要があると思われる
            List<JosnKabe> kabeShinList = new List<JosnKabe>();
            //1本目
            pointList.Add(new XYZ(-27.542497480746, -9.85183242606185, 0));
            pointList.Add(new XYZ(1.32889359536944, -9.85183242606194, 0));
            sePointList.Add(pointList.ToList());
            pointList.Clear();
            //2本目
            pointList.Add(new XYZ(1.32889359536944, -9.85183242606194, 0));
            pointList.Add(new XYZ(1.32889359536951, 11.145542902022, 0));
            sePointList.Add(pointList.ToList());
            pointList.Clear();
            //3本目
            pointList.Add(new XYZ(1.32889359536951, 11.1455429020221, 0));
            pointList.Add(new XYZ(-27.542497480746, 11.1455429020221, 0));
            sePointList.Add(pointList.ToList());
            pointList.Clear();
            //4本目
            pointList.Add(new XYZ(-27.542497480746, 11.1455429020221, 0));
            pointList.Add(new XYZ(-27.542497480746, -9.85183242606185, 0));
            sePointList.Add(pointList.ToList());
            pointList.Clear();

            foreach (var point in sePointList)
            {
                tmpStPoint = point[0];
                tmpEdPoint = point[1];

                var clsKabeShin = new ClsKabeShin();
                /*ここは読み込んだ値*/
                string levelName = "GL";
                string kabeType = "鋼矢板";
                /*ここは読み込んだ値*/

                ElementId createId = null;
                if (!ClsKabeShin.CreateKabeShin(doc, levelName, tmpStPoint, tmpEdPoint, ref createId))
                    continue;//kabeShinList.Add(new JosnKabe(createId, kabeType));


                var kabeShin = createId;
                var inst = doc.GetElement(kabeShin) as FamilyInstance;
                //var lCurve = inst.Location as LocationCurve;
                //tmpStPoint = lCurve.Curve.GetEndPoint(0);
                //tmpEdPoint = lCurve.Curve.GetEndPoint(1);
                var levelId = inst.Host.Id;

                switch (kabeType)
                {
                    case "鋼矢板":
                        {
                            ////////////////////鋼矢板/////////////////////
                            /*ここは読み込んだ値*/
                            var clsKouyaita = new ClsKouyaita();
                            clsKouyaita.m_case = ClsCommonUtils.ChangeStrToInt("1");
                            clsKouyaita.m_type = "鋼矢板";
                            clsKouyaita.m_size = "SP-3";
                            clsKouyaita.m_HTop = ClsCommonUtils.ChangeStrToInt("0");
                            clsKouyaita.m_HLen = ClsCommonUtils.ChangeStrToInt("10000");
                            clsKouyaita.m_bIzyou = false;//値が何で来るかは不明
                            clsKouyaita.m_zaishitu = "SY295";
                            clsKouyaita.m_zanti = "なし";
                            clsKouyaita.m_zantiLength = "0";
                            clsKouyaita.m_Kasho1 = ClsCommonUtils.ChangeStrToInt("2");
                            //clsKouyaita.m_Kasho2 = ClsCommonUtils.ChangeStrToInt("2");//1と一緒
                            clsKouyaita.m_Kotei1 = "ボルト" == "ボルト" ? Master.Kotei.Bolt : Master.Kotei.Yousetsu;
                            //clsKouyaita.m_Kotei2 = "ボルト" == "ボルト" ? Master.Kotei.Bolt : Master.Kotei.Yousetsu;
                            /*ここは読み込んだ値*/
                            //一緒枝番は何かに固定する必要がある
                            clsKouyaita.m_edaNum = "A";
                            //clsKouyaita.m_edaNum2 = "A";
                            clsKouyaita.m_way = 1;//多分いらない
                                                  //掘削深さの設定は要相談
                            clsKouyaita.m_void = ClsCommonUtils.ChangeStrToInt("5000");
                            clsKouyaita.m_putVec = "交互" == "交互" ? PutVec.Kougo : PutVec.Const;//作成方法によって必要性が変わる
                            clsKouyaita.m_kouyaitaSize = "SP-C3";//コーナー鋼矢板サイズは自動設定の可能性あり
                            clsKouyaita.m_KougoFlg = false;//交互配置は無いので確定

                            //設定があるか不明
                            clsKouyaita.TensetuVec1 = "腹側" == "腹側" ? TensetuVec.HaraSide : TensetuVec.SenakaSide;
                            //clsKouyaita.TensetuVec2 = "腹側" == "腹側" ? TensetuVec.HaraSide : TensetuVec.SenakaSide;

                            List<int> lstN1 = new List<int>();
                            int kuiLength1 = clsKouyaita.m_HLen / clsKouyaita.m_Kasho1;
                            for (int i = 0; i < clsKouyaita.m_Kasho1; i++)
                            {
                                //最終杭長さは余りを全て吸収する
                                if (i + 1 == clsKouyaita.m_Kasho1)
                                {
                                    lstN1.Add(clsKouyaita.m_HLen - kuiLength1 * i);
                                }
                                else
                                    lstN1.Add(kuiLength1);
                            }
                            clsKouyaita.m_ListPileLength1 = lstN1.ToList(); ;

                            string bolt = "BN-50";
                            string boltNum = "8";
                            clsKouyaita.m_BoltF1 = bolt;
                            clsKouyaita.m_BoltFNum1 = boltNum;
                            //Lightに設定項目が1つしかないので側やコーナーに関係なく同じと思われる
                            clsKouyaita.m_BoltW1 = bolt;
                            clsKouyaita.m_BoltWNum1 = boltNum;
                            clsKouyaita.m_BoltCornerF1 = bolt;
                            clsKouyaita.m_BoltCornerFNum1 = boltNum;
                            clsKouyaita.m_BoltCornerW1 = bolt;
                            clsKouyaita.m_BoltCornerWNum1 = boltNum;
                            VoidVec lastVoidVec = VoidVec.Kussaku;
                            clsKouyaita.CreateKouyaita1(doc, tmpStPoint, tmpEdPoint, levelId, VoidVec.Kussaku, ref lastVoidVec, kabeShinId: kabeShin);
                            break;
                        }
                    case "親杭":
                        {
                            /*ここは読み込んだ値*/
                            ClsOyakui clsOyakui = new ClsOyakui();
                            clsOyakui.m_case = ClsCommonUtils.ChangeStrToInt("1");
                            clsOyakui.m_void = ClsCommonUtils.ChangeStrToInt("5000");//横矢板が付く関係か掘削深さが設定できる
                            clsOyakui.m_refPDist = ClsCommonUtils.ChangeStrToInt("0");
                            clsOyakui.m_putPitch = ClsCommonUtils.ChangeStrToInt("500");
                            clsOyakui.m_type = "H形鋼 広幅";
                            clsOyakui.m_size = "H-300X300X10X15";
                            clsOyakui.m_HTop = ClsCommonUtils.ChangeStrToInt("0");
                            clsOyakui.m_HLen = ClsCommonUtils.ChangeStrToInt("10000");
                            clsOyakui.m_zanti = "なし";
                            clsOyakui.m_zantiLength = "0";
                            clsOyakui.m_Kasho1 = ClsCommonUtils.ChangeStrToInt("2");
                            //clsOyakui.m_Kasho2 = ClsCommonUtils.ChangeStrToInt("2");
                            clsOyakui.m_Kotei1 = "ボルト" == "ボルト" ? Master.Kotei.Bolt : Master.Kotei.Yousetsu;
                            //clsOyakui.m_Kotei2 = "ボルト" == "ボルト" ? Master.Kotei.Bolt : Master.Kotei.Yousetsu;
                            /*ここは読み込んだ値*/

                            clsOyakui.m_edaNum = "A";
                            //clsOyakui.m_edaNum2 = "A";
                            clsOyakui.m_KougoFlg = false;//Lightに交互フラグが無いため確定
                            clsOyakui.m_way = 1;
                            clsOyakui.m_putPosFlag = 1;//杭芯

                            clsOyakui.m_bYokoyaita = false;//Lightで付けられるか不明
                            clsOyakui.m_typeYokoyaita = "木矢板";
                            clsOyakui.m_sizeYokoyaita = "木矢板";
                            clsOyakui.m_putPosYokoyaitaFlag = 0;//内


                            List<int> lstN1 = new List<int>();
                            int kasho1 = clsOyakui.m_Kasho1 + 1;
                            int kuiLength1 = clsOyakui.m_HLen / kasho1;
                            for (int i = 0; i < kasho1; i++)
                            {
                                //最終杭長さは余りを全て吸収する
                                if (i + 1 == kasho1)
                                {
                                    lstN1.Add(clsOyakui.m_HLen - kuiLength1 * i);
                                }
                                else
                                    lstN1.Add(kuiLength1);
                            }
                            clsOyakui.m_ListPileLength1 = lstN1.ToList();

                            string bolt = "BN-50";
                            string boltNum = "8";
                            clsOyakui.m_BoltF1 = bolt;
                            clsOyakui.m_BoltFNum1 = boltNum;
                            clsOyakui.m_BoltW1 = bolt;
                            clsOyakui.m_BoltWNum1 = boltNum;


                            break;
                        }
                    case "SMW":
                        {
                            /*ここは読み込んだ値*/
                            ClsSMW clsSMW = new ClsSMW();
                            clsSMW.m_case = ClsCommonUtils.ChangeStrToInt("1");
                            clsSMW.m_zanti = "なし";
                            clsSMW.m_zantiLength = "0";
                            clsSMW.m_dia = ClsCommonUtils.ChangeStrToInt("550");
                            clsSMW.m_soil = ClsCommonUtils.ChangeStrToInt("450");
                            clsSMW.m_soilTop = ClsCommonUtils.ChangeStrToInt("0");
                            clsSMW.m_soilLen = ClsCommonUtils.ChangeStrToInt("10000");
                            clsSMW.m_type = "H形鋼 広幅";
                            clsSMW.m_size = "H-300X300X10X15";
                            clsSMW.m_HTop = ClsCommonUtils.ChangeStrToInt("0");
                            clsSMW.m_HLen = ClsCommonUtils.ChangeStrToInt("10000");
                            clsSMW.m_Kasho1 = ClsCommonUtils.ChangeStrToInt("2");
                            //clsSMW.m_Kasho2 = ClsCommonUtils.ChangeStrToInt("2");
                            clsSMW.m_Kotei1 = "ボルト" == "ボルト" ? Master.Kotei.Bolt : Master.Kotei.Yousetsu;
                            //clsSMW.m_Kotei2 = "ボルト" == "ボルト" ? Master.Kotei.Bolt : Master.Kotei.Yousetsu;
                            /*ここは読み込んだ値*/

                            clsSMW.m_edaNum = "A";
                            clsSMW.m_edaNum2 = "A";
                            clsSMW.m_way = 1;
                            clsSMW.m_void = ClsCommonUtils.ChangeStrToInt("5000");
                            clsSMW.m_bVoid = false;
                            clsSMW.m_KougoFlg = false;

                            //Lightで設定できるか不明
                            clsSMW.m_refPDist = ClsCommonUtils.ChangeStrToInt("0");
                            clsSMW.m_bTanbuS = true;
                            clsSMW.m_bTanbuE = false;
                            clsSMW.m_bCorner = false;//入隅判定が出来なさそう
                            clsSMW.m_putPtnFlag = ClsCommonUtils.ChangeStrToInt("10");


                            List<int> lstN1 = new List<int>();
                            int kasho1 = clsSMW.m_Kasho1 + 1;
                            int kuiLength1 = clsSMW.m_HLen / kasho1;
                            for (int i = 0; i < kasho1; i++)
                            {
                                //最終杭長さは余りを全て吸収する
                                if (i + 1 == kasho1)
                                {
                                    lstN1.Add(clsSMW.m_HLen - kuiLength1 * i);
                                }
                                else
                                    lstN1.Add(kuiLength1);
                            }
                            clsSMW.m_ListPileLength1 = lstN1.ToList();

                            string bolt = "BN-50";
                            string boltNum = "8";
                            clsSMW.m_BoltF1 = bolt;
                            clsSMW.m_BoltFNum1 = boltNum;
                            clsSMW.m_BoltW1 = bolt;
                            clsSMW.m_BoltWNum1 = boltNum;



                            break;
                        }
                }

            }
            sePointList.Clear();
            ////////////////////壁芯/////////////////////


            ///////////////////各種壁作成///////////////////////
            //個所数は設定できるので長さは等分
            //交互配置はなし
            //枝番なし
            //読み込んだJOSNで作成した壁芯と対応する壁を見つける必要がある

            //if ("鋼矢板" == "鋼矢板")
            //{
            //    ////////////////////鋼矢板/////////////////////
            //    /*ここは読み込んだ値*/
            //    var clsKouyaita = new ClsKouyaita();
            //    clsKouyaita.m_case = ClsCommonUtils.ChangeStrToInt("1");
            //    clsKouyaita.m_type = "鋼矢板";
            //    clsKouyaita.m_size = "SP-3";
            //    clsKouyaita.m_HTop = ClsCommonUtils.ChangeStrToInt("0");
            //    clsKouyaita.m_HLen = ClsCommonUtils.ChangeStrToInt("10000");
            //    clsKouyaita.m_bIzyou = false;//値が何で来るかは不明
            //    clsKouyaita.m_zaishitu = "SY295";
            //    clsKouyaita.m_zanti = "なし";
            //    clsKouyaita.m_zantiLength = "0";
            //    clsKouyaita.m_Kasho1 = ClsCommonUtils.ChangeStrToInt("2");
            //    //clsKouyaita.m_Kasho2 = ClsCommonUtils.ChangeStrToInt("2");//1と一緒
            //    clsKouyaita.m_Kotei1 = "ボルト" == "ボルト" ? Master.Kotei.Bolt : Master.Kotei.Yousetsu;
            //    //clsKouyaita.m_Kotei2 = "ボルト" == "ボルト" ? Master.Kotei.Bolt : Master.Kotei.Yousetsu;
            //    /*ここは読み込んだ値*/
            //    //一緒枝番は何かに固定する必要がある
            //    clsKouyaita.m_edaNum = "A";
            //    //clsKouyaita.m_edaNum2 = "A";
            //    clsKouyaita.m_way = 1;//多分いらない
            //    //掘削深さの設定は要相談
            //    clsKouyaita.m_void = ClsCommonUtils.ChangeStrToInt("5000");
            //    clsKouyaita.m_putVec = "交互" == "交互" ? PutVec.Kougo : PutVec.Const;//作成方法によって必要性が変わる
            //    clsKouyaita.m_kouyaitaSize = "SP-C3";//コーナー鋼矢板サイズは自動設定の可能性あり
            //    clsKouyaita.m_KougoFlg = false;//交互配置は無いので確定

            //    //設定があるか不明
            //    clsKouyaita.TensetuVec1 = "腹側" == "腹側" ? TensetuVec.HaraSide : TensetuVec.SenakaSide;
            //    //clsKouyaita.TensetuVec2 = "腹側" == "腹側" ? TensetuVec.HaraSide : TensetuVec.SenakaSide;

            //    List<int> lstN1 = new List<int>();
            //    int kuiLength1 = clsKouyaita.m_HLen / clsKouyaita.m_Kasho1;
            //    for (int i = 0; i < clsKouyaita.m_Kasho1; i++)
            //    {
            //        //最終杭長さは余りを全て吸収する
            //        if (i + 1 == clsKouyaita.m_Kasho1)
            //        {
            //            lstN1.Add(clsKouyaita.m_HLen - kuiLength1 * i);
            //        }
            //        else
            //            lstN1.Add(kuiLength1);
            //    }
            //    clsKouyaita.m_ListPileLength1 = lstN1.ToList(); ;

            //    string bolt = "BN-50";
            //    string boltNum = "8";
            //    clsKouyaita.m_BoltF1 = bolt;
            //    clsKouyaita.m_BoltFNum1 = boltNum;
            //    //Lightに設定項目が1つしかないので側やコーナーに関係なく同じと思われる
            //    clsKouyaita.m_BoltW1 = bolt;
            //    clsKouyaita.m_BoltWNum1 = boltNum;
            //    clsKouyaita.m_BoltCornerF1 = bolt;
            //    clsKouyaita.m_BoltCornerFNum1 = boltNum;
            //    clsKouyaita.m_BoltCornerW1 = bolt;
            //    clsKouyaita.m_BoltCornerWNum1 = boltNum;

            //}
            //////////////////////鋼矢板/////////////////////
            //////////////////////親杭/////////////////////
            //if ("親杭" == "親杭")
            //{
            //    /*ここは読み込んだ値*/
            //    ClsOyakui clsOyakui = new ClsOyakui();
            //    clsOyakui.m_case = ClsCommonUtils.ChangeStrToInt("1");
            //    clsOyakui.m_void = ClsCommonUtils.ChangeStrToInt("5000");//横矢板が付く関係か掘削深さが設定できる
            //    clsOyakui.m_refPDist = ClsCommonUtils.ChangeStrToInt("0");
            //    clsOyakui.m_putPitch = ClsCommonUtils.ChangeStrToInt("500");
            //    clsOyakui.m_type = "H形鋼 広幅";
            //    clsOyakui.m_size = "H-300X300X10X15";
            //    clsOyakui.m_HTop = ClsCommonUtils.ChangeStrToInt("0");
            //    clsOyakui.m_HLen = ClsCommonUtils.ChangeStrToInt("10000");
            //    clsOyakui.m_zanti = "なし";
            //    clsOyakui.m_zantiLength = "0";
            //    clsOyakui.m_Kasho1 = ClsCommonUtils.ChangeStrToInt("2");
            //    //clsOyakui.m_Kasho2 = ClsCommonUtils.ChangeStrToInt("2");
            //    clsOyakui.m_Kotei1 = "ボルト" == "ボルト" ? Master.Kotei.Bolt : Master.Kotei.Yousetsu;
            //    //clsOyakui.m_Kotei2 = "ボルト" == "ボルト" ? Master.Kotei.Bolt : Master.Kotei.Yousetsu;
            //    /*ここは読み込んだ値*/

            //    clsOyakui.m_edaNum = "A";
            //    //clsOyakui.m_edaNum2 = "A";
            //    clsOyakui.m_KougoFlg = false;//Lightに交互フラグが無いため確定
            //    clsOyakui.m_way = 1;
            //    clsOyakui.m_putPosFlag = 1;//杭芯

            //    clsOyakui.m_bYokoyaita = false;//Lightで付けられるか不明
            //    clsOyakui.m_typeYokoyaita = "木矢板";
            //    clsOyakui.m_sizeYokoyaita = "木矢板";
            //    clsOyakui.m_putPosYokoyaitaFlag = 0;//内


            //    List<int> lstN1 = new List<int>();
            //    int kuiLength1 = clsOyakui.m_HLen / clsOyakui.m_Kasho1;
            //    for (int i = 0; i < clsOyakui.m_Kasho1; i++)
            //    {
            //        //最終杭長さは余りを全て吸収する
            //        if (i + 1 == clsOyakui.m_Kasho1)
            //        {
            //            lstN1.Add(clsOyakui.m_HLen - kuiLength1 * i);
            //        }
            //        else
            //            lstN1.Add(kuiLength1);
            //    }
            //    clsOyakui.m_ListPileLength1 = lstN1.ToList(); 

            //    string bolt = "BN-50";
            //    string boltNum = "8";
            //    clsOyakui.m_BoltF1 = bolt;
            //    clsOyakui.m_BoltFNum1 = boltNum;
            //    clsOyakui.m_BoltW1 = bolt;
            //    clsOyakui.m_BoltWNum1 = boltNum;
            //}
            //////////////////////親杭/////////////////////
            //////////////////////SMW/////////////////////
            //if("SMW" == "SMW")
            //{
            //    /*ここは読み込んだ値*/
            //    ClsSMW clsSMW = new ClsSMW();
            //    clsSMW.m_case = ClsCommonUtils.ChangeStrToInt("1");
            //    clsSMW.m_zanti = "なし";
            //    clsSMW.m_zantiLength = "0";
            //    clsSMW.m_dia = ClsCommonUtils.ChangeStrToInt("550");
            //    clsSMW.m_soil = ClsCommonUtils.ChangeStrToInt("450");
            //    clsSMW.m_soilTop = ClsCommonUtils.ChangeStrToInt("0");
            //    clsSMW.m_soilLen = ClsCommonUtils.ChangeStrToInt("10000");
            //    clsSMW.m_type = "H形鋼 広幅";
            //    clsSMW.m_size = "H-300X300X10X15";
            //    clsSMW.m_HTop = ClsCommonUtils.ChangeStrToInt("0");
            //    clsSMW.m_HLen = ClsCommonUtils.ChangeStrToInt("10000");
            //    clsSMW.m_Kasho1 = ClsCommonUtils.ChangeStrToInt("2");
            //    //clsSMW.m_Kasho2 = ClsCommonUtils.ChangeStrToInt("2");
            //    clsSMW.m_Kotei1 = "ボルト" == "ボルト" ? Master.Kotei.Bolt : Master.Kotei.Yousetsu;
            //    //clsSMW.m_Kotei2 = "ボルト" == "ボルト" ? Master.Kotei.Bolt : Master.Kotei.Yousetsu;
            //    /*ここは読み込んだ値*/

            //    clsSMW.m_edaNum = "A";
            //    clsSMW.m_edaNum2 = "A";
            //    clsSMW.m_way = 1;
            //    clsSMW.m_void = ClsCommonUtils.ChangeStrToInt("5000");
            //    clsSMW.m_bVoid = false;
            //    clsSMW.m_KougoFlg = false;

            //    //Lightで設定できるか不明
            //    clsSMW.m_refPDist = ClsCommonUtils.ChangeStrToInt("0");
            //    clsSMW.m_bTanbuS = true;
            //    clsSMW.m_bTanbuE = false;
            //    clsSMW.m_bCorner = false;//入隅判定が出来なさそう
            //    clsSMW.m_putPtnFlag = ClsCommonUtils.ChangeStrToInt("10");


            //    List<int> lstN1 = new List<int>();
            //    int kuiLength1 = clsSMW.m_HLen / clsSMW.m_Kasho1;
            //    for (int i = 0; i < clsSMW.m_Kasho1; i++)
            //    {
            //        //最終杭長さは余りを全て吸収する
            //        if (i + 1 == clsSMW.m_Kasho1)
            //        {
            //            lstN1.Add(clsSMW.m_HLen - kuiLength1 * i);
            //        }
            //        else
            //            lstN1.Add(kuiLength1);
            //    }
            //    clsSMW.m_ListPileLength1 = lstN1.ToList();

            //    string bolt = "BN-50";
            //    string boltNum = "8";
            //    clsSMW.m_BoltF1 = bolt;
            //    clsSMW.m_BoltFNum1 = boltNum;
            //    clsSMW.m_BoltW1 = bolt;
            //    clsSMW.m_BoltWNum1 = boltNum;
            //}
            ////////////////////SMW/////////////////////
            
        }
        public static void CreateTanakui(Document doc)
        {
            List<XYZ> pointList = new List<XYZ>();
            ////本目
            //pointList.Add(new XYZ(0, 0, 0));

            ////////////////////壁芯/////////////////////
            //作成した壁芯のIDは保持しておく必要があると思われる
            List<JosnKabe> kabeShinList = new List<JosnKabe>();
            //1本目
            pointList.Add(new XYZ(-14.770538101, -0.665480720, 0));

            foreach (var point in pointList)
            {
                var clsTanaKui = new ClsTanakui();
                /*ここは読み込んだ値*/
                string levelName = "GL";
                clsTanaKui.m_HightFromGL = 0.0;
                clsTanaKui.m_KouzaiType = "H形鋼 広幅";
                clsTanaKui.m_KouzaiSize = "H-300X300X10X15";
                clsTanaKui.m_PileTotalLength = 10000.0;
                clsTanaKui.m_PileHaichiAngle = 90;
                clsTanaKui.m_CreateKiribariBracket = true;
                clsTanaKui.m_CreateKiribariOsaeBracket = true;
                clsTanaKui.m_TsugiteCount = 1;
                clsTanaKui.m_FixingType = "ボルト" == "ボルト" ? ClsTanakui.FixingType.Bolt : ClsTanakui.FixingType.Welding;
                /*ここは読み込んだ値*/

                List<int> lstN1 = new List<int>();
                int tsugiteCount = clsTanaKui.m_TsugiteCount + 1;
                int kuiLength1 = (int)clsTanaKui.m_PileTotalLength / tsugiteCount;
                for (int i = 0; i < tsugiteCount; i++)
                {
                    //最終杭長さは余りを全て吸収する
                    if (i + 1 == tsugiteCount)
                    {
                        lstN1.Add((int)clsTanaKui.m_PileTotalLength - kuiLength1 * i);
                    }
                    else
                        lstN1.Add(kuiLength1);
                }
                clsTanaKui.m_PileLengthList = lstN1.ToList();

                //ブラケットサイズを指定する項目がLightにはないため
                clsTanaKui.m_KiribariBracketSizeIsAuto = true;
                clsTanaKui.m_KiribariOsaeBracketSizeIsAuto = true;
                //プレロードとガイド材は作成しないと思われるため
                clsTanaKui.m_CreatePreloadGuide = false;
                clsTanaKui.m_CreateGuide = false;
                //Lightではずれ量を考慮した位置に配置されているためこちらでは1点配置に固定する
                clsTanaKui.m_CreateType = ClsTanakui.CreateType.OnePoint;

                string bolt = "BN-50";
                string boltNum = "8";
                clsTanaKui.m_TsugiteBoltSize_Flange = bolt;
                clsTanaKui.m_TsugiteBoltQuantity_Flange = boltNum;
                clsTanaKui.m_TsugiteBoltSize_Web = bolt;
                clsTanaKui.m_TsugiteBoltQuantity_Web = boltNum;

                ElementId levelId = ClsRevitUtil.GetLevelID(doc, levelName);
                string uniqueName = string.Empty;
                clsTanaKui.CreateTanaKui(doc, point, levelId, false, ref uniqueName);
                // 切梁ブラケットも同時に作成する場合
                if (clsTanaKui.m_CreateKiribariBracket)
                {
                    ClsBracket clsBracket = new ClsBracket();
                    clsBracket.m_BracketSize = clsTanaKui.m_KiribariBracketSize;
                    clsBracket.m_TargetKuiAngle = clsTanaKui.m_PileHaichiAngle;
                    clsBracket.m_TargetKuiCreatePosition = clsTanaKui.m_CreatePosition;
                    clsBracket.m_TargetKuiCreateType = clsTanaKui.m_CreateType;
                    if (clsBracket.CreateKiribariBracket(doc, clsTanaKui.m_CreateId, clsTanaKui.m_KiribariBracketSizeIsAuto))
                    {
                        // プレロードガイド材も作成する場合
                        if (clsTanaKui.m_CreatePreloadGuide)
                        {
                            for (int i = 0; i < clsBracket.m_CreateIds.Count(); i++)
                            {
                                clsBracket.CreatePreloadGuideZai(doc, clsBracket.m_CreateIds[i], clsBracket.m_TargetKiribariBaseIds[i]);
                            }
                        }
                    }
                }

                // 切梁押えブラケットも同時に作成する場合
                if (clsTanaKui.m_CreateKiribariOsaeBracket)
                {
                    ClsBracket clsBracket = new ClsBracket();
                    clsBracket.m_BracketSize = clsTanaKui.m_KiribariOsaeBracketSize;
                    clsBracket.m_TargetKuiAngle = clsTanaKui.m_PileHaichiAngle;
                    clsBracket.m_TargetKuiCreatePosition = clsTanaKui.m_CreatePosition;
                    clsBracket.m_TargetKuiCreateType = clsTanaKui.m_CreateType;
                    if (clsBracket.CreateKiribariOsaeBracket(doc, clsTanaKui.m_CreateId, clsTanaKui.m_KiribariOsaeBracketSizeIsAuto))
                    {
                        // ガイド材も作成する場合※
                        if (clsTanaKui.m_CreateGuide)
                        {
                            for (int i = 0; i < clsBracket.m_CreateIds.Count(); i++)
                            {
                                clsBracket.CreateGuideZai(doc, clsBracket.m_CreateIds[i], clsBracket.m_TargetKiribariBaseIds[i]);
                            }
                        }
                    }
                }
            }
            pointList.Clear();

        }
    }

    public class JosnKabe
    {
        public ElementId m_KabeShin { get; set; }

        public string m_KabeType { get; set; }

        public JosnKabe(ElementId kabeShin, string kabeType)
        {
            m_KabeShin = kabeShin;
            m_KabeType = kabeType;
        }
    }
    public class JosnData
    {
        [JsonProperty("metaData")]
        public MetaData MetaData { get; set; }
        [JsonProperty("rootLayers")]
        public List<RootLayers> RootLayers { get; set; }
    }
    /// <summary>
    /// 作成情報
    /// </summary>
    public class MetaData
    {
        [JsonProperty("version")]
        public string version { get; set; }
        [JsonProperty("created_at")]
        public string created_at { get; set; }
        [JsonProperty("updated_at")]
        public string updated_at { get; set; }
        [JsonProperty("created_by")]
        public string created_by { get; set; }
        [JsonProperty("updated_by")]
        public string updated_by { get; set; }
    }
    /// <summary>
    /// メインのList
    /// </summary>
    public class RootLayers
    {
        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }
        [JsonProperty("geometries")]
        public List<Geometries> Geometries { get; set; }
        [JsonProperty("object")]
        public Object Object { get; set; }
    }

    public class Metadata
    {
        [JsonProperty("version")]
        public double version { get; set; }
        [JsonProperty("type")]
        public string type { get; set; }
        [JsonProperty("generator")]
        public string generator { get; set; }
    }
    public class Geometries
    {
        [JsonProperty("uuid")]
        public string uuid { get; set; }
        [JsonProperty("type")]
        public string type { get; set; }
        [JsonProperty("data")]
        public Data data { get; set; }

    }
    public class Data
    {
        [JsonProperty("attributes")]
        public Attributes Attributes { get; set; }
        [JsonProperty("boundingSphere")]
        public BoundingSphere BoundingSphere { get; set; }
    }
    public class Attributes
    {
        [JsonProperty("position")]
        public Position Position { get; set; }
    }
    public class Position
    {
        [JsonProperty("itemSize")]
        public int itemSize { get; set; }
        [JsonProperty("type")]
        public string type { get; set; }
        /// <summary>
        /// 配置点3個ずつで一区切りX,Y,Zの順で入っている※始点3終点3の順番
        /// </summary>
        [JsonProperty("array")]
        public List<double> array { get; set; }
        [JsonProperty("normalized")]
        public bool normalized { get; set; }
    }
    public class BoundingSphere
    {
        /// <summary>
        /// 中心点3個ずつで一区切りX,Y,Zの順で入っている
        /// </summary>
        [JsonProperty("center")]
        public List<double> center { get; set; }
        [JsonProperty("radius")]
        public double radius { get; set; }
    }

    public class Object
    {
        [JsonProperty("uuid")]
        public string uuid { get; set; }
        [JsonProperty("type")]
        public string type { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("layers")]
        public double layers { get; set; }
        [JsonProperty("matrix")]
        public List<int> matrix { get; set; }
        [JsonProperty("up")]
        public List<int> up { get; set; }
        [JsonProperty("children")]
        public List<Children> children { get; set; }
        [JsonProperty("userData")]
        public UserData UserData { get; set; }

    }
    
    public class Children
    {
        [JsonProperty("uuid")]
        public string uuid { get; set; }
        [JsonProperty("type")]
        public string type { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("layers")]
        public double layers { get; set; }
        [JsonProperty("matrix")]
        public List<int> matrix { get; set; }
        [JsonProperty("up")]
        public List<int> up { get; set; }
        [JsonProperty("geometry")]
        public string geometry { get; set; }
        [JsonProperty("material")]
        public string material { get; set; }
        [JsonProperty("children")]
        public List<Children> children { get; set; }
        [JsonProperty("userData")]
        public UserData UserData { get; set; }
    }
    public class UserData
    {
        //[JsonProperty("userData")]//同名なのと入るデータが不明
        //public UserData userData { get; set; }
        [JsonProperty("customData")]
        public CustomData CustomData { get; set; }
    }

    public class CustomData
    {
        [JsonProperty("type")]
        public string type { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("lineWidth")]
        public int lineWidth { get; set; }
        [JsonProperty("color")]
        public string color { get; set; }
        [JsonProperty("height")]
        public double height { get; set; }
        [JsonProperty("processingMethod")]
        public string processingMethod { get; set; }
        /// <summary>
        /// 鋼材種類※主材、高強度など
        /// </summary>
        [JsonProperty("steelType")]
        public string steelType { get; set; }
        /// <summary>
        /// 主材サイズ
        /// </summary>
        [JsonProperty("steelSize")]
        public string steelSize { get; set; }
        /// <summary>
        /// 段
        /// </summary>
        [JsonProperty("stage")]
        public string stage { get; set; }
        /// <summary>
        /// 横本数
        /// </summary>
        [JsonProperty("horizontalBraceCount")]
        public string horizontalBraceCount { get; set; }
        /// <summary>
        /// 縦本数
        /// </summary>
        [JsonProperty("verticalBraceCount")]
        public string verticalBraceCount { get; set; }
        [JsonProperty("slipStopPointsCount")]
        public string slipStopPointsCount { get; set; }
        [JsonProperty("case")]
        public string Case { get; set; }
        /// <summary>
        /// 接続しているベースのID
        /// </summary>
        [JsonProperty("connectedLines")]
        public List<string> connectedLines { get; set; }
    }

    //public class 







    public class Points
    {//XYZで拾えるかは不明
        [JsonProperty("0")]
        public XYZ StartPoint { get; set; }

        [JsonProperty("1")]
        public XYZ EndPoint { get; set; }
    }
    public class CustomDataHaraokoshi
    {
        public string type { get; set; }
        public string name { get; set; }
        public int lineWidth { get; set; }
        public string color { get; set; }
        public string processingMethod { get; set; }
        public string steelType { get; set; }
        public string steelSize { get; set; }
        public string stage { get; set; }
        public string horizontalBraceCount { get; set; }
        public string verticalBraceCount { get; set; }
        public string slipStopPointsCount { get; set; }

    }
}
