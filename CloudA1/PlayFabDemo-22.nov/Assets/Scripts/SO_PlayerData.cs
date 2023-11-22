using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SO_PlayerData : ScriptableObject
{
    public List<Skill> SkillList;
    public bool skillsReady = true;
    public bool XpLevelReady = true;
    public int SC;
    public int XP = 0;
    public int Level = 1;

    public void Reset() {
        skillsReady = true;
        XpLevelReady = true;
        SC = 0;
        XP = 0;
        Level = 1;
    }
}
