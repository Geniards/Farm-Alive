using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CropInteractable : XRGrabInteractable
{
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        if (args.interactorObject is XRSocketInteractor)
        {
            SectionManager.Instance.Crops[SectionManager.Instance.CurSection].Add(GetComponent<Crop>());

            // ��ȣ�ۿ� X
            interactionLayers = (1 << 29);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        if (args.interactorObject is XRSocketInteractor)
        {
            SectionManager.Instance.Crops[SectionManager.Instance.CurSection].Remove(GetComponent<Crop>());

            // Plant ���̾� ����
            interactionLayers = (1 << 1);
        }
    }
}
