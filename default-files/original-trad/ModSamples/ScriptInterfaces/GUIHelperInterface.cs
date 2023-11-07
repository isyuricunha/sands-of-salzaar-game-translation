/********************************************************************************

** Copyright(c) 2020 汉家松鼠工作室 All Rights Reserved. 

** auth： kt
** date： 2020/5/7
** desc： 角色接口

*********************************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
using HanFramework;
using FairyGUI;

/// <summary>
/// 注意，这是GUIHelper的接口说明文档，不能直接调用！！！
/// LUA中使用此接口中的方法请使用GUIHelper类中的静态方法调用格式
/// 如： CS.GUIHelper.FuncName() 这样的格式来访问这些方法
/// </summary>
public interface GUIHelperInterface
{
    /// <summary>
    /// 显示一个指定的GUI窗体控件
    /// </summary>
    FGUIWindowLua ShowGUIWindow(string packageName, string componentName, string scriptPath, string windowArgs, Action<int> _callback);

    /// <summary>
    /// 显示在指定位置显示物品详情对话框
    /// </summary>
    UIGameItemDialog ShowItemDetailInfo(GameItemData tagItemInfo, Vector2 _fguiPos, bool _isShowPrice, bool _isLockedClick);

    /// <summary>
    /// 向指定的 GImag 控件对象中创建模型预览纹理
    /// </summary>
    GUIUnitModelCanvas CreateModelViewToTagGraphHolder(string modelInfo, GImage holderImage);

    /// <summary>
    /// 向指定的GGraph控件中创建一个显示角色立绘控件并返回
    /// </summary>
    RoleHeadUI CreateHeadAvataView(string avataInfo, GGraph tagHolder);

    /// <summary>
    /// 显示一条游戏内置的TOAST信息
    /// </summary>
    void ShowToastInfo(string _info, float _keepTime = 2.0f);

    /// <summary>
    /// 显示一个GUI对话框 _type 0:确认框 1：是否选择框  回调参数是确认结果 0：是 1：否
    /// </summary>
    void ShowConfirmDialog(string _info, int _type, Action<int> _callback);

    /// <summary>
    /// 转换一个UGUI格式的字符串颜色文本符合FairyGUI UBB文本格式标准
    /// </summary>
    string ConvertUGUIStr(string srcStr);

    /// <summary>
    /// 设置控件相对于其父级的位置
    /// </summary>
    /// <param name="_alignMode">0：左上 1：中上 2：右上 3：左中 4：正中 5：右中 6：左下 7：中下 8：右下</param>
    void SetComponentAlignForParent(GObject _tagObj, int _alignMode, Vector2 _offset);

    /// <summary>
    /// 为目标控件创建一个提醒标记控件
    /// </summary>
    GComponent CreateAlertMark(GObject tagObj, int _alignMode, Vector2 _offset);

    /// <summary>
    /// 为目标控件创建一个数字提醒标记控件
    /// </summary>
    GUICtlAlertNumBox CreateNumAlertMark(GComponent tagComp, int _alignMode, Vector2 _offset);

    /// <summary>
    /// 获取一个已注册热键的按钮名称
    /// </summary>
    string GetHotkeyDisplayName(int _hotkeyCode);

}
