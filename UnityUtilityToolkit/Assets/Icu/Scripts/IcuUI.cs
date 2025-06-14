using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Steam.Models.SteamCommunity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;

// using System.Windows.Forms;
namespace Icu
{
    public class RelationUserInfo
    {
        public Response response;
        public int depth;
        public string viaSteamId;
    }
    public class IcuUI : MonoBehaviour
    {
        // 链接输入矿
        public TMP_InputField urlInputField;
        // 范围输入
        public TMP_InputField rangInputField;
        public TMP_InputField keyInputField;
        // 查询按钮
        public Button searchBtn;
        public Button infoBtn;
        
        // log
        public TMP_Text logText;

        private string _path = Path.GetDirectoryName(Application.dataPath);
        // 请求
        private APIRequest _apiRequest;
        
        private string _selectedFolderPath = "";
        
        // headers
        private List<string> headers = new List<string>()
        {
            "Steam ID",
            "社区可见性状态",
            "个人资料状态",
            "个人名称",
            "评论权限",
            "个人资料 URL",
            "头像（小）",
            "头像（中）",
            "头像（大）",
            "头像哈希",
            "最后一次登出时间",
            "在线状态",
            "真实姓名",
            "主要群组 ID",
            "创建时间",
            "在线状态标志",
            "国家代码",
            "发现深度",
            "发现用户steamID",
            "关系路径"
            
        };
        
        // key
        private string _key = "5F3ACAFE0FC6A18B80C77B6D164C9298";
        private string _token = "";
        private string _steamId = "76561199558984174";
        private string _relationship = "friend";

        private string getUrl = "http://api.steampowered.com/ISteamUser/GetFriendList/v0001/?key=5F3ACAFE0FC6A18B80C77B6D164C9298&steamid=76561199558984174&relationship=friend";
        private HashSet<string> _visitedSteamIds = new HashSet<string>();
        string GetFriendListUrl(string key, string steamId)
        {
            //return  $"http://api.steampowered.com/ISteamUser/GetFriendList/v0001/?key={key}&steamid={steamId}&relationship=friend";
          return  $"https://api.steampowered.com/ISteamUserOAuth/GetFriendList/v1/?access_token=eyAidHlwIjogIkpXVCIsICJhbGciOiAiRWREU0EiIH0.eyAiaXNzIjogInI6MDAwOV8yNjcyNUM5OV8yQThCRiIsICJzdWIiOiAiNzY1NjExOTk1NTg5ODQxNzQiLCAiYXVkIjogWyAid2ViOnN0b3JlIiBdLCAiZXhwIjogMTc0OTg2NDExMSwgIm5iZiI6IDE3NDExMzcwMjIsICJpYXQiOiAxNzQ5Nzc3MDIyLCAianRpIjogIjAwMTZfMjY3M0IzRjJfQkQ2ODQiLCAib2F0IjogMTc0OTYzMzA5NywgInJ0X2V4cCI6IDE3NTIyMjg4ODUsICJwZXIiOiAwLCAiaXBfc3ViamVjdCI6ICIxMDMuMTgwLjI5LjU0IiwgImlwX2NvbmZpcm1lciI6ICIxMTcuODEuODEuMjMzIiB9.VbfkNjbxGSDUAcP5SXuczYC05K0HTkXQbxLvKtHQAvorbGy5BECGGZ4_b9p0XniG8k-bd1aKYNSLr93pBbVXDA&steamid={steamId}";
        }

        /*string GetFriendListUrl(string token, string steamId)
        {
            
        }*/

        string GetPlayerUrl(string key, string steamIds)
        {
            return
                $"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={key}&steamids={steamIds}";
        }
        private void Start()
        {
            urlInputField.text = _steamId;
            keyInputField.text = _key;
            

            _apiRequest = GetComponent<APIRequest>();
            searchBtn.onClick.AddListener(Search);
            
            infoBtn.onClick.AddListener(OnOff);
            
            
        }

        private bool _infoOnOff = false;
        void OnOff()
        {
            _infoOnOff = !_infoOnOff;
            if (_infoOnOff)
            {
                infoBtn.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text = "个人信息 开";
            }
            else
            {
                infoBtn.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text = "个人信息 关";
            }
            
        }


