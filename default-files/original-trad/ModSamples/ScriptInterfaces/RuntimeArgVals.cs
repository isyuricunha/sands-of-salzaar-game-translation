using System;
using System.Collections;

using HanFramework;
using System.Collections.Generic;
using XLua;

[Hotfix]
[CSharpCallLua]
public class RuntimeArgVals
{
 
    //附带的参数变量
    public string CurTagRoleID = "";
    public string CurSpawnUnitID = "";
    public GamePlaceRtData CurTagPlace = null;
    public GamePlaceRtData QuestPlace = null;
    public string QuestRoleID = "";
    public BuildingInPlace CurTagBuilding = null;
    public WarbandInfo CurWarband = null;
    public MapDummyInfo CurDummy = null;
    public TeamInMap CurTagTeam = null;
    public GameCampRtData CurTagCamp = null;
    public DiplomaticEventInfo CurDipInfo = null;
    public GameItemData CurTagItem = null;
    public string CurMapID = "";
    public string CurBattleID = "";
    public int argIntVal1 = 0;
    public int argIntVal2 = 0;
    public int argIntVal3 = 0;
    public string argStrVal1 = "";
    public string argStrVal2 = "";
    public string argStrVal3 = "";
    public int curResultVal = 0;
	public RandomEventRtData CurRandomEvent = null;
    public SkillPointData tagSkill = null;
    public SoldierInfoData tagSolider = null;
    public ChestOnMap tagChest = null;
    public CardRtData tagCard = null;
    public RandomQuestRtData randomQuest = null;
    public QuestRtData quest = null;
    public string eventId = null;
    public TextExploreRtData tagTextExplore = null;
    public BazzarRtData tagBazzar = null;
    public LocalProductRtData tagProduct = null;
    public GameMessageInfoData CurInactiveMsg = null;
    public IGameContainer CurContainer = null;
    public RoleInteractionEventPojo CurRoleRlEvent = null;
    public RoleRuntimeData[] CurRlEventRoles = null;
    public FightEventBattle CurFightBattle = null;
    public string subArg1 = null;
    public object customObj = null;

    /// <summary>
    /// 遭遇是否玩家主动触发
    /// 
    /// true：玩家主动点击与发生交互
    /// false：玩家被部队追上
    /// </summary>
    public bool IsEncounterPlayerTriggered = true;

    /// <summary>
    /// 当前相遇的地图单位，对玩家的攻击意图
    /// </summary>
    public int enemyCode = 0;

	public List<string> argvs = new List<string>();

    public RuntimeArgVals()
    {

    }

    public void SetRoleRlEvent(RoleInteractionEventPojo _eventPojo, List<RoleRuntimeData> _allEventRoles)
    {
        CurRoleRlEvent = _eventPojo;
        if (_allEventRoles != null && _allEventRoles.Count > 0)
        {
            CurRlEventRoles = new RoleRuntimeData[_allEventRoles.Count];
            for (int i = 0; i < _allEventRoles.Count; i++)
            {
                CurRlEventRoles[i] = _allEventRoles[i];
            }
        }
    }

    public RoleRuntimeData GetRoleRlEventActor(int _actorID)
    {
        if (CurRlEventRoles == null || CurRlEventRoles.Length <= 0)
            return null;
        if (_actorID >= 0 && _actorID < CurRlEventRoles.Length)
        {
            return CurRlEventRoles[_actorID];
        }
        else
        {
            return null;
        }
    }


}
