using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShapeEditScript : MonoBehaviour
{
    [SerializeField] GridBuilder gb;
    [SerializeField] TMP_InputField red_input_field;
    [SerializeField] TMP_InputField green_input_field;
    [SerializeField] TMP_InputField blue_input_field;
    [SerializeField] TextMeshProUGUI apply_text;

    private void Start()
    {
        gb.changed_selected_object_in_scene.AddListener(updateUI);
    }

    public void updateUI()
    {
        Color c = gb.GetColorSelectedObject();
        red_input_field.text = c.r.ToString();
        green_input_field.text = c.g.ToString();
        blue_input_field.text = c.b.ToString();
    }

    public void DestroySelectedObject()
    {
        gb.DestroySelectedBuilding();
    }

    public void PickupSelectedObject()
    {
        gb.PickupSelectedObjectInScene();
    }

    public void ChangeColourSelectedObject()
    {
        float r, g, b;
        if (!float.TryParse(red_input_field.text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out r)) return;
        if (!float.TryParse(green_input_field.text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out g)) return;
        if (!float.TryParse(blue_input_field.text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out b)) return;
        Color c = new Color(r,g,b);
        gb.ChangeColourSeletedObject(c);
        apply_text.text = "Changed Color!";
        Invoke("ResetApplyText", 2f);
    }

    private void ResetApplyText()
    {
        apply_text.text = "Press Apply To Change The Color.";
    }
}