        /*void SelectFolder()
        {
            // 创建文件夹选择对话框
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "选择保存文件夹";

            // 显示对话框并获取结果
            DialogResult result = dialog.ShowDialog();

            // 检查用户是否取消了选择
            if (result == DialogResult.Cancel)
            {
                Debug.Log("用户取消了选择");
            }
            else
            {
                // 保存选择的文件夹路径
                selectedFolderPath = dialog.SelectedPath;
                Debug.Log("选择的文件夹路径: " + selectedFolderPath);
            }
        }*/
        async void Search()
        {
            _steamId = urlInputField.text.Trim();
            _key = keyInputField.text.Trim();
            logText.text = "";
            string rang = rangInputField.text.Trim();


            /*var webInterfaceFactory = new SteamWebInterfaceFactory(_key);
            var steamInterface = webInterfaceFactory.CreateSteamWebInterface<SteamUser>(new HttpClient());

            try
            {
                // 获取当前用户的好友列表
                var response = await steamInterface.GetFriendsListAsync(ulong.Parse(_steamId));
                if (response != null && response.Data != null)
                {
                    foreach (var friend in response.Data)
                    {
                        Debug.Log($"Friend: {friend.SteamId}, Relationship: {friend.Relationship}, Friend Since: {friend.FriendSince}");
                    }

                    // 并行获取每个好友的好友列表
                    var friendTasks = response.Data.Select(friend => steamInterface.GetFriendsListAsync(friend.SteamId)).ToArray();
                    var friendResponses = await Task.WhenAll(friendTasks);

                    for (int i = 0; i < friendResponses.Length; i++)
                    {
                        var friendResponse = friendResponses[i];
                        if (friendResponse != null && friendResponse.Data != null)
                        {
                            Debug.Log($"Friends of {response.Data.ToList()[i].SteamId}:");
                            foreach (var friendOfFriend in friendResponse.Data)
                            {
                                Debug.Log($"  Friend of Friend: {friendOfFriend.SteamId}, Relationship: {friendOfFriend.Relationship}, Friend Since: {friendOfFriend.FriendSince}");
                            }
                        }
                        else
                        {
                            Debug.LogError($"No friends list found for friend {response.Data.ToList()[i].SteamId}.");
                        }
                    }
                }
                else
                {
                    Debug.LogError("No friends list found.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error: {ex.Message}");
            }*/
            // 调用 APIRequest 的方法发起请求
            if (_apiRequest != null)
            {
                // StartCoroutine(GetFriendsAndInfo(_steamId, int.Parse(rang)));
                GetFriendsList(_steamId, list =>
                {
                    foreach (var r in list)
                   {
                       Debug.Log(r.steamid);
                       GetFriendsList(r.steamid, ls =>
                       {
                           foreach (var l in ls)
                           {
                               Debug.Log(l.steamid);
                           }
                       });
                   }
                } );
            }
            else
            {
                Debug.LogError("APIRequest 组件未赋值！");
                logText.text = "组件未连接";
            }
        }
        
        

        public string GetSteamIdsFromFriendsList(List<Friend> friends)
        {
            string steamIds = string.Join(",", friends.Select(friend => friend.steamid));
            Debug.Log(steamIds);
            return steamIds;
        }
        public string GetSteamIdsFromFriendsList(List<string> friends)
        {
            // 使用 LINQ 提取所有 steamid 并拼接成一个逗号分隔的字符串
            string steamIds = string.Join(",", friends);
            return steamIds;
        }

        void GetPlayerInfo(string steamIds, Action<List<Player>> callback)
        {
            
            getUrl = GetPlayerUrl(_key, steamIds);
            List<Player> players = new List<Player>();
            _apiRequest.GetSteamData(getUrl, r =>
            {
                if (r == null)
                {
                    logText.text = "发生错误： 请求次数过多超出api限制"; 
                }
                
                // players = r;
                callback(players);
            } );
        }


