/********************************************************************************

** Copyright(c) 2020 汉家松鼠工作室 All Rights Reserved. 

** auth： kt
** date： 2020/5/7
** desc： 角色接口

*********************************************************************************/
using System.Collections.Generic;

namespace HanFramework
{

    /// <summary>
    /// 游戏内人物相关接口  RoleRuntimeData继承自IGameRole
    /// </summary>
    public interface IGameRole
    {
        /// <summary>
        /// 人物ID
        /// </summary>
        string roleID { get; }

        /// <summary>
        /// 人物的显示名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 人物的立绘信息
        /// </summary>
        string HeadAvata { get; }

        /// <summary>
        /// 人物的模型信息
        /// </summary>
        string ModelInfo { get; }

        /// <summary>
        /// 所持金钱数
        /// </summary>
        int roleMoney { get; }

        /// <summary>
        /// 当前等级
        /// </summary>
        int roleLevel { get; }


        /// <summary>
        /// 当前经验
        /// </summary>
        int roleExp { get; }

        /// <summary>
        /// 在玩家管辖下，是否可以领军出征
        /// 标记为1的才会自动带队出征，否则守城
        /// </summary>
        int isFreeLeaderInPlayerTeam { get; }

        /// <summary>
        /// 是否激活与玩家关系（可打听到）
        /// </summary>
        bool IsPlayerActive();

        /// <summary>
        /// 使其对玩家社交激活
        /// </summary>
        void SetPlayerContactActive();

        /// <summary>
        /// 获取角色的自定义TAG字段
        /// </summary>
        string GetTagValue(string key);

        /// <summary>
        /// 当前可支配技能点数
        /// </summary>
        int unusedSp { get; }

        /// <summary>
        /// 点亮技能标签页
        /// </summary>
        void AddSkillPage(string _pageIDs, bool alert = false);

        /// <summary>
        /// 移除技能标签页
        /// </summary>
        void RemoveSkillPage(string _pageIDs, bool alert = false);

        /// <summary>
        /// 角色当前位置信息
        /// </summary>
        string GetCurPosInfo();

        /// <summary>
        /// 当前生命值
        /// </summary>
        int curHp { get; }

        /// <summary>
        /// 当前气力值
        /// </summary>
        int curMp { get; }

        /// <summary>
        /// 当前所属阵营对象
        /// </summary>
        GameCampRtData GetRoleCamp();

        /// <summary>
        /// 增加经验
        /// </summary>
        void AddExp(int addVal);

        /// <summary>
        /// 获取角色升级所需经验
        /// </summary>
        int GetRoleLevelUpExp();

        /// <summary>
        /// 获取自定义存储值
        /// </summary>
        int GetAbstractVal(string _key);

        /// <summary>
        /// 设置自定义存储值
        /// </summary>
        void SetAbstractVal(string _key, int _val);

        /// <summary>
        /// 获取角色当前的某个属性值
        /// </summary>
        int GetRoleStat(string _key);
    }
}
