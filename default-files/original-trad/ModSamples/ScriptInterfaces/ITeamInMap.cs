/********************************************************************************

** Copyright(c) 2020 汉家松鼠工作室 All Rights Reserved. 

** auth： kt
** date： 2020/5/7
** desc： 队伍接口

*********************************************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace HanFramework
{
    public enum TeamType
    {
        /// <summary>
        /// 部队
        /// </summary>
        Army = 0,

        /// <summary>
        /// 脚本创建的队伍
        /// </summary>
        ScriptTeam = 1,

        /// <summary>
        /// 商队
        /// </summary>
        Caravan = 2,

        /// <summary>
        /// 假人
        /// </summary>
        Dummy = 3,

        /// <summary>
        /// 农民
        /// </summary>
        Farmer = 4,

        /// <summary>
        /// 用于剧情的军队
        /// </summary>
        StoryArmy = 5,
    }

    /// <summary>
    /// 资源拥有者接口
    /// </summary>
    public interface ITeamInMap
    {
        /// <summary>
        /// 队伍ID
        /// </summary>
        string TeamID { get; }

        /// <summary>
        /// 创建时间游戏内天数
        /// </summary>
        float createTime { get; }

        /// <summary>
        /// 队伍的TAG标签
        /// </summary>
        string TeamTag { get; }

        /// <summary>
        /// 队伍类型 0：角色队伍 1：指令创建队伍  2：商队  3：假人  4：农民（无阵营）
        /// </summary>
        int teamType { get; }

        /// <summary>
        /// 队长角色对象 无队长为NULL
        /// </summary>
        IGameRole LeaderRole { get; }

        /// <summary>
        /// 队伍中的成员角色快照
        /// </summary>
        IReadOnlyList<RoleRuntimeData> rolesInTeam { get; }

        /// <summary>
        /// 队伍内的兵种单位卡牌列表
        /// </summary>
        List<CardRtData> soldierCardsInTeam { get; }

        /// <summary>
        /// 向队伍中增加兵种卡牌
        /// </summary>
        void AddSoldierCardsInTeam(CardRtData data);

        /// <summary>
        /// 从队伍中移除兵种卡牌
        /// </summary>
        bool RemoveSoldierCardsInTeam(CardRtData data);

        /// <summary>
        /// 获取当前角色数量
        /// </summary>
        int GetCurRoleCount();

        /// <summary>
        /// 获取当前队伍中己方英雄数量
        /// </summary>
        /// <returns></returns>
        int GetHeroesCurNum();

        /// <summary>
        /// 获取当前角色上限
        /// </summary>
        int GetRolesCapacity();

        /// <summary>
        /// 获取当前兵牌数量
        /// </summary>
        int GetCurSoldierCount();

        /// <summary>
        /// 获取当前兵牌上限
        /// </summary>
        int GetSoldierCardsCapacity();

        /// <summary>
        /// 当前队伍状态 -1:已解散  0:正常  1:战斗中
        /// </summary>
        int teamState { get; }

        /// <summary>
        /// 当MapID为空时表示当前队伍在某地点内
        /// </summary>
        string curMapID { get; }

        /// <summary>
        /// 队伍在地图中的坐标
        /// </summary>
        Vector2 curPos { get; }

        /// <summary>
        /// 队伍所在地点的ID
        /// </summary>
        string curStayPlaceID { get; }

        /// <summary>
        /// 队伍所属的阵营ID
        /// </summary>
        string GetCampID();

        /// <summary>
        /// 队伍所属的阵营对象实体
        /// </summary>
        GameCampRtData CurCamp { get; }

        /// <summary>
        /// 开始对队伍执行解散操作
        /// _reason   0:自主解散  1:被打败
        /// </summary>
        void DismissTeam(int _reason, string _dismissByInfo);

        /// <summary>
        /// 队伍获得经验（全体卡牌每张都获得相同经验，平均分配）
        /// </summary>
        /// <param name="_totalExp"></param>
        /// <param name="distributeType"></param>
        int TeamGainExp(int totalExp);

        /// <summary>
        /// 获取当前队伍可用卡牌空位
        /// </summary>
        int GetTeamVacancyCardCount();

        /// <summary>
        /// 获取队伍在地图上显示的名称
        /// </summary>
        string GetMapDisplayName();
    }
}
