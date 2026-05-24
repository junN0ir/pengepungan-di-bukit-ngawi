using UnityEngine;

public class InteractableKorek : Item
{
    public override void Start()
    {
        objectName = "Korek Api";
        interactMessage = "Ambil Korek";
        base.Start();
    }

    public override (string, string) GetData()
    {
        return (objectName, interactMessage);
    }

    public override void OnInteract()
    {
        Debug.Log("InteractableKorek OnInteract dipanggil");
        base.OnInteract();
        OnInteractSucceed();
    }
}