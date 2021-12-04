using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class VR_DebugText : MonoBehaviour
{
    static private VR_DebugText instance;

    private Text textComponent;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
            Destroy(instance.gameObject);

        instance = this;
        textComponent = GetComponent<Text>();
    }

    
    static public void displayText(string text)
    {
        if (instance == null)
            return;

        instance.textComponent.text = text;
        instance.textComponent.enabled = true;
    }

    static public void hideText()
    {
        if (instance != null)
            instance.textComponent.enabled = false;
    }
}
