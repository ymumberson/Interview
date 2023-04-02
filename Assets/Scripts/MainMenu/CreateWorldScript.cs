using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CreateWorldScript : MonoBehaviour
{
    [SerializeField] TMP_InputField width_input_field;
    [SerializeField] TMP_InputField depth_input_field;
    [SerializeField] TMP_InputField height_input_field;

    private void Awake()
    {
        width_input_field.text = "10";
        depth_input_field.text = "10";
        height_input_field.text = "4";
    }

    public void CreateWorld()
    {
        int width, depth, height;
        if (!int.TryParse(width_input_field.text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out width)) return;
        if (!int.TryParse(depth_input_field.text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out depth)) return;
        if (!int.TryParse(height_input_field.text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out height)) return;

        /* Now load next scene */
        PlayerPrefs.SetInt("width", width);
        PlayerPrefs.SetInt("height", depth); /* Note, using the naming schemes used in the grid system, height would be a confusing name for depth to players */
        PlayerPrefs.SetInt("layers", height);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
