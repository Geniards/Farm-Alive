using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;

public class GrowingFilter : XRBaseTargetFilter
{
    public override void Process(IXRInteractor interactor, List<IXRInteractable> targets, List<IXRInteractable> results)
    {
        // Interactor�� ������ ��� Interactable���� �����ϴ� targets(���͸� ��) ��ȸ
        foreach (var target in targets)
        {
            Crop crop = target.transform.GetComponent<Crop>();
            if (crop == null)
                continue;

            // ���嵵 �˻�
            if (crop.CurState != Crop.E_CropState.GrowCompleted)
            {
                // �� �ڶ� �۹��� �ƴ� �� results(���͸� ��)�� �߰�
                results.Add(target);
            }
        }
    }
}