        void GetFriendsList(string steamId, Action<List<Friend>> callback)
        {
            getUrl = GetFriendListUrl(_key, steamId);
            _apiRequest.GetSteamData(getUrl, r =>
            {
                if (r == null)
                {
                    logText.text = "发生错误： 请求次数过多超出api限制";
                }
                callback(r);
            });
        }
        IEnumerator GetFriendsListCoroutine(string steamId, Action<List<Friend>> callback)
        {
            List<Friend> friends = null;
            GetFriendsList(steamId, result => {
                if (result == null)
                {
                    logText.text = "发生错误： 请求次数过多超出api限制";
                }
                friends = result;
            });
    
            yield return new WaitUntil(() => friends != null);
            callback(friends);
        }

        IEnumerator GetPlayerInfoCoroutine(string steamIds, Action<List<Player>> callback)
        {
            List<Player> players = null;
            GetPlayerInfo(steamIds, result => {
                players = result;
            });
    
            yield return new WaitUntil(() => players != null);
            callback(players);
        } 
        IEnumerator GetFriendsAndInfo(string steamId, int depth)
    {
        Dictionary<string, RelationUserInfo> relationUserInfos = new Dictionary<string, RelationUserInfo>();
        Queue<KeyValuePair<string, string>> steamIdQueue = new Queue<KeyValuePair<string, string>>();
        List<string> errorMessages = new List<string>();
        relationUserInfos.Add(steamId, new RelationUserInfo
        {
            depth = 0,
            viaSteamId = ""
        });
        List<Friend> friends = null;
        steamIdQueue.Enqueue(new KeyValuePair<string, string>(steamId, ""));

        Debug.Log($"当前深度 {depth}");
        while (steamIdQueue.Count > 0)
        {
            Debug.Log($"当前队列大小: {steamIdQueue.Count}");
            var current = steamIdQueue.Dequeue();
            string currentSteamId = current.Key;
            int currentDepth = relationUserInfos[currentSteamId].depth;
            if (currentDepth >= depth)
            {
                Debug.Log($"达到最大深度，跳过 Steam ID: {currentSteamId}");
                continue;
            }

            
            Debug.LogError(steamIdQueue.Count);
            if (friends != null)
            {
                foreach (var f in friends)
                {
                    if (!relationUserInfos.ContainsKey(f.steamid))
                    {
                        relationUserInfos[f.steamid] = new RelationUserInfo
                        {
                            depth = currentDepth + 1,
                            viaSteamId = currentSteamId
                        };
                        Debug.Log($"加入 {f.steamid}");
                        steamIdQueue.Enqueue(new KeyValuePair<string, string>(f.steamid, currentSteamId));
                    }
                }

            } 
            yield return StartCoroutine(GetFriendsListCoroutine(currentSteamId, result => friends = result));
            if (friends == null)
            {
                Debug.LogError("再次请求失败");
                errorMessages.Add($"请求朋友列表失败，Steam ID: {currentSteamId}");
                continue;
            }
           
          
            // 确保在每一步都有适当的延迟
            yield return new WaitForSeconds(20); 
        }
        List<Player> allPlayerInfos = new List<Player>();
        List<string> steamIds = relationUserInfos.Keys.ToList();
        if (_infoOnOff)
        {
            int batchSize = 100;
            for (int i = 0; i < steamIds.Count; i += batchSize)
            {
                List<string> batchSteamIds = steamIds.Skip(i).Take(batchSize).ToList();
                List<Player> batchplayers = null;
                string ss = GetSteamIdsFromFriendsList(batchSteamIds);
                yield return StartCoroutine(GetPlayerInfoCoroutine(ss, result => batchplayers = result));
                if (batchplayers == null)
                {
                    errorMessages.Add($"批量获取失败，批次: {i / batchSize + 1}");
                    continue;
                }
                allPlayerInfos.AddRange(batchplayers);
                yield return new WaitForSeconds(20); 
            } 
        }

        if (errorMessages.Count > 0)
        {
            logText.text = "发生以下错误：\n" + string.Join("\n", errorMessages);
        }
        else
        {
            logText.text = "操作成功完成";
        }
        CoverDataExcel(relationUserInfos, allPlayerInfos);
    }

