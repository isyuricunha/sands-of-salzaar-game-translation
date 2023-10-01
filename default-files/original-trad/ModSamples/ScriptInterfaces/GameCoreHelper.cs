using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HanFramework;
using HanSquirrel.ResourceManager;
using XLua;
using System.Linq;
using Blywd.Mod;
using GLib;
using HSFrameWork.ConfigTable;
using Cysharp.Threading.Tasks;
using FairyGUI;

[Hotfix]
[LuaCallCSharp]
public static class GameCoreHelper
{
    public static void LogError(string _log)
    {
        Debug.LogError(_log);
    }

    /// <summary>
    /// 返回当前的游戏世界内核
    /// </summary>
    public static WorldManager GetCurGameWorld()
    {
        return WorldManager.Instance;
    }

    /// <summary>
    /// 向游戏内信息框内发送一条消息  mode:0：普通 1：警告 2：系统
    /// </summary>
    public static void ShowGameMsg(string _str, int _mode)
    {
        ArchiveData.Current.ShowGameMessage(_str, (short)_mode);
    }

    /// <summary>
    /// 发送UI消息事件
    /// </summary>
    public static void SendSceneUIMessage(string msgType, string msgArg)
    {
        GameShared.Instance.SendSceneMessage(msgType, msgArg);
    }

    /// <summary>
    /// 运行标准的剧情脚本指令
    /// </summary>
    public static void RunCmd(string _cmdLines, RuntimeArgVals _contextArgs)
    {
        WorldManager.Instance.ExecuteDebugCmdLines(_cmdLines, null, _contextArgs);
    }

    /// <summary>
    /// 创建一个新的脚本指令环境变量
    /// </summary>
    public static RuntimeArgVals CreateEmptyRuntimeArgVal()
    {
        RuntimeArgVals argVal = new RuntimeArgVals();
        return argVal;
    }

    public static bool CheckConditionGroup(string _condition, RuntimeArgVals _rtArgs, bool _hasCvt)
    {
        return GameUtils.checkConditionGroup(_condition, _rtArgs, _hasCvt);
    }

    public static bool IsDebugFileExsit(string _filename)
    {
        return _filename.ExistsAsFile();
    }


    /// <summary>
    /// 检测角色关系日志系统是否开启（用于调试）
    /// </summary>
    public static bool IsEnableRoleRlLog()
    {
        return WorldManager.Instance.IsEnableRoleRlLog();
    }

    public static void SetIsEnableRoleRlLog(bool _enable)
    {
        WorldManager.Instance.SetIsEnableRoleRlLog(_enable);
    }

    /// <summary>
    /// 记录角色关系日志信息
    /// </summary>
    public static RoleRlLogEvent LogRoleRlInfo(string sInfo)
    {
        return WorldManager.Instance.LogRoleRlInfo(sInfo);
    }

    /// <summary>
    /// 开启角色关系日志调试面板
    /// </summary>
    public static void OpenRoleRlLogDebugUI()
    {
        GUIManager.Instance.ShowGUIWindow("BlywdMain", "RoleRelationsDebugWin", "gui/RoleRelationsDebugWin", string.Empty, null);
    }

    public static int VersionCompare(string versionA, string versionB)
    {
        return GameUtils.VersionCompare(versionA, versionB);
    }

