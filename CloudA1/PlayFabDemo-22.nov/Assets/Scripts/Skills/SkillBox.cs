using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class Skill {
    public string name;
    public int level;
    public Skill(string name, int level) {
        this.name = name;
        this.level = level;
    }
}

public class SkillBox : MonoBehaviour {
    [SerializeField] TMP_Text skillname;
    [SerializeField] TMP_Text SkillLevelText;
    [SerializeField] Button b_plus, b_minus;
    [SerializeField] TMP_Text costText;
    [SerializeField] int MaxLevel = 10;
    [SerializeField] int Cost = 10;
    private int Level;
    private int templevel; //to prevent levelling down past saved level

    private void OnEnable() {
        costText.text = Cost.ToString();
    }

    public Skill ReturnSkill() {
        Level = templevel;
        return new Skill(skillname.text,  templevel);
    }

    public void SetUI(Skill sk) {
        Level = sk.level;
        templevel = Level;
        skillname.text = sk.name;
        SkillLevelText.text = sk.level.ToString();
        costText.text = Cost.ToString();
    }

    public void LevelUp() {
        //not at max level, and can afford
        if (templevel < MaxLevel && SkillBoxManager.Instance.CanAfford(Cost)) {
            templevel++;
            SkillBoxManager.Instance.ChangeEndCost(Cost);
        }
        UpdateLevelText();
    }
    public void LevelDown() {
        //not less than the recorded level
        if (templevel > Level) {
            templevel--;
            SkillBoxManager.Instance.ChangeEndCost(-Cost);
        }
        UpdateLevelText();
    }

    public void ResetLevel() {
        //set level to 0, and refund the costs of levelling them up
        SkillBoxManager.Instance.ChangeEndCost(-Level * Cost);
        templevel = 0;
        UpdateLevelText();
    }

    public void UpdateLevelText() {
        //show +/- levels to be bought.
        SkillLevelText.text = Level.ToString() +(templevel-Level != 0 ? ((templevel < Level? " " :" + ") + (templevel-Level).ToString()) : "");
    }
}
