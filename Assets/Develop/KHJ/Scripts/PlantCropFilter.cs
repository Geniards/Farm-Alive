using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;

public class PlantCropFilter : XRBaseTargetFilter
{
    public override void Process(IXRInteractor interactor, List<IXRInteractable> targets, List<IXRInteractable> results)
    {
        // Interactor�� ������ ��� Interactable���� �����ϴ� targets(���͸� ��) ��ȸ
        foreach (var target in targets)
        {
            Crop crop = target.transform.GetComponent<Crop>();
            PlantGround plantGround = interactor.transform.GetComponent<PlantGround>();

            if (crop == null)
                continue;

            // ���嵵 & ���� Ƚ�� �˻�
            if (crop.CurState != Crop.E_CropState.GrowCompleted && plantGround.CanPlant(crop))
            {
                // �� �ڶ� �۹��� �ƴ� �� results(���͸� ��)�� �߰�
                results.Add(target);
            }
        }
    }
}
