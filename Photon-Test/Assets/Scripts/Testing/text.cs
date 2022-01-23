using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class text : MonoBehaviour
{

    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;
    public TextMeshProUGUI text3;
    public BedScript bed1;
    public BedScript bed2;
    public BedScript bed3;

    // Update is called once per frame
    void Update()
    {
        text1.text = bed1.getPlayer().Objects.name.text;
        text2.text = bed2.getPlayer().Objects.name.text;
        text3.text = bed3.getPlayer().Objects.name.text;

    }
}
