using UnityEngine;

[System.Serializable]
public class DialogData
{
    public string nama = "Yono";
    [TextArea(2, 5)]
    public string dialog = "...";
    public float durasi = 3f;
}