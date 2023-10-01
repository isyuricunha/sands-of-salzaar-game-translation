/********************************************************************************

** Copyright(c) 2020 汉家松鼠工作室 All Rights Reserved. 

** auth： kt
** date： 2020/4/20
** desc： 容器接口

*********************************************************************************/
using System.Collections.Generic;

namespace HanFramework
{
    /// <summary>
    /// 资源拥有者接口
    /// </summary>
    public interface IResourceOwner
    {
        /// <summary>
        /// 钱
        /// </summary>
        int Money { get; set; }

        /// <summary>
        /// 获取资源数
        /// </summary>
        /// <param name="resId"></param>
        /// <returns></returns>
        int GetResourceNum(string resId, float AIMulFactor = 1.0f);

        /// <summary>
        /// 设置资源数
        /// </summary>
        /// <param name="resId"></param>
        /// <param name="num"></param>
        void SetResourceNum(string resId, int num);

        /// <summary>
        /// 修改资源数
        /// </summary>
        /// <param name="resID"></param>
        /// <param name="_chgVal"></param>
        /// <param name="isAlert"></param>
        void ChangeResourceNum(string resID, int _chgVal, bool isAlert = false, float AIMulFactor = 1.0f);

        /// <summary>
        /// 按照材料表消耗势力的资源
        /// </summary>
        /// <param name="resListStr"></param>
        /// <param name="AICheating"></param>
        /// <param name="mulNum"></param>
        void ConsumeResources(string resListStr, bool AICheating = false, float mulNum = 1.0f);

        /// <summary>
        /// 按照材料表消耗势力的资源
        /// </summary>
        /// <param name="resList"></param>
        /// <param name="AICheating"></param>
        /// <param name="mulNum"></param>
        void ConsumeResources(List<GameItemData> resList, bool AICheating = false, float mulNum = 1.0f);

        /// <summary>
        /// 检查资源数
        /// </summary>
        /// <param name="resList"></param>
        /// <param name="AICheating"></param>
        /// <param name="mulNum"></param>
        /// <returns></returns>
        bool CheckResStorage(List<GameItemData> resList, bool AICheating = false, float mulNum = 1.0f);

        /// <summary>
        /// 检查资源数
        /// </summary>
        /// <param name="resListStr"></param>
        /// <param name="AICheating"></param>
        /// <param name="mulNum"></param>
        /// <returns></returns>
        bool CheckResStorage(string resListStr, bool AICheating = false, float mulNum = 1.0f);

        /// <summary>
        /// 获取显示名字
        /// </summary>
        /// <returns></returns>
        string GetDisplayName();


        /// <summary>
        /// 评估附有程度，获得综合得分
        /// </summary>
        /// <returns></returns>
        float GetResourceScore();
    }
}
