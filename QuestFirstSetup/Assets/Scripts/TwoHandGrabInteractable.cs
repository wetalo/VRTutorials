using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class TwoHandGrabInteractable : XRGrabInteractable
{
    public List<XRSimpleInteractable> secondHandGrabPoints = new List<XRSimpleInteractable>();
    private XRBaseInteractor secondInteractor;
    private Quaternion attachInitialRotation;

    public enum TwoHandRotationType {  None, First, Second }
    public TwoHandRotationType twoHandRotationType;

    public bool snapToSecondHand = true;
    private Quaternion initialRotationOffset;

    private void Start()
    {
        foreach (var item in secondHandGrabPoints)
        {
            item.onSelectEnter.AddListener(OnSecondHandGrab);
            item.onSelectExit.AddListener(OnSecondHandRelease);
        }
    }

    private void Update()
    {
        
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        if(secondInteractor && selectingInteractor)
        {
            //Compute the rotation
            if (snapToSecondHand)
            {
                selectingInteractor.attachTransform.rotation = GetTwoHandRotation();
            }
            else
            {
                selectingInteractor.attachTransform.rotation = GetTwoHandRotation() * initialRotationOffset;
            }
        }
        base.ProcessInteractable(updatePhase);
    }

    private Quaternion GetTwoHandRotation()
    {
        Quaternion targetRotation = Quaternion.identity;
        if(twoHandRotationType == TwoHandRotationType.None)
        {
            targetRotation = Quaternion.LookRotation(secondInteractor.attachTransform.position - selectingInteractor.attachTransform.position);
        }
        else if (twoHandRotationType == TwoHandRotationType.First)
        {
            targetRotation = Quaternion.LookRotation(secondInteractor.attachTransform.position - selectingInteractor.attachTransform.position, selectingInteractor.attachTransform.up);
        }
        else if (twoHandRotationType == TwoHandRotationType.Second)
        {
            targetRotation = Quaternion.LookRotation(secondInteractor.attachTransform.position - selectingInteractor.attachTransform.position, secondInteractor.attachTransform.up);
        }

        return targetRotation;
    }

    public override bool IsSelectableBy(XRBaseInteractor interactor)
    {
        bool isAlreadyGrabbed = selectingInteractor && !interactor.Equals(selectingInteractor);
        return base.IsSelectableBy(interactor) && !isAlreadyGrabbed;
    }

    public void OnSecondHandGrab(XRBaseInteractor interactor) {
        Debug.Log("SECOND HAND GRAB");
        secondInteractor = interactor;

        initialRotationOffset = Quaternion.Inverse(GetTwoHandRotation()) * selectingInteractor.attachTransform.rotation;
    }

    public void OnSecondHandRelease(XRBaseInteractor interactor)
    {
        Debug.Log("SECOND HAND RELEASE");
        secondInteractor = null;
    }

    protected override void OnSelectEnter(XRBaseInteractor interactor)
    {
        Debug.Log("First grab enter");
        base.OnSelectEnter(interactor);
        attachInitialRotation = interactor.attachTransform.localRotation;
    }

    protected override void OnSelectExit(XRBaseInteractor interactor)
    {
        Debug.Log("First grab exit");
        base.OnSelectExit(interactor);
        secondInteractor = null;
        interactor.attachTransform.localRotation = attachInitialRotation;
    }
}
