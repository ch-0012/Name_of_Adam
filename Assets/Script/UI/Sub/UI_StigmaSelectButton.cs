using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UI_StigmaSelectButton : UI_Base
{
    [SerializeReference] private Image _stigmaImage;
    [SerializeReference] private TextMeshProUGUI _stigmaName;
    [SerializeReference] private TextMeshProUGUI _stigmaDescription;

    private UI_StigmaSelectButtonPopup _popup;
    private Passive _stigma;

    public void init(UI_StigmaSelectButtonPopup popup, Passive stigma)
    {
        _popup = popup;
        _stigma = stigma;

        _stigmaName.text = stigma.GetName();
        _stigmaDescription.text = stigma.Description;
        _stigmaImage.sprite = stigma.Sprite;
    }

    public void OnClick()
    {
        _popup.OnClick(_stigma);
    }
}