    /// <summary>
    /// 检查存档版本号是否过期
    /// </summary>
    public static bool IsVersionExpired(string version)
    {
        if (string.IsNullOrEmpty(version))
        {
            version = "0.0.0.0";
        }

        var currentVer = Application.version.Split('.');
        var tagVer = version.Split('.');

        if (currentVer.Length < 2 || tagVer.Length < 2)
        {
            return true;
        }

        //必须头两位相等，才不算过期
        if (currentVer[0] == tagVer[0] && currentVer[1] == tagVer[1])
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// xlhy#heroes#0.1
    /// </summary>
    /// <returns></returns>
    public static bool IsDLC_HeroesActived()
    {
        return IsDLCActived("xlhy#heroes#0.1");
    }
    /// <summary>
    /// 检测指定的模组是否已加载
    /// </summary>
    public static bool IsModActived(string _modID)
    {
        return ModFacade4App.IncludedMods.Contains(_modID) || IsDLCActived(_modID);
    }

    /// <summary>
    /// 检测指定的DLC是否已激活 目前只有比武大会
    /// 比武大会： xlhy#matchgame#0.1
    /// </summary>
    /// <returns></returns>
    public static bool IsDLCActived(string _dlcID)
    {
        string[] allDlc = PlayerPrefs.GetString("ALLDLCSTATE", "").SplitNoGC('_');
        if (allDlc == null || allDlc.Length == 0)
            return false;
        if (_dlcID.IsNullOrEmpty())
            return false;
        return allDlc.Any(item =>
        {
            string[] dlc = item.Split('|');
            if (dlc != null && dlc.Length == 2 && dlc[0].Equals(_dlcID) && dlc[1].Equals("open"))
                return true;
            return false;
        });
    }
    /// <summary>
    /// 设置指定的DLC是否激活
    /// </summary>
    /// <returns></returns>
    public static void SetDLCActived(string _dlcID, bool _dlcOpen)
    {
        string tep = "{0}|{1}";
        string allDLCStr = PlayerPrefs.GetString("ALLDLCSTATE", "");
        if (string.IsNullOrEmpty(allDLCStr))
        {
            PlayerPrefs.SetString("ALLDLCSTATE", tep.Format(_dlcID, _dlcOpen ? "open" : "close"));
            return;
        }
        List<string> allDlc = allDLCStr.SplitNoGC2List('_');
        int index = allDlc.FindIndex(item => item.Contains(_dlcID));
        StringBuilder sb = new StringBuilder();
        if (index == -1)
        {
            PlayerPrefs.SetString("ALLDLCSTATE", sb.Append(allDLCStr).Append('_').AppendFormat(tep, _dlcID, _dlcOpen ? "open" : "close").ToString());
            return;
        }
        allDlc[index] = tep.Format(_dlcID, _dlcOpen ? "open" : "close");
        for (int i = 0; i < allDlc.Count; i++)
        {
            if (i != 0)
                sb.Append('_');
            sb.Append(allDlc[i]);
        }
        PlayerPrefs.SetString("ALLDLCSTATE", sb.ToString());
    }
    /// <summary>
    /// 获取全部保存过的DLC
    /// </summary>
    public static List<(string dlcId, bool isopen)> GetAllDLC()
    {
        string ALLDLCSTATE = PlayerPrefs.GetString("ALLDLCSTATE", "");
        string[] allDlc = ALLDLCSTATE.SplitNoGC('_');
        if (ALLDLCSTATE.IsNullOrEmpty() || allDlc.IsNullOrEmptyG())
            return new List<(string dlcId, bool isopen)>();
        string[] dlc;
        return allDlc.Select(item =>
        {
            dlc = item.Split('|');
            return (dlc[0], dlc[1].Equals("open"));
        }).ToList();
    }
    /// <summary>
    /// 获取全部DLC
    /// </summary>
    /// <returns></returns>
    public async static UniTask<bool> GetAllDLCByArchiveSummary(ArchiveSummaryInfo newSummary, Action anyBack = null)
    {
        //索引全部dlc 但因为一些问题 暂时搁置 只判断比武大会
        //Dictionary<string, string> dict = newSummary.GetActiveMods();
        //var allDlc = GetAllDLC();
        //for (int i = 0; i < allDlc.Count; i++)
        //{
        //    (bool, DlcItem) data = await PayManager.Instance.GetDlcInfo(allDlc[i].dlcId);
        //    if (data.Item1)
        //    {
        //        dict.Add(data.Item2.productId, data.Item2.productName);
        //    }
        //}
        //newSummary.SetActiveMods(dict);
        //await UniTask.SwitchToMainThread();
        //GRoot.inst.DispatchEvent("GetAllDLCByArchiveSummary_Complete");


        var allDlc = GetAllDLC();
        bool IsOK = false;
        Dictionary<string, string> dict = newSummary.GetActiveMods();
        for (int i = 0; i < allDlc.Count; i++)
        {
            if (dict.ContainsKey(allDlc[i].dlcId))
                continue;
            if (!allDlc[i].isopen)
                continue;

            if (allDlc[i].dlcId.Equals("xlhy#matchgame#0.1"))
            {
                bool isHas = await PayManager.Instance.HasBWDH();
                if (isHas)
                {
                    (bool, DlcItem) data = await PayManager.Instance.GetBWDHInfo();
                    if (data.Item1)
                    {
                        dict["xlhy#matchgame#0.1"] = data.Item2.productName;
                        newSummary.SetActiveMods(dict);
                        IsOK = true;
                        continue;
                    }
                }
            }
            else if (allDlc[i].dlcId.Equals("xlhy#heroes#0.1"))
            {
                dict["xlhy#heroes#0.1"] = GameTools.IsChineseLanguage() ? "《部落与弯刀DLC - 乱世英豪》" : "[Sands of Salzaar DLC - Champion of Chaos]";
                newSummary.SetActiveMods(dict);
                IsOK = true;
                continue;
            }
        }
        if (IsOK)
        {
            if (anyBack != null)
                anyBack();
            return true;
        }
        return false;
    }

    /// <summary>
    /// 零时方案 不要调用 Lua那边调用
    /// </summary>
    public static async UniTask TryUpdateArchiveSummaryByUI()
    {
        int max = GetMaxArchiveCount();
        ArchiveSummaryInfo summary;
        bool tryUpdate = false;
        for (int i = 0; i < max; i++)
        {
            summary = LoadArchiveSummaryInfo(i);
            if (summary != null)
            {
                if (await GetAllDLCByArchiveSummary(summary))
                {
                    tryUpdate = true;
                }
            }
        }
        if (tryUpdate)
        {
            await UniTask.SwitchToMainThread();
            GRoot.inst.DispatchEvent("GetAllDLCByArchiveSummary_Complete");
        }
    }

    private static Dictionary<string, object> _cachedTaProperties = new Dictionary<string, object>();

    /// <summary>
    /// 埋点追踪接口 参数格式 key1=val1|key2=val3|...
    /// </summary>
    public static void TrackEvent(string _eventID, string _args = null)
    {
        _cachedTaProperties.Clear();
        if (!string.IsNullOrEmpty(_args))
        {
            string[] argsList = _args.Split('|');
            string argKey;
            string argVal;
            int splitCharIndex;
            float _parseNum;
            foreach (string argLine in argsList)
            {
                splitCharIndex = argLine.IndexOf('=');
                if (splitCharIndex > 1)
                {
                    argKey = argLine.Substring(0, splitCharIndex).Trim();
                    argVal = argLine.Substring(splitCharIndex + 1).Trim();

                    if (!string.IsNullOrEmpty(argKey))
                    {
                        if (float.TryParse(argVal, out _parseNum))
                        {
                            _cachedTaProperties[argKey] = _parseNum;
                        }
                        else
                        {
                            _cachedTaProperties[argKey] = argVal;
                        }
                    }
                }
            }
        }
        PlatformManager.TrackEvent(_eventID, _cachedTaProperties);
    }

    /// <summary>
    /// 执行一次游戏内触发器检定
    /// </summary>
    /// <param name="triggerType">触发类型</param>
    /// <param name="tagType">对象类型</param>
    /// <param name="tagID">对象ID</param>
    /// <param name="_rtArgs">环境变量</param>
    /// <param name="_trigAll">是否触发全部符合条件的</param>
    public static void CheckGameTrigger(string triggerType, string tagType, string tagID, RuntimeArgVals _rtArgs, bool _trigAll = false)
    {
        WorldManager.Instance.OnTriggerEventCheck(triggerType, tagType, tagID, _rtArgs, _trigAll);
    }

    /// <summary>
    /// 暂停游戏
    /// </summary>
    public static void PauseGame()
    {
        WorldManager.Instance.PauseGame();
    }

    /// <summary>
    /// 恢复游戏
    /// </summary>
    public static void ResumeGame()
    {
        WorldManager.Instance.ResumeGame();
    }

    /// <summary>
    /// 返回一个min(包含) 到 max(包含)之间的实数
    /// </summary>
    public static float GetRandomNum(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    /// <summary>
    /// 返回一个大于等于min(包含) 小于max(不包含) 的整数
    /// </summary>
    public static int GetRandomInt(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    /// <summary>
    /// 根据百分比概率（0-100.0）返回一个BOOL值
    /// </summary>
    public static bool GetRandomBool(float prob)
    {
        return GameTools.GetRandomBool(prob);
    }

    /// <summary>
    /// 获取一段游戏中的文字（定义在游戏内文字表中）
    /// </summary>
    public static string GetGameString(string str)
    {
        return GameTools.getGameString(str);
    }

    /// <summary>
    /// 格式化一段文字
    /// </summary>
    public static string StringFormat(string _str, params object[] paras)
    {
        return string.Format(_str, paras);
    }

    /// <summary>
    /// 设置一个自定义可存储整形字段
    /// </summary>
    public static void SetIntVar(string _key, int _val)
    {
        WorldManager.Instance.SetIntVar(_key, _val);
    }
    /// <summary>
    /// 设置一个自定义可存储浮点型形字段
    /// </summary>
    public static void SetFloatVar(string _key, float _val)
    {
        WorldManager.Instance.SetFloatVar(_key, _val);
    }

    /// <summary>
    /// 获取一个自定义可存储整形字段
    /// </summary>
    public static int GetIntVar(string _key)
    {
        int retVal;
        if (ArchiveData.Current.CustomIntVar.TryGetValue(_key, out retVal))
        {
            return retVal;
        }
        return 0;
    }

    /// <summary>
    /// 获取一个自定义可存储浮点形字段
    /// </summary>
    public static float GetFloatVar(string _key)
    {
        float retVal;
        if (ArchiveData.Current.CustomFloatVar.TryGetValue(_key, out retVal))
        {
            return retVal;
        }
        return 0f;
    }

    /// <summary>
    /// 设置一个可存储的字符串字段
    /// </summary>
    public static void SetStrVar(string _key, string _val)
    {
        ArchiveData.Current.CustomStringVar[_key] = _val;
    }

    /// <summary>
    /// 获取一个自定义可存储字符串字段
    /// </summary>
    public static string GetStrVar(string _key)
    {
        string retVal;
        if (ArchiveData.Current.CustomStringVar.TryGetValue(_key, out retVal))
        {
            return retVal;
        }
        return string.Empty;
    }

    /// <summary>
    /// 设置一个自定义CD时间(单位：游戏内天数)
    /// </summary>
    public static void SetCustomCD(string cdID, float cdTime)
    {
        ArchiveData.Current.SetCustomCD(cdID, cdTime);
    }

    /// <summary>
    /// 获取一个自定义CD剩余的时间（游戏内天数）
    /// </summary>
    public static float GetCustomCD(string cdID)
    {
        return ArchiveData.Current.GetCustomCD(cdID);
    }

    /// <summary>
    /// 返回一个自定义CD时间是否就绪
    /// </summary>
    public static bool IsCustomCDReady(string tagID)
    {
        return ArchiveData.Current.IsCustomCDReady(tagID);
    }

    /// <summary>
    /// 查询并返回一个容器
    /// </summary>
    public static IGameContainer GetContainer(string _instID, string _infoID)
    {
        return ArchiveData.Current.GetContainer(_instID, true, _infoID);
    }

    public static ContainerRtData CreateDisposeContainer()
    {
        ContainerRtData _disposeContainer = new ContainerRtData();
        GameContainerPojo _disposeContainerPojo = new GameContainerPojo();
        _disposeContainerPojo.Key = "dispose";
        _disposeContainerPojo.display_name = GameTools.GetGameString("CreateDisposeContainer_1");
        _disposeContainerPojo.item_type = 0;
        _disposeContainer.ContainerInfo = _disposeContainerPojo;
        _disposeContainer.CurCapicity = 70;
        return _disposeContainer;
    }

    /// <summary>
    /// 获取当前角色未分配天赋点数
    /// </summary>
    public static int GetPlayerTalentPoints()
    {
        return ArchiveData.Current.playerTalentPoint;
    }

    /// <summary>
    /// 获取当前未读消息数目
    /// </summary>
    public static int GetUnreadMsgCount()
    {
        return ArchiveData.Current.UnreadMsgCount;
    }

    public static int GetRoleMaxLevel()
    {
        return GameSettings.AdjustedRoleMaxLevel;
    }

    public static RoleStatuFieldPojo GetRoleStatuFieldInfo(string _statKey)
    {
        return ResourceManager.Get<RoleStatuFieldPojo>(_statKey);
    }

    #region 角色存档操作相关
    /// <summary>
    /// 返回游戏主界面 （一般用于重大错误处理）
    /// </summary>
    public static void BackToMainMenu()
    {
        GameShared.Instance.LoadingToScene("HomeScene");
    }

    /// <summary>
    /// 获取当前手动存档的最大数目
    /// </summary>
    public static int GetMaxArchiveCount()
    {
        return GameShared.Instance.GetMaxArchiveFileCount();
    }

    /// <summary>
    /// 判断指定的存档位存档文件是否存在
    /// </summary>
    public static bool IsArchiveFileExist(int _archiveIndex)
    {
        string sTagPath = ArchiveData.GetArchiveFilePath(_archiveIndex);
        if (System.IO.File.Exists(sTagPath))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 读取存档的缩略信息
    /// </summary>
    public static ArchiveSummaryInfo LoadArchiveSummaryInfo(int _archiveIndex)
    {
        string sTagPath = ArchiveData.GetArchiveFilePath(_archiveIndex);
        if (System.IO.File.Exists(sTagPath))
        {
            return ArchiveData.LoadArchiveSummaryInfo(sTagPath, out _);
        }
        return null;
    }

    public static List<ArchiveSummaryInfo> GetEmptyArchiveList()
    {
        return new List<ArchiveSummaryInfo>();
    }

    public static ArchiveSummaryInfo CreateCurGameSummaryInfo()
    {
        ArchiveSummaryInfo newSummaryInfo = ArchiveData.GetCurGameSummaryInfo();
        return newSummaryInfo;
    }

    public static void SortArchiveListByTime(List<ArchiveSummaryInfo> _tagList)
    {
        _tagList.Sort(delegate (ArchiveSummaryInfo a, ArchiveSummaryInfo b)
        {
            if (a.isNewSave && !b.isNewSave)
            {
                return -1;
            }
            else if (!a.isNewSave && b.isNewSave)
            {
                return 1;
            }
            else
            {
                DateTime timeA, timeB;
                DateTime.TryParse(a.save_datetime, out timeA);
                DateTime.TryParse(b.save_datetime, out timeB);
                return timeB.CompareTo(timeA);
            }
        });
    }

    /// <summary>
    /// 立即读取指定的存档
    /// </summary>
    public static void LoadTagArchive(int _archiveIndex, bool _isInGame, Action<bool> _callback)
    {
        ShowWaittingBox(GameTools.GetGameString("UIArchiveDataSelDlg_CloseDialog"));
        var archive = ArchiveData.LoadArchive(_archiveIndex, false);

        if (archive.CurGameMode == ModManager.Instance.GetCurrentGameMode())
        {
            ArchiveData.Current = archive;
            GameShared.Instance.LoadGameModeEnviro(() =>
            {
                try
                {
                    ArchiveData.Current.OnLoad();
                    HideWaittingBox();
                    if (_callback != null)
                    {
                        _callback.Invoke(true);
                    }
                    RecordArchiveIndex(_archiveIndex);
                    GameShared.Instance.LoadingToScene(ArchiveData.Current.GetCurPlayerStayScene());
                }
                catch (Exception ex)
                {
                    HideWaittingBox();
                    if (_callback != null)
                    {
                        _callback.Invoke(false);
                    }

                    string sErrorInfo = GameTools.GetGameString("GameCoreHelper_LoadTagArchive");
                    if (_isInGame)
                    {
                        GlobalUI.Instance.ShowToastInfo(sErrorInfo, 2.0f);
                        BackToMainMenu();
                    }
                    else
                    {
                        GlobalUI.Instance.ShowConfirmDlg(sErrorInfo, 0, null);
                    }
                }
            });
        }
        else
        {
            if (_callback != null)
            {
                _callback.Invoke(false);
            }
        }
    }

    /// <summary>
    /// 立即保存当前游戏至指定存档位置
    /// </summary>
    public static void SaveCurrentArchive(int _archiveIndex, bool _isInGame)
    {
        RecordArchiveIndex(_archiveIndex);
        GameShared.Instance.SaveCurGame(_archiveIndex, false);
    }

    public static bool DeleteTagArchiveData(int _archiveIndex)
    {
        string sTagPath = ArchiveData.GetArchiveFilePath(_archiveIndex);
        if (System.IO.File.Exists(sTagPath))
        {
            ArchiveData.DeleteArchive(sTagPath);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 显示默认的系统等待UI
    /// </summary>
    public static void ShowWaittingBox(string strWaitInfo)
    {
        if (GlobalUI.Instance.globalNetUI != null)
            GlobalUI.Instance.globalNetUI.ShowWaittingBox(strWaitInfo);
    }

    /// <summary>
    /// 隐藏默认的系统等待UI
    /// </summary>
    public static void HideWaittingBox()
    {
        if (GlobalUI.Instance.globalNetUI != null)
            GlobalUI.Instance.globalNetUI.CloseWaittingBox();
    }

    public static string GetStoryClassName(string _storyID)
    {
        StoryModePojo _tagStory;
        if (GameShared.Instance.CachedStoryModePojo.TryGetValue(_storyID, out _tagStory))
        {
            return _tagStory.GetDisplayName();
        }
        return "";
    }

    #endregion

    #region 全局存档操作相关

    public static bool IsDisableAutoSave()
    {
        if (RuntimeData.Instance != null)
        {
            return RuntimeData.Instance.DisableAutoSave == 1;
        }
        return true;
    }

    public static void SetIsDisableAutoSave(bool _isDisable)
    {
        if (RuntimeData.Instance != null)
        {
            RuntimeData.Instance.DisableAutoSave = _isDisable ? 1 : 0;
        }
    }

    public static int GetGlobalInt(string _key)
    {
        if (RuntimeData.Instance == null)
            return 0;
        int outVal;
        if (RuntimeData.Instance.GlobalIntVar.TryGetValue(_key, out outVal))
        {
            return outVal;
        }
        return 0;
    }

    public static void SetGlobalInt(string _key, int _val, bool _saveImmed)
    {
        if (RuntimeData.Instance == null)
            return;
        RuntimeData.Instance.GlobalIntVar[_key] = _val;
        if (_saveImmed)
        {
            RuntimeData.Instance.SaveToLocal();
        }
    }

    public static string GetGlobalString(string _key)
    {
        if (RuntimeData.Instance == null)
            return null;
        string outVal;
        if (RuntimeData.Instance.GlobalStringVar.TryGetValue(_key, out outVal))
        {
            return outVal;
        }
        return string.Empty;
    }

    public static void SetGlobalString(string _key, string _val, bool _saveImmed)
    {
        if (RuntimeData.Instance == null)
            return;
        RuntimeData.Instance.GlobalStringVar[_key] = _val;
        if (_saveImmed)
        {
            RuntimeData.Instance.SaveToLocal();
        }
    }

    public static void SaveGlobalVals()
    {
        if (RuntimeData.Instance == null)
            return;
        RuntimeData.Instance.SaveToLocal();
    }

    public static int GetLastArchiveIndex()
    {
        int lastIndex = RuntimeData.Instance.LastOpArchiveIndex;
        if (lastIndex <= 0 || lastIndex >= GetMaxArchiveCount())
        {
            lastIndex = -1;
        }
        return lastIndex;
    }

    public static void RecordArchiveIndex(int _index)
    {
        if (_index > 0 && _index < GetMaxArchiveCount())
        {
            RuntimeData.Instance.LastOpArchiveIndex = _index;
        }
    }

    public static Dictionary<string, int> runtimeIntDic = new Dictionary<string, int>();
    /// <summary>
    /// 获取运行期间有效的临时变量
    /// </summary>
    public static int GetRuntimeInt(string _key)
    {
        int outVal;
        if (runtimeIntDic.TryGetValue(_key, out outVal))
        {
            return outVal;
        }
        return 0;
    }

    /// <summary>
    /// 设置运行期间有效的临时变量
    /// </summary>
    public static void SetRuntimeInt(string _key, int _val)
    {
        runtimeIntDic[_key] = _val;
    }

    #endregion

    #region 角色人物相关

    /// <summary>
    /// 返回主角对象
    /// </summary>
    public static RoleRuntimeData GetPlayerRole()
    {
        return ArchiveData.Current.playerRoleInfo;
    }

    /// <summary>
    /// 返回配置表中一个角色模板信息
    /// </summary>
    public static RoleTemplatePojo GetRoleTemplateInfo(string _roleTpltID)
    {
        return ResourceManager.Get<RoleTemplatePojo>(_roleTpltID);
    }

    /// <summary>
    /// 返回指定ID的角色信息
    /// </summary>
    public static RoleRuntimeData GetRole(string _roleID)
    {
        return ArchiveData.Current.GetRoleWithTemplateId(_roleID);
    }

    /// <summary>
    /// 返回指定ID的角色信息的副本
    /// </summary>
    public static RoleRuntimeData GetRoleCopy(string _roleID)
    {
        RoleRuntimeData tagRole = ArchiveData.Current.GetRoleWithTemplateId(_roleID);
        if (tagRole != null)
        {
            return tagRole.Clone();
        }
        return null;
    }

    /// <summary>
    /// 生成一个空的RoleRuntimeData 类型的LIST表
    /// </summary>
    public static List<RoleRuntimeData> GetEmptyRoleList()
    {
        return new List<RoleRuntimeData>(10);
    }

    /// <summary>
    /// 获取上一次人物选择框界面选中的角色列表
    /// </summary>
    public static List<RoleRuntimeData> GetLastSeledRoles()
    {
        return WorldManager.Instance.eventExecutor.curSeledRolesList;
    }

    /// <summary>
    /// 返回游戏内所有的已创建非模板人物列表
    /// </summary>
    public static List<RoleRuntimeData> GetAllGameRoles()
    {
        if (GameCoreHelper.IsDLCActived("xlhy#matchgame#0.1"))
        {
            return ArchiveData.Current.RoleInfoList;
        }
        else
        {
            List<RoleRuntimeData> roleRuntimeData = new List<RoleRuntimeData>();
            for (int i = 0; i < ArchiveData.Current.RoleInfoList.Count; ++i)
            {
                if (ArchiveData.Current.RoleInfoList[i].roleID == "阿古拉" || ArchiveData.Current.RoleInfoList[i].roleID == "赫拉尼松"
                    || ArchiveData.Current.RoleInfoList[i].roleID == "贾米拉" || ArchiveData.Current.RoleInfoList[i].roleID == "金若琳"
                    || ArchiveData.Current.RoleInfoList[i].roleID == "璨")
                {
                }
                else
                {
                    roleRuntimeData.Add(ArchiveData.Current.RoleInfoList[i]);
                }
            }
            return roleRuntimeData;
        }
    }

    /// <summary>
    /// 返回两个角色之间的好感度值
    /// </summary>
    public static int GetRoleRelationVal(string _role1, string _role2)
    {
        return ArchiveData.Current.GetRoleRelationVal(_role1, _role2);
    }

    /// <summary>
    /// 设置两个角色之间的好感度值
    /// </summary>
    public static void SetRoleRelationVal(RoleRuntimeData fromRole, RoleRuntimeData toRole, int _newVal)
    {
        ArchiveData.Current.SetRoleRelationVal(fromRole, toRole, _newVal, true, true);
    }

    /// <summary>
    /// 改变两个角色之间的好感度值  _isChainOp：是否链式操作，如是按照toRole的好友敌人列表处理相关对象
    /// </summary>
    public static void ChangeRoleRelationVal(RoleRuntimeData fromRole, RoleRuntimeData toRole, int _addVal, bool _isChainOp)
    {
        ArchiveData.Current.ChangeRoleRelationVal(fromRole, toRole, _addVal, _isChainOp, true);
    }

    #endregion

    #region 物品道具操作相关

    /// <summary>
    /// 判断主角当前道具（包括金钱、资源等）数量是否满足
    /// </summary>
    public static bool CheckInventoryForItem(string itemID, int count)
    {
        return ArchiveData.Current.CheckInventoryForItem(itemID, count);
    }

    /// <summary>
    /// 返回主角当前的道具（包括金钱、资源等）数量
    /// </summary>
    public static int GetItemStorageNum(string itemID)
    {
        return ArchiveData.Current.GetItemStorageNum(itemID);
    }

    /// <summary>
    /// 获取玩家当前的所持物品列表快照
    /// </summary>
    public static List<GameItemData> GetPlayerInventorySnapshot()
    {
        List<GameItemData> itemsList = new List<GameItemData>();
        itemsList.AddRange(ArchiveData.Current.GameItemList);
        return itemsList;
    }

    /// <summary>
    /// 获取玩家当前的所持物品列表实例
    /// </summary>
    public static IList<GameItemData> GetPlayerInventoryList()
    {
        return ArchiveData.Current.GameItemList;
    }

    /// <summary>
    /// 执行背包物品整理
    /// </summary>
    public static void SortPlayerInventoryList()
    {
        ArchiveData.Current.GameItemList.Sort(new System.Comparison<GameItemData>(GameItemData.ItemsInfoSortCompare));
    }

    /// <summary>
    /// 获取玩家当前的所持物品列表实例
    /// </summary>
    public static IList<GameItemData> GetPlayerFoodsList()
    {
        return ArchiveData.Current.FoodItemList;
    }

    /// <summary>
    /// 根据物品ID获取物品Pojo
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static GameItemPojo GetItemPojo(string id)
    {
        return ResourceManager.Get<GameItemPojo>(id);
    }

    /// <summary>
    /// 获取玩家当前食物栏容量
    /// </summary>
    public static int GetPlayerFoodsCapicity()
    {
        return ArchiveData.Current.FoodItemCapacity;
    }

    /// <summary>
    /// 获取当前玩家食物栏中的食物数量
    /// </summary>
    public static int GetPlayerFoodsListCount()
    {
        return ArchiveData.Current.FoodItemList.Count;
    }

    /// <summary>
    /// 执行食物物品整理
    /// </summary>
    public static void SortPlayerFoodsList()
    {
        ArchiveData.Current.FoodItemList.Sort(new System.Comparison<GameItemData>(GameItemData.ItemsInfoSortCompareFoods));
    }

    /// <summary>
    /// 将背包中的食物类道具转移到食物专用栏中（当食物栏已满时放弃）
    /// </summary>
    public static void TransferFoodItems()
    {
        ArchiveData.Current.TransferFoodItems();
    }

    /// <summary>
    /// 获取玩家当前背包容量
    /// </summary>
    public static int GetPlayerInvUsedCount()
    {
        return ArchiveData.Current.GetCurrentInventoryCount();
    }

    /// <summary>
    /// 获取玩家当前背包容量
    /// </summary>
    public static int GetPlayerInventoryCapicity()
    {
        return ArchiveData.Current.GetInventoryCapcity();
    }

    /// <summary>
    /// 生成一个空的GameItemData 类型的LIST表
    /// </summary>
    public static List<GameItemData> GetEmptyGameItemList()
    {
        return new List<GameItemData>(10);
    }

    public static GameItemData CreateGameItem(string _id, int _num)
    {
        return new GameItemData(_id, _num);
    }

    /// <summary>
    /// 判定目标物品是否满足查询条件
    /// </summary>
    public static bool CheckItemByCondition(GameItemData tagItem, string _conditionStr)
    {
        if (tagItem == null)
            return false;
        if (!string.IsNullOrEmpty(_conditionStr))
        {
            RuntimeArgVals tagArgVal = new RuntimeArgVals();
            tagArgVal.CurTagItem = tagItem;
            if (!GameUtils.checkConditionGroup(_conditionStr, tagArgVal, false))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 将道具加入玩家背包
    /// </summary>
    /// <param name="_checkSpace">是否检查背包控件</param>
    /// <param name="_alertMsg">是否提示消息</param>
    /// <returns></returns>
    public static bool AddItemToPlayerInv(GameItemData _addItem, bool _checkSpace, bool _alertMsg)
    {
        if (_checkSpace)
        {
            if (!ArchiveData.Current.CheckInventoryForItem(_addItem.itemID, _addItem.itemCount))
            {
                return false;
            }
        }
        ArchiveData.Current.AddToInventory(ArchiveData.Current.GetPlayerOwnedCamp(), ArchiveData.Current.playerTeam, _addItem, _alertMsg);
        return true;
    }

    /// <summary>
    /// 按数量移除道具
    /// </summary>
    public static void RemovePlayerItems(string itemID, int itemCount, bool _alertMsg)
    {
        ArchiveData.Current.RemoveFromInventory(itemID, itemCount, _alertMsg, true);
    }

    /// <summary>
    /// 移除指定道具
    /// </summary>
    public static void RemovePlayerSpecificItem(GameItemData _tagItem, int itemCount, bool _alertMsg)
    {
        ArchiveData.Current.RemoveSpecificItemFromInventory(_tagItem, itemCount, _alertMsg);
    }

    public static void AddItemsToPlayerInvByList(List<GameItemData> _addList)
    {
        foreach (GameItemData tagItem in _addList)
        {
            int _itemType = tagItem.ItemInfo.item_type;
            if (_itemType >= 6 && _itemType <= 7)
            {
                //需要进行逻辑判断的类型
                AddItemToPlayerInv(tagItem, false, false);
            }
            else
            {
                ArchiveData.Current.GameItemList.Add(tagItem);
            }
        }
        //更新界面统计数字
        GameShared.Instance.SendSceneMessage("update_infos", null);
    }

    /// <summary>
    /// 检查物品列表里是否满足指定的物品列表数量
    /// </summary>
    public static bool CheckInventoryForItems(List<GameItemData> _checkList)
    {
        return ArchiveData.Current.CheckInventoryForItems(_checkList);
    }

    /// <summary>
    /// 根据道具表里指定的列表对象移除道具
    /// </summary>
    public static void RemoveItemsFromPlayerInvByList(List<GameItemData> _removeList)
    {
        bool needUpdateRoleStatu = false;
        foreach (GameItemData tagItem in _removeList)
        {
            if (tagItem.ItemInfo.item_type == 10)
            {
                needUpdateRoleStatu = true;
            }
            if (ArchiveData.Current.playerRoleInfo.CurActiveItem == tagItem)
            {
                ArchiveData.Current.playerRoleInfo.CurActiveItem = null;
                needUpdateRoleStatu = true;
            }
            ArchiveData.Current.GameItemList.Remove(tagItem);
        }
        if (needUpdateRoleStatu)
        {
            //宝物更新属性逻辑
            ArchiveData.Current.playerRoleInfo.UpdateRoleStatu();
            GameShared.Instance.SendSceneMessage("player_status_updated", null);
        }
        //更新界面统计数字
        GameShared.Instance.SendSceneMessage("update_infos", null);
    }

    /// <summary>
    /// 根据物品ID移除指定数量的道具
    /// </summary>
    public static void RemoveItemsFormPlayerInv(string itemID, int itemCount, bool isAlert = true)
    {
        ArchiveData.Current.RemoveFromInventory(itemID, itemCount, isAlert);
    }

    /// <summary>
    /// 移除指定栏位的物品道具
    /// </summary>
    public static void RemoveTagItemInSlot(int _slotIndex, int removeCount = -1)
    {
        ArchiveData.Current.RemoveTagSlotItem(_slotIndex, removeCount);
    }

    /// <summary>
    /// 使用目标栏位的物品道具
    /// </summary>
    public static void UseTagItemInSlot(int itemIndex)
    {
        ArchiveData.Current.UseTagSlotItem(itemIndex);
    }

    /// <summary>
    /// 按数量使用目标栏位的物品道具
    /// </summary>
    public static void UseTagItemInSlotByCount(int itemIndex, int _useCount)
    {
        ArchiveData.Current.UseTagSlotItem(itemIndex, _useCount);
    }

    /// <summary>
    /// 装备道具栏中的一个道具装备
    /// </summary>
    public static void EquipTagItemInInventory(RoleRuntimeData _tagRole, GameItemData _tagItem)
    {
        EquipLogic.EquipFromInventory(_tagRole, _tagItem);
    }

    /// <summary>
    /// 卸下指定装备栏上的道具装备
    /// </summary>
    public static void UnequipTagItemToInventory(RoleRuntimeData _tagRole, int _slotIndex)
    {
        EquipLogic.UnequipToInventory(_tagRole, _slotIndex);
    }

    /// <summary>
    /// 获取上一次物品选择框界面选中的道具列表
    /// </summary>
    public static List<GameItemData> GetLastSeledItems()
    {
        return WorldManager.Instance.eventExecutor.curSeledItemsList;
    }

    private static List<GameItemData> _cachedItemsForAb = null;
    /// <summary>
    /// 获取可以用于快捷键配置的物品列表
    /// </summary>
    public static List<GameItemData> GetItemsForActionBar()
    {
        if (_cachedItemsForAb == null)
        {
            _cachedItemsForAb = new List<GameItemData>();
        }
        _cachedItemsForAb.Clear();
        foreach (GameItemData _tagItem in ArchiveData.Current.GameItemList)
        {
            if (_tagItem.ItemInfo.IsBattleUseEnable())
            {
                _cachedItemsForAb.Add(_tagItem);
            }
        }

        return _cachedItemsForAb;
    }

    /// <summary>
    /// 获取人物对礼物的喜好等级
    /// </summary>
    public static int QualifyAsPresentWithLikeLevel(GameItemData item, RoleRuntimeData tagRole)
    {
        return PresentLogic.QualifyAsPresentWithLikeLevel(item, tagRole.getRoleTemplate().Likes());
    }

    /// <summary>
    /// 根据物品信息字符串获取物品表
    /// </summary>
    public static List<GameItemData> GetItemsListByInfo(string _itemsInfo)
    {
        List<GameItemData> itemList = new List<GameItemData>();
        GameUtils.ReadItemListFromStr(itemList, _itemsInfo);
        return itemList;
    }

    /// <summary>
    /// 根据物品表获取物品信息字符串
    /// </summary>
    public static string GetItemsInfoStrByList(List<GameItemData> _itemsList)
    {
        return GameUtils.GetItemListStr(_itemsList);
    }

    /// <summary>
    /// 根据掉落信息字符串获取掉落物品表
    /// </summary>
    public static List<GameItemData> GetLootItemsByInfo(string _lootInfo)
    {
        List<GameItemData> itemList = new List<GameItemData>();
        GameUtils.GetLootItemsByStrInfo(itemList, _lootInfo);
        return itemList;
    }

    /// <summary>
    /// 根据物品信息字符串获取物品
    /// </summary>
    public static GameItemData GetItemByInfoData(string _itemInfo)
    {
        GameItemData tagItem = GameItemData.Parse(_itemInfo);
        return tagItem;
    }

    /// <summary>
    /// 根据物品获取物品信息字符串
    /// </summary>
    public static string GetItemInfoDataStr(GameItemData _tagItem)
    {

        return _tagItem.GetItemInfoStr();
    }

    /// <summary>
    /// 获取当前食物数目
    /// </summary>
    public static int GetFoodCount()
    {
        return ArchiveData.Current.GetFoodCount();
    }

    /// <summary>
    /// 修复道具
    /// </summary>
    public static void RepairItem(GameItemData _tagItem)
    {
        if (_tagItem.MaxDurability > 0)
        {
            _tagItem.currentDurability = _tagItem.MaxDurability;
        }
    }

    #endregion

    #region 交易系统相关

    public static TradeData GetCurrentTradeData()
    {
        return TradeData.Instance;
    }

    public static MerchantRtData GetMerchantByID(string _merchantID)
    {
        MerchantRtData tagStore = ArchiveData.Current.GetMerchantByID(_merchantID);
        return tagStore;
    }

    public static TradeData CreateTradeData(MerchantRtData _tagMerchant)
    {
        TradeData _tradeData = new TradeData(_tagMerchant);
        _tradeData.Init();
        return _tradeData;
    }

    public static void ReleaseCurTradeData()
    {
        TradeData.Instance = null;
    }

    #endregion

    #region 兵种卡牌操作相关

    /// <summary>
    /// 获取玩家当前队伍兵种卡牌表快照
    /// </summary>
    public static List<CardRtData> GetPlayerUnitCardsSnapshot()
    {
        List<CardRtData> itemsList = new List<CardRtData>();

        TeamInMap playerTeam = ArchiveData.Current.playerTeam;
        itemsList.AddRange(playerTeam.soldierCardsInTeam);

        //GameTools.AddCardsByStr("雪岭勇士|雪岭勇士|雪岭勇士|雪岭精英勇士|雇佣铠甲剑士|雇佣铠甲枪兵", itemsList);

        return itemsList;
    }

    /// <summary>
    /// 获取玩家当前兵种卡牌上限
    /// </summary>
    public static int GetPlayerUnitCardsCapicity()
    {
        TeamInMap playerTeam = ArchiveData.Current.playerTeam;
        return CardLogic.GetSoldierCardsCapacity(playerTeam);
    }

    /// <summary>
    /// 生成一个空的GameItemData 类型的LIST表
    /// </summary>
    public static List<CardRtData> GetEmptyCardsItemList()
    {
        return new List<CardRtData>();
    }

    public static void AddCardsToPlayerPartyByList(List<CardRtData> _addList)
    {
        if (_addList == null || _addList.Count <= 0)
        {
            return;
        }
        TeamInMap playerTeam = ArchiveData.Current.playerTeam;
        foreach (CardRtData tagItem in _addList)
        {
            playerTeam.AddSoldierCardsInTeam(tagItem);
        }
        CardLogic.UpdateTeam(playerTeam);
    }

    public static void RemoveCardsFromPlayerPartyByList(List<CardRtData> _removeList)
    {
        if (_removeList == null || _removeList.Count <= 0)
        {
            return;
        }
        TeamInMap playerTeam = ArchiveData.Current.playerTeam;
        foreach (CardRtData tagItem in _removeList)
        {
            playerTeam.RemoveSoldierCardsInTeam(tagItem);
        }
        CardLogic.UpdateTeam(playerTeam);
    }

    /// <summary>
    /// 判定目标卡牌是否满足查询条件
    /// </summary>
    public static bool CheckCardByCondition(CardRtData tagCard, string _conditionStr)
    {
        if (tagCard == null)
            return false;
        if (!string.IsNullOrEmpty(_conditionStr))
        {
            RuntimeArgVals tagArgVal = new RuntimeArgVals();
            tagArgVal.tagCard = tagCard;
            if (!GameUtils.checkConditionGroup(_conditionStr, tagArgVal, false))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 获取当前游戏环境中的所有卡牌信息表
    /// </summary>
    public static List<CardPojo> GetAllCardInfos()
    {
        List<CardPojo> retList = new List<CardPojo>();
        foreach (CardPojo tagPojo in ResourceManager.GetAll<CardPojo>())
        {
            retList.Add(tagPojo);
        }
        return retList;
    }

    /// <summary>
    /// 根据卡牌信息创建卡牌实例
    /// </summary>
    public static CardRtData CreateCardByInfo(CardPojo _cardInfo)
    {
        return new CardRtData(_cardInfo);
    }

    /// <summary>
    /// 获取上一次卡牌选择框界面选中的卡牌列表
    /// </summary>
    public static List<CardRtData> GetLastSeledCards()
    {
        return WorldManager.Instance.eventExecutor.curSeledCardList;
    }

    #endregion

    #region 队伍相关

    /// <summary>
    /// 返回主角队伍
    /// </summary>
    public static TeamInMap GetPlayerTeam()
    {
        return ArchiveData.Current.playerTeam;
    }

    /// <summary>
    /// 从全局中查询一个游戏内的队伍并返回
    /// _queryType 查询类型： "bind_id"：队伍的绑定ID，野怪为野怪ID，军队为队长人物ID  "leader"：队长的人物ID   "tag"：队伍的TAG
    /// </summary>
    public static TeamInMap GetTeamByQueryInfo(string _queryType, string _queryInfo)
    {
        switch (_queryType)
        {
            case "leader":
                {
                    return ArchiveData.Current.GetTeamByLeader(_queryInfo);
                }
            case "tag":
                {
                    return ArchiveData.Current.GetTeamByTAG(_queryInfo);
                }
            case "bind_id":
                {
                    return ArchiveData.Current.GetTeamByBindId(_queryInfo);
                }
        }
        return null;
    }

    public static int GetGroupDefInstruct(int _groupNo)
    {
        if (ArchiveData.Current.CurGroupInstruct.TryGetValue(_groupNo.ToString(), out int _retNo))
        {
            return _retNo;
        }
        return 0;
    }

    public static void SetGroupDefInstruct(int _groupNo, int _newInstruct)
    {
        ArchiveData.Current.CurGroupInstruct[_groupNo.ToString()] = _newInstruct;
    }

    #endregion

    #region 阵营相关

    /// <summary>
    /// 通过阵营ID返回一个游戏内阵营的实例
    /// </summary>
    public static GameCampRtData GetGameCamp(string campID)
    {
        return ArchiveData.Current.GetGameCamp(campID);
    }

    /// <summary>
    /// 返回游戏内所有的阵营列表
    /// </summary>
    public static List<GameCampRtData> GetAllGameCamps()
    {
        return ArchiveData.Current.gameCampList;
    }

    /// <summary>
    /// 返回一个空的阵营势力表
    /// </summary>
    public static List<GameCampRtData> GetEmptyGameCampList()
    {
        return new List<GameCampRtData>();
    }

    /// <summary>
    /// 获取两个阵营当前的关系信息
    /// </summary>
    public static CampRelationInfo GetCampRelation(string campA, string campB)
    {
        return ArchiveData.Current.GetCampRelationInfo(campA, campB);
    }

    /// <summary>
    /// 获取两个阵营当前的外交关系状态 0:中立 1：交战 2：结盟 3：己方
    /// </summary>
    public static int GetCampRlState(string _campA, string _campB)
    {
        return ArchiveData.Current.GetCampRlState(_campA, _campB);
    }

    /// <summary>
    /// 获取两个阵营当前的好感度值
    /// </summary>
    public static int GetCampRlFv(string _campA, string _campB)
    {
        return ArchiveData.Current.GetCampRlFv(_campA, _campB);
    }

    /// <summary>
    /// 设置两个阵营之间的外交状态及好感度，仅做数据存储使用，不包含业务逻辑判断
    /// </summary>
    /// <param name="rlState"></param>
    /// <param name="isSetFv">是否更改好感度值</param>
    /// <param name="fv">更改的目标好感度值</param>
    public static void SetCampRl(string _campA, string _campB, int rlState, bool isSetFv, int fv, bool _alertMsg = true)
    {
        ArchiveData.Current.SetCampRl(_campA, _campB, (short)rlState, isSetFv, (short)fv, _alertMsg);
    }

    /// <summary>
    /// 弃用接口
    /// </summary>
    public static void SetCampRlVal(string _campA, string _campB, int rlState, bool isSetFv, int fv)
    {
        ArchiveData.Current.DoSetCampRlVal(_campA, _campB, (short)rlState, isSetFv, fv);
    }

    /// <summary>
    /// 获取游戏内两个阵营之间的关系状态 0：中立 1：交战 2：盟友
    /// </summary>
    public static int GetCampAttitude(string campA, string campB)
    {
        if (string.IsNullOrEmpty(campA) || string.IsNullOrEmpty(campB))
        {
            return 0;
        }
        int nState = ArchiveData.Current.GetCampRlState(campA, campB);
        if (nState == 1)
        {
            return 1;
        }
        else if (nState >= 2)
        {
            return 2;
        }
        return 0;
    }

    /// <summary>
    /// 触发任务系统里阵营敌对相关的逻辑(内部逻辑，用于清除玩家在该阵营领取的任务)
    /// </summary>
    public static void CheckQuestStateCampEnemy(string campID)
    {
        QuestLogic.CampEnemy(campID);
    }

    /// <summary>
    /// 返回一个空的阵营关系存储表
    /// </summary>
    public static List<CampsDipInfoData> GetEmptyCampsDipInfoList()
    {
        return new List<CampsDipInfoData>();
    }

    /// <summary>
    /// 指定一个角色创建一个新势力，如果此角色已有势力并且拥有领地，则会将这些领地并入新势力中（势力领袖、队伍非队长成员、状态异常者无法执行此指令）
    /// </summary>
    public static GameCampRtData RoleCreateNewCamp(RoleRuntimeData _tagRole, string _campName, string _setCapitalCity)
    {
        if (_tagRole != null)
        {
            if (_tagRole.IsPlayer())
            {
                LegacyLogic.PlatformAchievement("OwnCamp");
            }
            return ArchiveData.Current.CreateNewCamp(_tagRole, _campName, _setCapitalCity);
        }
        return null;
    }

    #endregion

    #region 游戏地点相关

    /// <summary>
    /// 通过地点ID返回一个游戏内地点的实例
    /// </summary>
    public static GamePlaceRtData GetGamePlace(string placeID)
    {
        return ArchiveData.Current.GetGamePlace(placeID);
    }

    /// <summary>
    /// 返回游戏内所有的地点列表
    /// </summary>
    public static List<GamePlaceRtData> GetAllGamePlaces()
    {
        return ArchiveData.Current.AllGamePlaceList;
    }

    /// <summary>
    /// 返回两个地点之前的实际导航路径长度，-1为路径不存在或者对应地图未开放
    /// </summary>
    public static float GetPlace2PlaceLen(string placeA, string placeB)
    {
        GamePlaceRtData tagPlaceA = GetGamePlace(placeA);
        GamePlaceRtData tagPlaceB = GetGamePlace(placeB);
        if (tagPlaceA != null && tagPlaceB != null)
        {
            return WorldManager.Instance.GetPlaceToPlaceRouteLen(tagPlaceA, tagPlaceB);
        }
        return -1;
    }

    /// <summary>
    /// 通过地点ID返回一个游戏内地点的实例
    /// </summary>
    public static GamePlaceRtData GetCurVisitPlace()
    {
        return WorldManager.Instance.CurVisitPlace;
    }

    #endregion

    #region 音效相关

    /// <summary>
    /// 播放一个游戏音效
    /// </summary>
    public static void PlayAudio(string audioID)
    {
        AudioManager.Instance.Play(audioID);
    }

    #endregion

    #region 技能相关

    private static List<SkillPointData> _cachedEmptySpList = null;
    public static List<SkillPointData> GetEmptySpList()
    {
        if (_cachedEmptySpList == null) _cachedEmptySpList = new List<SkillPointData>();
        _cachedEmptySpList.Clear();
        return _cachedEmptySpList;
    }

    public static SkillPointData GetSkillPointByStr(string _spStr)
    {
        SkillPointData _spInfo = SkillPointData.Parse(_spStr);
        if (_spInfo.SkillInfo == null)
        {
            _spInfo = null;
        }
        return _spInfo;
    }

    public static SkillPagePojo GetSkillPageInfo(string _pageID)
    {
        SkillPagePojo tagPage = ResourceManager.Get<SkillPagePojo>(_pageID);
        return tagPage;
    }

    public static SkillPointData GetSkillPoint(string _skillID, int _rank)
    {
        return new SkillPointData(_skillID, _rank);
    }

    public static SkillInfoPojo GetSkillInfo(string _skillID)
    {
        return ResourceManager.Get<SkillInfoPojo>(_skillID);
    }

    public static List<SkillPointData> GetSkillsForActionBar()
    {
        RoleRuntimeData _playerRole = GetPlayerRole();
        if (_playerRole != null)
        {
            return _playerRole.GetAllSkillsForActionBar();
        }

        return null;
    }

    #endregion

    #region 常用配置表遍历接口

    public static List<StoryModePojo> GetAllList_StoryModePojo()
    {
        List<StoryModePojo> displayList = new List<StoryModePojo>();
        foreach (var storyMode in GameShared.Instance.CachedStoryModePojo)
        {
            displayList.Add(storyMode.Value);
        }
        displayList.Sort(StoryModePojo.StoryModePojoCompare);
        return displayList;
    }

    public static List<QuestPojo> GetAllList_QuestPojo()
    {
        List<QuestPojo> displayList = new List<QuestPojo>();
        displayList.AddRange(ResourceManager.GetAll<QuestPojo>());
        return displayList;
    }

    public static List<RoleModelPojo> GetAllList_RoleModelPojo()
    {
        return GameShared.Instance.CachedModelPojo.ToList();
    }

    public static List<RoleModelActionsPojo> GetAllList_RoleModelActionsPojo()
    {
        return GameShared.Instance.CachedModelActPojo.ToList();
    }

    public static List<SkillInfoPojo> GetAllList_SkillInfoPojo()
    {
        return GameShared.Instance.CachedSkillPojo.ToList();
    }

    public static List<BuffInfoPojo> GetAllList_BuffInfoPojo()
    {
        return GameShared.Instance.CachedBuffPojo.ToList();
    }

    public static List<BattleEffectPojo> GetAllList_BattleEffectPojo()
    {
        return GameShared.Instance.CachedEffectPojo.ToList();
    }

    public static List<HitEffectPojo> GetAllList_HitEffectPojo()
    {
        return GameShared.Instance.CachedHitEffectPojo.ToList();
    }

    public static List<SoldierUnitPojo> GetAllList_SoldierUnitPojo()
    {
        List<SoldierUnitPojo> displayList = new List<SoldierUnitPojo>();
        displayList.AddRange(ResourceManager.GetAll<SoldierUnitPojo>());
        return displayList;
    }

    public static List<CardPojo> GetAllList_CardPojo()
    {
        List<CardPojo> displayList = new List<CardPojo>();
        displayList.AddRange(ResourceManager.GetAll<CardPojo>());
        return displayList;
    }

    public static List<RoleTemplatePojo> GetAllList_RoleTemplatePojo()
    {
        List<RoleTemplatePojo> displayList = new List<RoleTemplatePojo>();
        displayList.AddRange(ResourceManager.GetAll<RoleTemplatePojo>());
        return displayList;
    }

    public static List<GamePlacePojo> GetAllList_GamePlacePojo()
    {
        List<GamePlacePojo> displayList = new List<GamePlacePojo>();
        displayList.AddRange(ResourceManager.GetAll<GamePlacePojo>());
        return displayList;
    }

    public static List<GameMapPojo> GetAllList_GameMapPojo()
    {
        List<GameMapPojo> displayList = new List<GameMapPojo>();
        displayList.AddRange(ResourceManager.GetAll<GameMapPojo>());
        return displayList;
    }

    public static List<MerchantPojo> GetAllList_MerchantPojo()
    {
        List<MerchantPojo> displayList = new List<MerchantPojo>();
        displayList.AddRange(ResourceManager.GetAll<MerchantPojo>());
        return displayList;
    }

    public static List<GameIntValConfigPojo> GetAllList_GameIntValConfigPojo()
    {
        List<GameIntValConfigPojo> displayList = new List<GameIntValConfigPojo>();
        displayList.AddRange(ResourceManager.GetAll<GameIntValConfigPojo>());
        return displayList;
    }

    public static List<LegacyPojo> GetAllList_LegacyPojo()
    {
        List<LegacyPojo> displayList = new List<LegacyPojo>();
        displayList.AddRange(ResourceManager.GetAll<LegacyPojo>());
        return displayList;
    }

    public static List<LegacyFormulaPojo> GetAllList_LegacyFormulaPojo()
    {
        List<LegacyFormulaPojo> displayList = new List<LegacyFormulaPojo>();
        displayList.AddRange(ResourceManager.GetAll<LegacyFormulaPojo>());
        return displayList;
    }

    #endregion
}
