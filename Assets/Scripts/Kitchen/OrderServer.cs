using UnityEngine;
using CookingPrototype.Controllers;
using JetBrains.Annotations;
using UnityEngine.EventSystems;

namespace CookingPrototype.Kitchen 
{
	[RequireComponent(typeof(OrderPlace))]
	public sealed class OrderServer : MonoBehaviour, IPointerClickHandler 
	{
		OrderPlace _orderPlace;

		void Start() 
		{
			_orderPlace = GetComponent<OrderPlace>();
		}

		[UsedImplicitly]
		public void TryServeOrder() 
		{
			var order = OrdersController.Instance.FindOrder(_orderPlace.CurOrder);
			if ( (order == null) || !GameplayController.Instance.TryServeOrder(order) ) 
			{
				return;
			}
			_orderPlace.FreePlace();
		}
		public void OnPointerClick(PointerEventData eventData) 
		{
			if ( eventData.clickCount == 2 ) 
			{
				_orderPlace.FreePlace();
			}
		}
	}
}