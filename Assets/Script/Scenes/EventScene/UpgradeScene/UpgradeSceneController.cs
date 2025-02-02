using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSceneController : MonoBehaviour
{
    private DeckUnit _upgradeUnit;

    [SerializeField] private Image _upgradeunitImage; // 강화 대상 유닛

    [SerializeField] private Image _releaseunitImage; // 교화 해소 유닛

    void Start()
    {
        Init();
    }

    private void Init()
    {
        List<Script> scripts = new List<Script>();

        if(GameManager.Data.GameData.isVisitUpgrade == false)
            scripts = GameManager.Data.ScriptData["강화소_입장_최초"];
        else
            scripts = GameManager.Data.ScriptData["강화소_입장"];

        GameManager.UI.ShowPopup<UI_Conversation>().Init(scripts);
    }

    public void OnUpgradeUnitButtonClick()
    {
        GameManager.UI.ShowPopup<UI_MyDeck>("UI_MyDeck").Init(false, OnSelect);
        //GameManager.Sound.Play("UI/ButtonSFX/ButtonClickSFX");
    }

    public  void OnSelect(DeckUnit unit)
    {
        _upgradeUnit = unit;
        _upgradeunitImage.sprite = unit.Data.Image;
        _upgradeunitImage.color = Color.white;

        GameManager.UI.ClosePopup();
        GameManager.UI.ClosePopup();
    }

    public void OnUpgradeButtonClick()
    {
        if (_upgradeUnit != null)
            GameManager.UI.ShowPopup<UI_UpgradeSelectButton>().init(this);
        GameManager.Sound.Play("UI/ButtonSFX/ButtonClickSFX");
    }

    public void OnUpgradeSelect(int select) 
    {
        GameManager.UI.ClosePopup();
        UI_Conversation script = GameManager.UI.ShowPopup<UI_Conversation>();

        if (select == 1)
        {
            _upgradeUnit.DeckUnitUpgradeStat.ATK += 5;
            script.Init(GameManager.Data.ScriptData["강화소_공격력"], false);
        }
        else if (select == 2) 
        {
            _upgradeUnit.DeckUnitUpgradeStat.MaxHP += 15;
            script.Init(GameManager.Data.ScriptData["강화소_체력"], false);
        }
        else if (select == 3)
        {
            _upgradeUnit.DeckUnitUpgradeStat.SPD += 25;
            script.Init(GameManager.Data.ScriptData["강화소_속도"], false);
        }
        else if (select == 4)
        {
            _upgradeUnit.DeckUnitUpgradeStat.ManaCost -= 10;
            script.Init(GameManager.Data.ScriptData["강화소_코스트"], false);
        }
        GameManager.Sound.Play("UI/UpgradeSFX/UpgradeSFX");
        // OnQuitClick();
        StartCoroutine(QuitScene(script));
    }

    public void OnQuitClick()
    {
        StartCoroutine(QuitScene());

        if (GameManager.Data.GameData.isVisitUpgrade == false)
        {
            GameManager.Data.GameData.isVisitUpgrade = true;
        }
        GameManager.Sound.Play("UI/ButtonSFX/ButtonClickSFX");
    }

    private IEnumerator QuitScene(UI_Conversation eventScript = null)
    {
        if (GameManager.Data.GameData.isVisitStigma == false)
        {
            GameManager.Data.GameData.isVisitStigma = true;
        }

        if (eventScript != null)
            yield return StartCoroutine(eventScript.PrintScript());

        UI_Conversation quitScript = GameManager.UI.ShowPopup<UI_Conversation>();

        if (GameManager.Data.GameData.isVisitUpgrade == false)
        {
            GameManager.Data.GameData.isVisitUpgrade = true;
            quitScript.Init(GameManager.Data.ScriptData["강화소_퇴장_최초"], false);
        }
        else
            quitScript.Init(GameManager.Data.ScriptData["강화소_퇴장"], false);

        yield return StartCoroutine(quitScript.PrintScript());
        SceneChanger.SceneChange("StageSelectScene");
    }
}