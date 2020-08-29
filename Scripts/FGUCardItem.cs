using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class FGUCardItem : MonoBehaviour
{
    #region
    private GComponent mainUI;
    private GList list;
    private GButton commonBtn;
    private GButton uncommonBtn;
    private GButton comBtn;
    private GButton enterBtn;
    private GLoader comCard1_cha;
    private GLoader comCard2_cha;
    private GLoader comCard1_bar;
    private GLoader comCard2_bar;
    private GLoader cardComRes_bar;
    private GLoader cardComRes_cha;
    private GObject gObjectComCard1;
    private GObject gObjectComCard2;
    private GObject gObjectComCardRes;
    private GObject gObjectCardMessage;
    private GObject gObjectStartEnter;
    private GGroup cardGroup;
    private Controller startEnterCom;
    private Transition comCardAnim;
    private Transition createCardAnim;
    private Transition enterCommonCardAnim;
    private Transition enterUncommonCardAnim;
    private Transition CardMessageShowAnim;
    private Dictionary<int,GObject> dicGobjectPos;
    private float time = 0;
    private int maxLevel=4;
    private int cardListMaxCount = 40;
    private int[] commonCardLevAry;
    private int[] unCommonCardLevAry;
    private bool timer = false;
    private bool isInCommonPlan=true;
    private bool isFirstEnter=true;
    private List<GObject> cardObjList;
    #endregion
    void Start()
    {
       
        init();
        ListCardCountCreate(list, cardListMaxCount);
        SwitchBtnClick();
        ComBtnOnClick(maxLevel);
        CardBackOnClick();
        EnterBtnOnClick();
        FirstEnter();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (timer)
        {
            time += Time.deltaTime;
            if (time >= 1.5f)
            {
                CardMessageShowAnim.timeScale = 2.0f;
                CardMessageShowAnim.Play(()=> { Reset(); });

            }
  
        }
      
    }
    /// <summary>
    /// 初始化数据
    /// </summary>
    private void init()
    {
        mainUI = GetComponent<UIPanel>().ui;
        list = mainUI.GetChild("cardList").asList;
        comCard1_bar = mainUI.GetChild("ComMat1Item").asButton.GetChild("icon_bar").asLoader;
        comCard2_bar = mainUI.GetChild("ComMat2Item").asButton.GetChild("icon_bar").asLoader;
        comCard1_cha = mainUI.GetChild("ComMat1Item").asButton.GetChild("icon_cha").asLoader;
        comCard2_cha = mainUI.GetChild("ComMat2Item").asButton.GetChild("icon_cha").asLoader;
        commonBtn = mainUI.GetChild("mainBg").asCom.GetChild("commonBtn").asButton;
        uncommonBtn = mainUI.GetChild("mainBg").asCom.GetChild("uncommonBtn").asButton;
        comBtn = mainUI.GetChild("comBtn").asButton;
        enterBtn = mainUI.GetChild("CardComSucce").asCom.GetChild("enterBtn").asButton;
        gObjectComCard1 = mainUI.GetChild("ComMat1Item");
        gObjectComCard2 = mainUI.GetChild("ComMat2Item");
        gObjectComCardRes = mainUI.GetChild("ComItem");
        gObjectCardMessage = mainUI.GetChild("cardMessage");
        gObjectStartEnter = mainUI.GetChild("n21");
        startEnterCom = gObjectStartEnter.asCom.GetController("c1");
        dicGobjectPos = new Dictionary<int, GObject>();
        commonCardLevAry = new int[cardListMaxCount];
        unCommonCardLevAry = new int[cardListMaxCount];
        cardObjList = new List<GObject>();
        comCardAnim = mainUI.GetTransition("comCardAnim");
        enterCommonCardAnim = mainUI.GetTransition("enterCommonCardAnim");
        enterUncommonCardAnim = mainUI.GetTransition("enterUncommonCardAnim");
        CardMessageShowAnim = mainUI.GetTransition("cardMessageShow");
        cardComRes_cha = gObjectComCardRes.asButton.GetChild("icon_cha").asLoader;
        cardComRes_bar = gObjectComCardRes.asButton.GetChild("icon_bar").asLoader;
    }
    /// <summary>
    /// 新手指引
    /// </summary>
    private void FirstEnter()
    {
        if (isFirstEnter)
        {
            SortListItem(0, commonCardLevAry);
            gObjectStartEnter.asCom.GetChild("n3").asButton.title = "进入";
            gObjectStartEnter.asCom.GetChild("n3").asButton.onClick.Add(() =>
            {
                startEnterCom.selectedIndex = 1;
            });
            gObjectStartEnter.visible = true;
            cardObjList[0].asButton.onClick.Add(() =>
            {
                if (!isFirstEnter) return;
                if (startEnterCom.selectedIndex == 1)
                {
                    startEnterCom.selectedIndex = 2;
                }
                else  if (startEnterCom.selectedIndex == 3)
                {
                    startEnterCom.selectedIndex = 4;
                }

            });
            cardObjList[1].asButton.onClick.Add(() =>
             {
                 if (!isFirstEnter) return;
                 startEnterCom.selectedIndex = 5;
             });
        }
        
    }
    /// <summary>
    ///放入卡包按钮事件 
    /// </summary>
    private void EnterBtnOnClick()
    {
        enterBtn.onClick.Add(() =>
        {
            mainUI.GetChild("CardComSucce").visible = false;
            int lev = CheckCardLev(cardComRes_bar.url);
            if (lev >= maxLevel)
            {
                enterUncommonCardAnim.Play(()=> 
                {
                    if (!isInCommonPlan) { PlayCreateCardTrans(lev, unCommonCardLevAry);return; }
                    cardComRes_cha.url = null;
                    cardComRes_bar.url = null;
                });
                return;
            }
            enterCommonCardAnim.Play(()=> { PlayCreateCardTrans(lev, commonCardLevAry); });
        });
    }
    /// <summary>
    /// 播放放入卡包特效
    /// </summary>
    /// <param name="_lev"></param>
    /// <param name="_levAry"></param>
    private void PlayCreateCardTrans(int _lev,int[] _levAry)
    {
        for (int i = 0; i < _levAry.Length; i++)
        {
            if (_levAry[i] <= 0)
            {      
                cardObjList[i-1].asButton.GetTransition("createCard").Play(()=> 
                {
                    ChangeCardCharAndBar(_lev.ToString(), cardObjList[i-1]);
                    cardComRes_cha.url = null;
                    cardComRes_bar.url = null;
                });
                return;
            }
        }
    }
    /// <summary>
    /// 再次点击合成栏卡牌返回卡牌事件
    /// </summary>
    private void CardBackOnClick()
    {
       
        gObjectComCard1.asButton.onClick.Add(() =>
        {
            if (dicGobjectPos[1] == null) return;
            if (isFirstEnter) startEnterCom.selectedIndex = 3;
            ClearCardChaAndBar(gObjectComCard1,false);
            CardBackToList(dicGobjectPos[1]);
            dicGobjectPos[1] = null;
        });
        gObjectComCard2.asButton.onClick.Add(() =>
        {
            if (dicGobjectPos[2] == null) return;
            ClearCardChaAndBar(gObjectComCard2,false);
            CardBackToList(dicGobjectPos[2]);
            dicGobjectPos[2] = null;
        });
    }
    /// <summary>
    /// 重置界面
    /// </summary>
    private void Reset()
    {
        if (comCard1_bar.url != null)
        {
            ClearCardChaAndBar(gObjectComCard1, false);
            CardBackToList(dicGobjectPos[1]);
            dicGobjectPos[1] = null;
        }
        if (comCard2_bar.url != null)
        {
            ClearCardChaAndBar(gObjectComCard2, false);
            CardBackToList(dicGobjectPos[2]);
            dicGobjectPos[2] = null;
        }
    }
    /// <summary>
    /// 将卡牌放回卡牌列表
    /// </summary>
    /// <param name="_obj"></param>
    private void CardBackToList( GObject _obj)
    {
        _obj.asCom.GetChild("mask").visible = false;
        _obj.asCom.GetChild("mask_bar").visible = false;
        
    }
    /// <summary>
    /// 记忆卡牌对应存放的位置
    /// </summary>
    /// <param name="_cardItemid"></param>
    /// <param name="_memoryObj"></param>
    private void MemoryCardObject(int _cardItemid,GObject _memoryObj)
    {
        if (!dicGobjectPos.ContainsKey(_cardItemid))
        {
            dicGobjectPos.Add(_cardItemid, _memoryObj);
            return;
        }
        dicGobjectPos[_cardItemid] = _memoryObj;
    }
    /// <summary>
    /// 合成按钮事件
    /// </summary>
    /// <param name="_max"></param>
    private void ComBtnOnClick(int _max)
    {
        string maxUrl = UIPackage.GetItemURL("FGUIdemo", "C" + _max);    
        comBtn.onClick.Add(()=>
        {
            if (comCard1_bar.url != comCard2_bar.url || comCard1_bar.url == null || comCard2_bar.url == null
           || comCard1_bar.url == maxUrl || comCard2_bar.url == maxUrl)
            {
                return;
            }
            startEnterCom.selectedIndex = 0;
            gObjectStartEnter.asCom.visible = false;
            isFirstEnter = false;
            comCardAnim.Play(1,0,0,0.6f,()=> {             
                ChangeCardCharAndBar((CheckCardLev(comCard1_bar.url) + 1).ToString(), gObjectComCardRes);
                comCard1_bar.url = null;
                comCard2_bar.url = null;
                comCard1_cha.url = null;
                comCard2_cha.url = null;
                comCardAnim.Play(1, 0, 0.6f, 2.1f, ()=> {
                    ClearCardChaAndBar(dicGobjectPos[1], true);
                    ClearCardChaAndBar(dicGobjectPos[2], true);
                    if (!isInCommonPlan)
                    {
                        SortListItem(0, unCommonCardLevAry);
                    }
                    else
                    {
                        SortListItem(0, commonCardLevAry);
                    }
                    if (CheckCardLev(cardComRes_bar.url) >= maxLevel)
                    {
                        AddCardLevToAry(unCommonCardLevAry);
                    }
                    else
                    {
                        AddCardLevToAry(commonCardLevAry);
                    }
                    
                });
               
             
               
               
            });       
        });
    }
    /// <summary>
    /// 将卡牌放入卡牌数组
    /// </summary>
    /// <param name="_levAry"></param>
    private void AddCardLevToAry(int[] _levAry)
    {
       
        for (int i = 0; i < _levAry.Length; i++)
        {
            if (_levAry[i] <= 0)
            {
                _levAry[i] = CheckCardLev(cardComRes_bar.url);
                return;
            }
        }
    }
    /// <summary>
    /// 获取卡牌的稀有度
    /// </summary>
    /// <param name="_url"></param>
    /// <returns></returns>
    private int CheckCardLev(string _url)
    {
        for (int i = 1; i <= maxLevel; i++)
        {
            if (_url == UIPackage.GetItemURL("FGUIdemo", "C" + i))
            {
                return i;
            }
        }
        return 0;
    }
    /// <summary>
    /// 切换按钮事件
    /// </summary>
    private void SwitchBtnClick()
    {
      
        commonBtn.icon = UIPackage.GetItemURL("FGUIdemo", "yelbar");
        uncommonBtn.icon = null;
        commonBtn.onClick.Add(() =>
        {
            if (commonBtn.icon == null) Reset();
            isInCommonPlan = true;
            uncommonBtn.icon = null;
            commonBtn.icon = UIPackage.GetItemURL("FGUIdemo", "yelbar");
            SortListItem(0, commonCardLevAry);
        });
        uncommonBtn.onClick.Add(() =>
        {
            if (uncommonBtn.icon == null) Reset();
            isInCommonPlan = false;
            commonBtn.icon = null;
            uncommonBtn.icon = UIPackage.GetItemURL("FGUIdemo", "yelbar");
            SortListItem(0, unCommonCardLevAry);
        });
        GButton cardMsgBtn = gObjectCardMessage.asCom.GetChild("n2").asButton;
        GButton cardSkillBtn = gObjectCardMessage.asCom.GetChild("n4").asButton;
        GButton cardMsgCloseBtn = gObjectCardMessage.asCom.GetChild("n5").asButton;
        GGroup cardMsgGrp = gObjectCardMessage.asCom.GetChild("cardStates").asGroup;
        GGroup cardSkillGrp = gObjectCardMessage.asCom.GetChild("skill").asGroup;
        GObject cardMsgIcon = cardMsgBtn.GetChild("icon");
        GObject cardSkillIcon = cardSkillBtn.GetChild("icon");
        cardMsgBtn.title = "属性";
        cardSkillBtn.title = "技能";
        cardMsgIcon.visible = true;
        cardSkillIcon.visible = false;
        cardMsgBtn.onClick.Add(() =>
        {
            cardSkillIcon.visible = false;
            cardMsgIcon.visible = true;          
            cardMsgGrp.visible = true;
            cardSkillGrp.visible = false;
        });
        cardSkillBtn.onClick.Add(() =>
        {
            cardMsgIcon.visible = false;
            cardSkillIcon.visible = true;
            cardMsgGrp.visible = false;
            cardSkillGrp.visible = true;
        });
        cardMsgCloseBtn.onClick.Add(() =>
        {
            gObjectCardMessage.asCom.visible = false;
        });
    }
    /// <summary>
    /// 卡牌列表创建
    /// </summary>
    /// <param name="_list"></param>
    /// <param name="_count"></param>
    private void ListCardCountCreate(GList _list,int _count)
    {
       
        _list.itemRenderer = RenderListItem;     
        _list.numItems = 40;       
        for (int i = 0; i < _count; i++)
        {
            GObject obj = _list.GetChildAt(i).asCom;
            GLoader charIcon = obj.asButton.GetChild("icon_cha").asLoader;
            GLoader barIcon = obj.asButton.GetChild("icon_bar").asLoader;
            commonCardLevAry[i] = CheckCardLev(barIcon.url);           
            cardObjList.Add(obj);
            CardMessageShow(obj);
            #region todo
            // GGroup cardGroup = obj.asButton.GetChild("card").asGroup;
            //obj.draggable = true;
            //// obj.dragBounds = new Rect(1350, 750, 1350, 750);
            //obj.onDragStart.Add(DragEnd);
            #endregion
            obj.asButton.onClick.Add(() => {             
                AddComCardCha(obj,charIcon.url,barIcon.url); });
            
        }
    }
    /// <summary>
    /// 卡牌信息显示
    /// </summary>
    /// <param name="_obj"></param>
    private void CardMessageShow(GObject _obj)
    {
        _obj.onTouchBegin.Add(() =>
        {
            timer = true;
        });
        _obj.onTouchEnd.Add(() =>
        {
            timer = false;
            time = 0;
        });
    }
    //private void DragEnd(EventContext _context)
    //{
    //    _context.PreventDefault();
    //}
    /// <summary>
    /// 列表item渲染
    /// </summary>
    /// <param name="_index"></param>
    /// <param name="_obj"></param>
    private void RenderListItem(int _index, GObject _obj)
    {
        ChangeCardCharAndBar( Random.Range(1, 4).ToString() , _obj);
      
    }
    /// <summary>
    /// 整理卡牌列表（大到小）
    /// </summary>
    /// <param name="_startPos"></param>
    /// <param name="_LevAry"></param>
    private void SortListItem(int _startPos,int[] _LevAry)
    {
        Arithmetic.Instance.QuickSort(_LevAry, 0, _LevAry.Length-1);
        for (int i = _startPos; i < cardObjList.Count; i++)
        {
            ChangeCardCharAndBar(_LevAry[i].ToString(), cardObjList[i]);
        }
    }
    /// <summary>
    /// 将卡牌放入合成槽
    /// </summary>
    /// <param name="_obj"></param>
    /// <param name="_chaUrl"></param>
    /// <param name="_barUrl"></param>
    private void AddComCardCha(GObject _obj,string _chaUrl,string _barUrl)
    {

        if (_chaUrl == null) return;
        if (comCard1_cha.url == null)
        {
            ChangeCardCharAndBar(_chaUrl, _barUrl, gObjectComCard1);
            ShowMask(_obj);
            MemoryCardObject(1, _obj);
        }
        else if (_obj == dicGobjectPos[1])
        {
            ClearCardChaAndBar(gObjectComCard1,false);
            CardBackToList(dicGobjectPos[1]);
            dicGobjectPos[1] = null;
            return;
        }
        else if (comCard2_cha.url == null)
        {

            ChangeCardCharAndBar(_chaUrl, _barUrl, gObjectComCard2);
            ShowMask(_obj);
            MemoryCardObject(2, _obj);
        }
        else if (_obj == dicGobjectPos[2])
        {
            ClearCardChaAndBar(gObjectComCard2,false);
            CardBackToList(dicGobjectPos[2]);
            dicGobjectPos[2] = null;
            return;
        }
        else if (comCard1_cha.url != _chaUrl)
        {
            ChangeCardCharAndBar(_chaUrl, _barUrl, gObjectComCard1);
            ShowMask(_obj);
            HideMask(dicGobjectPos[1]);
            MemoryCardObject(1, _obj);
        }
        else if (comCard2_cha.url != _chaUrl)
        {
            ChangeCardCharAndBar(_chaUrl, _barUrl, gObjectComCard2);
            ShowMask(_obj);
            HideMask(dicGobjectPos[2]);
            MemoryCardObject(2, _obj);
        }     
    }
    /// <summary>
    /// 清除卡牌
    /// </summary>
    /// <param name="_obj"></param>
    /// <param name="_isDel"></param>
    private void ClearCardChaAndBar(GObject _obj,bool _isDel)
    {
        
        GLoader loaderCha = _obj.asButton.GetChild("icon_cha").asLoader;
        GLoader loaderbar = _obj.asButton.GetChild("icon_bar").asLoader;
        loaderCha.url = null;
        loaderbar.url = null;
       
        if (_isDel)
        {
            HideMask(_obj);
            if (isInCommonPlan)
            {
                commonCardLevAry[cardObjList.IndexOf(_obj)] = 0;             
                return;
            }
            unCommonCardLevAry[cardObjList.IndexOf(_obj)] = 0;
        }
      
    }
    /// <summary>
    /// 更换卡牌
    /// </summary>
    /// <param name="_itemUrl"></param>
    /// <param name="_obj"></param>
    private void ChangeCardCharAndBar(string _itemUrl,GObject _obj)
    {
        GLoader loaderCha = _obj.asButton.GetChild("icon_cha").asLoader;
        GLoader loaderbar = _obj.asButton.GetChild("icon_bar").asLoader;
        loaderCha.url = UIPackage.GetItemURL("FGUIdemo", _itemUrl);
        loaderbar.url = UIPackage.GetItemURL("FGUIdemo", "C" + _itemUrl);
    }
    /// <summary>
    /// 更换卡牌
    /// </summary>
    /// <param name="_chaUrl"></param>
    /// <param name="_barUrl"></param>
    /// <param name="_object"></param>
    private void ChangeCardCharAndBar(string _chaUrl,string _barUrl, GObject _object)
    {
        GLoader loaderCha = _object.asButton.GetChild("icon_cha").asLoader;
        GLoader loaderbar = _object.asButton.GetChild("icon_bar").asLoader;
        loaderCha.url = _chaUrl;
        loaderbar.url = _barUrl;
    }
    /// <summary>
    /// 显示遮罩
    /// </summary>
    /// <param name="_obj"></param>
    private void ShowMask(GObject _obj)
    {
        _obj.asCom.GetChild("mask").visible = true;
        _obj.asCom.GetChild("mask_bar").visible = true;
    }
    /// <summary>
    /// 隐藏遮罩
    /// </summary>
    /// <param name="_obj"></param>
    private void HideMask(GObject _obj)
    {
        _obj.asCom.GetChild("mask").visible = false;
        _obj.asCom.GetChild("mask_bar").visible = false;
    }

   
}
