using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VersionNumberToText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI Text_VersionNumber;
    [SerializeField]
    private string Prefix = "v";

    // Start is called before the first frame update
    void Start()
    {
        Text_VersionNumber = GetComponent<TextMeshProUGUI>();
        Text_VersionNumber.text = Prefix+Application.version;
    }

}
