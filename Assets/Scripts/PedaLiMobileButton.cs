using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Attach to a UI Button or Image with Raycast Target enabled.
/// Set Action in the Inspector, then assign the PedaLiController.
/// </summary>
public class PedaLiMobileButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public enum ButtonAction { MoveLeft, MoveRight, Grab, Jump }

    [SerializeField] private PedaLiController controller;
    [SerializeField] private ButtonAction action;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (controller == null) return;
        switch (action)
        {
            case ButtonAction.MoveLeft: controller.SetMoveLeft(true); break;
            case ButtonAction.MoveRight: controller.SetMoveRight(true); break;
            case ButtonAction.Grab: controller.SetGrab(true); break;
            case ButtonAction.Jump: controller.Jump(); break;
        }
    }

    public void OnPointerUp(PointerEventData eventData) => ReleaseHeldAction();
    public void OnPointerExit(PointerEventData eventData) => ReleaseHeldAction();

    private void ReleaseHeldAction()
    {
        if (controller == null) return;
        switch (action)
        {
            case ButtonAction.MoveLeft: controller.SetMoveLeft(false); break;
            case ButtonAction.MoveRight: controller.SetMoveRight(false); break;
            case ButtonAction.Grab: controller.SetGrab(false); break;
        }
    }
}
