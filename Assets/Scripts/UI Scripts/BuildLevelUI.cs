using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuildLevelUI : MonoBehaviour
{
    public GridBuilder gb;
    public TextMeshProUGUI build_level_text;

    private void Start()
    {
        gb.changed_build_level.AddListener(UpdateBuildLevelText);
    }

    public void UpdateBuildLevelText()
    {
        build_level_text.text = "Current Build Level: " + gb.GetCurrentBuildLevel().ToString();
    }
}
