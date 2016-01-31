using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PointerObjective : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, ISelectHandler
{
	public GameObject Pointer;

	public void OnPointerEnter(PointerEventData eventData)
	{
		DoYourJob();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		DoYourJob();
	}

	public void OnSelect(BaseEventData eventData)
	{
		DoYourJob();
	}

	private void DoYourJob()
	{
		if (Pointer == null)
			return;

		Pointer.transform.SetParent(transform, false);
	}
}