        string GetRelationPath(string steamId, Dictionary<string, RelationUserInfo> relationUserInfos, Dictionary<string, Player> players)
        {
            if (steamId == "") return "";
            var relationInfo = relationUserInfos[steamId];
            if (relationInfo.viaSteamId == "")
            {
                return players[steamId].personaname;
            }

            // 递归获取路径，并在当前用户的名字前加上父路径
            return $"{GetRelationPath(relationInfo.viaSteamId, relationUserInfos, players)} -> {players[steamId].personaname}";
        }
 
      void CoverDataExcel(Dictionary<string, RelationUserInfo> relationUserInfos, List<Player> players)
    {
        var newFile = Path.Combine(_path, $"输出.xlsx");
        IWorkbook newWorkbook = null;
        try
        {
            using (var fs = new FileStream(newFile, FileMode.Create, FileAccess.Write))
            {
                var steamDict = players.ToDictionary(s => s.steamid, s => s);
                newWorkbook = new XSSFWorkbook();

                // 创建第一个表：用户信息
                ISheet sheet1 = newWorkbook.CreateSheet("用户信息");
                IRow annotationRow1 = sheet1.CreateRow(0);
                for (int i = 0; i < headers.Count; i++)
                {
                    annotationRow1.CreateCell(i).SetCellValue(headers[i]);
                    sheet1.AutoSizeColumn(i);
                }

                int rowIndex1 = 1;
                foreach (var kvp in relationUserInfos)
                {
                    string steamId = kvp.Key;
                    var relationInfo = kvp.Value;
                    if (steamDict.TryGetValue(steamId, out Player player))
                    {
                        string relationPath = GetRelationPath(steamId, relationUserInfos, steamDict);
                        IRow row = sheet1.CreateRow(rowIndex1++);
                        row.CreateCell(0).SetCellValue(player.steamid);
                        row.CreateCell(1).SetCellValue(player.communityvisibilitystate);
                        row.CreateCell(2).SetCellValue(player.profilestate);
                        row.CreateCell(3).SetCellValue(player.personaname);
                        row.CreateCell(5).SetCellValue(player.profileurl);
                        row.CreateCell(6).SetCellValue(player.avatar);
                        row.CreateCell(7).SetCellValue(player.avatarmedium);
                        row.CreateCell(8).SetCellValue(player.avatarfull);
                        row.CreateCell(9).SetCellValue(player.avatarhash);
                        row.CreateCell(11).SetCellValue(player.personastate);
                        row.CreateCell(12).SetCellValue(player.realname);
                        row.CreateCell(13).SetCellValue(player.primaryclanid);
                        row.CreateCell(14).SetCellValue(player.timecreated);
                        row.CreateCell(15).SetCellValue(player.personastateflags);
                        row.CreateCell(16).SetCellValue(player.loccountrycode);
                        row.CreateCell(17).SetCellValue(player.loccityid);
                        row.CreateCell(18).SetCellValue(relationInfo.depth);
                        row.CreateCell(19).SetCellValue(relationPath);
                    }
                }

                // 创建第二个表：朋友关系
                ISheet sheet2 = newWorkbook.CreateSheet("朋友关系");
                IRow annotationRow2 = sheet2.CreateRow(0);
                annotationRow2.CreateCell(0).SetCellValue("Steam ID");
                annotationRow2.CreateCell(1).SetCellValue("关系路径");

                int rowIndex2 = 1;
                foreach (var kvp in relationUserInfos)
                {
                    string steamId = kvp.Key;
                    var relationInfo = kvp.Value;
                    IRow row = sheet2.CreateRow(rowIndex2++);
                    row.CreateCell(0).SetCellValue(steamId);
                    row.CreateCell(1).SetCellValue(relationInfo.viaSteamId);
                }

                // 写入文件
                newWorkbook.Write(fs);
            }
        }
        catch (Exception e)
        {
            logText.text = $"无法创建新表 {e.Message}";
        }
        finally
        {
            if (newWorkbook != null)
            {
                newWorkbook.Close();
            }
        }

        logText.text = $"文件已生成 {_path}";
    }
        
    }
}