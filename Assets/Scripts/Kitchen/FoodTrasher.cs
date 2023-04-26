using UnityEngine;
using JetBrains.Annotations;
using UnityEngine.EventSystems;

namespace CookingPrototype.Kitchen 
{
	[RequireComponent(typeof(FoodPlace))]
	public sealed class FoodTrasher : MonoBehaviour, IPointerClickHandler 
	{
		FoodPlace _place = null;
		float _timer = 0f;

		void Start() 
		{
			_place = GetComponent<FoodPlace>();
			_timer = Time.realtimeSinceStartup;
		}

		[UsedImplicitly]
		public void TryTrashFood() // Освобождает место по двойному тапу если еда на этом месте сгоревшая.
		{
			var food = _place.CurFood.CurStatus;
			if ( food == Food.FoodStatus.Overcooked ) 
			{
				_place.Free();
			}
		}

		public void OnPointerClick(PointerEventData eventData) 
		{
			if ( eventData.clickCount == 2 ) 
			{
				TryTrashFood();
			}
		}
	}
}