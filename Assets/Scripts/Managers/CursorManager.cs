using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D m_DefaultCursorTexture;
    [SerializeField] private Vector2 m_CursorHotspot = Vector2.zero; 
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(m_DefaultCursorTexture, m_CursorHotspot, CursorMode.Auto);
    }
}
