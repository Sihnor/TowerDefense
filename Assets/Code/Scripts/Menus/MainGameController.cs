using System;
using System.Collections;
using System.Collections.Generic;
using Code.Scripts.Enums;
using Code.Scripts.State;
using UnityEngine;
using UnityEngine.UIElements;

public class MainGameController : MonoBehaviour
{
    private Button RerollButton;
    private Button NextWaveButton;
    
    VisualElement[] Wizards = new VisualElement[5];
    private VisualElement WizardOne;
    private VisualElement WizardTwo;
    private VisualElement WizardThree;
    private VisualElement WizardFour;
    private VisualElement WizardFive;
    
    VisualElement[] Borders = new VisualElement[5];
    private VisualElement BorderOne;
    private VisualElement BorderTwo;
    private VisualElement BorderThree;
    private VisualElement BorderFour;
    private VisualElement BorderFive;

    
    private void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        
        this.RerollButton = root.Q<Button>("RerollButton");
        this.NextWaveButton = root.Q<Button>("NextWaveButton");
        
        this.RerollButton.RegisterCallback<ClickEvent>(Reroll);
        this.NextWaveButton.RegisterCallback<ClickEvent>(NextWave);
        
        this.WizardOne = root.Q<VisualElement>("WizardOne");
        this.Wizards[0] = this.WizardOne;
        this.WizardTwo = root.Q<VisualElement>("WizardTwo");
        this.Wizards[1] = this.WizardTwo;
        this.WizardThree = root.Q<VisualElement>("WizardThree");
        this.Wizards[2] = this.WizardThree;
        this.WizardFour = root.Q<VisualElement>("WizardFour");
        this.Wizards[3] = this.WizardFour;
        this.WizardFive = root.Q<VisualElement>("WizardFive");
        this.Wizards[4] = this.WizardFive;
        
        this.BorderOne = root.Q<VisualElement>("BorderOne");
        this.Borders[0] = this.BorderOne;
        this.BorderTwo = root.Q<VisualElement>("BorderTwo");
        this.Borders[1] = this.BorderTwo;
        this.BorderThree = root.Q<VisualElement>("BorderThree");
        this.Borders[2] = this.BorderThree;
        this.BorderFour = root.Q<VisualElement>("BorderFour");
        this.Borders[3] = this.BorderFour;
        this.BorderFive = root.Q<VisualElement>("BorderFive");
        this.Borders[4] = this.BorderFive;
        
        Reroll(null);
    }
    

    private void NextWave(ClickEvent evt)
    {
        
        GameMode.Instance.StartWavePhase();
        this.NextWaveButton.visible = false;
    }

    private void Reroll(ClickEvent evt)
    {
        for (int i = 0; i < 5; i++)
        {
           RerollBorder(this.Borders[i]); 
           RerollWizard(this.Wizards[i]);
        }
    }

    private void RerollBorder(VisualElement border)
    {

        int rarity = UnityEngine.Random.Range(0, 10000);
        
        int borderIndex = rarity switch
        {
            < 200 => 6,
            >= 200 and < 500 => 5,
            >= 500 and < 1000 => 4,
            >= 1000 and < 2000 => 3,
            >= 2000 and < 3500 => 2,
            >= 3500 and < 6000 => 1,
            >= 6000 => 0,
        };

        string borderName = Enum.GetName(typeof(ERarity), borderIndex);
        Sprite borderSprite = Resources.Load<Sprite>($"Art/Sprites/Rarity/{borderName}");
        border.style.backgroundImage = new StyleBackground(borderSprite);

    }

    private void RerollWizard(VisualElement wizard)
    {
        int wizardIndex = UnityEngine.Random.Range(0, 4);
        string wizardName = Enum.GetName(typeof(EWizard), wizardIndex);
        Sprite wizardSprite = Resources.Load<Sprite>($"Art/Sprites/Wizards/{wizardName}");
        wizard.style.backgroundImage = new StyleBackground(wizardSprite);
    }
    
}
