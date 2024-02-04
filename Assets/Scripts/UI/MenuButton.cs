using FMODUnity;
using Game.GeneralManagers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
	public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField]
		private EventReference HoverSound, PressSound;

		[SerializeField]
		private UnityEvent OnPressed, OnHovered, OnUnhovered;

		private void Awake()
		{
			var button = GetComponent<Button>();
			button.onClick.AddListener(() =>
			{
				OnPressed?.Invoke();

				if (!PressSound.IsNull)
				{
					AudioManager.PlayOneShot(PressSound, transform.position);
				}
			});
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			OnHovered?.Invoke();

			if (!HoverSound.IsNull)
			{
				AudioManager.PlayOneShot(HoverSound, transform.position);
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			OnUnhovered?.Invoke();
		}
	}
}
