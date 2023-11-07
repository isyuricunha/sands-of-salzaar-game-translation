/********************************************************************************

** Copyright(c) 2019 汉家松鼠工作室 All Rights Reserved. 

** auth： cg
** date： 2019/7/28 14:04:00 
** desc： 资源拥有者接口

*********************************************************************************/
using System.Collections.Generic;

namespace HanFramework
{
    /// <summary>
    /// 资源拥有者接口
    /// </summary>
    public interface IGameContainer
    {
        /// <summary>
        /// 当前最大容量
        /// </summary>
        int CurCapicity { get; }

        /// <summary>
        /// 当前容器包含元素的数量
        /// </summary>
        int CurItemsNum { get; }

        /// <summary>
        /// 为容器增加元素
        /// </summary>
        void AddItems(string addItemsInfo);

        /// <summary>
        /// 获取当前容器内的道具表
        /// </summary>
        List<GameItemData> GetGameItemsListSnapshot();

        /// <summary>
        /// 获取当前容器内的卡牌表
        /// </summary>
        List<CardRtData> GetCardsListSnapshot();

        /// <summary>
        /// 获取名称
        /// </summary>
        string GetName();

        /// <summary>
        /// 获取目标容器支持放入的筛选条件
        /// </summary>
        string GetItemsFilterInfo();

        /// <summary>
        /// 根据指定列表增加物品
        /// </summary>
        void AddItemsByList(List<GameItemData> _addList);

        /// <summary>
        /// 根据指定列表移除物品
        /// </summary>
        void RemoveItemsByList(List<GameItemData> _removeList);

        /// <summary>
        /// 根据指定列表增加卡牌
        /// </summary>
        void AddCardsByList(List<CardRtData> _addList);

        /// <summary>
        /// 根据指定列表移除卡牌
        /// </summary>
        void RemoveCardsByList(List<CardRtData> _removeList);

        /// <summary>
        /// 移除容器内所有的元素
        /// </summary>
        void ClearItems();

        /// <summary>
        /// 为容器移除元素
        /// </summary>
        /// <param name="removeInfos"></param>
        void RemoveItems(string removeInfos);
    }
}
