using UnityEngine;

[System.Serializable]
public class Resource {
    public string name;
    public int id;
    public string image;
}

[System.Serializable]
public class ResourceList {
    public Resource[] resources;

    public static ResourceList CreateFromJSON(string jsonString) {
        return JsonUtility.FromJson<ResourceList>(jsonString);
    }
}

