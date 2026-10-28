using UnityEngine;

public static class PrototypeBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreatePrototype()
    {
        if (Object.FindObjectOfType<PrototypeDirector>() != null) return;

        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        cameraObject.AddComponent<Camera>();

        GameObject directorObject = new GameObject("Prototype Director");
        directorObject.AddComponent<PrototypeDirector>();
    }
}
