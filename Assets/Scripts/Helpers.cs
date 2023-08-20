using UnityEngine.UIElements;

namespace GameWallet.Helpers
{
    public class UIHelpers
    {
        public static void UpdateButtonState(Button button, bool shouldEnable)
        {
            button.SetEnabled(shouldEnable);
        }
    }
}