using Game.Controllers;
using Game.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Prefabs
{
    public class ArenaItemPrefab : MonoBehaviour
    {
        [Header("UI Elements")] public Image m_ItemImage;

        [Header("Listeners")] public Toggle m_ItemToggle;

        public void Init(GameDataManager.DroneWithBlueprint droneWithBlueprint, ToggleGroup toggleGroup)
        {
            m_ItemToggle.group = toggleGroup;
            m_ItemToggle.onValueChanged.RemoveAllListeners();
            m_ItemToggle.onValueChanged.AddListener(isOn => OnItemToggleChanged(isOn, droneWithBlueprint));
            m_ItemImage.sprite =
                Resources.Load<Sprite>(droneWithBlueprint.drone.image);
        }

        private void OnItemToggleChanged(bool isOn, GameDataManager.DroneWithBlueprint drone)
        {
            if (!isOn) return;

            ArenaController.Instance.SelectItem(drone);
        }
    }
}