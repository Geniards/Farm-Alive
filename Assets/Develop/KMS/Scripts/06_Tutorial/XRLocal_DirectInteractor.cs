using UnityEngine.XR.Interaction.Toolkit;

public class XRLocal_DirectInteractor : XRDirectInteractor
{
    /// <summary>
    /// ��ü�� ����� �� �����ϴ� �޼���.
    /// </summary>
    /// <param name="args"></param>
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        IXRSelectInteractable selectInteractable = args.interactableObject;
    }

    /// <summary>
    /// ��ü�� ������ �� �����ϴ� �޼���.
    /// </summary>
    /// <param name="args"></param>
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        IXRSelectInteractable selectInteractable = args.interactableObject;
    }
}
